# Security Policy

**English** | [فارسی](SECURITY.md)

## Supported Versions

| Version | Supported |
| :--- | :--- |
| 1.x | ✅ |

## Reporting Vulnerabilities

To report a security vulnerability, please use **GitHub Private Vulnerability Reporting** (accessible via the "Security" tab of this repository). Please refrain from reporting security issues in public issues or pull requests.

Include the following details in your report:

* Affected package name and version
* Steps to reproduce the vulnerability
* Potential impact assessment

Target initial response time: within 7 business days.

## Security & Design Architecture

* **Input Size Constraints:** Every validator rejects inputs longer than 128 characters with `ValidationErrorCode.ValueTooLarge` BEFORE normalization — oversized payloads (including whitespace/dash/Persian-digit-heavy strings) fail fast, without allocation and without work scaling with input size.
* **Safe Normalization:** The normalizer uses stack-allocated scratch buffers only for small inputs (≤ 1024 characters) and rents pooled heap buffers beyond that, so oversized input can never exhaust the thread stack and crash the process.
* **Zero Memory Allocation:** The primary validation execution paths do not perform heap allocations, preventing GC pressure and allocation-based Denial of Service (DoS) attacks.
* **Stateless & Thread-Safe:** All validator singletons maintain no state and are safe for multi-threaded environments.
* **HTTP Exception Handling:** `UseIranValidation` maps only validation failures (`ValidationException`) to 400; any other exception returns a generic 500 Problem Details response (never leaking the internal exception message) and is logged server-side.
* **Minimal Dependencies:** The core library only depends on standard framework components (PolySharp and System.Memory for netstandard2.0).
* **Data Privacy:** This library performs no cryptographic operations and stores no personal data; it evaluates input format integrity only.
