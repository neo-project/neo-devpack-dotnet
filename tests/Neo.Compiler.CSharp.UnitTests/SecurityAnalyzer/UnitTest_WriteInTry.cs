// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_WriteInTry.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class WriteInTryAnalyzeTryCatchTests : DebugAndTestBase<Contract_TryCatch>
    {
        [TestMethod]
        public void Test_WriteInTryAnalyzeTryCatch()
        {
            ContractInBasicBlocks contractInBasicBlocks = new(NefFile, Manifest);
            TryCatchFinallyCoverage tryCatchFinallyCoverage = new(contractInBasicBlocks);
            Assert.AreEqual(tryCatchFinallyCoverage.allTry.Count, 22);

            WriteInTryAnalyzer.WriteInTryVulnerability v =
                WriteInTryAnalyzer.AnalyzeWriteInTry(NefFile, Manifest);
            Assert.AreEqual(v.Vulnerabilities.Count, 0);
            v.GetWarningInfo(print: false);
        }
    }

    [TestClass]
    public class WriteInTryTests : DebugAndTestBase<Contract_WriteInTry>
    {
        [TestMethod]
        public void Test_WriteInTry()
        {
            ContractInBasicBlocks contractInBasicBlocks = new(NefFile, Manifest);
            TryCatchFinallyCoverage tryCatchFinallyCoverage = new(contractInBasicBlocks);
            Assert.AreEqual(tryCatchFinallyCoverage.allTry.Count, 14);

            WriteInTryAnalyzer.WriteInTryVulnerability v =
                WriteInTryAnalyzer.AnalyzeWriteInTry(NefFile, Manifest);
            // because most try throws or aborts in catch, or has no catch, or throws or aborts in finally
            Assert.AreEqual(v.Vulnerabilities.Count, 2);
            v.GetWarningInfo(print: false);
        }

        [TestMethod]
        public void Test_WriteInTryWithEnhancedDiagnostics()
        {
            // Test enhanced diagnostic messages without debug info (fallback behavior)
            WriteInTryAnalyzer.WriteInTryVulnerability v =
                WriteInTryAnalyzer.AnalyzeWriteInTry(NefFile, Manifest, null);
            Assert.AreEqual(v.Vulnerabilities.Count, 2);

            // Test that warning message contains enhanced diagnostic information
            string warningInfo = v.GetWarningInfo(print: false);

            // Verify enhanced diagnostic format
            Assert.IsTrue(warningInfo.Contains("[SECURITY] Writing storage in `try` block is risky"));
            Assert.IsTrue(warningInfo.Contains("Recommendation:"));
            Assert.IsTrue(warningInfo.Contains("writes may not be properly reverted on exceptions"));
            Assert.IsTrue(warningInfo.Contains("Try block addresses:"));
            Assert.IsTrue(warningInfo.Contains("Write instruction addresses:"));

            // Message should be more detailed than just addresses
            Assert.IsTrue(warningInfo.Length > 150, "Enhanced diagnostic message should be more detailed than simple address listing");
        }

        [TestMethod]
        public void Test_WriteInTry_SourceLocation_Respects_Method_Range()
        {
            string json = $@"{{
  ""hash"": ""{UInt160.Zero}"",
  ""document-root"": """",
  ""documents"": [""a.cs"", ""b.cs""],
  ""methods"": [
    {{
      ""id"": ""0"",
      ""name"": ""Test,MethodA"",
      ""range"": ""0-5"",
      ""params"": [],
      ""sequence-points"": [""0[0]1:1-1:2""]
    }},
    {{
      ""id"": ""1"",
      ""name"": ""Test,MethodB"",
      ""range"": ""10-20"",
      ""params"": [],
      ""sequence-points"": [""10[1]2:1-2:2""]
    }}
  ]
}}";

            var debugInfo = (JObject)JToken.Parse(json)!;

            using ScriptBuilder sb = new();
            sb.EmitSysCall(ApplicationEngine.System_Storage_Put);
            var instruction = ((Script)sb.ToArray()).EnumerateInstructions().First().instruction;
            var block = new BasicBlock(10, new List<Neo.VM.Instruction> { instruction });
            var vulnerabilities = new Dictionary<BasicBlock, HashSet<int>>
            {
                [block] = new HashSet<int> { 10 }
            };

            var vuln = new WriteInTryAnalyzer.WriteInTryVulnerability(vulnerabilities, debugInfo);
            string warningInfo = vuln.GetWarningInfo(print: false);

            Assert.IsTrue(warningInfo.Contains("At: b.cs:2:1"), "Expected mapping to b.cs based on method range");
        }

        [TestMethod]
        public void Test_WriteInTry_InvalidDebugInfo_FallsBack_To_Address_Format()
        {
            var debugInfo = new JObject
            {
                ["methods"] = "invalid"
            };

            using ScriptBuilder sb = new();
            sb.EmitSysCall(ApplicationEngine.System_Storage_Put);
            var instruction = ((Script)sb.ToArray()).EnumerateInstructions().First().instruction;
            var block = new BasicBlock(0, new List<Neo.VM.Instruction> { instruction });
            var vulnerabilities = new Dictionary<BasicBlock, HashSet<int>>
            {
                [block] = new HashSet<int> { 0 }
            };

            var vuln = new WriteInTryAnalyzer.WriteInTryVulnerability(vulnerabilities, debugInfo);
            string warningInfo = vuln.GetWarningInfo(print: false);

            Assert.IsTrue(warningInfo.Contains("Try block addresses: {0}"));
            Assert.IsTrue(warningInfo.Contains("Write instruction addresses: 0"));
        }

        [TestMethod]
        public void Test_WriteInTry_DebugInfo_WithoutMatchingSequencePoint_FallsBack_To_InstructionAddress()
        {
            string json = $@"{{
  ""hash"": ""{UInt160.Zero}"",
  ""document-root"": """",
  ""documents"": [""a.cs""],
  ""methods"": [
    {{
      ""id"": ""0"",
      ""name"": ""Test,MethodA"",
      ""range"": ""10-20"",
      ""params"": [],
      ""sequence-points"": [""15[0]1:1-1:2""]
    }}
  ]
}}";

            var debugInfo = (JObject)JToken.Parse(json)!;

            using ScriptBuilder sb = new();
            sb.EmitSysCall(ApplicationEngine.System_Storage_Put);
            var instruction = ((Script)sb.ToArray()).EnumerateInstructions().First().instruction;
            var block = new BasicBlock(10, new List<Neo.VM.Instruction> { instruction });
            var vulnerabilities = new Dictionary<BasicBlock, HashSet<int>>
            {
                [block] = new HashSet<int> { 10 }
            };

            var vuln = new WriteInTryAnalyzer.WriteInTryVulnerability(vulnerabilities, debugInfo);
            string warningInfo = vuln.GetWarningInfo(print: false);

            Assert.IsTrue(warningInfo.Contains("At instruction address: 10"));
        }

        [TestMethod]
        public void Test_FindAllBasicBlocksWritingStorageInTryCatchFinally_ReturnsEmpty_WhenAlreadyVisited()
        {
            BasicBlock block = CreateStorageWriteBlock(0);
            var coverage = new TryCatchFinallySingleCoverage(
                null!,
                0, -1, -1,
                block, null, null,
                new HashSet<BasicBlock> { block },
                new HashSet<BasicBlock>(),
                new HashSet<BasicBlock>(),
                new HashSet<BasicBlock>(),
                new HashSet<TryCatchFinallySingleCoverage>(),
                new HashSet<TryCatchFinallySingleCoverage>(),
                new HashSet<TryCatchFinallySingleCoverage>());

            var result = WriteInTryAnalyzer.FindAllBasicBlocksWritingStorageInTryCatchFinally(
                coverage,
                new HashSet<TryCatchFinallySingleCoverage> { coverage },
                new HashSet<BasicBlock> { block });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_FindAllBasicBlocksWritingStorageInTryCatchFinally_TraversesNestedTrys()
        {
            BasicBlock block = CreateStorageWriteBlock(0);
            var nested = new TryCatchFinallySingleCoverage(
                null!,
                1, -1, -1,
                block, null, null,
                new HashSet<BasicBlock> { block },
                new HashSet<BasicBlock>(),
                new HashSet<BasicBlock>(),
                new HashSet<BasicBlock>(),
                new HashSet<TryCatchFinallySingleCoverage>(),
                new HashSet<TryCatchFinallySingleCoverage>(),
                new HashSet<TryCatchFinallySingleCoverage>());
            var outer = new TryCatchFinallySingleCoverage(
                null!,
                0, -1, -1,
                block, null, null,
                new HashSet<BasicBlock>(),
                new HashSet<BasicBlock>(),
                new HashSet<BasicBlock>(),
                new HashSet<BasicBlock>(),
                new HashSet<TryCatchFinallySingleCoverage> { nested },
                new HashSet<TryCatchFinallySingleCoverage>(),
                new HashSet<TryCatchFinallySingleCoverage>());

            var result = WriteInTryAnalyzer.FindAllBasicBlocksWritingStorageInTryCatchFinally(
                outer,
                new HashSet<TryCatchFinallySingleCoverage>(),
                new HashSet<BasicBlock> { block });

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(block));
        }

        [TestMethod]
        public void Test_WriteInTry_WarningInfo_Print_WritesToConsole()
        {
            BasicBlock block = CreateStorageWriteBlock(0);
            var vulnerabilities = new Dictionary<BasicBlock, HashSet<int>>
            {
                [block] = new HashSet<int> { 0 }
            };
            var vuln = new WriteInTryAnalyzer.WriteInTryVulnerability(vulnerabilities, null);
            var writer = new StringWriter();
            TextWriter originalOut = Console.Out;

            try
            {
                Console.SetOut(writer);
                string warningInfo = vuln.GetWarningInfo(print: true);
                Assert.IsTrue(warningInfo.Contains("Write instruction addresses: 0"));
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            Assert.IsTrue(writer.ToString().Contains("Write instruction addresses: 0"));
        }

        private static BasicBlock CreateStorageWriteBlock(int startAddress)
        {
            using ScriptBuilder sb = new();
            sb.EmitSysCall(ApplicationEngine.System_Storage_Put);
            var instruction = ((Script)sb.ToArray()).EnumerateInstructions().First().instruction;
            return new BasicBlock(startAddress, new List<Neo.VM.Instruction> { instruction });
        }
    }
}
