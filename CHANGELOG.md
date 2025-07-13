# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.1] - 2025-07-13

### Added
- **R3E Neo Contract Compiler (rncc)**: Renamed compiler binary from `nccs` to `rncc` (r3e neo contract compiler)
- **Web GUI Generation**: Added `--generate-webgui` option to automatically generate interactive web interfaces for compiled contracts
  - Generates HTML, JavaScript, and CSS files for contract interaction
  - Includes method invocation interface, transaction history, and state monitoring
  - Supports wallet connection and real-time updates
- **Plugin Generation**: Complete implementation of `--generate-plugin` for Neo N3 plugin creation
  - Generates CLI commands for all contract methods
  - Creates contract wrapper classes for easy integration
  - Includes plugin configuration and project files
- **Enhanced Deployment Toolkit**: Production-ready deployment toolkit with simplified API
  - Support for mainnet, testnet, and local networks
  - Automatic wallet management
  - Comprehensive deployment options

### Changed
- Renamed all references from `nccs` to `rncc` throughout the codebase
- Updated compiler description to include "r3e neo contract compiler"
- Version reset to 0.0.1 for new release cycle

### Fixed
- Web GUI generation integration with CLI tool
- Plugin generation for contracts with complex parameter types
- Deployment toolkit error handling for network connectivity issues

### Technical Details
- Built on Neo devpack-dotnet framework
- Requires .NET 9.0 or higher
- Compatible with Neo N3 protocol

### Migration Guide
For users upgrading from previous versions:
1. Replace all `nccs` commands with `rncc`
2. Update build scripts to use the new binary name
3. Web GUI generation is now available with `--generate-webgui`
4. Plugin generation continues to work with `--generate-plugin`

[0.0.1]: https://github.com/neo-project/neo-devpack-dotnet/releases/tag/v0.0.1