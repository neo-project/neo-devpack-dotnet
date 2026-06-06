// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ContractInterfaceGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ContractInterfaceGenerator
    {
        [TestMethod]
        public void TestGenerateInterfaceEscapesAbiNames()
        {
            var manifest = new ContractManifest
            {
                Name = "Bad-Contract",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods =
                    [
                        new ContractMethodDescriptor
                        {
                            Name = "bad\"Name\\x41\\u0042",
                            Parameters =
                            [
                                new ContractParameterDefinition
                                {
                                    Name = "arg-name",
                                    Type = ContractParameterType.String
                                }
                            ],
                            ReturnType = ContractParameterType.Integer,
                            Safe = true
                        }
                    ],
                    Events = []
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create()
            };

            var source = ContractInterfaceGenerator.GenerateInterface(manifest.Name, manifest, UInt160.Zero);
            var diagnostics = CSharpSyntaxTree.ParseText(source).GetDiagnostics().ToArray();

            Assert.AreEqual(0, diagnostics.Length, string.Join("\n", diagnostics.Select(u => u.ToString())));
            StringAssert.Contains(source, "namespace Neo.SmartContract.Generated.Bad_Contract");
            StringAssert.Contains(source, "public interface IBad_Contract");
            StringAssert.Contains(source, "[DisplayName(\"bad\\\"Name\\\\x41\\\\u0042\")]");
            StringAssert.Contains(source, "extern BigInteger bad_Name_x41_u0042(string arg_name);");
        }

        [TestMethod]
        public void TestGenerateInterfaceSanitizesPropertiesAndKeywordContractName()
        {
            var manifest = new ContractManifest
            {
                Name = "class",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods =
                    [
                        new ContractMethodDescriptor
                        {
                            Name = "get_bad-name",
                            Parameters = [],
                            ReturnType = ContractParameterType.String,
                            Safe = true
                        },
                        new ContractMethodDescriptor
                        {
                            Name = "set_bad-name",
                            Parameters =
                            [
                                new ContractParameterDefinition
                                {
                                    Name = "value",
                                    Type = ContractParameterType.String
                                }
                            ],
                            ReturnType = ContractParameterType.Void
                        },
                        new ContractMethodDescriptor
                        {
                            Name = "get_lonely",
                            Parameters = [],
                            ReturnType = ContractParameterType.Integer,
                            Safe = true
                        }
                    ],
                    Events = []
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create()
            };

            var source = ContractInterfaceGenerator.GenerateInterface(manifest.Name, manifest, UInt160.Zero);
            var diagnostics = CSharpSyntaxTree.ParseText(source).GetDiagnostics().ToArray();

            Assert.AreEqual(0, diagnostics.Length, string.Join("\n", diagnostics.Select(u => u.ToString())));
            StringAssert.Contains(source, "namespace Neo.SmartContract.Generated.@class");
            StringAssert.Contains(source, "public interface Iclass");
            StringAssert.Contains(source, "string bad_name { [DisplayName(\"get_bad-name\")] get; [DisplayName(\"set_bad-name\")] set; }");
            StringAssert.Contains(source, "BigInteger lonely { [DisplayName(\"get_lonely\")] get; }");
        }

        [TestMethod]
        public void TestGenerateInterfaceUsesFallbackContractName()
        {
            var manifest = new ContractManifest
            {
                Name = "",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods = [],
                    Events = []
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create()
            };

            var source = ContractInterfaceGenerator.GenerateInterface(manifest.Name, manifest, UInt160.Zero);
            var diagnostics = CSharpSyntaxTree.ParseText(source).GetDiagnostics().ToArray();

            Assert.AreEqual(0, diagnostics.Length, string.Join("\n", diagnostics.Select(u => u.ToString())));
            StringAssert.Contains(source, "namespace Neo.SmartContract.Generated.Contract");
            StringAssert.Contains(source, "public interface IContract");
        }
    }
}
