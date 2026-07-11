// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_VTableInitialization.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_VTableInitialization
{
    [TestMethod]
    public void VTablesAreInitializedBeforeStaticObjects()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static int _order;
                private static int _first = Record(1);
                private static VirtualValue _indirectValue = CreateValue();
                private static VirtualValue _directValue = new();
                private static int _second = Record(2);

                [DisplayName("run")]
                public static int Run()
                {
                    return _order * 10000 + _indirectValue.GetValue() * 100 + _directValue.GetValue();
                }

                private static VirtualValue CreateValue() => new();

                private static int Record(int value)
                {
                    _order = _order * 10 + value;
                    return value;
                }

                public class VirtualValue
                {
                    public virtual int GetValue()
                    {
                        return 42;
                    }
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<VTableInitializationContract>(
            context.CreateExecutable(),
            context.CreateManifest());

        Assert.AreEqual(new BigInteger(124242), contract.Run());
    }

    public abstract class VTableInitializationContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")]
        public abstract BigInteger? Run();
    }
}
