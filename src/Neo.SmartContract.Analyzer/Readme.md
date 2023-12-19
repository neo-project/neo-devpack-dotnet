# Neo Smart Contract Roslyn Analyzers

This repository contains a set of Roslyn analyzers and code fix providers.

## Content
### NeoContractAnalyzer
A set of neo smart contract syntax analyzers. **To see the effects of these analyzers in the IDE, you must build this project.**

- [BigIntegerCreationAnalyzer.cs](NeoContractAnalyzer/BigIntegerCreationAnalyzer.cs): This analyzer checks for creation patterns of the `BigInteger` class that are not supported.
- [BigIntegerUsageAnalyzer.cs](NeoContractAnalyzer/BigIntegerUsageAnalyzer.cs): This analyzer checks for specific methods of the `BigInteger` class, like `Add` and `Multiply`, and reports if they are used.
- [CharMethodsUsageAnalyzer.cs](NeoContractAnalyzer/CharMethodsUsageAnalyzer.cs): An analyzer that reports specific methods of the `char` class that are not recommended for use in smart contracts.
- [CollectionTypesUsageAnalyzer.cs](NeoContractAnalyzer/CollectionTypesUsageAnalyzer.cs): This analyzer flags the usage of unsupported collection types like `List` and `Dictionary`.
- [DecimalUsageAnalyzer.cs](NeoContractAnalyzer/DecimalUsageAnalyzer.cs): This analyzer checks for specific methods of the `decimal` class, like `Add` and `Multiply`, and reports if they are used.
- [DoubleUsageAnalyzer.cs](NeoContractAnalyzer/DoubleUsageAnalyzer.cs): This analyzer checks for specific methods of the `double` class, like `Add` and `Multiply`, and reports if they are used.
- [LinqUsageAnalyzer.cs](NeoContractAnalyzer/LinqUsageAnalyzer.cs): An analyzer that flags the usage of LINQ methods which are not supported in smart contracts.
- [OutKeywordUsageAnalyzer.cs](NeoContractAnalyzer/OutKeywordUsageAnalyzer.cs): An analyzer that flags the usage of the `out` keyword, which might not be supported in smart contracts.
- [RefKeywordUsageAnalyzer.cs](NeoContractAnalyzer/RefKeywordUsageAnalyzer.cs): An analyzer that flags the usage of the `ref` keyword, which might not be supported in smart contracts.
- [StringMethodUsageAnalyzer.cs](NeoContractAnalyzer/StringMethodUsageAnalyzer.cs): An analyzer that identifies and reports specific methods of the `string` class, such as `Substring` and `Join`.
- [SystemMathUsageAnalyzer.cs](NeoContractAnalyzer/SystemMathUsageAnalyzer.cs): An analyzer that flags certain `System.Math` method calls, such as `Math.Pow`, for being unsupported or deprecated.
- [SmartContractMethodNamingAnalyzer.cs](NeoContractAnalyzer/SmartContractMethodNamingAnalyzer.cs): An analyzer that flags smart contract methods that do not follow the naming convention.
### NeoContractAnalyzer.Sample
A sample project that references the NeoContract analyzers. Notice the `ProjectReference` parameters in [NeoContractAnalyzer.Sample.csproj](../NeoContractAnalyzer.Sample/NeoContractAnalyzer.Sample.csproj), which ensure that the project is referenced as a set of analyzers.

### NeoContractAnalyzer.Tests
Unit tests for the sample analyzers. Unit testing is a highly recommended approach when developing language-related features, as it allows for precise and controlled testing scenarios.

## How To?
### How to debug?
- Use the [launchSettings.json](Properties/launchSettings.json) profile for setting up the debugging environment.
- Debug the unit tests to step through the analyzer code.

### How to add a new analyzer?
- Create a new analyzer class that inherits from `DiagnosticAnalyzer`.
- Add the new analyzer to the [Analyzers](Analyzers) folder.

### How to use the analyzers?
- Build the [NeoContractAnalyzer](NeoContractAnalyzer/NeoContractAnalyzer.csproj) project.
- Add the [NeoContractAnalyzer](NeoContractAnalyzer/NeoContractAnalyzer.csproj) project as a reference to your project.
