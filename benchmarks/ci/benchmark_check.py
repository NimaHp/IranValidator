#!/usr/bin/env python3
"""Benchmark baseline compare/update helper for IranValidator CI.

Parses BenchmarkDotNet console summary tables and compares against a
committed baseline so regressions (e.g. a validator going from 13 ns to
40 ns, or 0 B allocations reappearing) are caught automatically.

Subcommands:
  check  <logfile> <baseline.json>   parse a BDN log, compare to baseline.
                                     Exit code 1 on regression.
                                     If baseline is missing/empty it is
                                     created from this log (first-run
                                     baseline establishment) and the check
                                     passes.
  update <logfile> <baseline.json>   (re)write baseline from a BDN log.

Thresholds (intentionally lenient - CI must not flake on noise):
  time:       fail if mean > baseline_mean * 1.5
  allocated:  fail if allocated > baseline_allocated + 16 bytes
              (catches the 0 B -> 48 B class of regressions)

New benchmarks not present in the baseline are reported but never fail.
"""

import json
import re
import sys

TIME_FACTOR = 1.5
ALLOC_SLACK_BYTES = 16

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


def check(log_path, baseline_path):
    results = parse_log(log_path)
    if not results:
        print("ERROR: no benchmark rows parsed from log; cannot compare.")
        return 2

    baseline = load_baseline(baseline_path)
    if not baseline:
        write_baseline(baseline_path, results)
        print(f"Baseline created at {baseline_path} with {len(results)} benchmarks (first run).")
        return 0

    failures = []
    new_benchmarks = []
    for method in sorted(results):
        row = results[method]
        base = baseline.get(method)
        if base is None:
            new_benchmarks.append(method)
            continue
        if row["ns"] > base["ns"] * TIME_FACTOR:
            failures.append(
                f"{method}: {row['ns']:.1f} ns vs baseline {base['ns']:.1f} ns "
                f"(>{TIME_FACTOR:.1f}x)"
            )
        if row["alloc"] > base["alloc"] + ALLOC_SLACK_BYTES:
            failures.append(
                f"{method}: {row['alloc']} B vs baseline {base['alloc']} B "
                f"(> +{ALLOC_SLACK_BYTES} B)"
            )

    print(f"Compared {len(results)} benchmarks against {len(baseline)} baselines.")
    if new_benchmarks:
        print(f"New benchmarks (not failing): {', '.join(new_benchmarks)}")
    if failures:
        print(f"REGRESSIONS ({len(failures)}):")
        for f in failures:
            print(f"  - {f}")
        return 1
    print("OK: no regressions detected.")
    return 0


def main():
    if len(sys.argv) < 4:
        print(__doc__)
        return 2
    cmd, log_path, baseline_path = sys.argv[1], sys.argv[2], sys.argv[3]
    if cmd == "check":
        return check(log_path, baseline_path)
    if cmd == "update":
        results = parse_log(log_path)
        if not results:
            print("ERROR: no benchmark rows parsed from log.")
            return 2
        write_baseline(baseline_path, results)
        print(f"Baseline updated: {len(results)} benchmarks -> {baseline_path}")
        return 0
    print(f"Unknown command: {cmd}")
    return 2


if __name__ == "__main__":
    sys.exit(main())
