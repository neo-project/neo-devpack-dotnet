// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_EventArguments.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_EventArguments
{
    private const string Source = """
        using Neo.SmartContract.Framework;

        public delegate void Pair(int a, int b);
        public delegate void Mixed(int a, string s);
        public delegate void Values(params int[] xs);
        public delegate void Optional(int a, int b = 5);

        public class Contract : SmartContract
        {
            public static event Pair OnPair;
            public static event Mixed OnMixed;
            public static event Values OnValues;
            public static event Optional OnOptional;
            private static int _trace;

            public static void Named() => OnPair(b: 22, a: 11);
            public static void NamedInvoke() => OnPair.Invoke(b: 22, a: 11);
            public static void MixedNamed() => OnMixed(s: "x", a: 7);
            public static void ParamsExpanded() => OnValues(1, 2, 3);
            public static void ParamsArray() => OnValues(new int[] { 1, 2, 3 });
            public static void OptionalOmitted() => OnOptional(1);
            public static void OptionalFull() => OnOptional(1, 2);

            public static int NamedEvaluationOrder()
            {
                _trace = 0;
                OnPair(b: Next(2), a: Next(1));
                return _trace;
            }

            private static int Next(int value)
            {
                _trace = _trace * 10 + value;
                return value;
            }
        }
        """;

    [TestMethod]
    public void EventArgumentsMatchDelegateDeclaration()
    {
        var context = TestHelper.CompileSingleContract(Source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<EventArgumentsContract>(context.CreateExecutable(), context.CreateManifest());
        using var watcher = new NotificationWatcher(engine);

        contract.Named();
        contract.NamedInvoke();
        foreach (var notification in watcher.Notifications.Where(p => p.EventName == "OnPair"))
        {
            Assert.AreEqual(new BigInteger(11), notification.State[0].GetInteger());
            Assert.AreEqual(new BigInteger(22), notification.State[1].GetInteger());
        }
        Assert.AreEqual(2, watcher.Notifications.Count);

        contract.MixedNamed();
        var mixed = watcher.Notifications.Last();
        Assert.AreEqual(new BigInteger(7), mixed.State[0].GetInteger());
        Assert.AreEqual("x", mixed.State[1].GetString());

        contract.ParamsExpanded();
        contract.ParamsArray();
        foreach (var notification in watcher.Notifications.Where(p => p.EventName == "OnValues"))
        {
            Assert.AreEqual(1, notification.State.Count);
            var values = (Neo.VM.Types.Array)notification.State[0];
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values.Select(p => (int)p.GetInteger()).ToArray());
        }

        contract.OptionalOmitted();
        contract.OptionalFull();
        var optional = watcher.Notifications.Where(p => p.EventName == "OnOptional").ToArray();
        Assert.AreEqual(new BigInteger(5), optional[0].State[1].GetInteger());
        Assert.AreEqual(new BigInteger(2), optional[1].State[1].GetInteger());

        Assert.AreEqual(new BigInteger(21), contract.NamedEvaluationOrder());
        var ordered = watcher.Notifications.Last();
        Assert.AreEqual(new BigInteger(1), ordered.State[0].GetInteger());
        Assert.AreEqual(new BigInteger(2), ordered.State[1].GetInteger());
    }

    public abstract class EventArgumentsContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("named")] public abstract void Named();
        [DisplayName("namedInvoke")] public abstract void NamedInvoke();
        [DisplayName("mixedNamed")] public abstract void MixedNamed();
        [DisplayName("paramsExpanded")] public abstract void ParamsExpanded();
        [DisplayName("paramsArray")] public abstract void ParamsArray();
        [DisplayName("optionalOmitted")] public abstract void OptionalOmitted();
        [DisplayName("optionalFull")] public abstract void OptionalFull();
        [DisplayName("namedEvaluationOrder")] public abstract BigInteger? NamedEvaluationOrder();
    }
}
