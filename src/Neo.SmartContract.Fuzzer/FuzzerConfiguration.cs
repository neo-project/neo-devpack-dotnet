using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Neo.SmartContract.Fuzzer
{
    /// <summary>
    /// Configuration for the smart contract fuzzer
    /// </summary>
    public class FuzzerConfiguration
    {
        /// <summary>
        /// Path to the NEF file
        /// </summary>
        public string NefPath { get; set; } = string.Empty;

        /// <summary>
        /// Path to the manifest file
        /// </summary>
        public string ManifestPath { get; set; } = string.Empty;

        /// <summary>
        /// Path to the source code file (optional)
        /// </summary>
        public string SourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Directory to output fuzzing results
        /// </summary>
        public string OutputDirectory { get; set; } = "fuzzer-output";

        /// <summary>
        /// Number of iterations to run for each method
        /// </summary>
        public int IterationsPerMethod { get; set; } = 10;

        /// <summary>
        /// Number of iterations to run (alias for IterationsPerMethod for backward compatibility)
        /// </summary>
        public int Iterations { get => IterationsPerMethod; set => IterationsPerMethod = value; }

        /// <summary>
        /// Gas limit for contract execution
        /// </summary>
        public long GasLimit { get; set; } = 20_000_000;

        /// <summary>
        /// Random seed for parameter generation
        /// </summary>
        public int Seed { get; set; } = (int)DateTime.Now.Ticks;

        /// <summary>
        /// Whether to enable coverage tracking
        /// </summary>
        public bool EnableCoverage { get; set; } = true;

        /// <summary>
        /// Format for coverage reports (html, json, text)
        /// </summary>
        public string CoverageFormat { get; set; } = "html";

        /// <summary>
        /// Whether to enable coverage-guided fuzzing
        /// </summary>
        public bool EnableCoverageGuidedFuzzing { get; set; } = true;

        /// <summary>
        /// Whether to prioritize test cases that increase branch coverage
        /// </summary>
        public bool PrioritizeBranchCoverage { get; set; } = true;

        /// <summary>
        /// Whether to prioritize test cases that increase path coverage
        /// </summary>
        public bool PrioritizePathCoverage { get; set; } = true;

        /// <summary>
        /// Whether to persist the blockchain state between method calls within a single fuzzing run.
        /// If false (default), each method call starts with a clean state.
        /// If true, state changes (storage, etc.) are carried over to the next call.
        /// </summary>
        public bool PersistStateBetweenCalls { get; set; } = false;

        /// <summary>
        /// If true, only saves the detailed input/result JSON files for iterations that result in a VM FAULT or exception.
        /// If false (default), saves details for all iterations.
        /// </summary>
        public bool SaveFailingInputsOnly { get; set; } = false;

        /// <summary>
        /// List of methods to fuzz (empty means all methods)
        /// </summary>
        public List<string> MethodsToFuzz { get; set; } = new List<string>();

        /// <summary>
        /// List of methods to include in vulnerability detection (empty means all methods)
        /// </summary>
        public List<string>? MethodsToInclude { get; set; } = new List<string>();

        /// <summary>
        /// List of methods to exclude from fuzzing and vulnerability detection
        /// </summary>
        public List<string>? MethodsToExclude { get; set; } = new List<string>();

        /// <summary>
        /// Maximum number of steps to execute during symbolic execution
        /// </summary>
        public int MaxSteps { get; set; } = 10000;

        /// <summary>
        /// Whether to enable feedback-guided fuzzing
        /// </summary>
        public bool EnableFeedbackGuidedFuzzing { get; set; } = true;

        /// <summary>
        /// Whether to enable static analysis
        /// </summary>
        public bool EnableStaticAnalysis { get; set; } = true;

        /// <summary>
        /// Whether to enable symbolic execution
        /// </summary>
        public bool EnableSymbolicExecution { get; set; } = false;

        /// <summary>
        /// Maximum depth for symbolic execution
        /// </summary>
        public int SymbolicExecutionDepth { get; set; } = 100;

        /// <summary>
        /// Maximum number of paths to explore in symbolic execution
        /// </summary>
        public int SymbolicExecutionPaths { get; set; } = 1000;

        /// <summary>
        /// Whether to enable test case minimization
        /// </summary>
        public bool EnableTestCaseMinimization { get; set; } = true;

        /// <summary>
        /// Report formats (json, html, markdown, text)
        /// </summary>
        public List<string> ReportFormats { get; set; } = new List<string> { "json" };

        /// <summary>
        /// Execution engine to use (neo-express, rpc, in-memory)
        /// </summary>
        public string ExecutionEngine { get; set; } = "neo-express";

        /// <summary>
        /// RPC URL for RPC execution engine
        /// </summary>
        public string RpcUrl { get; set; } = "http://localhost:10332";

        /// <summary>
        /// Maximum number of test cases in corpus
        /// </summary>
        public int MaxCorpusSize { get; set; } = 1000;

        /// <summary>
        /// Maximum execution time in milliseconds for a single method call
        /// </summary>
        public int MaxExecutionTimeMs { get; set; } = 10000;

        /// <summary>
        /// Load configuration from a JSON file
        /// </summary>
        /// <param name="path">Path to the configuration file</param>
        /// <returns>The loaded configuration</returns>
        public static FuzzerConfiguration LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");

            string json = File.ReadAllText(path);
            var config = System.Text.Json.JsonSerializer.Deserialize<FuzzerConfiguration>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config == null)
                throw new InvalidOperationException("Failed to deserialize configuration file");

            return config;
        }

        /// <summary>
        /// Save configuration to a JSON file
        /// </summary>
        /// <param name="path">Path to save the configuration file</param>
        public void SaveToFile(string path)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
    }
}
