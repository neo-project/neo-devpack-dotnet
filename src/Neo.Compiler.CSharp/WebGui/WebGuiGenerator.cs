// Copyright (C) 2015-2025 The Neo Project.
//
// WebGuiGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Neo.Compiler.WebGui
{
    /// <summary>
    /// Generates interactive web-based GUIs for compiled smart contracts
    /// </summary>
    public class WebGuiGenerator : IWebGuiGenerator
    {
        private readonly HtmlTemplateEngine _templateEngine;

        public WebGuiGenerator()
        {
            _templateEngine = new HtmlTemplateEngine();
        }

        public WebGuiGenerationResult GenerateWebGui(
            string contractName,
            ContractManifest manifest,
            byte[] nefBytes,
            string? contractHash = null,
            string? outputDirectory = null,
            WebGuiOptions? options = null)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(contractName))
                throw new ArgumentException("Contract name cannot be null or empty", nameof(contractName));
            if (manifest == null)
                throw new ArgumentNullException(nameof(manifest));
            if (nefBytes == null || nefBytes.Length == 0)
                throw new ArgumentException("NEF bytes cannot be null or empty", nameof(nefBytes));

            var stopwatch = Stopwatch.StartNew();
            options ??= new WebGuiOptions();
            outputDirectory ??= Path.Combine(Path.GetTempPath(), $"{contractName}_gui_{DateTime.Now:yyyyMMdd_HHmmss}");

            try
            {
                // Create output directory
                Directory.CreateDirectory(outputDirectory);

                var result = new WebGuiGenerationResult();
                var generatedFiles = new List<string>();

                // Generate main HTML file
                var htmlContent = GenerateHtmlContent(contractName, manifest, nefBytes, contractHash, options);
                var htmlFilePath = Path.Combine(outputDirectory, "index.html");
                File.WriteAllText(htmlFilePath, htmlContent, Encoding.UTF8);
                generatedFiles.Add(htmlFilePath);

                // Generate CSS file
                var cssContent = _templateEngine.GenerateCssContent(options);
                var cssFilePath = Path.Combine(outputDirectory, "styles.css");
                File.WriteAllText(cssFilePath, cssContent, Encoding.UTF8);
                generatedFiles.Add(cssFilePath);

                // Generate JavaScript file
                var jsContent = _templateEngine.GenerateJavaScriptContent(contractName, manifest, options);
                var jsFilePath = Path.Combine(outputDirectory, "contract.js");
                File.WriteAllText(jsFilePath, jsContent, Encoding.UTF8);
                generatedFiles.Add(jsFilePath);

                // Generate configuration file
                var configContent = GenerateConfigurationFile(contractName, manifest, contractHash, options);
                var configFilePath = Path.Combine(outputDirectory, "config.json");
                File.WriteAllText(configFilePath, configContent, Encoding.UTF8);
                generatedFiles.Add(configFilePath);

                // Copy static assets if needed
                CopyStaticAssets(outputDirectory, generatedFiles);

                stopwatch.Stop();

                // Build result
                result.Success = true;
                result.HtmlFilePath = htmlFilePath;
                result.OutputDirectory = outputDirectory;
                result.GeneratedFiles = generatedFiles;
                result.HtmlContent = htmlContent;
                result.AccessUrl = $"file://{htmlFilePath}";
                result.Statistics = new WebGuiStatistics
                {
                    MethodCount = manifest.Abi.Methods.Length,
                    EventCount = manifest.Abi.Events.Length,
                    FileCount = generatedFiles.Count,
                    TotalFileSize = generatedFiles.Sum(f => new FileInfo(f).Length),
                    GenerationTimeMs = stopwatch.ElapsedMilliseconds
                };

                return result;
            }
            catch (Exception ex)
            {
                return WebGuiGenerationResult.Failure($"Web GUI generation failed: {ex.Message}");
            }
        }

        public string GenerateHtmlContent(
            string contractName,
            ContractManifest manifest,
            byte[] nefBytes,
            string? contractHash = null,
            WebGuiOptions? options = null)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(contractName))
                throw new ArgumentException("Contract name cannot be null or empty", nameof(contractName));
            if (manifest == null)
                throw new ArgumentNullException(nameof(manifest));
            if (nefBytes == null || nefBytes.Length == 0)
                throw new ArgumentException("NEF bytes cannot be null or empty", nameof(nefBytes));

            options ??= new WebGuiOptions();

            var templateData = new HtmlTemplateData
            {
                ContractName = contractName,
                ContractHash = contractHash,
                Manifest = manifest,
                NefBytes = nefBytes,
                Options = options,
                GeneratedAt = DateTime.UtcNow
            };

            return _templateEngine.GenerateHtml(templateData);
        }

        private string GenerateConfigurationFile(
            string contractName,
            ContractManifest manifest,
            string? contractHash,
            WebGuiOptions options)
        {
            var config = new
            {
                contract = new
                {
                    name = contractName,
                    hash = contractHash,
                    abi = new
                    {
                        methods = manifest.Abi.Methods.Select(m => new
                        {
                            name = m.Name,
                            parameters = m.Parameters.Select(p => new
                            {
                                name = p.Name,
                                type = p.Type.ToString()
                            }).ToArray(),
                            returnType = m.ReturnType.ToString(),
                            safe = m.Safe
                        }).ToArray(),
                        events = manifest.Abi.Events.Select(e => new
                        {
                            name = e.Name,
                            parameters = e.Parameters.Select(p => new
                            {
                                name = p.Name,
                                type = p.Type.ToString()
                            }).ToArray()
                        }).ToArray()
                    }
                },
                network = new
                {
                    rpcEndpoint = options.RpcEndpoint,
                    magic = options.NetworkMagic
                },
                features = new
                {
                    transactionHistory = options.IncludeTransactionHistory,
                    balanceMonitoring = options.IncludeBalanceMonitoring,
                    methodInvocation = options.IncludeMethodInvocation,
                    stateMonitoring = options.IncludeStateMonitoring,
                    eventMonitoring = options.IncludeEventMonitoring,
                    walletConnection = options.IncludeWalletConnection,
                    advancedFeatures = options.ShowAdvancedFeatures
                },
                ui = new
                {
                    refreshInterval = options.RefreshInterval,
                    darkTheme = options.DarkTheme
                }
            };

            return JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        private void CopyStaticAssets(string outputDirectory, List<string> generatedFiles)
        {
            // For now, we'll generate everything inline
            // In the future, we could copy static assets like icons, libraries, etc.
        }
    }

    /// <summary>
    /// Template data for HTML generation
    /// </summary>
    internal class HtmlTemplateData
    {
        public string ContractName { get; set; } = string.Empty;
        public string? ContractHash { get; set; }
        public ContractManifest Manifest { get; set; } = new();
        public byte[] NefBytes { get; set; } = Array.Empty<byte>();
        public WebGuiOptions Options { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }
}
