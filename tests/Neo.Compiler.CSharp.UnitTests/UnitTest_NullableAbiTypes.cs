// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableAbiTypes.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableAbiTypes
{
    private const string Source = @"#nullable enable
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public delegate void CollectionsChangedHandler(List<int>? list, Map<string, int>? map);
    public static event CollectionsChangedHandler? OnCollectionsChanged;

    public static void Notify(List<int>? list, Map<string, int>? map) => OnCollectionsChanged!(list, map);

    public static List<int> EchoList(List<int> value) => value;
    public static List<int>? EchoListN(List<int>? value) => value;
    public static List<string> EchoStringList(List<string> value) => value;
    public static List<string>? EchoStringListN(List<string>? value) => value;
    public static List<List<int>>? EchoNestedListN(List<List<int>>? value) => value;
    public static Map<string, int> EchoMap(Map<string, int> value) => value;
    public static Map<string, int>? EchoMapN(Map<string, int>? value) => value;
    public static Map<int, string>? EchoMapN2(Map<int, string>? value) => value;
    public static Iterator<int> EchoIterator(Iterator<int> value) => value;
    public static Iterator<int>? EchoIteratorN(Iterator<int>? value) => value;
    public static int EchoInt(int value) => value;
    public static int? EchoIntN(int? value) => value;
    public static bool EchoBool(bool value) => value;
    public static bool? EchoBoolN(bool? value) => value;
    public static string EchoString(string value) => value;
    public static string? EchoStringN(string? value) => value;
    public static byte[] EchoBytes(byte[] value) => value;
    public static byte[]? EchoBytesN(byte[]? value) => value;
    public static UInt160 EchoHash160(UInt160 value) => value;
    public static UInt160? EchoHash160N(UInt160? value) => value;
    public static UInt256 EchoHash256(UInt256 value) => value;
    public static UInt256? EchoHash256N(UInt256? value) => value;
}";

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.Experimental)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void Nullable_Types_Keep_Their_Abi_Type(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var abi = context.CreateManifest().Abi;

        void Check(string name, ContractParameterType expected)
        {
            var method = abi.GetMethod(name, 1);
            Assert.IsNotNull(method, $"Missing ABI method: {name}");
            Assert.AreEqual(expected, method.ReturnType, $"{name} return type");
            Assert.AreEqual(expected, method.Parameters.Single().Type, $"{name} parameter type");
        }

        Check("echoIterator", ContractParameterType.InteropInterface);
        Check("echoIteratorN", ContractParameterType.InteropInterface);
        Check("echoList", ContractParameterType.Array);
        Check("echoListN", ContractParameterType.Array);
        Check("echoStringList", ContractParameterType.Array);
        Check("echoStringListN", ContractParameterType.Array);
        Check("echoNestedListN", ContractParameterType.Array);
        Check("echoMap", ContractParameterType.Map);
        Check("echoMapN", ContractParameterType.Map);
        Check("echoMapN2", ContractParameterType.Map);
        Check("echoInt", ContractParameterType.Integer);
        Check("echoIntN", ContractParameterType.Integer);
        Check("echoBool", ContractParameterType.Boolean);
        Check("echoBoolN", ContractParameterType.Boolean);
        Check("echoString", ContractParameterType.String);
        Check("echoStringN", ContractParameterType.String);
        Check("echoBytes", ContractParameterType.ByteArray);
        Check("echoBytesN", ContractParameterType.ByteArray);
        Check("echoHash160", ContractParameterType.Hash160);
        Check("echoHash160N", ContractParameterType.Hash160);
        Check("echoHash256", ContractParameterType.Hash256);
        Check("echoHash256N", ContractParameterType.Hash256);

        var parameters = abi.Events.Single(p => p.Name == "OnCollectionsChanged").Parameters;
        CollectionAssert.AreEqual(
            new[] { ContractParameterType.Array, ContractParameterType.Map },
            parameters.Select(p => p.Type).ToArray());
    }

    [TestMethod]
    [DataRow("double")]
    [DataRow("double?")]
    [DataRow("decimal")]
    [DataRow("decimal?")]
    [DataRow("float")]
    [DataRow("float?")]
    [DataRow("System.Threading.Tasks.Task<int>")]
    [DataRow("System.Threading.Tasks.Task<int>?")]
    public void Unsupported_Types_Are_Still_Rejected(string type)
    {
        var source = $@"#nullable enable
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{{
    public static {type} Echo({type} value) => value;
}}";
        var context = TestHelper.CompileSingleContract(source);
        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics);
        Assert.IsFalse(context.Success, $"'{type}' must not be accepted in the ABI.");
        Assert.IsTrue(context.Diagnostics.Any(p => p.Id == DiagnosticId.SyntaxNotSupported
            && p.GetMessage() == $"Type '{type}' is not supported in contract ABI."), diagnostics);
    }
}
