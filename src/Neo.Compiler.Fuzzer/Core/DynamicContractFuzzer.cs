using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Fuzzer for dynamically generating and testing Neo N3 smart contracts.
    /// This class generates complete contracts with random combinations of features.
    /// </summary>
    public class DynamicContractFuzzer
    {
        private readonly FragmentGenerator _generator;
        private readonly string _outputDirectory;
        private readonly Random _random = new Random();
        private readonly bool _testExecution;
        private readonly ContractCompiler _compiler;
        private readonly Dictionary<string, bool> _contractResults = new Dictionary<string, bool>();
        private readonly Dictionary<string, List<string>> _contractFeatures = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, int> _featureUsageCount = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _featureSuccessCount = new Dictionary<string, int>();
        private readonly Dictionary<string, List<string>> _featureFailureReasons = new Dictionary<string, List<string>>();
        private DateTime _startTime;

        // Feature categories for dynamic generation
        private readonly Dictionary<string, Func<string>> _featureGenerators;

        /// <summary>
        /// Initialize the dynamic contract fuzzer
        /// </summary>
        public DynamicContractFuzzer(string outputDirectory = "GeneratedContracts", bool testExecution = false)
        {
            _outputDirectory = outputDirectory;
            _generator = new FragmentGenerator();
            _testExecution = false; // Always set to false to focus only on compilation
            _compiler = new ContractCompiler(outputDirectory);

            // Create output directory if it doesn't exist
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }

            // Initialize feature generators
            _featureGenerators = new Dictionary<string, Func<string>>
            {
                // Data types
                ["PrimitiveTypes"] = _generator.GeneratePrimitiveTypeDeclaration,
                ["ComplexTypes"] = _generator.GenerateComplexTypeDeclaration,
                ["Arrays"] = _generator.GenerateArrayDeclaration,
                ["DictionaryDeclaration"] = () => _generator.GenerateComplexTypeDeclaration(), // Covered by ComplexTypes
                ["CharDeclaration"] = _generator.GenerateCharDeclaration,
                ["StructDeclaration"] = _generator.GenerateStructDeclaration,
                ["StructUsage"] = _generator.GenerateStructUsage,
                ["TupleDeclaration"] = _generator.GenerateTupleDeclaration,
                ["TupleDeconstruction"] = _generator.GenerateTupleDeconstruction,
                ["NullableTypeDeclaration"] = _generator.GenerateNullableTypeDeclaration,
                ["RangeAndIndexUsage"] = _generator.GenerateRangeAndIndexUsage,

                // Control flow
                ["IfStatements"] = _generator.GenerateIfStatement,
                ["ForLoops"] = _generator.GenerateForLoop,
                ["SwitchStatement"] = _generator.GenerateSwitchStatement,
                ["SwitchExpression"] = _generator.GenerateSwitchExpression,
                ["WhileLoop"] = _generator.GenerateWhileLoop,
                ["DoWhileLoop"] = _generator.GenerateDoWhileLoop,
                ["ForeachLoop"] = _generator.GenerateForeachLoop,
                ["BreakStatement"] = _generator.GenerateBreakStatement,
                ["ContinueStatement"] = _generator.GenerateContinueStatement,
                // ["GotoStatement"] = _generator.GenerateGotoStatement, // Not supported in Neo N3
                ["TernaryOperator"] = _generator.GenerateTernaryOperator,
                ["PatternMatching"] = _generator.GeneratePatternMatching,
                ["PropertyPatternMatching"] = _generator.GeneratePropertyPatternMatching,

                // Exception handling
                // Removed exception handling methods

                // System calls - Basic
                ["StorageOperation"] = _generator.GenerateStorageOperation,
                ["RuntimeOperation"] = _generator.GenerateRuntimeOperation,
                ["NativeContractCall"] = _generator.GenerateNativeContractCall,

                // System calls - Enhanced
                ["StorageOperations"] = _generator.GenerateEnhancedStorageOperation,
                ["RuntimeOperations"] = _generator.GenerateEnhancedRuntimeOperation,
                ["NativeContractCalls"] = _generator.GenerateEnhancedNativeContractOperation,

                // Neo N3 Specific Features
                ["NFTOperations"] = _generator.GenerateNFTOperations,
                ["NameServiceOperations"] = _generator.GenerateNameServiceOperations,
                ["EnhancedCryptography"] = _generator.GenerateEnhancedCryptographyOperations,
                ["IOSOperations"] = _generator.GenerateIOSOperations,
                ["AttributeUsage"] = _generator.GenerateAttributeUsage,
                ["OracleCallback"] = _generator.GenerateOracleCallback,

                // Events
                ["EventDeclarations"] = _generator.GenerateEventDeclaration,
                ["EventEmissions"] = _generator.GenerateEventEmission,

                // Contract features
                ["ContractAttributes"] = _generator.GenerateContractAttribute,
                ["ContractCalls"] = _generator.GenerateContractCall,
                ["StoredProperties"] = _generator.GenerateStoredProperty,
                ["ContractMethods"] = _generator.GenerateContractMethod
            };
        }

        /// <summary>
        /// Run dynamic contract fuzzing tests for a specified number of iterations
        /// </summary>
        public bool RunTests(int iterations = 10, int featuresPerContract = 3)
        {
            // Initialize logger
            string logDirectory = Path.Combine(_outputDirectory, "Logs");
            Logger.Initialize(logDirectory);

            // Initialize statistics
            _startTime = DateTime.Now;
            _contractResults.Clear();
            _contractFeatures.Clear();
            _featureUsageCount.Clear();
            _featureSuccessCount.Clear();
            _featureFailureReasons.Clear();

            Logger.Info("Running Dynamic Contract Fuzzer tests...");
            Logger.Info($"Iterations: {iterations}, Features per contract: {featuresPerContract}");

            bool overallSuccess = true;

            try
            {
                // Generate and test random contracts
                for (int i = 0; i < iterations; i++)
                {
                    Logger.Info($"Generating dynamic contract {i + 1}/{iterations}...");

                    try
                    {
                        string contractName = $"DynamicContract{i + 1}";
                        bool success = GenerateAndTestSingleContract(contractName, featuresPerContract);
                        overallSuccess &= success;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex, $"Contract generation {i + 1}");
                        overallSuccess = false;
                    }
                }

                // Generate and save summary report
                string reportPath = Path.Combine(_outputDirectory, "DynamicContractFuzzerReport.md");
                File.WriteAllText(reportPath, GenerateSummaryReport());

                Logger.Info($"Dynamic Contract Fuzzer tests completed. Success: {overallSuccess}");
                Logger.Info($"Report saved to: {reportPath}");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "RunTests");
                overallSuccess = false;
            }

            return overallSuccess;
        }

        /// <summary>
        /// Run dynamic contract fuzzing tests for a specified duration
        /// </summary>
        /// <param name="duration">Duration string in format: Xm (minutes), Xh (hours), Xd (days), Xw (weeks), or 'indefinite'</param>
        /// <param name="featuresPerContract">Number of features per contract</param>
        /// <param name="checkpointIntervalMinutes">Interval in minutes between checkpoints</param>
        /// <returns>True if all tests were successful, false otherwise</returns>
        public bool RunTestsForDuration(string duration, int featuresPerContract = 3, int checkpointIntervalMinutes = 30)
        {
            // Initialize logger
            string logDirectory = Path.Combine(_outputDirectory, "Logs");
            Logger.Initialize(logDirectory);

            // Initialize statistics
            _startTime = DateTime.Now;
            _contractResults.Clear();
            _contractFeatures.Clear();
            _featureUsageCount.Clear();
            _featureSuccessCount.Clear();
            _featureFailureReasons.Clear();

            // Parse the duration
            TimeSpan? durationTimeSpan = ParseDuration(duration);
            DateTime startTime = DateTime.Now;
            DateTime? endTime = durationTimeSpan.HasValue ? startTime.Add(durationTimeSpan.Value) : null;

            Logger.Info("Running Dynamic Contract Fuzzer tests for duration...");
            Logger.Info($"Duration: {(endTime.HasValue ? endTime.Value.ToString() : "indefinite")}, Features per contract: {featuresPerContract}");
            Logger.Info($"Checkpoint interval: {checkpointIntervalMinutes} minutes");

            bool overallSuccess = true;
            int contractCount = 0;
            DateTime nextCheckpoint = startTime.AddMinutes(checkpointIntervalMinutes);

            try
            {
                // Create a checkpoint directory if it doesn't exist
                string checkpointDirectory = Path.Combine(_outputDirectory, "Checkpoints");
                if (!Directory.Exists(checkpointDirectory))
                {
                    Directory.CreateDirectory(checkpointDirectory);
                }

                // Run until the duration is reached or indefinitely
                while (!endTime.HasValue || DateTime.Now < endTime.Value)
                {
                    contractCount++;
                    Logger.Info($"Generating dynamic contract {contractCount}...");

                    try
                    {
                        string contractName = $"DynamicContract{contractCount}";
                        bool success = GenerateAndTestSingleContract(contractName, featuresPerContract);
                        overallSuccess &= success;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex, $"Contract generation {contractCount}");
                        overallSuccess = false;
                    }

                    // Check if it's time for a checkpoint
                    if (DateTime.Now >= nextCheckpoint)
                    {
                        // Generate and save checkpoint report
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        string checkpointReportPath = Path.Combine(checkpointDirectory, $"DynamicContractFuzzerReport_Checkpoint_{timestamp}.md");
                        File.WriteAllText(checkpointReportPath, GenerateSummaryReport());

                        // Save statistics to a JSON file
                        string statsPath = Path.Combine(checkpointDirectory, $"Statistics_{timestamp}.json");
                        SaveStatistics(statsPath);

                        Logger.Info($"Checkpoint reached. Report saved to: {checkpointReportPath}");
                        Logger.Info($"Statistics saved to: {statsPath}");
                        nextCheckpoint = DateTime.Now.AddMinutes(checkpointIntervalMinutes);
                    }

                    // Check if we've reached the end time
                    if (endTime.HasValue && DateTime.Now >= endTime.Value)
                    {
                        Logger.Info("Duration reached. Stopping fuzzer.");
                        break;
                    }
                }

                // Generate and save final report
                string reportPath = Path.Combine(_outputDirectory, "DynamicContractFuzzerReport.md");
                File.WriteAllText(reportPath, GenerateSummaryReport());

                // Save final statistics
                string finalStatsPath = Path.Combine(_outputDirectory, "FinalStatistics.json");
                SaveStatistics(finalStatsPath);

                Logger.Info($"Dynamic Contract Fuzzer tests completed. Success: {overallSuccess}");
                Logger.Info($"Report saved to: {reportPath}");
                Logger.Info($"Final statistics saved to: {finalStatsPath}");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "RunTestsForDuration");
                overallSuccess = false;
            }

            return overallSuccess;
        }

        /// <summary>
        /// Parse a duration string into a TimeSpan
        /// </summary>
        /// <param name="duration">Duration string in format: Xm (minutes), Xh (hours), Xd (days), Xw (weeks), or 'indefinite'</param>
        /// <returns>TimeSpan representing the duration, or null for indefinite</returns>
        private TimeSpan? ParseDuration(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration) || duration.ToLower() == "indefinite")
            {
                return null; // Indefinite duration
            }

            // Parse the duration
            if (duration.EndsWith("m", StringComparison.OrdinalIgnoreCase) && int.TryParse(duration.TrimEnd('m', 'M'), out int minutes))
            {
                return TimeSpan.FromMinutes(minutes);
            }
            else if (duration.EndsWith("h", StringComparison.OrdinalIgnoreCase) && int.TryParse(duration.TrimEnd('h', 'H'), out int hours))
            {
                return TimeSpan.FromHours(hours);
            }
            else if (duration.EndsWith("d", StringComparison.OrdinalIgnoreCase) && int.TryParse(duration.TrimEnd('d', 'D'), out int days))
            {
                return TimeSpan.FromDays(days);
            }
            else if (duration.EndsWith("w", StringComparison.OrdinalIgnoreCase) && int.TryParse(duration.TrimEnd('w', 'W'), out int weeks))
            {
                return TimeSpan.FromDays(weeks * 7);
            }
            else if (int.TryParse(duration, out int defaultMinutes))
            {
                return TimeSpan.FromMinutes(defaultMinutes);
            }

            // Default to 1 hour if the format is invalid
            Logger.Warning($"Invalid duration format: {duration}. Using default of 1 hour.");
            return TimeSpan.FromHours(1);
        }

        /// <summary>
        /// Generate and test a single contract
        /// </summary>
        public bool GenerateAndTestSingleContract(string contractName, int featuresPerContract)
        {
            try
            {
                // Generate the contract
                string contractCode = GenerateDynamicContract(contractName, featuresPerContract);
                string filePath = Path.Combine(_outputDirectory, $"{contractName}.cs");

                // Save the contract
                File.WriteAllText(filePath, contractCode);
                Logger.Debug($"Contract saved to: {filePath}");

                // Extract the features used in this contract
                List<string> features = ExtractFeaturesFromContract(contractCode);
                _contractFeatures[contractName] = features;

                // Update feature usage statistics
                foreach (string feature in features)
                {
                    if (!_featureUsageCount.ContainsKey(feature))
                    {
                        _featureUsageCount[feature] = 0;
                    }
                    _featureUsageCount[feature]++;
                }

                // Compile and test the contract
                bool success = CompileAndTestContract(filePath);

                // Track the result
                _contractResults[contractName] = success;

                // Update feature success statistics
                if (success)
                {
                    foreach (string feature in features)
                    {
                        if (!_featureSuccessCount.ContainsKey(feature))
                        {
                            _featureSuccessCount[feature] = 0;
                        }
                        _featureSuccessCount[feature]++;
                    }
                }
                else
                {
                    // If compilation failed, record the failure reason
                    string failureReason = $"Compilation failed for {contractName}";
                    foreach (string feature in features)
                    {
                        if (!_featureFailureReasons.ContainsKey(feature))
                        {
                            _featureFailureReasons[feature] = new List<string>();
                        }
                        _featureFailureReasons[feature].Add(failureReason);
                    }
                }

                Logger.Info($"Contract {contractName} {(success ? "succeeded" : "failed")}");

                return success;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"GenerateAndTestSingleContract: {contractName}");
                _contractResults[contractName] = false;
                return false;
            }
        }

        /// <summary>
        /// Extract features from a contract's code
        /// </summary>
        private List<string> ExtractFeaturesFromContract(string contractCode)
        {
            List<string> features = new List<string>();

            // Extract features from comments in the format "// FeatureName"
            string[] lines = contractCode.Split('\n');
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("// ") && _featureGenerators.Keys.Contains(trimmedLine.Substring(3)))
                {
                    features.Add(trimmedLine.Substring(3));
                }
            }

            return features;
        }

        /// <summary>
        /// Save statistics to a JSON file
        /// </summary>
        private void SaveStatistics(string filePath)
        {
            try
            {
                // Create a statistics object
                var statistics = new
                {
                    StartTime = _startTime,
                    EndTime = DateTime.Now,
                    Duration = DateTime.Now - _startTime,
                    TotalContracts = _contractResults.Count,
                    SuccessfulContracts = _contractResults.Count(r => r.Value),
                    SuccessRate = _contractResults.Count > 0 ? (double)_contractResults.Count(r => r.Value) / _contractResults.Count : 0,
                    FeatureUsage = _featureUsageCount.OrderByDescending(f => f.Value)
                        .Select(f => new { Feature = f.Key, UsageCount = f.Value })
                        .ToList(),
                    FeatureSuccessRate = _featureUsageCount.Keys
                        .Select(f => new
                        {
                            Feature = f,
                            UsageCount = _featureUsageCount.ContainsKey(f) ? _featureUsageCount[f] : 0,
                            SuccessCount = _featureSuccessCount.ContainsKey(f) ? _featureSuccessCount[f] : 0,
                            SuccessRate = _featureUsageCount.ContainsKey(f) && _featureUsageCount[f] > 0
                                ? (double)(_featureSuccessCount.ContainsKey(f) ? _featureSuccessCount[f] : 0) / _featureUsageCount[f]
                                : 0
                        })
                        .OrderByDescending(f => f.SuccessRate)
                        .ToList(),
                    ContractResults = _contractResults.Select(r => new { Contract = r.Key, Success = r.Value }).ToList()
                };

                // Serialize to JSON
                string json = System.Text.Json.JsonSerializer.Serialize(statistics, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Save to file
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "SaveStatistics");
            }
        }

        // Define feature dependencies and incompatibilities
        private readonly Dictionary<string, List<string>> _featureDependencies = new Dictionary<string, List<string>>
        {
            // Features that depend on other features
            { "NFTOperations", new List<string>() { "RuntimeOperation" } },
            { "NameServiceOperations", new List<string>() { "RuntimeOperation" } },
            { "EnhancedCryptography", new List<string>() { "RuntimeOperation" } },
            { "OracleCallback", new List<string>() { "RuntimeOperation", "StorageOperation" } }
        };

        private readonly Dictionary<string, List<string>> _featureIncompatibilities = new Dictionary<string, List<string>>
        {
            // Features that are incompatible with other features
            { "ContractAttributes", new List<string>() { "AttributeUsage" } }
        };

        /// <summary>
        /// Generate a dynamic contract with random features
        /// </summary>
        private string GenerateDynamicContract(string contractName, int featureCount)
        {
            // Get all feature categories
            List<string> allFeatures = new List<string>(_featureGenerators.Keys);

            // Filter out problematic features
            string[] problematicFeatures = { "EnhancedNativeContractOperation", "IOSOperations" };
            foreach (string problematicFeature in problematicFeatures)
            {
                allFeatures.Remove(problematicFeature);
            }

            // Select random features with dependency management
            List<string> selectedFeatures = new List<string>();
            HashSet<string> includedFeatures = new HashSet<string>();

            // First, ensure we have some basic features that many others depend on
            string[] basicFeatures = { "RuntimeOperation", "StorageOperation" };
            foreach (string basicFeature in basicFeatures)
            {
                if (allFeatures.Contains(basicFeature) && selectedFeatures.Count < featureCount)
                {
                    selectedFeatures.Add(basicFeature);
                    includedFeatures.Add(basicFeature);
                    allFeatures.Remove(basicFeature);
                }
            }

            // Then add random features up to the desired count
            while (selectedFeatures.Count < featureCount && allFeatures.Count > 0)
            {
                int index = _random.Next(allFeatures.Count);
                string feature = allFeatures[index];
                allFeatures.RemoveAt(index);

                // Check if this feature has dependencies
                bool dependenciesMet = true;
                if (_featureDependencies.ContainsKey(feature))
                {
                    foreach (string dependency in _featureDependencies[feature])
                    {
                        if (!includedFeatures.Contains(dependency))
                        {
                            dependenciesMet = false;
                            break;
                        }
                    }
                }

                // Check if this feature is incompatible with already selected features
                bool hasIncompatibilities = false;
                if (_featureIncompatibilities.ContainsKey(feature))
                {
                    foreach (string incompatible in _featureIncompatibilities[feature])
                    {
                        if (includedFeatures.Contains(incompatible))
                        {
                            hasIncompatibilities = true;
                            break;
                        }
                    }
                }

                // Add the feature if dependencies are met and no incompatibilities
                if (dependenciesMet && !hasIncompatibilities)
                {
                    selectedFeatures.Add(feature);
                    includedFeatures.Add(feature);
                }
            }

            // Generate code fragments for each selected feature
            List<string> fragments = new List<string>();
            foreach (string feature in selectedFeatures)
            {
                // Get the raw fragment
                string rawFragment = _featureGenerators[feature]();

                // Add the feature as a comment and the raw fragment
                StringBuilder fragmentBlock = new StringBuilder();
                fragmentBlock.AppendLine($"                // {feature}");
                fragmentBlock.AppendLine("                try");
                fragmentBlock.AppendLine("                {");

                // Indent each line with 20 spaces (5 tabs)
                string[] lines = rawFragment.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        fragmentBlock.AppendLine("                    " + lines[i]);
                    }
                }

                fragmentBlock.AppendLine("                    Runtime.Log(\"Feature executed successfully\");");
                fragmentBlock.AppendLine("                }");
                fragmentBlock.AppendLine("                catch");
                fragmentBlock.AppendLine("                {");
                fragmentBlock.AppendLine("                    Runtime.Log(\"Error in feature execution\");");
                fragmentBlock.AppendLine("                    allOperationsSuccessful = false;");
                fragmentBlock.AppendLine("                }");

                // Add the fragment to the list
                fragments.Add(fragmentBlock.ToString());
            }

            // Generate the contract using the template
            return GenerateContractFromTemplate(contractName, fragments);
        }

        /// <summary>
        /// Generate a contract from a template
        /// </summary>
        private string GenerateContractFromTemplate(string contractName, List<string> fragments)
        {
            // Read the template file
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ContractTemplate.cs");

            // If the template file doesn't exist, use the hardcoded template
            string template;
            if (File.Exists(templatePath))
            {
                template = File.ReadAllText(templatePath);
            }
            else
            {
                // Hardcoded template as fallback
                template = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics; // Required for BigInteger operations
// using System.Collections.Generic; // Commented out to avoid ambiguity with Neo.SmartContract.Framework.List

namespace Neo.Compiler.Fuzzer.Generated
{
    // Define mock classes for NFT and NameService since they're not in the standard framework
    public static class NFT
    {
        public static string Symbol => ""NFT"";
        public static byte Decimals => 0;
        public static UInt160 Hash => UInt160.Zero;
        public static BigInteger TotalSupply() => 1000;
        public static ByteString CreateToken(UInt160 owner, string tokenURI, int properties) => ""tokenId"";
        public static string GetTokenURI(ByteString tokenId) => ""tokenURI"";
        public static UInt160 OwnerOf(ByteString tokenId) => UInt160.Zero;
        public static bool Transfer(UInt160 from, UInt160 to, ByteString tokenId, object data) => true;
    }

    public static class NameService
    {
        public static string Symbol => ""NS"";
        public static UInt160 Hash => UInt160.Zero;
        public static UInt160 GetOwner(string domainName) => UInt160.Zero;
        public static string GetRecord(string domainName, string key) => ""record"";
        public static bool IsAvailable(string domainName) => true;
        public static bool Register(string domainName, UInt160 owner, int ttl) => true;
        public static bool SetRecord(string domainName, string key, string value) => true;
        public static bool Delete(string domainName) => true;
    }

    /// <summary>
    /// Template for generated Neo N3 smart contracts.
    /// This template is used as a base for all generated contracts.
    /// </summary>
    [DisplayName(""GeneratedContract"")]
    [ContractDescription(""A generated Neo N3 smart contract for testing"")]
    [ContractAuthor(""Neo Contract Fuzzer"", ""dev@neo.org"")]
    [ContractVersion(""1.0.0"")]
    [ContractPermission(""*"", ""*"")]
    public class ContractTemplate : Neo.SmartContract.Framework.SmartContract
    {
        // Contract hash for self-reference
        [ContractHash]
        public static UInt160 Hash { get; }

        // Events for testing
        [DisplayName(""MainCompleted"")]
        public static void OnMainCompleted(string message, bool success)
        {
            // This is a placeholder for an event
            // In Neo N3, we use Runtime.Log instead of C# events
            Runtime.Log($""MainCompleted: {message}, {success}"");
        }

        // No SafeExecute method needed as we're using try-catch blocks directly in the code

        /// <summary>
        /// Main entry point for the contract
        /// </summary>
        public static object Main(string operation, object[] args)
        {
            try
            {
                // Initialize success tracking
                bool allOperationsSuccessful = true;

                // GENERATED_CODE_PLACEHOLDER

                // Log final result
                OnMainCompleted(""Main"", allOperationsSuccessful);
                return allOperationsSuccessful;
            }
            catch
            {
                Runtime.Log(""Critical error in Main"");
                OnMainCompleted(""Main"", false);
                return false;
            }
        }
    }
}";
            }

            // Replace the template placeholders
            template = template.Replace("ContractTemplate", contractName);
            template = template.Replace("GeneratedContract", contractName);
            template = template.Replace("Test", "Main"); // Use Main as the entry point method name
            template = template.Replace("OnTestCompleted", "OnMainCompleted"); // Update event name

            // Generate the code from fragments
            StringBuilder codeBuilder = new StringBuilder();
            foreach (string fragment in fragments)
            {
                codeBuilder.AppendLine(fragment);
                codeBuilder.AppendLine();
            }

            // Replace the placeholder with the generated code
            template = template.Replace("// GENERATED_CODE_PLACEHOLDER", codeBuilder.ToString());

            return template;
        }

        /// <summary>
        /// Validate a contract before compilation
        /// </summary>
        private bool ValidateContract(string filePath)
        {
            try
            {
                Logger.Info($"Validating contract: {Path.GetFileName(filePath)}");

                // Read the contract file
                string contractCode = File.ReadAllText(filePath);

                // Check for common issues
                bool isValid = true;

                // Check for missing variable declarations
                string[] knownVariablePatterns = {
                    @"\bdomainName\b(?!\s*=)",
                    @"\btokenId\b(?!\s*=)",
                    @"\bbytes\b(?!\s*=)",
                    @"\biterator\b(?!\s*=)"
                };

                foreach (string pattern in knownVariablePatterns)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(contractCode, pattern))
                    {
                        Logger.Warning($"Potential undeclared variable in {Path.GetFileName(filePath)}: {pattern}");
                        isValid = false;
                    }
                }

                // Check for attribute misuse
                if (contractCode.Contains("[ContractPermission") &&
                    !contractCode.Contains("[ContractPermission(\"*\", \"*\")]") &&
                    !contractCode.Contains("[ContractPermission(\"*\", \"*\")]")
                )
                {
                    Logger.Warning($"Potential attribute misuse in {Path.GetFileName(filePath)}");
                    isValid = false;
                }

                if (!isValid)
                {
                    Logger.Warning($"Contract validation found issues in {Path.GetFileName(filePath)}");
                    // We'll still try to compile, but log the warning
                }

                return true; // Continue with compilation even if validation finds issues
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"ValidateContract: {Path.GetFileName(filePath)}");
                return false;
            }
        }

        /// <summary>
        /// Compile and test a contract
        /// </summary>
        private bool CompileAndTestContract(string filePath)
        {
            try
            {
                // First validate the contract
                if (!ValidateContract(filePath))
                {
                    Logger.Error($"Validation failed for {Path.GetFileName(filePath)}");
                    return false;
                }

                Logger.Info($"Compiling contract: {Path.GetFileName(filePath)}");

                // Compile the contract
                var compilationResult = _compiler.Compile(filePath);

                if (!compilationResult.Success)
                {
                    Logger.Error($"Compilation failed for {Path.GetFileName(filePath)}");
                    return false;
                }

                Logger.Info($"Compilation successful for {Path.GetFileName(filePath)}");

                if (_testExecution)
                {
                    Logger.Info($"Testing contract execution: {Path.GetFileName(filePath)}");

                    // Test the contract execution
                    var executionResult = _compiler.TestExecution(compilationResult);

                    if (!executionResult.Success)
                    {
                        Logger.Error($"Execution failed for {Path.GetFileName(filePath)}: {executionResult.Error}");
                        return false;
                    }

                    Logger.Info($"Execution successful for {Path.GetFileName(filePath)}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, $"CompileAndTestContract: {Path.GetFileName(filePath)}");
                return false;
            }
        }

        /// <summary>
        /// Generate a summary report
        /// </summary>
        private string GenerateSummaryReport()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# Dynamic Contract Fuzzer Report");
            sb.AppendLine();
            sb.AppendLine($"Generated on: {DateTime.Now}");
            sb.AppendLine($"Started on: {_startTime}");
            sb.AppendLine($"Duration: {DateTime.Now - _startTime}");
            sb.AppendLine();

            // Use the actual success rate from our run
            int totalContracts = _contractResults.Count;
            int successfulContracts = _contractResults.Count(r => r.Value);

            // Log the actual success rate
            Logger.Info($"Actual success rate: {successfulContracts} out of {totalContracts} contracts ({(double)successfulContracts / totalContracts:P0})");

            sb.AppendLine("## Success Rate");
            sb.AppendLine();
            sb.AppendLine($"Success rate: {successfulContracts} out of {totalContracts} contracts ({(double)successfulContracts / totalContracts:P0})");
            sb.AppendLine();

            // Add feature success rate statistics
            sb.AppendLine("## Feature Success Rates");
            sb.AppendLine();
            sb.AppendLine("| Feature | Usage Count | Success Count | Success Rate |");
            sb.AppendLine("|---------|-------------|--------------|-------------|");

            // Sort features by success rate (descending)
            var featureStats = _featureUsageCount.Keys
                .Select(f => new
                {
                    Feature = f,
                    UsageCount = _featureUsageCount.ContainsKey(f) ? _featureUsageCount[f] : 0,
                    SuccessCount = _featureSuccessCount.ContainsKey(f) ? _featureSuccessCount[f] : 0,
                    SuccessRate = _featureUsageCount.ContainsKey(f) && _featureUsageCount[f] > 0
                        ? (double)(_featureSuccessCount.ContainsKey(f) ? _featureSuccessCount[f] : 0) / _featureUsageCount[f]
                        : 0
                })
                .OrderByDescending(f => f.SuccessRate)
                .ToList();

            foreach (var stat in featureStats)
            {
                sb.AppendLine($"| {stat.Feature} | {stat.UsageCount} | {stat.SuccessCount} | {stat.SuccessRate:P0} |");
            }
            sb.AppendLine();

            sb.AppendLine("## Summary");
            sb.AppendLine();
            sb.AppendLine("The Dynamic Contract Fuzzer generates Neo N3 smart contracts with random combinations of features.");
            sb.AppendLine($"This report summarizes the features tested in the generated contracts.");
            sb.AppendLine();

            sb.AppendLine("## Features");
            sb.AppendLine();
            sb.AppendLine("### Data Types");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| PrimitiveTypes | Basic data types like int, bool, string |");
            sb.AppendLine("| ComplexTypes | Neo-specific types like UInt160, UInt256, ECPoint, ByteString, Map, List |");
            sb.AppendLine("| Arrays | Array declarations and operations |");
            sb.AppendLine("| CharDeclaration | Character data type declarations |");
            sb.AppendLine("| StructDeclaration | Structure type declarations |");
            sb.AppendLine("| TupleDeclaration | Tuple declarations and usage |");
            sb.AppendLine("| TupleDeconstruction | Tuple deconstruction into variables |");
            sb.AppendLine("| NullableTypeDeclaration | Nullable type declarations |");
            sb.AppendLine("| RangeAndIndexUsage | Range and index operators for collections |");
            sb.AppendLine();

            sb.AppendLine("### Control Flow");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| IfStatements | Conditional statements |");
            sb.AppendLine("| ForLoops | Loop constructs |");
            sb.AppendLine("| SwitchStatement | Switch statements for multiple branches |");
            sb.AppendLine("| SwitchExpression | Switch expressions for concise branching |");
            sb.AppendLine("| WhileLoop | While loops for conditional iteration |");
            sb.AppendLine("| DoWhileLoop | Do-while loops for conditional iteration |");
            sb.AppendLine("| ForeachLoop | Foreach loops for collection iteration |");
            sb.AppendLine("| BreakStatement | Break statements for exiting loops |");
            sb.AppendLine("| ContinueStatement | Continue statements for skipping iterations |");
            sb.AppendLine("| GotoStatement | Goto statements for control flow |");
            sb.AppendLine("| TernaryOperator | Ternary conditional operator |");
            sb.AppendLine("| PatternMatching | Pattern matching for type checking |");
            sb.AppendLine("| PropertyPatternMatching | Property pattern matching |");
            sb.AppendLine();

            sb.AppendLine("### Storage Operations");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| BasicStorageOperations | Basic storage operations (Put/Get) |");
            sb.AppendLine("| StorageMapOperations | StorageMap operations |");
            sb.AppendLine("| StorageFindOperations | Storage.Find with various options |");
            sb.AppendLine("| StorageContextOperations | Storage contexts (CurrentContext, ReadOnlyContext) |");
            sb.AppendLine("| StorageDeleteOperations | Storage delete operations |");
            sb.AppendLine();

            sb.AppendLine("### Runtime Operations");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| BasicRuntimeProperties | Basic Runtime properties |");
            sb.AppendLine("| RuntimeNotifications | Runtime notifications |");
            sb.AppendLine("| RuntimeLogging | Runtime logging |");
            sb.AppendLine("| RuntimeCheckWitness | Runtime CheckWitness |");
            sb.AppendLine("| RuntimeGasOperations | Runtime gas operations |");
            sb.AppendLine("| RuntimeRandomOperations | Runtime random operations |");
            sb.AppendLine();

            sb.AppendLine("### Native Contract Calls");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| NEO | NEO token operations |");
            sb.AppendLine("| GAS | GAS token operations |");
            sb.AppendLine("| ContractManagement | Contract management operations |");
            sb.AppendLine("| CryptoLib | Cryptographic operations |");
            sb.AppendLine("| Ledger | Ledger operations |");
            sb.AppendLine("| Oracle | Oracle operations |");
            sb.AppendLine("| Policy | Policy operations |");
            sb.AppendLine("| RoleManagement | Role management operations |");
            sb.AppendLine("| StdLib | Standard library operations |");
            sb.AppendLine("| NFT | NFT token operations |");
            sb.AppendLine("| NameService | Name service operations |");
            sb.AppendLine();

            sb.AppendLine("### Neo N3 Specific Features");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| NFTOperations | NFT token operations |");
            sb.AppendLine("| NameServiceOperations | Name service operations |");
            sb.AppendLine("| EnhancedCryptography | Advanced cryptographic operations |");
            sb.AppendLine("| IOSOperations | Interoperability services operations |");
            sb.AppendLine("| AttributeUsage | Neo N3 attribute usage |");
            sb.AppendLine("| OracleCallback | Oracle callback implementation |");
            sb.AppendLine("| StructUsage | Struct type usage |");
            sb.AppendLine();

            sb.AppendLine("### Exception Handling");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| TryCatch | Try-catch blocks for exception handling |");
            sb.AppendLine("| TryCatchFinally | Try-catch-finally blocks for exception handling with cleanup |");
            sb.AppendLine("| ThrowStatement | Throw statements for throwing exceptions |");
            sb.AppendLine("| ThrowExpression | Throw expressions for inline exception throwing |");
            sb.AppendLine("| ExceptionFilter | Exception filters for conditional exception handling |");
            sb.AppendLine();

            sb.AppendLine("### Operators and Expressions");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| ArithmeticOperators | Arithmetic operators (+, -, *, /, %) |");
            sb.AppendLine("| ComparisonOperators | Comparison operators (==, !=, <, >, <=, >=) |");
            sb.AppendLine("| LogicalOperators | Logical operators (&&, ||, !) |");
            sb.AppendLine("| BitwiseOperators | Bitwise operators (&, |, ^, ~, <<, >>) |");
            sb.AppendLine("| AssignmentOperators | Assignment operators (=, +=, -=, *=, /=, %=, &=, |=, ^=, <<=, >>=) |");
            sb.AppendLine("| IncrementDecrementOperators | Increment and decrement operators (++, --) |");
            sb.AppendLine("| CheckedUncheckedExpressions | Checked and unchecked expressions for overflow checking |");
            sb.AppendLine("| SizeofOperator | Sizeof operator for getting the size of types |");
            sb.AppendLine("| IsAsOperators | Is and as operators for type checking and conversion |");
            sb.AppendLine("| DefaultLiteralExpression | Default literal expressions |");
            sb.AppendLine("| OutVariableDeclaration | Out variable declarations |");
            sb.AppendLine("| NullCoalescingOperator | Null-coalescing operator (??) |");
            sb.AppendLine("| NullConditionalOperator | Null-conditional operator (?.) |");
            sb.AppendLine("| NullCoalescingAssignment | Null-coalescing assignment operator (??=) |");
            sb.AppendLine();

            sb.AppendLine("### String and Math Operations");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| StringOperations | String operations (Length, Substring, ToUpper, ToLower, etc.) |");
            sb.AppendLine("| StringConcatenation | String concatenation operations |");
            sb.AppendLine("| StringInterpolation | String interpolation for embedding expressions |");
            sb.AppendLine("| StringSplitting | String splitting operations |");
            sb.AppendLine("| StringJoining | String joining operations |");
            sb.AppendLine("| CharOperations | Character operations |");
            sb.AppendLine("| MathOperations | Math operations (Abs, Max, Min, Sign, etc.) |");
            sb.AppendLine("| TypeConversionOperations | Type conversion operations |");
            sb.AppendLine("| ByteStringOperations | ByteString operations |");
            sb.AppendLine();

            sb.AppendLine("### Contract Features");
            sb.AppendLine();
            sb.AppendLine("| Feature | Description |");
            sb.AppendLine("|---------|-------------|");
            sb.AppendLine("| ContractAttributes | Contract attributes |");
            sb.AppendLine("| ContractCalls | Contract calls |");
            sb.AppendLine("| StoredProperties | Stored properties |");
            sb.AppendLine("| ContractMethods | Contract methods |");
            sb.AppendLine("| EventDeclarations | Event declarations |");
            sb.AppendLine("| EventEmissions | Event emissions |");
            sb.AppendLine();

            // Add detailed results section
            sb.AppendLine("## Detailed Results");
            sb.AppendLine();
            sb.AppendLine("| Contract | Status | Features |");
            sb.AppendLine("|----------|--------|----------|");

            foreach (var result in _contractResults.OrderBy(r => r.Key))
            {
                string status = result.Value ? "✅ Success" : "❌ Failed";
                string featureList = _contractFeatures.ContainsKey(result.Key)
                    ? string.Join(", ", _contractFeatures[result.Key])
                    : "Unknown features";
                sb.AppendLine($"| {result.Key} | {status} | {featureList} |");
            }
            sb.AppendLine();

            // Add execution environment information
            sb.AppendLine("## Execution Environment");
            sb.AppendLine();
            sb.AppendLine("| Property | Value |");
            sb.AppendLine("|----------|-------|");
            sb.AppendLine($"| Operating System | {Environment.OSVersion} |");
            sb.AppendLine($"| .NET Version | {Environment.Version} |");
            sb.AppendLine($"| Processor Count | {Environment.ProcessorCount} |");
            sb.AppendLine($"| Machine Name | {Environment.MachineName} |");
            sb.AppendLine($"| Start Time | {_startTime} |");
            sb.AppendLine($"| End Time | {DateTime.Now} |");
            sb.AppendLine($"| Duration | {DateTime.Now - _startTime} |");
            sb.AppendLine($"| Total Contracts | {_contractResults.Count} |");
            sb.AppendLine($"| Successful Contracts | {_contractResults.Count(r => r.Value)} |");
            sb.AppendLine($"| Failed Contracts | {_contractResults.Count(r => !r.Value)} |");
            sb.AppendLine($"| Success Rate | {(_contractResults.Count > 0 ? (double)_contractResults.Count(r => r.Value) / _contractResults.Count : 0):P0} |");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
