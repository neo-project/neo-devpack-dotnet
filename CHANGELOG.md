# Changelog

All notable changes to Neo DevPack for .NET are documented in this file.

The project follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and versions use [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Changed

- Changes merged after `v3.10.1` are tracked here until the next release is published.

## [3.10.1] - 2026-07-23

### Added

- Added the `nccs` artifact diff command.
- Added a divisible NEP-11 example.
- Added `LastGasPerVote` support to `NeoAccountState`.

### Changed

- Updated the Neo dependency to `v3.10.0`.
- Mapped `BitOperations.RotateLeft`, `RotateRight`, and `LeadingZeroCount`.
- Improved numeric promotions in checked shift handling.

### Fixed

- Corrected boolean bitwise operation results.
- Corrected `Exception.ToString` and string compound assignment behavior.
- Preserved consistent behavior between compound and simple arithmetic operators.
- Fixed generated ABI source names and Dockerfile source copying.

## [3.10.0] - 2026-06-12

### Added

- Added char and enum system-call support.
- Added parsing validation helpers and documentation.
- Added additional native framework APIs, including cryptographic helpers.
- Added notary coverage to the test suite.

### Changed

- Modernized system-call implementations and expanded XML documentation.
- Added property support for `SafeAttribute`.
- Improved published `nccs` framework reference resolution.

## [3.9.1] - 2026-01-28

### Changed

- See the canonical [v3.9.1 release notes](https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.9.1).

## [3.9.0] - 2026-01-28

### Changed

- See the canonical [v3.9.0 release notes](https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.9.0).

## [3.8.1] - 2025-05-12

### Changed

- See the canonical [v3.8.1 release](https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.8.1).

[Unreleased]: https://github.com/neo-project/neo-devpack-dotnet/compare/v3.10.1...master-n3
[3.10.1]: https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.10.1
[3.10.0]: https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.10.0
[3.9.1]: https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.9.1
[3.9.0]: https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.9.0
[3.8.1]: https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v3.8.1
