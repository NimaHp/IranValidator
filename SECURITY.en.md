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

* **Input Size Constraints:** Every validator verifies input lengths prior to execution, rejecting oversized inputs efficiently.
* **Zero Memory Allocation:** The primary validation execution paths do not perform heap allocations, preventing GC pressure and allocation-based Denial of Service (DoS) attacks.
* **Stateless & Thread-Safe:** All validator singletons maintain no state and are safe for multi-threaded environments.
* **Minimal Dependencies:** The core library only depends on standard framework components (PolySharp and System.Memory for netstandard2.0).
* **Data Privacy:** This library performs no cryptographic operations and stores no personal data; it evaluates input format integrity only.
