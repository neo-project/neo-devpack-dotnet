using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Enhanced fuzzer for dynamically generating and testing Neo N3 smart contracts.
    /// This class generates complete contracts with comprehensive coverage of Neo N3 features.
    /// </summary>
    public class EnhancedDynamicContractFuzzer
    {
        private readonly FragmentGenerator _fragmentGenerator;
        private readonly FeatureGenerator _featureGenerator;
        private readonly string _outputDirectory;
        private readonly Random _random;
        private readonly bool _testExecution;
        private readonly ContractCompiler _compiler;
        private readonly Dictionary<string, bool> _contractResults = new Dictionary<string, bool>();
        private readonly Dictionary<string, Exception> _contractErrors = new Dictionary<string, Exception>();

        // Feature categories for dynamic generation
        private readonly Dictionary<string, Func<string>> _basicFeatureGenerators;
        private readonly Dictionary<string, Func<string>> _advancedFeatureGenerators;
        private readonly Dictionary<string, Func<string>> _nepStandardGenerators;
        private readonly Dictionary<string, Func<string>> _nativeContractGenerators;

        /// <summary>
        /// Initialize the enhanced dynamic contract fuzzer
        /// </summary>
        /// <param name="outputDirectory">Directory for output files</param>
        /// <param name="testExecution">Whether to test execution of generated contracts</param>
        /// <param name="seed">Optional random seed for reproducible fuzzing</param>
        public EnhancedDynamicContractFuzzer(string outputDirectory = "EnhancedDynamicContractFuzzer", bool testExecution = true, int? seed = null)
        {
            _outputDirectory = outputDirectory;
            _testExecution = testExecution;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _fragmentGenerator = new FragmentGenerator(seed);
            _featureGenerator = new FeatureGenerator(_fragmentGenerator, seed);
            _compiler = new ContractCompiler(_outputDirectory);

            // Create output directory if it doesn't exist
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }

            // Initialize feature generators
            _basicFeatureGenerators = new Dictionary<string, Func<string>>
            {
                { "Variable Declaration", () => _fragmentGenerator.GeneratePrimitiveTypeDeclaration() },
                { "Complex Type Declaration", () => _fragmentGenerator.GenerateComplexTypeDeclaration() },
                { "Array Declaration", () => _fragmentGenerator.GenerateArrayDeclaration() },
                { "Arithmetic Expression", () => "int a = 10;\nint b = 20;\nint c = a + b;" },
                { "Conditional Expression", () => "bool condition = true;\nint result = condition ? 10 : 20;" },
                { "For Loop", () => _fragmentGenerator.GenerateForLoop() },
                { "While Loop", () => "int i = 0;\nwhile (i < 5) { i++; }" },
                { "If Statement", () => _fragmentGenerator.GenerateIfStatement() },
                { "Switch Statement", () => "int value = 1;\nswitch (value) { case 1: break; case 2: break; default: break; }" },
                { "Method Declaration", () => "public static int Add(int a, int b) { return a + b; }" },
                { "Method Call", () => _fragmentGenerator.GenerateNativeContractCall() }
            };

            _advancedFeatureGenerators = new Dictionary<string, Func<string>>
            {
                { "Exception Handling", _featureGenerator.GenerateExceptionHandling },
                { "Advanced Data Structures", _featureGenerator.GenerateAdvancedDataStructures },
                { "Cryptographic Operations", _featureGenerator.GenerateCryptographicOperations },
                { "Storage Patterns", _featureGenerator.GenerateStoragePatterns },
                { "Contract Callbacks", _featureGenerator.GenerateContractCallbacks },
                { "Contract Attributes", _featureGenerator.GenerateContractAttributes }
            };

            _nepStandardGenerators = new Dictionary<string, Func<string>>
            {
                { "NEP-11 Contract", _featureGenerator.GenerateNep11Contract },
                { "NEP-17 Contract", _featureGenerator.GenerateNep17Contract }
            };

            _nativeContractGenerators = new Dictionary<string, Func<string>>
            {
                { "NEO Native Contract", _featureGenerator.GenerateNeoNativeContractInteraction },
                { "GAS Native Contract", _featureGenerator.GenerateGasNativeContractInteraction },
                { "ContractManagement", _featureGenerator.GenerateContractManagementInteraction },
                { "Oracle", _featureGenerator.GenerateOracleInteraction }
            };
        }

        /// <summary>
        /// Run enhanced dynamic contract fuzzing tests
        /// </summary>
        /// <param name="iterations">Number of contracts to generate</param>
        /// <param name="featuresPerContract">Number of features per contract</param>
        /// <param name="includeNepStandards">Whether to include NEP standard contracts</param>
        /// <param name="includeNativeContracts">Whether to include native contract interactions</param>
        /// <returns>True if all tests passed</returns>
        public bool RunTests(int iterations = 10, int featuresPerContract = 5, bool includeNepStandards = true, bool includeNativeContracts = true)
        {
            Console.WriteLine("Running Enhanced Dynamic Contract Fuzzer tests...");
            Console.WriteLine($"Iterations: {iterations}");
            Console.WriteLine($"Features per contract: {featuresPerContract}");
            Console.WriteLine($"Include NEP standards: {includeNepStandards}");
            Console.WriteLine($"Include native contracts: {includeNativeContracts}");
            Console.WriteLine();

            bool overallSuccess = true;

            try
            {
                // Generate and test random contracts
                for (int i = 0; i < iterations; i++)
                {
                    Console.WriteLine($"Generating dynamic contract {i + 1}/{iterations}...");

                    try
                    {
                        string contractName = $"EnhancedContract{i + 1}";
                        bool success = GenerateAndTestSingleContract(contractName, featuresPerContract, includeNepStandards, includeNativeContracts);
                        overallSuccess &= success;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in contract generation {i + 1}: {ex.Message}");
                        overallSuccess = false;
                    }
                }

                // Generate NEP standard contracts separately
                if (includeNepStandards)
                {
                    Console.WriteLine("Generating NEP standard contracts...");

                    try
                    {
                        bool nep11Success = GenerateAndTestNepStandardContract("Nep11Contract", "NEP-11 Contract");
                        overallSuccess &= nep11Success;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in NEP-11 contract generation: {ex.Message}");
                        overallSuccess = false;
                    }

                    try
                    {
                        bool nep17Success = GenerateAndTestNepStandardContract("Nep17Contract", "NEP-17 Contract");
                        overallSuccess &= nep17Success;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in NEP-17 contract generation: {ex.Message}");
                        overallSuccess = false;
                    }
                }

                // Generate and save summary report
                string reportPath = Path.Combine(_outputDirectory, "EnhancedDynamicContractFuzzerReport.md");
                File.WriteAllText(reportPath, GenerateSummaryReport());

                Console.WriteLine($"Enhanced Dynamic Contract Fuzzer tests completed. Success: {overallSuccess}");
                Console.WriteLine($"Report saved to: {reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running tests: {ex.Message}");
                overallSuccess = false;
            }

            return overallSuccess;
        }

        /// <summary>
        /// Generate and test a single contract
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="featuresPerContract">Number of features to include</param>
        /// <param name="includeNepStandards">Whether to include NEP standard features</param>
        /// <param name="includeNativeContracts">Whether to include native contract interactions</param>
        /// <returns>True if the contract was successfully generated and tested</returns>
        public bool GenerateAndTestSingleContract(string contractName, int featuresPerContract, bool includeNepStandards, bool includeNativeContracts)
        {
            try
            {
                // Generate the contract
                string contractCode = GenerateDynamicContract(contractName, featuresPerContract, includeNepStandards, includeNativeContracts);
                string filePath = Path.Combine(_outputDirectory, $"{contractName}.cs");

                // Save the contract
                File.WriteAllText(filePath, contractCode);
                Console.WriteLine($"Contract saved to: {filePath}");

                // Compile and test the contract
                bool success = CompileAndTestContract(filePath, contractName);

                // Track the result
                _contractResults[contractName] = success;

                Console.WriteLine($"Contract {contractName} {(success ? "succeeded" : "failed")}");

                return success;
            }
            catch (Exception ex)
            {
                _contractErrors[contractName] = ex;
                Console.WriteLine($"Error in contract {contractName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generate and test a NEP standard contract
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="standardType">Type of NEP standard</param>
        /// <returns>True if the contract was successfully generated and tested</returns>
        public bool GenerateAndTestNepStandardContract(string contractName, string standardType)
        {
            try
            {
                // Get the NEP standard generator
                if (!_nepStandardGenerators.TryGetValue(standardType, out var generator))
                {
                    throw new ArgumentException($"Unknown NEP standard type: {standardType}");
                }

                // Generate the contract
                string contractCode = generator();

                // Use InputChunker to break large methods into smaller ones if needed
                contractCode = InputChunker.ChunkContractMethods(contractCode);

                string filePath = Path.Combine(_outputDirectory, $"{contractName}.cs");

                // Save the contract
                File.WriteAllText(filePath, contractCode);
                Console.WriteLine($"NEP standard contract saved to: {filePath}");

                // Compile and test the contract
                bool success = CompileAndTestContract(filePath, contractName);

                // Track the result
                _contractResults[contractName] = success;

                Console.WriteLine($"NEP standard contract {contractName} {(success ? "succeeded" : "failed")}");

                return success;
            }
            catch (Exception ex)
            {
                _contractErrors[contractName] = ex;
                Console.WriteLine($"Error in NEP standard contract {contractName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generate a dynamic contract with random features
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="featureCount">Number of features to include</param>
        /// <param name="includeNepStandards">Whether to include NEP standard features</param>
        /// <param name="includeNativeContracts">Whether to include native contract interactions</param>
        /// <returns>Generated contract code</returns>
        private string GenerateDynamicContract(string contractName, int featureCount, bool includeNepStandards, bool includeNativeContracts)
        {
            // Combine all feature generators based on inclusion flags
            var allFeatureGenerators = new Dictionary<string, Func<string>>(_basicFeatureGenerators);

            // Add advanced features
            foreach (var feature in _advancedFeatureGenerators)
            {
                allFeatureGenerators.Add(feature.Key, feature.Value);
            }

            // Add native contract features if requested
            if (includeNativeContracts)
            {
                foreach (var feature in _nativeContractGenerators)
                {
                    allFeatureGenerators.Add(feature.Key, feature.Value);
                }
            }

            // Use InputChunker to break features into manageable groups if there are too many
            if (allFeatureGenerators.Count > featureCount * 2)
            {
                var featureGroups = InputChunker.ChunkFeatures(allFeatureGenerators, featureCount * 2);
                // Select a random group of features to use
                allFeatureGenerators = featureGroups[_random.Next(featureGroups.Count)];
            }

            // Select random features
            List<string> selectedFeatures = new List<string>();
            List<string> availableFeatures = new List<string>(allFeatureGenerators.Keys);

            for (int i = 0; i < featureCount; i++)
            {
                if (availableFeatures.Count == 0)
                {
                    break;
                }

                int index = _random.Next(availableFeatures.Count);
                selectedFeatures.Add(availableFeatures[index]);
                availableFeatures.RemoveAt(index);
            }

            // Generate code fragments for each selected feature
            List<string> fragments = new List<string>();
            foreach (string feature in selectedFeatures)
            {
                // Get the raw fragment
                string rawFragment = allFeatureGenerators[feature]();

                // Indent each line with 16 spaces (4 tabs)
                string[] lines = rawFragment.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        lines[i] = "                " + lines[i];
                    }
                }

                // Add the feature comment and the indented fragment
                fragments.Add($"                // {feature}\n{string.Join("\n", lines)}");
            }

            // Generate the contract using the template
            string contractCode = GenerateContractFromTemplate(contractName, fragments);

            // Use InputChunker to break large methods into smaller ones if needed
            return InputChunker.ChunkContractMethods(contractCode);
        }

        /// <summary>
        /// Generate a contract from the template
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="fragments">Code fragments to include</param>
        /// <returns>Complete contract code</returns>
        private string GenerateContractFromTemplate(string contractName, List<string> fragments)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using Neo.SmartContract.Framework;");
            sb.AppendLine("using Neo.SmartContract.Framework.Attributes;");
            sb.AppendLine("using Neo.SmartContract.Framework.Native;");
            sb.AppendLine("using Neo.SmartContract.Framework.Services;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Numerics;");
            sb.AppendLine();
            sb.AppendLine("namespace Neo.Compiler.Fuzzer.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    [DisplayName(\"" + contractName + "\")]");
            sb.AppendLine("    [ContractDescription(\"A generated Neo N3 smart contract for testing\")]");
            sb.AppendLine("    [ContractAuthor(\"Neo Compiler Fuzzer\", \"dev@neo.org\")]");
            sb.AppendLine("    [ContractVersion(\"1.0.0\")]");
            sb.AppendLine("    [ContractPermission(\"*\", \"*\")]");
            sb.AppendLine("    public class " + contractName + " : SmartContract");
            sb.AppendLine("    {");
            sb.AppendLine("        // Contract hash for self-reference");
            sb.AppendLine("        [ContractHash]");
            sb.AppendLine("        public static extern UInt160 Hash { get; }");
            sb.AppendLine();
            sb.AppendLine("        // Events for testing");
            sb.AppendLine("        [DisplayName(\"TestCompleted\")]");
            sb.AppendLine("        public static event Action<string, bool> OnTestCompleted;");
            sb.AppendLine();
            sb.AppendLine("        // Main method");
            sb.AppendLine("        public static object Main(string operation, object[] args)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (operation == \"test\")");
            sb.AppendLine("            {");
            sb.AppendLine("                return Test();");
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // Test method");
            sb.AppendLine("        public static bool Test()");
            sb.AppendLine("        {");
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                // Generated code fragments");

            // Add all fragments
            foreach (string fragment in fragments)
            {
                sb.AppendLine();
                sb.AppendLine(fragment);
            }

            sb.AppendLine();
            sb.AppendLine("                // Emit success event");
            sb.AppendLine("                OnTestCompleted?.Invoke(\"Test completed successfully\", true);");
            sb.AppendLine("                return true;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                // Emit failure event");
            sb.AppendLine("                OnTestCompleted?.Invoke(ex.Message, false);");
            sb.AppendLine("                return false;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Compile and test a contract
        /// </summary>
        /// <param name="filePath">Path to the contract file</param>
        /// <param name="contractName">Name of the contract</param>
        /// <returns>True if compilation and testing succeeded</returns>
        private bool CompileAndTestContract(string filePath, string contractName)
        {
            try
            {
                // Compile the contract
                var result = _compiler.Compile(filePath);

                if (!result.Success)
                {
                    Console.WriteLine($"Compilation failed for {contractName}:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"  - {error}");
                    }
                    return false;
                }

                Console.WriteLine($"Compilation succeeded for {contractName}");

                // Test execution if enabled
                if (_testExecution)
                {
                    var executionResult = _compiler.TestExecution(result);
                    Console.WriteLine($"Execution test {(executionResult.Success ? "succeeded" : "failed")} for {contractName}");
                    return executionResult.Success;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error compiling/testing {contractName}: {ex.Message}");
                _contractErrors[contractName] = ex;
                return false;
            }
        }

        /// <summary>
        /// Generate a summary report of the fuzzing results
        /// </summary>
        /// <returns>Report content</returns>
        private string GenerateSummaryReport()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# Enhanced Dynamic Contract Fuzzer Report");
            sb.AppendLine();
            sb.AppendLine($"Generated on: {DateTime.Now}");
            sb.AppendLine();

            // Summary statistics
            int totalContracts = _contractResults.Count;
            int successfulContracts = _contractResults.Count(r => r.Value);
            int failedContracts = totalContracts - successfulContracts;

            sb.AppendLine("## Summary");
            sb.AppendLine();
            sb.AppendLine($"- Total contracts generated: {totalContracts}");
            sb.AppendLine($"- Successfully compiled and tested: {successfulContracts}");
            sb.AppendLine($"- Failed: {failedContracts}");
            sb.AppendLine($"- Success rate: {(totalContracts > 0 ? (double)successfulContracts / totalContracts * 100 : 0):F2}%");
            sb.AppendLine();

            // Contract details
            sb.AppendLine("## Contract Details");
            sb.AppendLine();
            sb.AppendLine("| Contract | Status | Error |");
            sb.AppendLine("|----------|--------|-------|");

            foreach (var result in _contractResults.OrderBy(r => r.Key))
            {
                string status = result.Value ? "✅ Success" : "❌ Failed";
                string error = _contractErrors.TryGetValue(result.Key, out var ex) ? ex.Message : "";

                sb.AppendLine($"| {result.Key} | {status} | {error} |");
            }

            return sb.ToString();
        }
    }
}
