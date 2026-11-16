// Copyright (C) 2015-2026 The Neo Project.
//
// This file belongs to the neo project and is free software distributed under
// the MIT software license, see the accompanying LICENSE in the main directory.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_FrameworkTypeNameCollision
{
    private const string Source = """
        using Neo.SmartContract.Framework;

        namespace Custom
        {
            public class Map
            {
                public int X { get; set; }
                public int Y;
            }

            public class List
            {
                public int Value;
            }
        }

        public class Contract : SmartContract
        {
            public static int makeMapProp()
            {
                var value = new Custom.Map { X = 42 };
                return value.X;
            }

            public static int makeMapWithArg(int argument)
            {
                var value = new Custom.Map { X = 42 };
                return value.X + argument;
            }

            public static int frameworkMapInitializer()
            {
                var value = new Neo.SmartContract.Framework.Map<int, int> { [1] = 7 };
                return value[1];
            }

            public static int takeMap(Custom.Map value) => value.Y;
            public static int takeList(Custom.List value) => value.Value;
            public static int takeFrameworkMap(Neo.SmartContract.Framework.Map<int, int> value) => value[1];
            public static int takeFrameworkList(Neo.SmartContract.Framework.List<int> value) => value[0];
        }
        """;

    [TestMethod]
    public void UserTypesNamedMapAndListKeepTheirOwnAbiAndInitializer()
    {
        var context = TestHelper.CompileSingleContract(Source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var manifest = context.CreateManifest();
        Assert.AreEqual(ContractParameterType.Any, ParameterType("takeMap"));
        Assert.AreEqual(ContractParameterType.Any, ParameterType("takeList"));
        Assert.AreEqual(ContractParameterType.Map, ParameterType("takeFrameworkMap"));
        Assert.AreEqual(ContractParameterType.Array, ParameterType("takeFrameworkList"));

        var contract = new TestEngine(true).Deploy<NameCollisionContract>(context.CreateExecutable(), manifest);
        Assert.AreEqual(new BigInteger(42), contract.MakeMapProp());
        Assert.AreEqual(new BigInteger(47), contract.MakeMapWithArg(5));
        Assert.AreEqual(new BigInteger(7), contract.FrameworkMapInitializer());

        ContractParameterType ParameterType(string method) => manifest.Abi.Methods
            .Single(m => m.Name == method).Parameters.Single().Type;
    }

    public abstract class NameCollisionContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("makeMapProp")]
        public abstract BigInteger? MakeMapProp();

        [DisplayName("makeMapWithArg")]
        public abstract BigInteger? MakeMapWithArg(BigInteger? argument);

        [DisplayName("frameworkMapInitializer")]
        public abstract BigInteger? FrameworkMapInitializer();
    }
}
