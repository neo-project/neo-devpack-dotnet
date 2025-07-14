# Changelog

All notable changes to the R3E DevPack will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Advanced WebGUI analytics and usage metrics
- Theme marketplace for custom WebGUI designs
- GraphQL API for advanced integrations
- Multi-network deployment automation
- Plugin marketplace and distribution system

## [0.0.2] - 2025-01-14

### Added
- **R3E WebGUI Service**: Complete hosting service for Neo smart contract web interfaces
  - JSON-based configuration system replacing static HTML files
  - Automatic WebGUI generation from contract manifests
  - Signature-based authentication without user registration
  - Multi-wallet support (NeoLine, O3, WalletConnect)
  - Plugin upload and distribution with integrity validation
  - Subdomain routing for contract interfaces
  - Professional, responsive design templates
  - Real-time blockchain data integration
  - Docker containerization with production configurations
  - Comprehensive API with rate limiting and security headers
  - Complete documentation and deployment guides

- **R3E WebGUI Deploy Tool**: CLI tool for WebGUI deployment automation
  - Command-line interface for WebGUI management
  - Signature-based deployment workflow
  - Integration with existing development tools

- **Enhanced Testing Framework**
  - Unit tests for all WebGUI service components
  - Integration tests for API endpoints
  - Production workflow validation scripts
  - Comprehensive test coverage reporting

- **Documentation**
  - Complete setup and deployment guides
  - Signature authentication documentation
  - Production deployment checklist
  - Docker deployment configurations
  - Troubleshooting guides
  - API reference documentation

### Enhanced
- Improved contract compilation error handling
- Enhanced NEP-17 token template generation
- Better integration between compiler and WebGUI service
- Expanded test coverage across all projects

### Technical Details
- ASP.NET Core 9.0 web service
- Entity Framework Core with SQL Server
- Nginx reverse proxy configuration
- Docker multi-stage builds
- Clean architecture implementation
- Comprehensive logging and monitoring
- Rate limiting and security middleware

## [0.0.1] - 2025-01-13

### Added
- **R3E Compiler.CSharp**: Neo smart contract compiler with R3E branding
  - Complete rebranding from Neo.Compiler.CSharp
  - Maintained full compatibility with Neo ecosystem
  - Enhanced error handling and validation
  - Support for latest Neo 3.8.x features

- **R3E SmartContract Framework**: Framework for R3E smart contract development
  - Rebranded from Neo.SmartContract.Framework
  - Full compatibility with existing Neo contracts
  - Enhanced documentation and examples
  - Support for modern C# language features

- **Package Publishing Pipeline**
  - GitHub Actions workflow for automated package publishing
  - NuGet package generation for all R3E components
  - Symbol packages for enhanced debugging
  - Automated version management

- **Comprehensive Testing**
  - Full unit test suite with updated branding
  - Integration tests for compiler functionality
  - Compatibility tests with Neo ecosystem
  - Performance benchmarking

- **Documentation**
  - Updated README with R3E branding
  - Migration guide from Neo DevPack
  - Contribution guidelines
  - Development setup instructions

### Changed
- Rebranded all packages from `Neo.*` to `R3E.*` namespace
- Updated assembly names and package identifiers
- Modified package descriptions and metadata
- Updated documentation to reflect R3E community focus

### Technical Details
- .NET 9.0 target framework
- Neo 3.8.2 compatibility
- Maintained API compatibility for seamless migration
- Enhanced build and packaging scripts

## Migration Guide

### From Neo DevPack to R3E DevPack v0.0.1
- Replace `Neo.Compiler.CSharp` with `R3E.Compiler.CSharp`
- Replace `Neo.SmartContract.Framework` with `R3E.SmartContract.Framework`
- Update using statements and namespaces
- No code changes required - full API compatibility maintained

### Upgrading to v0.0.2
- No breaking changes - fully backward compatible
- Optional: Deploy WebGUIs for existing contracts using the new service
- Optional: Use the new deployment tools for enhanced workflow

## Support

- **Issues**: [GitHub Issues](https://github.com/neo-project/neo-devpack-dotnet/issues)
- **Discussions**: [GitHub Discussions](https://github.com/neo-project/neo-devpack-dotnet/discussions)
- **Documentation**: Available in each project's README and docs folder
- **Community**: Join the Neo developer community for support

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details on how to get started.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.