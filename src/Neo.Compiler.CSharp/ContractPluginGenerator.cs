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
        public static void GeneratePlugin(string contractName, ContractManifest manifest, UInt160 contractHash, string outputPath)
        {
            string pluginName = $"{contractName}Plugin";
            string pluginPath = Path.Combine(outputPath, pluginName);
            Directory.CreateDirectory(pluginPath);

            // Generate main plugin file
            GenerateMainPluginFile(pluginName, contractName, manifest, contractHash, pluginPath);

            // Generate project file
            GenerateProjectFile(pluginName, pluginPath);

            // Generate configuration file
            GenerateConfigurationFile(pluginName, contractHash, pluginPath);

            // Generate RPC methods file
            GenerateRpcMethodsFile(pluginName, contractName, manifest, contractHash, pluginPath);

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
            sourceCode.WriteLine("using System.Threading.Tasks;");
            sourceCode.WriteLine("using Neo;");
            sourceCode.WriteLine("using Neo.Plugins;");
            sourceCode.WriteLine("using Neo.Network.P2P.Payloads;");
            sourceCode.WriteLine("using Neo.Persistence;");
            sourceCode.WriteLine("using Neo.SmartContract;");
            sourceCode.WriteLine("using Neo.SmartContract.Native;");
            sourceCode.WriteLine("using Neo.VM;");
            sourceCode.WriteLine("using Neo.Wallets;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"namespace Neo.Plugins.{pluginName}");
            sourceCode.WriteLine("{");
            sourceCode.WriteLine($"    public class {pluginName} : Plugin");
            sourceCode.WriteLine("    {");
            sourceCode.WriteLine($"        public override string Name => \"{pluginName}\";");
            sourceCode.WriteLine($"        public override string Description => \"Plugin for interacting with {contractName} contract\";");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        private NeoSystem _system;");
            sourceCode.WriteLine($"        private {contractName}RpcMethods _rpcMethods;");
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
            sourceCode.WriteLine($"            _rpcMethods = new {contractName}RpcMethods(this, _contractWrapper);");
            sourceCode.WriteLine();
            sourceCode.WriteLine("            // RPC method registration will be handled by RpcServer plugin if available");
            sourceCode.WriteLine($"            Log($\"{{Name}}: Initialized for {contractName} contract\", LogLevel.Info);");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        public override void Dispose()");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            // Cleanup if needed");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"        public {contractName}Wrapper GetContractWrapper() => _contractWrapper;");
            sourceCode.WriteLine($"        public {contractName}RpcMethods GetRpcMethods() => _rpcMethods;");
            sourceCode.WriteLine("    }");
            sourceCode.WriteLine("}");

            File.WriteAllText(Path.Combine(pluginPath, $"{pluginName}.cs"), builder.ToString());
        }

        private static void GenerateProjectFile(string pluginName, string pluginPath)
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
            sourceCode.WriteLine("    <ProjectReference Include=\"../../neo/src/Neo/Neo.csproj\" />");
            sourceCode.WriteLine("    <ProjectReference Include=\"../../neo/src/Plugins/RpcServer/RpcServer.csproj\" />");
            sourceCode.WriteLine("  </ItemGroup>");
            sourceCode.WriteLine();
            sourceCode.WriteLine("  <ItemGroup>");
            sourceCode.WriteLine($"    <None Update=\"{pluginName}.json\" CopyToOutputDirectory=\"PreserveNewest\" />");
            sourceCode.WriteLine("  </ItemGroup>");
            sourceCode.WriteLine("</Project>");

            File.WriteAllText(Path.Combine(pluginPath, $"{pluginName}.csproj"), builder.ToString());
        }

        private static void GenerateConfigurationFile(string pluginName, UInt160 contractHash, string pluginPath)
        {
            var config = new JObject
            {
                ["PluginConfiguration"] = new JObject
                {
                    ["ContractHash"] = contractHash.ToString(),
                    ["Network"] = 860833102, // Default to Neo TestNet
                    ["MaxGasPerTransaction"] = 50_00000000L, // 50 GAS
                    ["DefaultAccount"] = "" // Will be set by user
                },
                ["Dependency"] = new JArray("RpcServer")
            };

            File.WriteAllText(Path.Combine(pluginPath, $"{pluginName}.json"), config.ToString(true));
        }

        private static void GenerateRpcMethodsFile(string pluginName, string contractName, ContractManifest manifest, UInt160 contractHash, string pluginPath)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder) { NewLine = "\n" };

            sourceCode.WriteLine($"// RPC methods for {contractName}");
            sourceCode.WriteLine();
            sourceCode.WriteLine("using System;");
            sourceCode.WriteLine("using System.Linq;");
            sourceCode.WriteLine("using System.Numerics;");
            sourceCode.WriteLine("using System.Threading.Tasks;");
            sourceCode.WriteLine("using Neo.Json;");
            sourceCode.WriteLine("using Neo.Network.RPC;");
            sourceCode.WriteLine("using Neo.Cryptography.ECC;");
            sourceCode.WriteLine("using Neo.SmartContract;");
            sourceCode.WriteLine("using Neo.SmartContract.Native;");
            sourceCode.WriteLine("using Neo.VM;");
            sourceCode.WriteLine("using Neo.VM.Types;");
            sourceCode.WriteLine("using Neo.Wallets;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"namespace Neo.Plugins.{pluginName}");
            sourceCode.WriteLine("{");
            sourceCode.WriteLine($"    public class {contractName}RpcMethods");
            sourceCode.WriteLine("    {");
            sourceCode.WriteLine($"        private readonly {pluginName} _plugin;");
            sourceCode.WriteLine($"        private readonly {contractName}Wrapper _wrapper;");
            sourceCode.WriteLine();
            sourceCode.WriteLine($"        public {contractName}RpcMethods({pluginName} plugin, {contractName}Wrapper wrapper)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            _plugin = plugin;");
            sourceCode.WriteLine("            _wrapper = wrapper;");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        public JObject[] GetRpcMethods()");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            var methods = new System.Collections.Generic.List<JObject>();");
            sourceCode.WriteLine();

            // Register each contract method as an RPC method
            foreach (var method in manifest.Abi.Methods.Where(m => !m.Name.StartsWith('_')))
            {
                string rpcMethodName = $"{contractName.ToLower()}_{method.Name.ToLower()}";
                sourceCode.WriteLine($"            methods.Add(new JObject");
                sourceCode.WriteLine("            {");
                sourceCode.WriteLine($"                [\"name\"] = \"{rpcMethodName}\",");
                sourceCode.WriteLine($"                [\"handler\"] = (Func<JArray, Task<JToken>>){method.Name}RpcHandler,");

                var parameters = new JArray();
                foreach (var param in method.Parameters)
                {
                    parameters.Add(new JObject
                    {
                        ["name"] = param.Name,
                        ["type"] = param.Type.ToString()
                    });
                }
                sourceCode.WriteLine($"                [\"parameters\"] = {parameters.ToString(false)},");
                sourceCode.WriteLine($"                [\"returntype\"] = \"{method.ReturnType}\",");
                sourceCode.WriteLine($"                [\"safe\"] = {method.Safe.ToString().ToLower()}");
                sourceCode.WriteLine("            });");
                sourceCode.WriteLine();
            }

            sourceCode.WriteLine("            return methods.ToArray();");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();

            // Generate RPC handler methods
            foreach (var method in manifest.Abi.Methods.Where(m => !m.Name.StartsWith('_')))
            {
                GenerateRpcHandlerMethod(sourceCode, contractName, method);
            }

            // Add helper methods
            sourceCode.WriteLine("        private object ParseParameter(JToken token, string paramName, ContractParameterType type)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            try");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                return type switch");
            sourceCode.WriteLine("                {");
            sourceCode.WriteLine("                    ContractParameterType.Boolean => token.GetBoolean(),");
            sourceCode.WriteLine("                    ContractParameterType.Integer => BigInteger.Parse(token.GetString()),");
            sourceCode.WriteLine("                    ContractParameterType.String => token.GetString(),");
            sourceCode.WriteLine("                    ContractParameterType.ByteArray => Convert.FromBase64String(token.GetString()),");
            sourceCode.WriteLine("                    ContractParameterType.Hash160 => UInt160.Parse(token.GetString()),");
            sourceCode.WriteLine("                    ContractParameterType.Hash256 => UInt256.Parse(token.GetString()),");
            sourceCode.WriteLine("                    ContractParameterType.PublicKey => ECPoint.Parse(token.GetString(), ECCurve.Secp256r1),");
            sourceCode.WriteLine("                    ContractParameterType.Array => ((JArray)token).Select(t => ParseParameter(t, paramName, ContractParameterType.Any)).ToArray(),");
            sourceCode.WriteLine("                    _ => token");
            sourceCode.WriteLine("                };");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("            catch (Exception ex)");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine($"                throw new ArgumentException($\"Invalid parameter '{{paramName}}': {{ex.Message}}\");");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        private JToken FormatResult(object result)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            if (result == null) return JToken.Null;");
            sourceCode.WriteLine("            if (result is BigInteger bi) return bi.ToString();");
            sourceCode.WriteLine("            if (result is byte[] bytes) return Convert.ToBase64String(bytes);");
            sourceCode.WriteLine("            if (result is UInt160 hash160) return hash160.ToString();");
            sourceCode.WriteLine("            if (result is UInt256 hash256) return hash256.ToString();");
            sourceCode.WriteLine("            if (result is ECPoint point) return point.ToString();");
            sourceCode.WriteLine("            if (result is object[] array) return new JArray(array.Select(FormatResult));");
            sourceCode.WriteLine("            return JToken.FromObject(result);");
            sourceCode.WriteLine("        }");
            sourceCode.WriteLine();
            sourceCode.WriteLine("        private JObject CreateErrorResponse(string message)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            return new JObject");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                [\"error\"] = new JObject");
            sourceCode.WriteLine("                {");
            sourceCode.WriteLine("                    [\"code\"] = -32602,");
            sourceCode.WriteLine("                    [\"message\"] = message");
            sourceCode.WriteLine("                }");
            sourceCode.WriteLine("            };");
            sourceCode.WriteLine("        }");

            sourceCode.WriteLine("    }");
            sourceCode.WriteLine("}");

            File.WriteAllText(Path.Combine(pluginPath, $"{contractName}RpcMethods.cs"), builder.ToString());
        }

        private static void GenerateRpcHandlerMethod(StringWriter sourceCode, string contractName, ContractMethodDescriptor method)
        {
            sourceCode.WriteLine($"        private async Task<JToken> {method.Name}RpcHandler(JArray @params)");
            sourceCode.WriteLine("        {");
            sourceCode.WriteLine("            try");
            sourceCode.WriteLine("            {");

            // Parse parameters
            if (method.Parameters.Length > 0)
            {
                sourceCode.WriteLine("                // Parse parameters");
                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    var param = method.Parameters[i];
                    sourceCode.WriteLine($"                var {param.Name} = ParseParameter(@params[{i}], \"{param.Name}\", ContractParameterType.{param.Type});");
                }
                sourceCode.WriteLine();

                // Call wrapper method with parameters
                string parameters = string.Join(", ", method.Parameters.Select(p => p.Name));
                sourceCode.WriteLine($"                var result = await _wrapper.{method.Name}Async({parameters});");
            }
            else
            {
                // Call wrapper method without parameters
                sourceCode.WriteLine($"                var result = await _wrapper.{method.Name}Async();");
            }

            sourceCode.WriteLine("                return FormatResult(result);");
            sourceCode.WriteLine("            }");
            sourceCode.WriteLine("            catch (Exception ex)");
            sourceCode.WriteLine("            {");
            sourceCode.WriteLine("                return CreateErrorResponse(ex.Message);");
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
            sourceCode.WriteLine("using Neo.Network.P2P.Payloads;");
            sourceCode.WriteLine("using Neo.Network.RPC;");
            sourceCode.WriteLine("using Neo.SmartContract;");
            sourceCode.WriteLine("using Neo.SmartContract.Native;");
            sourceCode.WriteLine("using Neo.VM;");
            sourceCode.WriteLine("using Neo.VM.Types;");
            sourceCode.WriteLine("using Neo.Wallets;");
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
