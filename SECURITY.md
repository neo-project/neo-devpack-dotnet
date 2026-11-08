# Security Policy

## Supported versions

Security fixes are developed against the `master-n3` branch and the latest published DevPack release. Older release lines may not receive fixes unless a maintainer explicitly announces support for them.

## Reporting a vulnerability

Please do not disclose an unpatched vulnerability in a public issue or pull request. Use GitHub's private advisory reporting form:

<https://github.com/neo-project/neo-devpack-dotnet/security/advisories/new>

Include the affected branch or release, the smallest reproducible example, the expected and observed behavior, and any relevant compiler output or contract bytecode. Do not include private keys, credentials, or production data.

The maintainers will acknowledge a report when they can, reproduce the issue in an isolated environment, determine the affected components, and coordinate a fix and disclosure timeline with the reporter.

## Safe disclosure

After a fix is available, the maintainers will publish the affected versions, the fixed version or commit, and any required migration guidance. Please allow time for downstream applications and contract authors to update before publishing exploit details.
