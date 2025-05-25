// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_DebugInfo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_DebugInfo
    {
        [TestMethod]
        public void Test_DebugInfo()
        {
            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract_Event.cs").FullName;
            var results = new CompilationEngine(new CompilationOptions()
            {
                Debug = CompilationOptions.DebugType.Extended,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.Basic,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(testContractsPath);

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);

            var debugInfo = results[0].CreateDebugInformation();
            var nef = results[0].CreateExecutable();

            Assert.AreEqual(nef.Script.Span.ToScriptHash().ToString(), debugInfo["hash"]!.GetString());
            Assert.IsTrue(debugInfo.ContainsProperty("documents"));
            Assert.IsInstanceOfType(debugInfo["documents"], typeof(JArray));
            Assert.IsTrue((debugInfo["documents"] as JArray)!.Count > 0);
            Assert.IsTrue((debugInfo["documents"] as JArray)!.All(n => n is JString), "All documents items should be string!");
            Assert.IsTrue(debugInfo.ContainsProperty("methods"));
            Assert.IsInstanceOfType(debugInfo["methods"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["methods"] as JArray)!.Count);
            Assert.AreEqual("Neo.Compiler.CSharp.TestContracts.Contract_Event,test", (debugInfo["methods"] as JArray)![0]!["name"]!.AsString());
            Assert.AreEqual("0[0]37:28-37:29;1[0]37:13-37:29;2[0]37:13-37:29;3[0]37:13-37:29;4[0]38:28-38:32;5[0]38:13-38:32;6[0]38:13-38:32;7[0]38:13-38:32;8[0]39:73-39:85;9[0]39:49-39:71;14[0]39:49-39:71;16[0]39:25-39:47;21[0]39:25-39:47;23[0]39:13-39:86;24[0]39:13-39:86;25[0]39:13-39:86;35[0]39:13-39:86;40[0]40:9-40:10",
                string.Join(';', ((debugInfo["methods"] as JArray)![0]!["sequence-points"] as JArray)!.Select(u => u!.AsString())));
            Assert.IsTrue(debugInfo.ContainsProperty("events"));
            Assert.IsInstanceOfType(debugInfo["events"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["events"] as JArray)!.Count);
            Assert.AreEqual("Neo.Compiler.CSharp.TestContracts.Contract_Event,Transferred", (debugInfo["events"] as JArray)![0]!["name"]!.AsString());
            Assert.AreEqual("arg1,ByteArray,0;arg2,ByteArray,1;arg3,Integer,2", string.Join(';', ((debugInfo["events"] as JArray)![0]!["params"] as JArray)!.Select(u => u!.AsString())));
            Assert.IsTrue(debugInfo.ContainsProperty("static-variables"));
            Assert.AreEqual("MyStaticVar1,Integer,0;MyStaticVar2,Boolean,1", string.Join(';', (debugInfo["static-variables"] as JArray)!.Select(u => u!.AsString())));
        }
    }
}
