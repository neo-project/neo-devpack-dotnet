// Copyright (C) 2015-2024 The Neo Project.
//
// ContractInterfaceGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.Extensions;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler
{
    public static class ContractInterfaceGenerator
    {
        /// <summary>
        /// Generate an interface for a contract
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        /// <param name="nef">NEF file</param>
        /// <param name="manifest">Contract manifest</param>
        /// <param name="contractHash">Contract hash</param>
        /// <returns>The generated interface code</returns>
        public static string GenerateInterface(string contractName, NefFile nef, ContractManifest manifest, UInt160 contractHash)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };

            sourceCode.WriteLine("// Auto-generated interface for " + contractName);
            sourceCode.WriteLine("// Contract Hash: " + contractHash.ToString());
            sourceCode.WriteLine();
            sourceCode.WriteLine("using Neo;");
            sourceCode.WriteLine("using Neo.SmartContract;");
            sourceCode.WriteLine("using Neo.SmartContract.Framework;");
            sourceCode.WriteLine("using Neo.SmartContract.Framework.Attributes;");
            sourceCode.WriteLine("using Neo.SmartContract.Framework.Native;");
            sourceCode.WriteLine("using Neo.SmartContract.Framework.Services;");
            sourceCode.WriteLine("using System;");
            sourceCode.WriteLine("using System.ComponentModel;");
            sourceCode.WriteLine("using System.Numerics;");
            sourceCode.WriteLine();
            sourceCode.WriteLine("namespace " + GetNamespace(contractName));
            sourceCode.WriteLine("{");
            sourceCode.WriteLine($"    [Contract(\"{contractHash}\")]");
            sourceCode.WriteLine($"    public interface I{contractName}");
            sourceCode.WriteLine("    {");
            sourceCode.WriteLine("        [ContractHash]");
            sourceCode.WriteLine("        static extern UInt160 Hash { get; }");
            sourceCode.WriteLine();

            // Process methods for properties
            var (methods, properties) = ProcessAbiMethods(manifest.Abi.Methods);

            // Add properties
            if (properties.Any())
            {
                foreach (var property in properties.OrderBy(p => p.getter.Name))
                {
                    string returnType = ConvertTypeToString(property.getter.ReturnType);
                    string propertyName = GetPropertyName(property.getter.Name);

                    sourceCode.WriteLine($"        {(property.getter.Safe ? "[Safe]" : "")}");

                    if (property.setter != null)
                    {
                        sourceCode.WriteLine($"        {returnType} {propertyName} {{ get; set; }}");
                    }
                    else
                    {
                        sourceCode.WriteLine($"        {returnType} {propertyName} {{ get; }}");
                    }

                    sourceCode.WriteLine();
                }
            }

            // Add methods
            foreach (var method in methods.OrderBy(m => m.Name))
            {
                // Skip internal methods
                if (method.Name.StartsWith('_'))
                    continue;

                string returnType = ConvertTypeToString(method.ReturnType);
                string parameters = string.Join(", ", method.Parameters.Select(p => $"{ConvertTypeToString(p.Type)} {p.Name}"));

                sourceCode.WriteLine($"        {(method.Safe ? "[Safe]" : "")}");
                sourceCode.WriteLine($"        extern {returnType} {method.Name}({parameters});");
                sourceCode.WriteLine();
            }

            sourceCode.WriteLine("    }");
            sourceCode.WriteLine("}");

            return builder.ToString();
        }

        private static string GetNamespace(string contractName)
        {
            return $"Neo.SmartContract.Generated.{contractName}";
        }

        private static string GetPropertyName(string methodName)
        {
            // Extract property name from get/set_X method names
            if (methodName.StartsWith("get_") || methodName.StartsWith("set_"))
            {
                return methodName.Substring(4);
            }
            return methodName;
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
                ContractParameterType.Map => "Map<object, object>",
                ContractParameterType.InteropInterface => "InteropInterface",
                ContractParameterType.Void => "void",
                _ => "object",
            };
        }

        private static (ContractMethodDescriptor[] methods, (ContractMethodDescriptor getter, ContractMethodDescriptor? setter)[] properties)
            ProcessAbiMethods(ContractMethodDescriptor[] methods)
        {
            var properties = new List<(ContractMethodDescriptor getter, ContractMethodDescriptor? setter)>();
            var getters = methods.Where(m => m.Name.StartsWith("get_")).ToArray();
            var remainingMethods = new List<ContractMethodDescriptor>(methods);

            foreach (var getter in getters)
            {
                string propertyName = getter.Name.Substring(4);
                var setter = methods.FirstOrDefault(m => m.Name == $"set_{propertyName}");

                if (setter != null)
                    remainingMethods.Remove(setter);

                properties.Add((getter, setter));
                remainingMethods.Remove(getter);
            }

            return (remainingMethods.ToArray(), properties.ToArray());
        }
    }
}
