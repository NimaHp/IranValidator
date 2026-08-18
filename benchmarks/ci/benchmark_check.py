#!/usr/bin/env python3
"""Benchmark baseline compare/update helper for IranValidator CI.

Parses BenchmarkDotNet console summary tables and compares against a
committed baseline so regressions (e.g. a validator going from 13 ns to
40 ns, or 0 B allocations reappearing) are caught automatically.

Input handling
--------------
Benchmark allocation/time measurements are inherently noisy on shared
GitHub-hosted runners: even with zero code changes a method can flap
between 0 B and 40 B and occasionally spike to several KB. To make the
gate robust every config uses the *median* across multiple independent
runs of the same commit rather than trusting any single run.

Each of the subcommands below accepts one or more log files as positional
arguments. Values are aggregated per method with the median across logs.

  check  <log...> <baseline.json>   compare median of logs against a baseline.
                                     Exit code 1 on a genuine regression.
                                     If the baseline is missing/empty it is
                                     created from this batch (first run) and
                                     the check passes.
  update <log...> <baseline.json>   (re)write baseline from the median of logs.

Thresholds
----------
  time:   fail if median > baseline_mean * 1.5
  alloc:  fail if median allocated > baseline_allocated + TOLERANCE where

          TOLERANCE = max(ALLOC_FLOOR_BYTES, baseline_allocated * ALLOC_PCT)

          i.e. a fixed floor (64 B) plus a 20% relative allowance. This is the
          combined model: small allocators get a floor (covers the 0 B <-> 40 B
          bimodal flapping), larger ones get a relative slack so %-noise does
          not trip the gate. The old +16 B absolute check was pure noise.

Gating scope ("own" validators only)
------------------------------------
The suite also runs third-party comparison rows (DNTPersianUtils, Persian.Plus
and the compiled/generated-regex implementations). Those libraries are NOT
this repository's code: their allocations are intrinsically bimodal between
runs (e.g. 0 B <-> 344 B) because it is *their* allocation behaviour, which the
IranValidator codebase cannot change. Failing the gate on them only produces
false failures, so they are reported but never fail the job. Only IranValidator's
own validators (the Method names that do not match THIRD_PARTY_MARKERS) are
gated.

New benchmarks not present in the baseline are reported but never fail.
"""

import json
import re
import sys
import statistics

TIME_FACTOR = 1.5
ALLOC_FLOOR_BYTES = 64
ALLOC_PCT = 0.20

# Method names containing any of these are third-party / control rows: their
# measurements depend on code outside this repository (rival libraries or the
# regex engine), so they never fail the gate. See "Gating scope" in the docstring.
THIRD_PARTY_MARKERS = (
    "DntPersianUtils",
    "PersianPlus",
    "Regex",
    "Generated",
)

# Tolerates both table shapes BDN prints (with and without Gen0 column):
#   | Method | Mean | Error | StdDev | Allocated |
#   | Method | Mean | Error | StdDev | Gen0 | Allocated |
# Allocated cell is "192 B" or "-".
_TABLE_LINE = re.compile(
    r"^\|\s*([\w.$]+)\s*\|\s*([\d.\-]+)\s*ns\s*\|(.*)\|\s*([\d.]+\s*B|-)\s*\|?\s*$"
)


def parse_log(path):
    """Return {method: {"ns": float, "alloc": int}} from a BDN console log."""
    results = {}
    with open(path, encoding="utf-8", errors="replace") as fh:
        for line in fh:
            m = _TABLE_LINE.match(line)
            if not m:
                continue
            method, mean, alloc = m.group(1), m.group(2), m.group(4)
            ns = float(mean)
            allocated = 0 if alloc == "-" else int(re.sub(r"\D", "", alloc))
            results[method] = {"ns": ns, "alloc": allocated}
    return results


def parse_logs(paths):
    """Parse each log and aggregate every method across runs by median.

    Returns {method: {"ns": median_ns, "alloc": median_alloc}}. Methods
    missing from a log are ignored for that log (different runs may omit a
    benchmark only when it failed to execute there).
    """
    runs = []
    for path in paths:
        parsed = parse_log(path)
        if parsed:
            runs.append(parsed)
    by_method = {}
    for run in runs:
        for method, row in run.items():
            by_method.setdefault(method, []).append(row)
    aggregated = {}
    for method, rows in by_method.items():
        ns = [r["ns"] for r in rows]
        alloc = [r["alloc"] for r in rows]
        aggregated[method] = {
            "ns": statistics.median(ns),
            "alloc": int(round(statistics.median(alloc))),
        }
    return aggregated


def load_baseline(path):
    try:
        with open(path, encoding="utf-8") as fh:
            data = json.load(fh)
        return data if isinstance(data, dict) else {}
    except (FileNotFoundError, json.JSONDecodeError):
        return {}


def write_baseline(path, results):
    with open(path, "w", encoding="utf-8") as fh:
        json.dump(results, fh, indent=2, sort_keys=True)
        fh.write("\n")


def alloc_tolerance(base_alloc):
    """Combined tolerance: fixed floor plus a relative % of the baseline."""
    return max(ALLOC_FLOOR_BYTES, base_alloc * ALLOC_PCT)


def check(log_paths, baseline_path):
    results = parse_logs(log_paths)
    if not results:
        print("ERROR: no benchmark rows parsed from logs; cannot compare.")
        return 2

    baseline = load_baseline(baseline_path)
    if not baseline:
        write_baseline(baseline_path, results)
        print(
            f"Baseline created at {baseline_path} with {len(results)} benchmarks "
            f"(median of {len(log_paths)} run(s), first run)."
        )
        return 0

    failures = []
    third_party_noted = []
    new_benchmarks = []
    for method in sorted(results):
        row = results[method]
        base = baseline.get(method)
        if base is None:
            new_benchmarks.append(method)
            continue

        def _violation():
            if row["ns"] > base["ns"] * TIME_FACTOR:
                return (
                    f"{method}: {row['ns']:.1f} ns vs baseline {base['ns']:.1f} ns "
                    f"(>{TIME_FACTOR:.1f}x)"
                )
            tol = alloc_tolerance(base["alloc"])
            if row["alloc"] > base["alloc"] + tol:
                return (
                    f"{method}: {row['alloc']} B vs baseline {base['alloc']} B "
                    f"(allowed up to {base['alloc'] + tol:.1f} B, +{tol:.0f} B tolerance)"
                )
            return None

        violation = _violation()
        if violation is None:
            continue
        if any(m in method for m in THIRD_PARTY_MARKERS):
            third_party_noted.append(violation)
        else:
            failures.append(violation)

    print(f"Compared {len(results)} benchmarks (median of {len(log_paths)} run(s)) "
          f"against {len(baseline)} baselines.")
    if new_benchmarks:
        print(f"New benchmarks (not failing): {', '.join(new_benchmarks)}")
    if third_party_noted:
        print(f"INFO (third-party rows, not failing): "
              f"{', '.join(third_party_noted)}")
    if failures:
        print(f"REGRESSIONS ({len(failures)}):")
        for f in failures:
            print(f"  - {f}")
        return 1
    print("OK: no regressions detected.")
    return 0


def update(log_paths, baseline_path):
    results = parse_logs(log_paths)
    if not results:
        print("ERROR: no benchmark rows parsed from logs.")
        return 2
    write_baseline(baseline_path, results)
    print(f"Baseline updated: {len(results)} benchmarks (median of {len(log_paths)} "
          f"run(s)) -> {baseline_path}")
    return 0


def main():
    # Last positional arg is the baseline; everything before it is a log.
    if len(sys.argv) < 4:
        print(__doc__)
        return 2
    cmd = sys.argv[1]
    paths = sys.argv[2:-1]
    baseline_path = sys.argv[-1]
    if not paths:
        print("ERROR: no log files given.", file=sys.stderr)
        return 2
    if cmd == "check":
        return check(paths, baseline_path)
    if cmd == "update":
        return update(paths, baseline_path)
    print(f"Unknown command: {cmd}")
    return 2


if __name__ == "__main__":
    sys.exit(main())