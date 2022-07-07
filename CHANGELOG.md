# Neo Test Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

This project uses [NerdBank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning)
to manage version numbers. This tool automatically sets the Semantic Versioning Patch
value based on the [Git height](https://github.com/dotnet/Nerdbank.GitVersioning#what-is-git-height)
of the commit that generated the build. As such, released versions of these packages
will not have contiguous patch numbers. Initial major and minor releases will be documented
in this file without a patch number. Patch version will be included for bug fixes, but
may not exactly match a publicly released version.

## [3.3.23] - 2022-07-06

### Added
* Adds `ContractNameOverride` property to `NeoContractReference` items to control the name of the generated contract interface (#35)

### Changed
* Pass .csproj file instead of .cs sources to NeoCsc task due to [existing nccs issue](https://github.com/neo-project/neo-devpack-dotnet/issues/759) (#34)

### Fixed
* `NeoContractInterface` fails if generated contract interface name isn't a valid C# type name (#33)
* Test projects that specify `NeoContractReference` items correctly build referenced projects first (#35)


## [3.3] - 2022-06-28

### Added

* NeoCsc and NeoExpressBatch MSBuild tasks (plus .targets file)
* Abstract DotNetToolTask for invoking dotnet tools installed globally or locally
  * Added ValidateVersion virtual to DotNetToolTask (#25)
* `ScriptBuilder.EmitContractCall` extension methods
* `NativeContracts` static class 
  * `NeoToken` and `GasToken` contract hashes
* `Nep17Token` and `NeoToken` contract interfaces for use with `NeoTestHarness`

### Changed

* Use ContractParameterParser.ConvertObject to convert object instances to ContractParameter in NeoTestHarness.Extensions.CreateScript (#20)

## [3.1.10] - 2021-12-14

### Changed

* Update to Neo 3.1.0, target framework net6.0 and language version 10
* Update to BlockchainToolkitLibrary 3.1.21
* `CheckpointFixture.ProtocolSettings` changed from read only field to get only property.

### Added

* Neo.Test.Runner tool (#17)
* `CheckpointFixture.CheckpointStore` get only property.

### Removed

* `CheckpointFixture.GetStorageProvider` method.

## [3.0.4] - 2021-10-12

### Changed

* Update dependencies for Neo 3.0.3 release

## [3.0.3] - 2021-08-06

### Changed

* Update dependencies for Neo 3.0.2 release

## [3.0] - 2021-08-02

### Changed

* Neo N3 release support
* Bumped major version to 3 for consistency with Neo N3 release
* Update dependencies

## [1.0.40-preview] - 2021-07-21

### Changed

* Neo N3 RC4 support
* handle Array, Map, Signature and InteropInterface parameter types in ContractGenerator 
* Update dependencies

### Added

* add StackItemAssertions BeEquivalentTo ReadOnlySpan<byte> overload (#14)


## [1.0.37-preview] - 2021-06-15

### Changed

* Update Neo.BlockchainToolkit3 dependency
* Update GitHub + Azure Pipeline Build files

### Fixed

* Create test transactions for TestApplicationEngine (#33)

## [1.0.34-preview] - 2021-06-06

### Changed

* support DescriptionAttribute in GetContract<T>
* Update Neo.BlockchainToolkit3 dependency

## [1.0.32-preview] - 2021-06-04

### Changed

* Neo N3 RC3 support
* Move contract interface generation to new class (#12)

## [1.0.28-preview] - 2021-05-17

### Changed

* Update Neo.BlockchainToolkit3 dependency

### Fixed

* ensure length before accessing string char by index + test (#16) fixes https://github.com/neo-project/neo-express/issues/136

## [1.0.26-preview] - 2021-05-04

### Changed

* Neo N3 RC2 support
* MSBuild build targets 
  * `NeoExpressBatch` property triggers execution of `neoxp batch <batch file>`
  * `TargetNeoContractName` property triggers execution of `nccs <project file>`
  * `NeoContractReference` item triggers generation of contract interface from contract manifest

### Added

* Non-generic `ExecuteScript` and `GetContract` methods in Neo.Test.Harness package

## [1.0.19-preview] - 2021-03-18

### Changed

* Neo N3 RC1 support

## [1.0.6-preview] - 2021-02-08

Initial Release