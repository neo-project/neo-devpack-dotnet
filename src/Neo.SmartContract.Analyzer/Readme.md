# Neo Smart Contract Roslyn Analyzers

This package contains a comprehensive set of Roslyn analyzers and code fix providers for Neo smart contracts. These analyzers help developers write better, more secure, and more efficient smart contracts by detecting common issues and providing automated fixes.

## Analyzer Categories

### Type Safety Analyzers
- **FloatUsageAnalyzer** (NC4002): Detects usage of float type, which is not supported in Neo smart contracts
- **DecimalUsageAnalyzer** (NC4003): Detects usage of decimal type, which is not supported in Neo smart contracts
- **DoubleUsageAnalyzer** (NC4004): Detects usage of double type, which is not supported in Neo smart contracts
- **NullableTypeUsageAnalyzer** (NC4033): Flags usage of nullable types that may cause issues in smart contracts

### Method Usage Analyzers
- **SystemMathUsageAnalyzer** (NC4005): Flags usage of System.Math methods that are not supported in Neo smart contracts
- **BigIntegerUsageAnalyzer** (NC4006): Checks for specific BigInteger methods that are not supported
- **StringMethodUsageAnalyzer** (NC4007): Identifies string methods that are not supported in smart contracts
- **CharMethodsUsageAnalyzer** (NC4012): Reports char methods that are not recommended for smart contracts
- **ArrayMethodsUsageAnalyzer** (NC4016): Detects usage of unsupported array methods
- **EnumMethodsUsageAnalyzer** (NC4025): Flags unsupported enum methods
- **NumericMethodsUsageAnalyzer** (NC4029): Checks for unsupported numeric methods

### Usage Pattern Analyzers
- **BigIntegerCreationAnalyzer** (NC4008): Checks for unsupported BigInteger creation patterns
- **RefKeywordUsageAnalyzer** (NC4010): Flags usage of the 'ref' keyword
- **LinqUsageAnalyzer** (NC4011): Detects LINQ usage which is not supported in smart contracts
- **CollectionTypesUsageAnalyzer** (NC4013): Flags usage of unsupported collection types like List and Dictionary
- **VolatileKeywordUsageAnalyzer** (NC4014): Warns about 'volatile' keyword usage
- **KeywordUsageAnalyzer** (NC4015): Detects usage of restricted keywords
- **BanCastMethodAnalyzer** (NC4017): Flags usage of certain cast methods that are not supported
- **BigIntegerUsingUsageAnalyzer** (NC4022): Warns about incorrect BigInteger usage in using statements
- **StaticFieldInitializationAnalyzer** (NC4023): Checks for proper static field initialization
- **MultipleCatchBlockAnalyzer** (NC4024): Checks for multiple catch blocks in try statements
- **SystemDiagnosticsUsageAnalyzer** (NC4026): Detects System.Diagnostics usage
- **CatchOnlySystemExceptionAnalyzer** (NC4027): Ensures only System.Exception is caught
- **BitOperationsUsageAnalyzer** (NC4028): Flags unsupported bit operations

### Smart Contract Specific Analyzers
- **SmartContractMethodNamingAnalyzer** (NC4018): Ensures methods follow correct naming conventions
- **NotifyEventNameAnalyzer** (NC4019): Checks for correct event names in notify calls
- **SmartContractMethodNamingAnalyzerUnderline** (NC4020): Warns about method names with underscores
- **SupportedStandardsAnalyzer** (NC4021): Checks for correct implementation of supported standards
- **SafeAttributeAnalyzer** (NC4034): Validates usage of Safe attribute
- **EventRegistrationAnalyzer** (NC4036): Checks for proper event registration
- **CheckWitnessUsageAnalyzer** (NC4037): Validates CheckWitness usage patterns
- **ContractAttributeAnalyzer** (NC4046/NC4047): Validates contract attributes and suggests missing ones

### NEP Standard Compliance Analyzers
- **NEP17ComplianceAnalyzer** (NC4030): Ensures NEP-17 token standard compliance
- **NEP11ComplianceAnalyzer** (NC4031): Ensures NEP-11 NFT standard compliance
- **NEP26ComplianceAnalyzer** (NC4032): Ensures NEP-26 standard compliance

### Security Analyzers
- **ReentrancyPatternAnalyzer** (NC4035): Detects potential reentrancy vulnerabilities

### Performance Analyzers
- **GasOptimizationAnalyzer** (NC4039-NC4041): Detects patterns that may consume excessive gas:
  - Nested loops
  - Large string literals
  - Unnecessary type conversions

### Utility Analyzers
- **InitialValueAnalyzer** (NC4009): Suggests converting attribute-based initializations to literals
- **NepStandardAnalyzer** (NC4038): Validates NEP standard usage

## How to Use

To use these analyzers in your Neo smart contract project:

1. Add a reference to the Neo.SmartContract.Analyzer project in your smart contract project.
2. Build the Neo.SmartContract.Analyzer project.
3. The analyzers will automatically run when you build your smart contract project.

## Contributing

Contributions to improve existing analyzers or add new ones are welcome. Please submit a pull request with your changes.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
