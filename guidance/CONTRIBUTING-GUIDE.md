# Contributing to NEO DevPack for .NET

Thank you for your interest in contributing to the NEO DevPack for .NET! This document provides guidelines and information for contributors.

## 🤝 How to Contribute

There are many ways to contribute to this project:

- 🐛 **Report bugs** - Help us identify and fix issues
- 💡 **Suggest features** - Propose new functionality or improvements
- 📚 **Improve documentation** - Make our docs clearer and more comprehensive
- 💻 **Submit code** - Fix bugs, implement features, or add examples
- 🧪 **Write tests** - Improve test coverage and quality
- 🎓 **Create examples** - Help others learn with practical examples
- 💬 **Help others** - Answer questions in issues and discussions

## 🚀 Getting Started

### Prerequisites

See the [Getting Started Guide](GETTING-STARTED-GUIDE.md#prerequisites) for complete setup requirements including hardware specifications and development environment setup.

### Development Setup

1. **Fork the repository**
   ```bash
   # Click the "Fork" button on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/neo-devpack-dotnet.git
   cd neo-devpack-dotnet
   ```

2. **Initialize submodules**
   ```bash
   git submodule update --init --recursive
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Create a branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/your-bug-fix
   ```

## 📋 Issue Guidelines

### Before Creating an Issue

1. **Search existing issues** to avoid duplicates
2. **Check the documentation** - your question might already be answered
3. **Use the latest version** - ensure you're using the current release

### Creating Good Issues

- **Use issue templates** - They help provide necessary information
- **Be specific** - Provide clear, detailed descriptions
- **Include context** - Version numbers, OS, error messages, code samples
- **One issue per topic** - Don't combine multiple unrelated issues

### Issue Labels

We use labels to categorize and prioritize issues:

- `bug` - Something isn't working
- `enhancement` - New feature or improvement
- `documentation` - Documentation related
- `example` - Example related
- `good first issue` - Good for newcomers
- `help wanted` - We'd love community help
- `needs-triage` - Needs initial review

## 🔄 Development Workflow

### Branch Strategy

We use a structured branching model to maintain code quality:

- `master` - Production-ready code, stable releases
- `develop` - Integration branch for features
- `feature/*` - New features (e.g., `feature/add-nep24-support`)
- `fix/*` - Bug fixes (e.g., `fix/storage-overflow`)
- `docs/*` - Documentation updates (e.g., `docs/update-examples`)
- `security/*` - Security patches (handled with priority)

### Commit Message Format

Follow the conventional commits specification:

```
type(scope): subject

body (optional)

footer (optional)
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code changes that neither fix bugs nor add features
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(compiler): add support for LINQ expressions

Implement LINQ to method call transformation for common operations
like Select, Where, and OrderBy.

Closes #123
```

```
fix(framework): prevent integer overflow in token transfer

Add SafeMath checks to ensure amount calculations don't overflow
when processing large token transfers.
```

### Pull Request Workflow

1. **Create feature branch** from `develop`
2. **Make changes** following coding standards
3. **Commit** with descriptive messages
4. **Push** to your fork
5. **Create PR** against `develop` branch
6. **Address reviews** promptly
7. **Squash commits** if requested
8. **Merge** after approval

## 🔧 Development Guidelines

### Code Style

- **Follow C# conventions** - Use standard C# coding conventions
- **Be consistent** - Match the existing codebase style
- **Use meaningful names** - Variables, methods, and classes should be descriptive
- **Comment wisely** - Explain why, not what

### Smart Contract Guidelines

- **Security first** - Always consider security implications
- **Gas efficiency** - Optimize for gas consumption
- **Follow NEO standards** - Implement NEP standards correctly
- **Comprehensive testing** - Include thorough unit tests
- **Clear documentation** - Document all public methods and complex logic

### Testing Requirements

- **Unit tests** for all new functionality
- **Integration tests** for complex features
- **Example tests** for all examples
- **Security tests** for security-critical code
- **Performance tests** where appropriate

### Performance Guidelines

- **Benchmark critical paths** before optimization
- **Document gas consumption** for public methods
- **Include performance tests** for gas-intensive operations
- **Profile memory usage** in test environment
- **Optimize storage access** patterns
- **Minimize external contract calls**
- **Use batch operations** where possible

Example performance documentation:
```csharp
/// <summary>
/// Transfers tokens between accounts
/// Gas consumption: ~0.005 GAS for single transfer
/// </summary>
public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount) { }
```

### Documentation Standards

- **Clear and concise** - Easy to understand for the target audience
- **Complete** - Cover all important aspects
- **Up-to-date** - Keep documentation current with code changes
- **Examples included** - Provide practical, working examples
- **Properly formatted** - Use consistent Markdown formatting

### Dependency Management

Follow these practices for dependency management:

- **Keep dependencies current** with regular security updates
- **Scan for vulnerabilities** using `dotnet list package --vulnerable`
- **Document changes** in PR descriptions
- **Minimize external dependencies** to reduce attack surface
- **Lock versions** for production deployments

Quick commands:
```bash
dotnet list package --outdated          # Check outdated packages
dotnet list package --vulnerable        # Security scan
dotnet add package <name> --version X.Y.Z  # Specific version install
```

## 📝 Pull Request Process

### Before Submitting

1. **Update documentation** - Include relevant doc updates
2. **Add/update tests** - Ensure good test coverage
3. **Run all tests** - Make sure everything passes
4. **Check code style** - Follow project conventions
5. **Update examples** - If your changes affect examples

### Pull Request Guidelines

1. **Use descriptive titles** - Clearly describe what the PR does
2. **Fill out the template** - Provide all requested information
3. **Link related issues** - Reference any related issues
4. **Small, focused changes** - Easier to review and merge
5. **Update changelog** - Add entry for significant changes

### Review Process

1. **Automated checks** - CI/CD pipeline must pass
2. **Code review** - At least one maintainer review required
3. **Testing** - Manual testing for complex changes
4. **Documentation review** - For documentation changes
5. **Security review** - For security-critical changes

## 📚 Contributing Examples

Examples are crucial for helping developers learn. Here's how to contribute examples:

### Example Categories

- **Beginner** - Basic concepts and simple contracts
- **Intermediate** - More complex patterns and interactions
- **Advanced** - Sophisticated features and optimizations
- **Token Standards** - NEP implementations
- **Specialized** - Domain-specific applications

### Example Requirements

1. **Complete implementation** - Working, compilable code
2. **Comprehensive tests** - Full test coverage
3. **Clear documentation** - README with explanation and usage
4. **Best practices** - Demonstrate proper patterns
5. **Security considerations** - Address potential vulnerabilities

### Example Structure

```
ExampleName/
├── ExampleContract.cs           # Main contract
├── ExampleContract.csproj       # Project file
├── README.md                   # Documentation
└── ExampleName.UnitTests/      # Unit tests
    ├── ExampleTests.cs
    ├── ExampleName.UnitTests.csproj
    └── TestingArtifacts/       # Generated test artifacts
```

## 🔒 Security

### Reporting Security Issues

**DO NOT** create public issues for security vulnerabilities. Instead:

1. **Use GitHub Security Advisories** - Report privately through GitHub
2. **Email the team** - Contact maintainers directly
3. **Provide details** - Include reproduction steps and impact assessment

### Security Review Process

All security-related changes undergo additional review:

1. **Security-focused code review**
2. **Threat modeling** for new features
3. **Security testing** with various attack scenarios
4. **Documentation** of security considerations

## 🏷️ Release Process

### Versioning

We follow [Semantic Versioning](https://semver.org/):

- **Major** (X.0.0) - Breaking changes
- **Minor** (X.Y.0) - New features, backward compatible
- **Patch** (X.Y.Z) - Bug fixes, backward compatible

### Release Criteria

- All tests passing
- Documentation updated
- Security review completed
- Community feedback addressed
- Breaking changes documented

## 👥 Community

### Communication Channels

- **GitHub Issues** - Bug reports and feature requests
- **GitHub Discussions** - General questions and community discussions
- **Discord** - Real-time chat and community support
- **Twitter** - News and announcements

### Code of Conduct

We are committed to providing a welcoming and inclusive environment. Please:

- **Be respectful** - Treat everyone with kindness and respect
- **Be collaborative** - Work together constructively
- **Be patient** - Help others learn and grow
- **Be inclusive** - Welcome newcomers and diverse perspectives

### Recognition

We appreciate all contributors! Contributors are recognized through:

- **Contributors list** - Acknowledged in project documentation
- **Release notes** - Mentioned in release announcements
- **Special recognition** - For significant contributions

## ❓ Getting Help

### Documentation

- [Getting Started Guide](docs/getting-started.md)
- [Examples](examples/)
- [API Documentation](https://docs.neo.org/)

### Community Support

- [NEO Discord](https://discord.gg/rvZFQ5382k) - Join the #smart-contracts channel
- [GitHub Discussions](https://github.com/neo-project/neo-devpack-dotnet/discussions)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/neo-blockchain) - Tag with `neo-blockchain`

### Mentorship

New to open source or NEO development? We're here to help!

- Look for `good first issue` labels
- Ask questions in Discord or GitHub Discussions
- Join community calls and events
- Find a mentor in the community

## 📄 License

By contributing to this project, you agree that your contributions will be licensed under the same [MIT License](LICENSE) that covers the project.

---

Thank you for contributing to NEO DevPack for .NET! Together, we're building the future of blockchain development. 🚀