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