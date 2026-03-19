// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ManifestStandards.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ContractParameterType = Neo.SmartContract.ContractParameterType;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ManifestStandards
    {
        [TestMethod]
        public void Nep11_InvalidTokensOfAndOwnerOfStillProduceExpectedDiagnostics()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            JObject tokensOf = (JObject)methods.First(m => m!["name"]!.GetString() == "tokensOf")!;
            tokensOf["parameters"]![0]!["type"] = "ByteArray";

            JObject ownerOf = (JObject)methods.First(m => m!["name"]!.GetString() == "ownerOf")!;
            ownerOf["parameters"]![0]!["type"] = "Hash160";

            ContractManifest manifest = ContractManifest.FromJson(json);
            var stdout = new StringWriter();
            TextWriter originalOut = Console.Out;

            try
            {
                Console.SetOut(stdout);
                manifest.CheckStandards();
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            string output = stdout.ToString();
            StringAssert.Contains(output, "tokensOf, it's parameters type is not a Hash160");
            StringAssert.Contains(output, "ownerOf, it's parameters type is not a ByteArray");
        }

        [TestMethod]
        public void Nep11_InvalidTokensOfAndOwnerOfShapeVariantsStillProduceExpectedDiagnostics()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            // Remove tokensOf entirely to hit the helper's null branch.
            JToken tokensOf = methods.First(m => m!["name"]!.GetString() == "tokensOf");
            methods.Remove(tokensOf);

            // Change ownerOf to hit unsafe, return-type, and parameter-type branches while preserving lookup shape.
            JObject ownerOf = (JObject)methods.First(m => m!["name"]!.GetString() == "ownerOf")!;
            ownerOf["safe"] = false;
            ownerOf["returntype"] = "ByteArray";
            ownerOf["parameters"] = new JArray(
                new JObject { ["name"] = "tokenId", ["type"] = "Hash160" });

            ContractManifest manifest = ContractManifest.FromJson(json);
            var stdout = new StringWriter();
            TextWriter originalOut = Console.Out;

            try
            {
                Console.SetOut(stdout);
                manifest.CheckStandards();
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            string output = stdout.ToString();
            StringAssert.Contains(output, "tokensOf, it is not found in the ABI");
            StringAssert.Contains(output, "ownerOf, it is not safe");
            StringAssert.Contains(output, "ownerOf, it's return type is not a Hash160");
            StringAssert.Contains(output, "ownerOf, it's parameters type is not a ByteArray");
        }

        [TestMethod]
        public void Nep11_Helper_Reports_LengthMismatch_For_SingleParameterMethods()
        {
            Type extensionsType = typeof(CompilationEngine).Assembly.GetType("Neo.Compiler.ContractManifestExtensions")!;
            MethodInfo helper = extensionsType.GetMethod(
                "ValidateNep11SingleParameterSafeMethod",
                BindingFlags.NonPublic | BindingFlags.Static)!;

            var errors = new System.Collections.Generic.List<CompilationException>();
            Type methodParameterType = helper.GetParameters()[1].ParameterType;
            Type descriptorType = Nullable.GetUnderlyingType(methodParameterType) ?? methodParameterType;
            object descriptor = Activator.CreateInstance(descriptorType)!;
            SetMember(descriptorType, descriptor, "Name", "ownerOf");
            SetMember(descriptorType, descriptor, "ReturnType", ContractParameterType.ByteArray);
            SetMember(descriptorType, descriptor, "Safe", false);
            SetMember(descriptorType, descriptor, "Parameters", Array.Empty<ContractParameterDefinition>());

            object? boxedDescriptor = methodParameterType == descriptorType
                ? descriptor
                : Activator.CreateInstance(methodParameterType, descriptor);

            helper.Invoke(null,
            [
                errors,
                boxedDescriptor,
                "ownerOf",
                ContractParameterType.Hash160,
                "a Hash160",
                ContractParameterType.ByteArray,
                "a ByteArray"
            ]);

            string[] messages = errors.Select(e => e.Diagnostic.GetMessage()).ToArray();
            CollectionAssert.Contains(messages, "Incomplete or unsafe NEP standard NEP-11 implementation: ownerOf, it is not safe, you should add a 'Safe' attribute to the ownerOf method");
            CollectionAssert.Contains(messages, "Incomplete or unsafe NEP standard NEP-11 implementation: ownerOf, it's return type is not a Hash160");
            CollectionAssert.Contains(messages, "Incomplete or unsafe NEP standard NEP-11 implementation: ownerOf, it's parameters length is not 1");
        }

        private static void SetMember(Type type, object instance, string name, object value)
        {
            FieldInfo? field = type.GetField(name);
            if (field is not null)
            {
                field.SetValue(instance, value);
                return;
            }

            PropertyInfo? property = type.GetProperty(name);
            if (property is not null)
            {
                property.SetValue(instance, value);
                return;
            }

            Assert.Fail($"Could not find field or property '{name}' on {type.FullName}.");
        }
    }
}
