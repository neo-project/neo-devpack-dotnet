// Copyright (C) 2015-2025 The Neo Project.
//
// ContractPluginGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.Json;
using Neo.Extensions;

namespace Neo.Compiler
{
    public static class ContractPluginGenerator
    {
        /// <summary>
        /// Generate a complete Neo N3 plugin project for interacting with a contract
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="manifest">Contract manifest</param>
        /// <param name="contractHash">Contract hash</param>
        /// <param name="outputPath">Base output directory</param>
        /// <param name="options">Compilation options with plugin configuration</param>
        public static void GeneratePlugin(string contractName, ContractManifest manifest, UInt160 contractHash, string outputPath, Options? options = null)
        {
            string pluginName = $"{contractName}Plugin";
            string pluginPath = Path.Combine(outputPath, pluginName);
            Directory.CreateDirectory(pluginPath);

            // Generate main plugin file
            GenerateMainPluginFile(pluginName, contractName, manifest, contractHash, pluginPath);

            // Generate project file
            GenerateProjectFile(pluginName, pluginPath, options);

            // Generate configuration file
            GenerateConfigurationFile(pluginName, contractHash, pluginPath, options);

            // Generate CLI commands file
            GenerateCliCommandsFile(pluginName, contractName, manifest, contractHash, pluginPath);

            // Generate contract wrapper file
            GenerateContractWrapperFile(pluginName, contractName, manifest, contractHash, pluginPath);

            Console.WriteLine($"Created plugin project at: {pluginPath}");
        }

        private static void GenerateMainPluginFile(string pluginName, string contractName, ContractManifest manifest, UInt160 contractHash, string pluginPath)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder) { NewLine = "\n" };

            sourceCode.WriteLine("// Auto-generated plugin for " + contractName);
            sourceCode.WriteLine("// Contract Hash: " + contractHash.ToString());
            sourceCode.WriteLine();
            sourceCode.WriteLine("using System;");
            sourceCode.WriteLine("using Neo;");
            sourceCode.WriteLine("using Neo.ConsoleService;");
            sourceCode.WriteLine("using Neo.Plugins;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"namespace Neo.Plugins.{pluginName}");
            sourceCode.WriteLine("{");
            sourceCode.WriteLine($"    public class {pluginName} : Plugin");
            sourceCode.WriteLine("    {");
            sourceCode.WriteLine($"        public override string Name => \"{pluginName}\";");
            sourceCode.WriteLine($"        public override string Description => \"CLI plugin for interacting with {contractName} contract\";");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        private NeoSystem _system;");
            sourceCode.WriteLine($"        private {contractName}Commands _{contractName.ToLower()}Commands;");
            sourceCode.WriteLine($"        private {contractName}Wrapper _contractWrapper;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"        public {pluginName}()");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        protected override void Configure()");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            // Load configuration if needed");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        protected override void OnSystemLoaded(NeoSystem system)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            _system = system;");
            sourceCode.WriteLine($"            _contractWrapper = new {contractName}Wrapper(system);");
            sourceCode.WriteLine($"            _{contractName.ToLower()}Commands = new {contractName}Commands(_contractWrapper);");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            // Register CLI commands");
            sourceCode.WriteLine($"            ConsoleHelper.RegisterCommand(\"{contractName.ToLower()}\", _{contractName.ToLower()}Commands.Handle);");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"            Log($\"{{Name}}: CLI commands registered for {contractName} contract\", LogLevel.Info);");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        public override void Dispose()");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            // Cleanup if needed");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine("    }");
            sourceCode.WriteLine("}");

            File.WriteAllText(Path.Combine(pluginPath, $"{pluginName}.cs"), builder.ToString());
        }

        private static void GenerateProjectFile(string pluginName, string pluginPath, Options? options = null)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder) { NewLine = "\n" };

            sourceCode.WriteLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            sourceCode.WriteLine("  <PropertyGroup>");
            sourceCode.WriteLine("    <TargetFramework>net9.0</TargetFramework>");
            sourceCode.WriteLine("    <LangVersion>latest</LangVersion>");
            sourceCode.WriteLine("    <Nullable>enable</Nullable>");
            sourceCode.WriteLine("    <EnableDynamicLoading>true</EnableDynamicLoading>");
            sourceCode.WriteLine("  </PropertyGroup>");
            sourceCode.WriteLine();
            sourceCode.WriteLine("  <ItemGroup>");
            sourceCode.WriteLine($"    <PackageReference Include=\"Neo\" Version=\"{options?.PluginNeoVersion ?? "3.*"}\" />");
            sourceCode.WriteLine("  </ItemGroup>");
            sourceCode.WriteLine();
            sourceCode.WriteLine("  <ItemGroup>");
            sourceCode.WriteLine($"    <None Update=\"{pluginName}.json\" CopyToOutputDirectory=\"PreserveNewest\" />");
            sourceCode.WriteLine("  </ItemGroup>");
            sourceCode.WriteLine("</Project>");

            File.WriteAllText(Path.Combine(pluginPath, $"{pluginName}.csproj"), builder.ToString());
        }

        private static void GenerateConfigurationFile(string pluginName, UInt160 contractHash, string pluginPath, Options? options = null)
        {
            var config = new JObject
            {
                ["PluginConfiguration"] = new JObject
                {
                    ["ContractHash"] = contractHash.ToString(),
                    ["Network"] = options?.PluginNetworkId ?? 860833102, // Default to Neo TestNet
                    ["MaxGasPerTransaction"] = options?.PluginMaxGas ?? 50_00000000L, // 50 GAS
                    ["DefaultAccount"] = "" // Will be set by user
                }
            };

            File.WriteAllText(Path.Combine(pluginPath, $"{pluginName}.json"), config.ToString(true));
        }

        private static void GenerateCliCommandsFile(string pluginName, string contractName, ContractManifest manifest, UInt160 contractHash, string pluginPath)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder) { NewLine = "\n" };

            sourceCode.WriteLine($"// CLI commands for {contractName}");
            sourceCode.WriteLine();
            sourceCode.WriteLine("using System;");
            sourceCode.WriteLine("using System.Linq;");
            sourceCode.WriteLine("using System.Numerics;");
            sourceCode.WriteLine("using Neo;");
            sourceCode.WriteLine("using Neo.ConsoleService;");
            sourceCode.WriteLine("using Neo.Cryptography.ECC;");
            sourceCode.WriteLine("using Neo.SmartContract;");
            sourceCode.WriteLine("using Neo.VM.Types;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"namespace Neo.Plugins.{pluginName}");
            sourceCode.WriteLine("{");
            sourceCode.WriteLine($"    public class {contractName}Commands");
            sourceCode.WriteLine("    {");
            sourceCode.WriteLine($"        private readonly {contractName}Wrapper _wrapper;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"        public {contractName}Commands({contractName}Wrapper wrapper)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            _wrapper = wrapper;");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        public async void Handle(string[] args)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            if (args.Length == 0)");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                ShowHelp();");
            sourceCode.WriteLine("                return;");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            try");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                string command = args[0].ToLower();");
            sourceCode.WriteLine("                string[] parameters = args.Skip(1).ToArray();");
            sourceCode.WriteLine();
            sourceCode.WriteLine("                switch (command)");
            sourceCode.WriteLine("                {");

            // Generate case statements for each contract method
            foreach (var method in manifest.Abi.Methods.Where(m => !m.Name.StartsWith('_')))
            {
                sourceCode.WriteLine($"                    case \"{method.Name.ToLower()}\":");
                sourceCode.WriteLine($"                        await Handle{method.Name}(parameters);");
                sourceCode.WriteLine("                        break;");
            }

            sourceCode.WriteLine("                    case \"help\":");
            sourceCode.WriteLine("                    case \"--help\":");
            sourceCode.WriteLine("                    case \"-h\":");
            sourceCode.WriteLine("                        ShowHelp();");
            sourceCode.WriteLine("                        break;");
            sourceCode.WriteLine("                    default:");
            sourceCode.WriteLine($"                        ConsoleHelper.Warning($\"Unknown command: {{command}}\");");
            sourceCode.WriteLine("                        ShowHelp();");
            sourceCode.WriteLine("                        break;");
            sourceCode.WriteLine("                }");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("            catch (Exception ex)");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine($"                ConsoleHelper.Error($\"Error executing command: {{ex.Message}}\");");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();

            // Generate method handlers for each contract method
            foreach (var method in manifest.Abi.Methods.Where(m => !m.Name.StartsWith('_')))
            {
                GenerateCliMethodHandler(sourceCode, method);
            }

            // Generate help method
            sourceCode.WriteLine("        private void ShowHelp()");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine($"            ConsoleHelper.Info(\"{contractName} Contract Commands:\");");
            sourceCode.WriteLine("            ConsoleHelper.Info(\"\");");

            foreach (var method in manifest.Abi.Methods.Where(m => !m.Name.StartsWith('_')))
            {
                string paramsList = string.Join(" ", method.Parameters.Select(p => $"<{p.Name}:{p.Type}>"));
                string safeIndicator = method.Safe ? " [SAFE]" : "";
                sourceCode.WriteLine($"            ConsoleHelper.Info(\"  {method.Name.ToLower()} {paramsList}{safeIndicator}\");");
            }

            sourceCode.WriteLine("            ConsoleHelper.Info(\"\");");
            sourceCode.WriteLine($"            ConsoleHelper.Info(\"Contract Hash: {contractHash}\");");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();

            // Generate parameter parsing helper methods
            sourceCode.WriteLine("        private object ParseParameter(string value, ContractParameterType type)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            try");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                return type switch");
            sourceCode.WriteLine("                {");
            sourceCode.WriteLine("                    ContractParameterType.Boolean => bool.Parse(value),");
            sourceCode.WriteLine("                    ContractParameterType.Integer => BigInteger.Parse(value),");
            sourceCode.WriteLine("                    ContractParameterType.String => value,");
            sourceCode.WriteLine("                    ContractParameterType.ByteArray => Convert.FromBase64String(value),");
            sourceCode.WriteLine("                    ContractParameterType.Hash160 => UInt160.Parse(value),");
            sourceCode.WriteLine("                    ContractParameterType.Hash256 => UInt256.Parse(value),");
            sourceCode.WriteLine("                    ContractParameterType.PublicKey => ECPoint.Parse(value, ECCurve.Secp256r1),");
            sourceCode.WriteLine("                    _ => value");
            sourceCode.WriteLine("                };");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("            catch (Exception ex)");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine($"                throw new ArgumentException($\"Invalid parameter value '{{value}}' for type {{type}}: {{ex.Message}}\");");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        private void DisplayResult(object result)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            if (result == null)");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                ConsoleHelper.Info(\"Result: null\");");
            sourceCode.WriteLine("                return;");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            string output = result switch");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                BigInteger bi => bi.ToString(),");
            sourceCode.WriteLine("                byte[] bytes => Convert.ToBase64String(bytes),");
            sourceCode.WriteLine("                UInt160 hash160 => hash160.ToString(),");
            sourceCode.WriteLine("                UInt256 hash256 => hash256.ToString(),");
            sourceCode.WriteLine("                ECPoint point => point.ToString(),");
            sourceCode.WriteLine("                bool b => b.ToString(),");
            sourceCode.WriteLine("                string s => s,");
            sourceCode.WriteLine("                _ => result.ToString()");
            sourceCode.WriteLine("            };");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"            ConsoleHelper.Info($\"Result: {{output}}\");");
            sourceCode.WriteLine("        }");

            sourceCode.WriteLine("    }");
            sourceCode.WriteLine("}");

            File.WriteAllText(Path.Combine(pluginPath, $"{contractName}Commands.cs"), builder.ToString());
        }

        private static void GenerateCliMethodHandler(StringWriter sourceCode, ContractMethodDescriptor method)
        {
            sourceCode.WriteLine($"        private async Task Handle{method.Name}(string[] parameters)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine($"            // {method.Name}: {(method.Safe ? "Safe method" : "State-changing method")}");

            if (method.Parameters.Length > 0)
            {
                sourceCode.WriteLine($"            if (parameters.Length != {method.Parameters.Length})");
                sourceCode.WriteLine("            {");
                string paramsList = string.Join(" ", method.Parameters.Select(p => $"<{p.Name}:{p.Type}>"));
                sourceCode.WriteLine($"                ConsoleHelper.Error($\"Usage: {method.Name.ToLower()} {paramsList}\");");
                sourceCode.WriteLine("                return;");
                sourceCode.WriteLine("            }");
                sourceCode.WriteLine();
                sourceCode.WriteLine("            try");
                sourceCode.WriteLine("            {");

                // Parse parameters
                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    var param = method.Parameters[i];
                    sourceCode.WriteLine($"                var {param.Name} = ({ConvertTypeToString(param.Type)})ParseParameter(parameters[{i}], ContractParameterType.{param.Type});");
                }

                sourceCode.WriteLine();
                string parameters = string.Join(", ", method.Parameters.Select(p => p.Name));
                sourceCode.WriteLine($"                var result = await _wrapper.{method.Name}Async({parameters});");
            }
            else
            {
                sourceCode.WriteLine("            try");
                sourceCode.WriteLine("            {");
                sourceCode.WriteLine($"                var result = await _wrapper.{method.Name}Async();");
            }

            sourceCode.WriteLine("                DisplayResult(result);");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("            catch (Exception ex)");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine($"                ConsoleHelper.Error($\"Error calling {method.Name}: {{ex.Message}}\");");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
        }

        private static void GenerateContractWrapperFile(string pluginName, string contractName, ContractManifest manifest, UInt160 contractHash, string pluginPath)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder) { NewLine = "\n" };

            sourceCode.WriteLine($"// Contract wrapper for {contractName}");
            sourceCode.WriteLine();
            sourceCode.WriteLine("using System;");
            sourceCode.WriteLine("using System.Linq;");
            sourceCode.WriteLine("using System.Numerics;");
            sourceCode.WriteLine("using System.Threading.Tasks;");
            sourceCode.WriteLine("using Neo;");
            sourceCode.WriteLine("using Neo.Cryptography.ECC;");
            sourceCode.WriteLine("using Neo.SmartContract;");
            sourceCode.WriteLine("using Neo.VM;");
            sourceCode.WriteLine("using Neo.VM.Types;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"namespace Neo.Plugins.{pluginName}");
            sourceCode.WriteLine("{");
            sourceCode.WriteLine($"    public class {contractName}Wrapper");
            sourceCode.WriteLine("    {");
            sourceCode.WriteLine($"        private static readonly UInt160 ContractHash = UInt160.Parse(\"{contractHash}\");");
            sourceCode.WriteLine("        private readonly NeoSystem _system;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"        public {contractName}Wrapper(NeoSystem system)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            _system = system;");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();

            // Generate wrapper methods for each contract method
            foreach (var method in manifest.Abi.Methods.Where(m => !m.Name.StartsWith('_')))
            {
                GenerateWrapperMethod(sourceCode, method);
            }

            // Add helper methods
            sourceCode.WriteLine("        private async Task<T> InvokeAsync<T>(string method, params object[] args)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            using var snapshot = _system.GetSnapshot();");
            sourceCode.WriteLine("            var script = ContractHash.MakeScript(method, args);");
            sourceCode.WriteLine("            var engine = ApplicationEngine.Run(script, snapshot, settings: _system.Settings);");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            if (engine.State == VMState.FAULT)");
            sourceCode.WriteLine("                throw new Exception($\"Contract execution failed: {engine.FaultException?.Message}\");");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            if (engine.ResultStack.Count == 0)");
            sourceCode.WriteLine("                return default(T);");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            var result = engine.ResultStack.Pop();");
            sourceCode.WriteLine("            return ConvertToType<T>(result);");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        private T ConvertToType<T>(StackItem item)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            // Implement type conversion logic based on T");
            sourceCode.WriteLine("            if (typeof(T) == typeof(BigInteger))");
            sourceCode.WriteLine("                return (T)(object)item.GetInteger();");
            sourceCode.WriteLine("            if (typeof(T) == typeof(string))");
            sourceCode.WriteLine("                return (T)(object)item.GetString();");
            sourceCode.WriteLine("            if (typeof(T) == typeof(bool))");
            sourceCode.WriteLine("                return (T)(object)item.GetBoolean();");
            sourceCode.WriteLine("            if (typeof(T) == typeof(byte[]))");
            sourceCode.WriteLine("                return (T)(object)item.GetSpan().ToArray();");
            sourceCode.WriteLine("            if (typeof(T) == typeof(UInt160))");
            sourceCode.WriteLine("                return (T)(object)new UInt160(item.GetSpan());");
            sourceCode.WriteLine("            if (typeof(T) == typeof(UInt256))");
            sourceCode.WriteLine("                return (T)(object)new UInt256(item.GetSpan());");
            sourceCode.WriteLine("            if (typeof(T) == typeof(ECPoint))");
            sourceCode.WriteLine("                return (T)(object)ECPoint.DecodePoint(item.GetSpan(), ECCurve.Secp256r1);");
            sourceCode.WriteLine("            if (typeof(T) == typeof(object[]))");
            sourceCode.WriteLine("                return (T)(object)((Array)item).Select(i => ConvertToType<object>(i)).ToArray();");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            return default(T);");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine("    }");
            sourceCode.WriteLine("}");

            File.WriteAllText(Path.Combine(pluginPath, $"{contractName}Wrapper.cs"), builder.ToString());
        }

        private static void GenerateWrapperMethod(StringWriter sourceCode, ContractMethodDescriptor method)
        {
            string returnType = ConvertTypeToString(method.ReturnType);
            string asyncReturnType = method.ReturnType == ContractParameterType.Void ? "Task" : $"Task<{returnType}>";
            string parameters = string.Join(", ", method.Parameters.Select(p => $"{ConvertTypeToString(p.Type)} {p.Name}"));
            string parameterNames = string.Join(", ", method.Parameters.Select(p => p.Name));

            sourceCode.WriteLine($"        public async {asyncReturnType} {method.Name}Async({parameters})");
            sourceCode.WriteLine("        {");

            if (method.ReturnType == ContractParameterType.Void)
            {
                sourceCode.WriteLine($"            await InvokeAsync<object>(\"{method.Name}\"{(parameterNames.Length > 0 ? ", " + parameterNames : "")});");
            }
            else
            {
                sourceCode.WriteLine($"            return await InvokeAsync<{returnType}>(\"{method.Name}\"{(parameterNames.Length > 0 ? ", " + parameterNames : "")});");
            }

            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
        }

        private static string ConvertTypeToString(ContractParameterType type)
        {
            return type switch
            {
                ContractParameterType.Any => "object",
                ContractParameterType.Boolean => "bool",
                ContractParameterType.Integer => "BigInteger",
                ContractParameterType.ByteArray => "byte[]",
                ContractParameterType.String => "string",
                ContractParameterType.Hash160 => "UInt160",
                ContractParameterType.Hash256 => "UInt256",
                ContractParameterType.PublicKey => "ECPoint",
                ContractParameterType.Signature => "byte[]",
                ContractParameterType.Array => "object[]",
                ContractParameterType.Map => "Dictionary<object, object>",
                ContractParameterType.InteropInterface => "object",
                ContractParameterType.Void => "void",
                _ => "object",
            };
        }
    }
}
