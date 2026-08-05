#!/usr/bin/env python3
"""Regenerate the benchmark result tables and environment versions in
benchmarks/README.md and benchmarks/README.en.md from a BenchmarkDotNet
console log.

Usage:
    python3 benchmarks/ci/update_readme.py <bdn-logfile>

Only the content between marker comments is rewritten, so hand-written
prose around the tables is never touched:

    <!-- bench-table:<id> -->   table content   <!-- /bench-table:<id> -->
    <!-- bench-date -->YYYY-MM-DD<!-- /bench-date -->
    <!-- bench-dotnet -->10.0.10<!-- /bench-dotnet -->   (from the BDN
    header line "// .NET X.Y.Z (…), …")
    <!-- bench-bdn -->v0.14.0<!-- /bench-bdn -->        (from the BDN
    header line "// BenchmarkDotNet vX.Y.Z, …")

Exit codes: 0 = ok, 2 = no rows parsed or bad usage.
"""

import datetime
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from benchmark_check import parse_log

TABLES = ["overloads", "vs-rivals", "regex"]

# Every file is regenerated from the same single BDN log so the main README
# summary tables and the detailed benchmark READMEs can never disagree.
FILES = [
    "benchmarks/README.md",
    "benchmarks/README.en.md",
    "README.md",
    "README.en.md",
]
TABLES_BY_FILE = {
    "benchmarks/README.md": ["overloads", "vs-rivals", "regex"],
    "benchmarks/README.en.md": ["overloads", "vs-rivals", "regex"],
    "README.md": ["summary-time", "summary-alloc"],
    "README.en.md": ["summary-time", "summary-alloc"],
}

VALIDATORS_10 = [
    "NationalCode", "CompanyId", "EconomicCode", "Mobile", "Telephone",
    "PostalCode", "CardNumber", "Passport", "VehiclePlate", "Iban",
]
RIVALS_6 = ["NationalCode", "CompanyId", "Mobile", "PostalCode", "CardNumber", "Iban"]
RIVAL_SUFFIXES = ["IranValidator", "PersianPlus", "DntPersianUtils"]
RIVAL_LABELS = {
    "fa": {
        "NationalCode": "کد ملی",
        "CompanyId": "شناسه شرکت",
        "Mobile": "موبایل",
        "PostalCode": "کد پستی",
        "CardNumber": "کارت بانکی",
        "Iban": "شبا",
    },
    "en": {
        "NationalCode": "National Code",
        "CompanyId": "Company ID",
        "Mobile": "Mobile",
        "PostalCode": "Postal Code",
        "CardNumber": "Card Number",
        "Iban": "IBAN",
    },
}
REGEX_ROWS = [
    "MobileSpanValidator", "MobileRegexCompiled", "MobileGeneratedRegex",
    "PostalSpanValidator", "PostalRegexCompiled", "PostalGeneratedRegex",
]

HEADERS = {
    "fa": {
        "overloads": ("| متد | میانگین زمان اجرا | حافظه تخصیص‌یافته |", "| :--- | :--- | :--- |"),
        "vs-rivals": ("| متد | میانگین زمان اجرا | حافظه تخصیص‌یافته |", "| :--- | :--- | :--- |"),
        "regex": ("| پیاده‌سازی | میانگین |", "|---|---|"),
        "summary-time": ("| اعتبارسنج | IranValidator | Persian.Plus | DNTPersianUtils |", "| :--- | :--- | :--- | :--- |"),
        "summary-alloc": ("| اعتبارسنج | IranValidator | Persian.Plus | DNTPersianUtils |", "| :--- | :--- | :--- | :--- |"),
    },
    "en": {
        "overloads": ("| Method | Mean | Allocated |", "|---|---|---|"),
        "vs-rivals": ("| Method | Mean | Allocated |", "|---|---|---|"),
        "regex": ("| Implementation | Mean |", "|---|---|"),
        "summary-time": ("| Validator | IranValidator | Persian.Plus | DNTPersianUtils |", "| :--- | :--- | :--- | :--- |"),
        "summary-alloc": ("| Validator | IranValidator | Persian.Plus | DNTPersianUtils |", "| :--- | :--- | :--- | :--- |"),
    },
}

_BDN_VER_RE = re.compile(r"^// BenchmarkDotNet v([\d.]+)")
# BDN prints both "// .NET 10.0.10 (…)" and "// Runtime=.NET 10.0.10 (…)".
_DOTNET_VER_RE = re.compile(r"^// (?:Runtime=)?\.NET (\d+\.\d+\.\d+)")


def parse_env(log_path):
    """Extract BenchmarkDotNet + .NET runtime versions from the BDN header."""
    env = {}
    with open(log_path, encoding="utf-8", errors="replace") as fh:
        for line in fh:
            m = _BDN_VER_RE.match(line)
            if m:
                env["bdn"] = m.group(1)
                continue
            m = _DOTNET_VER_RE.match(line)
            if m:
                env["dotnet"] = m.group(1)
    return env

REGEX_LABELS = {
    "fa": {
        "MobileSpanValidator": "پردازش مستقیم روی اسپن (موبایل)",
        "MobileRegexCompiled": "کامپایل‌شده با رجکس",
        "MobileGeneratedRegex": "`GeneratedRegex`",
        "PostalSpanValidator": "پردازش مستقیم روی اسپن (کدپستی)",
        "PostalRegexCompiled": "کامپایل‌شده با رجکس",
        "PostalGeneratedRegex": "`GeneratedRegex`",
    },
    "en": {
        "MobileSpanValidator": "Hand-rolled span (mobile)",
        "MobileRegexCompiled": "`Regex` compiled",
        "MobileGeneratedRegex": "`GeneratedRegex`",
        "PostalSpanValidator": "Hand-rolled span (postal)",
        "PostalRegexCompiled": "`Regex` compiled",
        "PostalGeneratedRegex": "`GeneratedRegex`",
    },
}


def build_rows(table, results, lang):
    rows, missing = [], []
    if table == "overloads":
        for v in VALIDATORS_10:
            for suffix in ("String", "Span"):
                method = f"Validate{v}{suffix}"
                if method in results:
                    r = results[method]
                    rows.append(f"| {method} | {r['ns']:.2f} ns | {r['alloc']} B |")
                else:
                    missing.append(method)
    elif table == "vs-rivals":
        for v in RIVALS_6:
            for suffix in RIVAL_SUFFIXES:
                method = f"Validate{v}{suffix}"
                if method in results:
                    r = results[method]
                    rows.append(f"| {method} | {r['ns']:.2f} ns | {r['alloc']} B |")
                else:
                    missing.append(method)
    elif table in ("summary-time", "summary-alloc"):
        key = "ns" if table == "summary-time" else "alloc"
        for v in RIVALS_6:
            vals, ok = [], True
            for suffix in RIVAL_SUFFIXES:
                method = f"Validate{v}{suffix}"
                if method not in results:
                    missing.append(method)
                    ok = False
                    break
                r = results[method]
                vals.append(f"{r[key]:.2f}" if key == "ns" else str(r[key]))
            if ok:
                label = RIVAL_LABELS[lang].get(v, v)
                rows.append(f"| {label} | " + " | ".join(vals) + " |")
    else:  # regex
        for method in REGEX_ROWS:
            if method in results:
                r = results[method]
                label = REGEX_LABELS[lang].get(method, method)
                rows.append(f"| {label} | {r['ns']:.2f} ns |")
            else:
                missing.append(method)
    return rows, missing


def html_table(header, rows, rtl=False):
    """Render benchmark rows as an HTML table.

    GitHub renders Markdown tables with a forced-LTR layout, which scrambles
    mixed Persian/Latin cells; fa tables are emitted with dir="rtl" so the
    whole table inherits right-to-left and Latin runs render as-is.
    Backticks are stripped because HTML blocks are not Markdown-processed
    (`` `Regex` `` would render the literal backticks).
    """
    ths = "".join(f"<th>{c.strip()}</th>" for c in header.strip("|").split("|"))
    trs = [
        "<tr>"
        + "".join(
            f"<td>{c.strip().replace('`', '')}</td>"
            for c in row.strip("|").split("|")
        )
        + "</tr>"
        for row in rows
    ]
    attr = ' dir="rtl"' if rtl else ""
    return (
        f"<table{attr}>\n<thead>\n<tr>{ths}</tr>\n</thead>\n"
        f"<tbody>\n" + "\n".join(trs) + "\n</tbody>\n</table>"
    )


def rewrite(readme_path, results, env):
    with open(readme_path, encoding="utf-8") as fh:
        text = fh.read()

    # Language follows the file-name convention (README.en.md / *.en.md);
    # content-based sniffing is unreliable because the "فارسی | English"
    # navigation line appears at the top of the English files too.
    lang = "fa" if ".en." not in readme_path else "en"
    today = datetime.date.today().isoformat()

    # 1) run date
    text = re.sub(
        r"<!-- bench-date -->.*?<!-- /bench-date -->",
        f"<!-- bench-date -->`{today}`<!-- /bench-date -->",
        text,
        flags=re.S,
    )

    # 2) tables
    all_missing = []
    for table in TABLES_BY_FILE.get(readme_path, []):
        start, end = f"<!-- bench-table:{table} -->", f"<!-- /bench-table:{table} -->"
        if start not in text or end not in text:
            print(f"WARN: markers for '{table}' not found in {readme_path}")
            continue
        rows, missing = build_rows(table, results, lang)
        all_missing += missing
        header, sep = HEADERS[lang][table]
        if lang == "fa":
            body = html_table(header, rows, rtl=True)
        else:
            body = "\n".join([header, sep] + rows)
        text = re.sub(
            re.escape(start) + r".*?" + re.escape(end),
            start + "\n" + body + "\n" + end,
            text,
            flags=re.S,
        )

    # 3) environment versions (only when present in the log header)
    if "dotnet" in env:
        text = re.sub(
            r"<!-- bench-dotnet -->.*?<!-- /bench-dotnet -->",
            f"<!-- bench-dotnet -->`{env['dotnet']}`<!-- /bench-dotnet -->",
            text,
            flags=re.S,
        )
    if "bdn" in env:
        text = re.sub(
            r"<!-- bench-bdn -->.*?<!-- /bench-bdn -->",
            f"<!-- bench-bdn -->`v{env['bdn']}`<!-- /bench-bdn -->",
            text,
            flags=re.S,
        )

    with open(readme_path, "w", encoding="utf-8") as fh:
        fh.write(text)
    return all_missing


def main():
    if len(sys.argv) != 2:
        print(__doc__)
        return 2
    results = parse_log(sys.argv[1])
    if not results:
        print("ERROR: no benchmark rows parsed from the log.")
        return 2
    print(f"Parsed {len(results)} benchmark rows.")
    env = parse_env(sys.argv[1])
    if "dotnet" in env:
        print(f"Environment: .NET {env['dotnet']}, BDN v{env.get('bdn', '?')}.")

    missing = []
    for path in FILES:
        missing += rewrite(path, results, env)
    if missing:
        print("WARN: expected rows absent from log (skipped): "
              + ", ".join(sorted(set(missing))))
    print("README benchmark tables updated.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
