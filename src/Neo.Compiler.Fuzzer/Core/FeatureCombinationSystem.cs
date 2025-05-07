using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Implements a systematic feature combination system for Neo N3 smart contract fuzzing.
    /// This class ensures that features can be validly combined with dependency tracking and context awareness.
    /// </summary>
    public class FeatureCombinationSystem
    {
        private readonly Random _random;
        private readonly FragmentGenerator _fragmentGenerator;
        private readonly FeatureGenerator _baseFeatureGenerator;
        private readonly EnhancedFeatureGenerator _enhancedFeatureGenerator;
        private readonly NeoSpecificTypeOperations _neoTypeOperations;
        private readonly CollectionTypeOperations _collectionOperations;

        // Track dependencies between features
        private readonly Dictionary<string, HashSet<string>> _featureDependencies;

        // Track declared variables for context awareness
        private readonly HashSet<string> _declaredVariables;

        // Track imported namespaces
        private readonly HashSet<string> _importedNamespaces;

        public FeatureCombinationSystem(FragmentGenerator fragmentGenerator, int? seed = null)
        {
            _fragmentGenerator = fragmentGenerator;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _baseFeatureGenerator = new FeatureGenerator(fragmentGenerator, seed);
            _enhancedFeatureGenerator = new EnhancedFeatureGenerator(fragmentGenerator, seed);
            _neoTypeOperations = new NeoSpecificTypeOperations(fragmentGenerator, seed);
            _collectionOperations = new CollectionTypeOperations(fragmentGenerator, seed);

            _featureDependencies = InitializeFeatureDependencies();
            _declaredVariables = new HashSet<string>();
            _importedNamespaces = new HashSet<string>
            {
                "System",
                "Neo.SmartContract.Framework"
            };
        }

        /// <summary>
        /// Initialize the dependencies between features
        /// </summary>
        private Dictionary<string, HashSet<string>> InitializeFeatureDependencies()
        {
            var dependencies = new Dictionary<string, HashSet<string>>
            {
                // Storage operations require Storage namespace
                { "Storage", new HashSet<string> { "Neo.SmartContract.Framework.Services" } },

                // Native contracts require their respective namespaces
                { "NEO", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "GAS", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "ContractManagement", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "CryptoLib", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "Ledger", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "Oracle", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "Policy", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "RoleManagement", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },
                { "StdLib", new HashSet<string> { "Neo.SmartContract.Framework.Native" } },

                // Runtime operations require Runtime namespace
                { "Runtime", new HashSet<string> { "Neo.SmartContract.Framework.Services" } },

                // Neo-specific types require their namespaces
                { "UInt160", new HashSet<string> { "Neo.SmartContract.Framework" } },
                { "UInt256", new HashSet<string> { "Neo.SmartContract.Framework" } },
                { "ECPoint", new HashSet<string> { "Neo.SmartContract.Framework" } },
                { "ByteString", new HashSet<string> { "Neo.SmartContract.Framework" } },

                // Collection types require their namespaces
                { "List", new HashSet<string> { "Neo.SmartContract.Framework" } },
                { "Map", new HashSet<string> { "Neo.SmartContract.Framework" } }
            };

            return dependencies;
        }

        /// <summary>
        /// Shuffle a list and select a specified number of items
        /// </summary>
        private List<string> ShuffleAndSelect(List<string> items, int count)
        {
            // Fisher-Yates shuffle
            for (int i = items.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                string temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }

            return items.Take(count).ToList();
        }

        /// <summary>
        /// Add dependencies for selected features
        /// </summary>
        private void AddDependenciesForFeatures(List<string> features)
        {
            foreach (string feature in features)
            {
                if (_featureDependencies.TryGetValue(feature, out HashSet<string> dependencies))
                {
                    foreach (string dependency in dependencies)
                    {
                        _importedNamespaces.Add(dependency);
                    }
                }
            }
        }

        /// <summary>
        /// Generate code for a specific feature
        /// </summary>
        public string GenerateFeatureCode(string feature)
        {
            switch (feature)
            {
                case "Storage":
                    return _enhancedFeatureGenerator.GenerateComplexStorageOperations();

                case "NativeContracts":
                    return _enhancedFeatureGenerator.GenerateComprehensiveNativeContractCalls();

                case "Runtime":
                    return _baseFeatureGenerator.GenerateNeoRuntimeOperations();

                case "NeoTypes":
                    return _neoTypeOperations.GenerateNeoTypeOperations();

                case "Collections":
                    return _collectionOperations.GenerateCollectionOperation();

                case "Exceptions":
                    return _baseFeatureGenerator.GenerateExceptionHandling();

                case "ControlFlow":
                    return _baseFeatureGenerator.GenerateControlFlowStatements();

                case "Operators":
                    return _baseFeatureGenerator.GenerateOperatorExpressions();

                case "StringMath":
                    return _baseFeatureGenerator.GenerateStringAndMathOperations();

                default:
                    return "// Unknown feature: " + feature;
            }
        }

        /// <summary>
        /// Generate a valid combination of Neo N3 features
        /// </summary>
        public string GenerateFeatureCombination(int featureCount = 5)
        {
            StringBuilder sb = new StringBuilder();

            // Add required imports
            sb.AppendLine("using System;");
            sb.AppendLine("using Neo.SmartContract.Framework;");
            sb.AppendLine("using Neo.SmartContract.Framework.Services;");
            sb.AppendLine("using Neo.SmartContract.Framework.Native;");
            sb.AppendLine("using System.Numerics;");
            sb.AppendLine();

            // Select random features based on feature count
            List<string> availableFeatures = new List<string>
            {
                "Storage",
                "NativeContracts",
                "Runtime",
                "NeoTypes",
                "Collections",
                "Exceptions",
                "ControlFlow",
                "Operators",
                "StringMath"
            };

            // Shuffle and select features
            List<string> selectedFeatures = ShuffleAndSelect(availableFeatures, Math.Min(featureCount, availableFeatures.Count));

            // Add dependencies for selected features
            AddDependenciesForFeatures(selectedFeatures);

            // Generate contract structure
            sb.AppendLine("namespace Neo.Compiler.Fuzzer.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    [ManifestExtra(\"Author\", \"Neo.Compiler.Fuzzer\")]");
            sb.AppendLine("    [ManifestExtra(\"Email\", \"dev@neo.org\")]");
            sb.AppendLine("    [ManifestExtra(\"Description\", \"Dynamically generated contract for fuzzing\")]");
            sb.AppendLine("    public class DynamicContract : SmartContract");
            sb.AppendLine("    {");
            sb.AppendLine("        // Contract entry point");
            sb.AppendLine("        public static object Main(string operation, object[] args)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (operation == \"test\")");
            sb.AppendLine("            {");
            sb.AppendLine("                return TestFeatures();");
            sb.AppendLine("            }");
            sb.AppendLine("            return true;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static bool TestFeatures()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Generated feature combinations");
            sb.AppendLine("            try");
            sb.AppendLine("            {");

            // Generate code for each selected feature
            foreach (string feature in selectedFeatures)
            {
                string featureCode = GenerateFeatureCode(feature);
                sb.AppendLine("                " + featureCode.Replace("\n", "\n                "));
            }

            sb.AppendLine("                return true;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception e)");
            sb.AppendLine("            {");
            sb.AppendLine("                Runtime.Log(\"Error: \" + e.Message);");
            sb.AppendLine("                return false;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");

            // Close class and namespace
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate a complete contract with valid feature combinations
        /// </summary>
        public string GenerateCompleteContract(int complexity = 3)
        {
            // Select features based on complexity
            int featureCount = 2 + complexity; // 2-5 features based on complexity

            // Generate the full contract with feature combinations
            string featureCombination = GenerateFeatureCombination(featureCount);

            // The featureCombination already contains all necessary imports and contract structure
            // No need to add additional imports or validation code as they're already included

            return featureCombination;
        }

        /// <summary>
        /// Validate that a feature combination is valid
        /// </summary>
        public bool ValidateFeatureCombination(List<string> features)
        {
            // Check for incompatible feature combinations
            if (features.Contains("NEP11") && features.Contains("NEP17"))
            {
                return false; // Can't implement both token standards in one contract
            }

            // Ensure all dependencies are satisfied
            foreach (string feature in features)
            {
                if (_featureDependencies.TryGetValue(feature, out HashSet<string> dependencies))
                {
                    foreach (string dependency in dependencies)
                    {
                        if (!_importedNamespaces.Contains(dependency))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Generate a random feature based on the specified feature type
        /// </summary>
        public string GenerateRandomFeature(string featureType = null)
        {
            string[] featureTypes = {
                "Storage",
                "NativeContract",
                "Runtime",
                "NeoType",
                "Collection",
                "ControlFlow",
                "ExceptionHandling"
            };

            string selectedFeatureType = featureType ?? featureTypes[_random.Next(featureTypes.Length)];

            switch (selectedFeatureType)
            {
                case "Storage":
                    return _enhancedFeatureGenerator.GenerateComplexStorageOperations();
                case "NativeContract":
                    return _enhancedFeatureGenerator.GenerateComprehensiveNativeContractCalls();
                case "Runtime":
                    return _baseFeatureGenerator.GenerateNeoRuntimeOperations();
                case "NeoType":
                    return _neoTypeOperations.GenerateNeoTypeOperations();
                case "Collection":
                    return _collectionOperations.GenerateCollectionOperation();
                case "ControlFlow":
                    return _baseFeatureGenerator.GenerateControlFlowStatements();
                case "ExceptionHandling":
                    return _baseFeatureGenerator.GenerateExceptionHandling();
                default:
                    return _fragmentGenerator.GeneratePrimitiveTypeDeclaration();
            }
        }

        /// <summary>
        /// Generate a Neo runtime operation
        /// </summary>
        public string GenerateNeoRuntimeOperation()
        {
            string[] operations = {
                "bool isWitness = Runtime.CheckWitness(UInt160.Zero);",
                "UInt160 executingScriptHash = Runtime.ExecutingScriptHash;",
                "UInt160 callingScriptHash = Runtime.CallingScriptHash;",
                "TriggerType trigger = Runtime.Trigger;",
                "uint time = Runtime.Time;",
                "ulong invocationCounter = Runtime.InvocationCounter;",
                "byte[] randomBytes = Runtime.GetRandom();"
            };

            return operations[_random.Next(operations.Length)];
        }

        /// <summary>
        /// Generate a control flow statement
        /// </summary>
        public string GenerateControlFlowStatement()
        {
            string[] operations = {
                // If statement
                "if (Runtime.Trigger == TriggerType.Application)\n{\n    Runtime.Log(\"Application trigger\");\n}",

                // If-else statement
                "if (Runtime.Trigger == TriggerType.Application)\n{\n    Runtime.Log(\"Application trigger\");\n}\nelse\n{\n    Runtime.Log(\"Non-application trigger\");\n}",

                // For loop
                "for (int i = 0; i < 5; i++)\n{\n    Runtime.Log(i.ToString());\n}",

                // While loop
                "int counter = 0;\nwhile (counter < 5)\n{\n    Runtime.Log(counter.ToString());\n    counter++;\n}",

                // Switch statement
                "switch (Runtime.Trigger)\n{\n    case TriggerType.Application:\n        Runtime.Log(\"Application\");\n        break;\n    case TriggerType.Verification:\n        Runtime.Log(\"Verification\");\n        break;\n    default:\n        Runtime.Log(\"Other\");\n        break;\n}"
            };

            return operations[_random.Next(operations.Length)];
        }

        /// <summary>
        /// Generate exception handling code
        /// </summary>
        public string GenerateExceptionHandling()
        {
            string[] operations = {
                // Try-catch block
                "try\n{\n    Runtime.Log(\"Trying operation\");\n    // Some operation that might throw\n    if (Runtime.Trigger != TriggerType.Application)\n        throw new Exception(\"Invalid trigger\");\n}\ncatch (Exception e)\n{\n    Runtime.Log(\"Caught exception: \" + e.Message);\n}",

                // Throw exception
                "if (Runtime.Trigger != TriggerType.Application)\n{\n    throw new Exception(\"Invalid trigger type\");\n}"
            };

            return operations[_random.Next(operations.Length)];
        }

        /// <summary>
        /// Validate that a contract is syntactically correct
        /// </summary>
        public bool ValidateContractSyntax(string contractCode)
        {
            // Basic syntax validation
            int braceCount = 0;
            foreach (char c in contractCode)
            {
                if (c == '{') braceCount++;
                if (c == '}') braceCount--;
                if (braceCount < 0) return false; // Unbalanced braces
            }
            if (braceCount != 0) return false; // Unbalanced braces

            return true;
        }
    }
}
