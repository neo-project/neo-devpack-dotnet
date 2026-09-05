// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SpecialReceiverEvaluationOrder.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_SpecialReceiverEvaluationOrder
    {
        private const string Source = """
        using System;
        using Neo.SmartContract.Framework;

        public class Contract : SmartContract
        {
            private static int counter;
            private static string marker = "";

            public static int Run(int scenario)
            {
                switch (scenario)
                {
                    case 0:
                        marker = "b";
                        return GetStringReceiver().StartsWith(marker) ? 1 : 0;

                    case 1:
                        counter = 1;
                        return GetDelegate().Invoke(counter);

                    default:
                        return -1;
                }
            }

            private static string GetStringReceiver()
            {
                marker = "a";
                return "abc";
            }

            private static Func<int, int> GetDelegate()
            {
                counter = 2;
                return new Func<int, int>(Identity);
            }

            private static int Identity(int value)
            {
                return value;
            }
        }
        """;

        [DataTestMethod]
        [DataRow(CompilationOptions.OptimizationType.None, 0, 1)]
        [DataRow(CompilationOptions.OptimizationType.All, 0, 1)]
        [DataRow(CompilationOptions.OptimizationType.None, 1, 2)]
        [DataRow(CompilationOptions.OptimizationType.All, 1, 2)]
        public void SpecialReceiversAreEvaluatedBeforeArguments(
            CompilationOptions.OptimizationType optimization,
            int scenario,
            int expected)
        {
            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;

            var context = TestHelper.CompileSingleContract(Source, options);

            Assert.IsTrue(
                context.Success,
                string.Join(System.Environment.NewLine, context.Diagnostics));

            var (nef, manifest, _) = context.CreateResults();

            var engine = new TestEngine(true);
            var contract = engine.Deploy<ReceiverContract>(nef, manifest);

            Assert.AreEqual(
                new BigInteger(expected),
                contract.Run(scenario));
        }

        public abstract class ReceiverContract(SmartContractInitialize initialize)
            : SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("run")]
            public abstract BigInteger? Run(BigInteger? scenario);
        }
    }
}
