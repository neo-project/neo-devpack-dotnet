// Copyright (C) 2015-2026 The Neo Project.
// This file belongs to the neo project and is distributed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringBuilderArgumentMutation
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void AppendObservesArgumentMutations(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System.Text;
            public class Contract : SmartContract
            {
                private static int calls;
                private static StringBuilder Receiver(StringBuilder value) { calls++; return value; }
                private static int ClearAndNumber(StringBuilder value) { value.Clear(); return 7; }
                public static string Run(int scenario)
                {
                    calls = 0;
                    var builder = new StringBuilder("A");
                    switch (scenario)
                    {
                        case 0: Receiver(builder).Append(builder.Append("B").ToString()); break;
                        case 1: Receiver(builder).AppendLine(builder.Clear().Append("B").ToString()); break;
                        case 2: Receiver(builder).Append(builder.Append("B")); break;
                        case 3: Receiver(builder).Append(ClearAndNumber(builder)); break;
                        case 4: Receiver(builder).Append(builder.Clear().Append("B").Length); break;
                        case 5: Receiver(builder).Append(builder); break;
                        case 6: Receiver(builder).AppendLine(); break;
                    }
                    return calls + ":" + builder.ToString();
                }
            }
            """, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<MutationContract>(context.CreateExecutable(), context.CreateManifest());
        for (var scenario = 0; scenario < 7; scenario++)
        {
            var builder = new StringBuilder("A");
            switch (scenario)
            {
                case 0: builder.Append(builder.Append("B").ToString()); break;
                case 1: builder.AppendLine(builder.Clear().Append("B").ToString()); break;
                case 2: builder.Append(builder.Append("B")); break;
                case 3: builder.Clear(); builder.Append(7); break;
                case 4: builder.Append(builder.Clear().Append("B").Length); break;
                case 5: builder.Append(builder); break;
                case 6: builder.AppendLine(); break;
            }
            Assert.AreEqual("1:" + builder.ToString().Replace(Environment.NewLine, "\n"), contract.Run(scenario), $"Scenario {scenario}");
        }
    }

    public abstract class MutationContract(SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")] public abstract string? Run(int scenario);
    }
}
