// Copyright (C) 2015-2025 The Neo Project.
//
// ContractInterfaceGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Generates C# interface code from contract manifests.
    /// This allows for type-safe contract invocation with IntelliSense support.
    /// </summary>
    public static class ContractInterfaceGenerator
    {
        /// <summary>
        /// Generates a C# interface from a contract manifest.
        /// </summary>
        /// <param name="manifest">The contract manifest</param>
        /// <param name="contractName">The name for the generated contract interface</param>
        /// <param name="namespaceName">The namespace for the generated interface</param>
        /// <returns>The generated C# interface code</returns>
        public static string GenerateInterface(ContractManifest? manifest, string contractName, string namespaceName = "GeneratedContracts")
        {
            if (!manifest.HasValue)
                throw new ArgumentNullException(nameof(manifest));
            if (string.IsNullOrEmpty(contractName))
                throw new ArgumentNullException(nameof(contractName));

            var sb = new StringBuilder();

            // Add file header
            sb.AppendLine("// This file was auto-generated from a contract manifest.");
            sb.AppendLine("// Do not modify this file manually as it will be overwritten.");
            sb.AppendLine();

            // Add usings
            sb.AppendLine("using Neo.SmartContract.Framework;");
            sb.AppendLine("using Neo.SmartContract.Framework.ContractInvocation;");
            sb.AppendLine("using Neo.SmartContract.Framework.ContractInvocation.Attributes;");
            sb.AppendLine("using Neo.SmartContract.Framework.Services;");
            sb.AppendLine("using System.Numerics;");
            sb.AppendLine();

            // Add namespace
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");

            // Generate interface
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// Interface for {contractName} contract.");
            if (!string.IsNullOrEmpty(manifest.Value.Name))
                sb.AppendLine($"    /// Contract name: {manifest.Value.Name}");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    public interface I{contractName}Contract");
            sb.AppendLine("    {");

            // Generate methods
            foreach (var method in manifest.Value.Abi.Methods.Where(m => m.Name != "_deploy"))
            {
                GenerateMethodSignature(sb, method);
            }

            sb.AppendLine("    }");

            // Generate proxy class
            sb.AppendLine();
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// Proxy class for {contractName} contract invocation.");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    public class {contractName}ContractProxy : ContractProxyBase, I{contractName}Contract");
            sb.AppendLine("    {");
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        /// Initializes a new {contractName}ContractProxy.");
            sb.AppendLine($"        /// </summary>");
            sb.AppendLine($"        /// <param name=\"contractReference\">The contract reference</param>");
            sb.AppendLine($"        public {contractName}ContractProxy(IContractReference contractReference)");
            sb.AppendLine("            : base(contractReference)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();

            // Generate method implementations
            foreach (var method in manifest.Value.Abi.Methods.Where(m => m.Name != "_deploy"))
            {
                GenerateMethodImplementation(sb, method);
            }

            sb.AppendLine("    }");

            // Close namespace
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static void GenerateMethodSignature(StringBuilder sb, ContractMethodDescriptor method)
        {
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        /// Invokes the {method.Name} method.");
            sb.AppendLine($"        /// </summary>");

            // Add parameter documentation
            foreach (var param in method.Parameters)
            {
                sb.AppendLine($"        /// <param name=\"{param.Name}\">The {param.Name} parameter</param>");
            }

            if (method.ReturnType != ContractParameterType.Void)
            {
                sb.AppendLine($"        /// <returns>The method result</returns>");
            }

            // Generate method signature
            var returnType = GetCSharpType(method.ReturnType);
            var parameters = string.Join(", ", method.Parameters.Select(p => $"{GetCSharpType(p.Type)} {p.Name}"));

            sb.AppendLine($"        {returnType} {method.Name}({parameters});");
            sb.AppendLine();
        }

        private static void GenerateMethodImplementation(StringBuilder sb, ContractMethodDescriptor method)
        {
            // Add method attribute if needed
            var isReadOnly = IsReadOnlyMethod(method);
            if (isReadOnly)
            {
                sb.AppendLine($"        [ContractMethod(ReadOnly = true)]");
            }

            // Generate method signature
            var returnType = GetCSharpType(method.ReturnType);
            var parameters = string.Join(", ", method.Parameters.Select(p => $"{GetCSharpType(p.Type)} {p.Name}"));

            sb.AppendLine($"        public {returnType} {method.Name}({parameters})");
            sb.AppendLine("        {");

            // Generate method body
            var paramNames = method.Parameters.Select(p => p.Name).ToList();
            var argsString = paramNames.Count > 0 ? string.Join(", ", paramNames) : "";

            if (method.ReturnType == ContractParameterType.Void)
            {
                if (isReadOnly)
                {
                    sb.AppendLine($"            InvokeReadOnly(\"{method.Name}\"{(string.IsNullOrEmpty(argsString) ? "" : $", {argsString}")});");
                }
                else
                {
                    sb.AppendLine($"            InvokeWithAllFlags(\"{method.Name}\"{(string.IsNullOrEmpty(argsString) ? "" : $", {argsString}")});");
                }
            }
            else
            {
                var castType = returnType == "object" ? "" : $"({returnType})";
                if (isReadOnly)
                {
                    sb.AppendLine($"            return {castType}InvokeReadOnly(\"{method.Name}\"{(string.IsNullOrEmpty(argsString) ? "" : $", {argsString}")});");
                }
                else
                {
                    sb.AppendLine($"            return {castType}InvokeWithAllFlags(\"{method.Name}\"{(string.IsNullOrEmpty(argsString) ? "" : $", {argsString}")});");
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine();
        }

        private static string GetCSharpType(ContractParameterType parameterType)
        {
            return parameterType switch
            {
                ContractParameterType.Boolean => "bool",
                ContractParameterType.Integer => "BigInteger",
                ContractParameterType.ByteArray => "byte[]",
                ContractParameterType.String => "string",
                ContractParameterType.Hash160 => "UInt160",
                ContractParameterType.Hash256 => "UInt256",
                ContractParameterType.PublicKey => "ECPoint",
                ContractParameterType.Signature => "byte[]",
                ContractParameterType.Array => "object[]",
                ContractParameterType.Map => "object",
                ContractParameterType.InteropInterface => "object",
                ContractParameterType.Void => "void",
                _ => "object"
            };
        }

        private static bool IsReadOnlyMethod(ContractMethodDescriptor method)
        {
            // Simple heuristic: methods starting with "get", "is", "has", "check", "view", etc. are likely read-only
            var readOnlyPrefixes = new[] { "get", "is", "has", "check", "view", "read", "query", "find", "search" };
            return readOnlyPrefixes.Any(prefix => method.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}
