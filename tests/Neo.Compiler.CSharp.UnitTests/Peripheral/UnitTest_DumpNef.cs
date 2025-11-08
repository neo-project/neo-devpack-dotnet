// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_DumpNef.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_DumpNef
    {
        public static readonly Regex OpCodeRegex = new Regex(@"^(\d+)\s(.*?)\s?(#\s.*)?$");  // 8039 SYSCALL 62-7D-5B-52 # System.Contract.Call SysCall
        public static readonly Regex SourceCodeRegex = new Regex(@"^#\sCode\s(.*\.cs)\sline\s(\d+):\s""(.*)""$");  // # Code NFTLoan.cs line 523: "ExecutionEngine.Assert((bool)Contract.Call(token, "transfer", CallFlags.All, tenant, Runtime.ExecutingScriptHash, neededAmount, tokenId, TRANSACTION_DATA), "NFT payback failed");"
        public static readonly Regex MethodStartRegex = new Regex(@"^# Method\sStart\s(.*)$");  // # Method Start NFTLoan.NFTLoan.FlashBorrowDivisible
        public static readonly Regex MethodEndRegex = new Regex(@"^# Method\sEnd\s(.*)$");  // # Method End NFTLoan.NFTLoan.FlashBorrowDivisible

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        NefFile nef;
        ContractManifest manifest;
        JObject debugInfo;
        string dumpNefWithManifest;
        string dumpNefWithoutManifest;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Init()
        {
            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract_NEP11.cs").FullName;
            var results = CompilationTestHelper.CompileSource(testContractsPath, options =>
            {
                options.Debug = CompilationOptions.DebugType.Extended;
                options.CompilerVersion = "TestingEngine";
                options.Optimize = CompilationOptions.OptimizationType.All;
            });

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);
            debugInfo = results[0].CreateDebugInformation();
            manifest = results[0].CreateManifest();
            nef = results[0].CreateExecutable();
            dumpNefWithManifest = DumpNef.GenerateDumpNef(nef, debugInfo, manifest);
            dumpNefWithoutManifest = DumpNef.GenerateDumpNef(nef, debugInfo, null);
        }

        //[TestMethod]
        public void Test_DumpNefSyntax()
        {
            foreach (string dumpNef in new string[] { dumpNefWithManifest, dumpNefWithoutManifest })
            {
                string[] lines = dumpNef.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
                foreach (string line in lines)
                {
                    foreach (Regex statement in new Regex[] { OpCodeRegex, SourceCodeRegex, MethodStartRegex, MethodEndRegex })
                        if (line == "" || statement.Match(line).Success)
                            goto CORRECT_SYNTAX;
                    throw new InvalidDataException($"Invalid DumpNef syntax {line}");
                CORRECT_SYNTAX:;
                }
            }
        }

        [TestMethod]
        public void Test_MethodStart()
        {
            Test_DumpNefSyntax();

            Dictionary<int, string> methodStartAddrToName = new();
            Dictionary<int, string> methodEndAddrToName = new();
            foreach (JToken? method in (JArray)debugInfo["methods"]!)
            {
                GroupCollection rangeGroups = DumpNef.RangeRegex.Match(method!["range"]!.AsString()).Groups;
                (int methodStartAddr, int methodEndAddr) = (int.Parse(rangeGroups[1].ToString()), int.Parse(rangeGroups[2].ToString()));
                methodStartAddrToName.Add(methodStartAddr, method!["id"]!.AsString());
                methodEndAddrToName.Add(methodEndAddr, method["id"]!.AsString());
            }
            foreach (ContractMethodDescriptor method in manifest.Abi.Methods)
                methodStartAddrToName.TryAdd(method.Offset, method.Name);

            string[] lines = dumpNefWithManifest.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                Match methodStartMatch = MethodStartRegex.Match(lines[i]);
                if (methodStartMatch.Success)
                {
                    string methodName = methodStartMatch.Groups[1].ToString();
                    for (int j = i; j < lines.Length; j++)
                    {
                        Match opCodeMatch = OpCodeRegex.Match(lines[j]);
                        if (opCodeMatch.Success)
                        {
                            int instructionPointer = int.Parse(opCodeMatch.Groups[1].ToString());
                            Assert.AreEqual(methodStartAddrToName[instructionPointer], methodName);
                            i = j + 1;
                            break;
                        }
                    }
                }
                Match methodEndMatch = MethodEndRegex.Match(lines[i]);
                if (methodEndMatch.Success)
                {
                    string methodName = methodEndMatch.Groups[1].ToString();
                    for (int j = i; j < lines.Length; j++)
                    {
                        Match opCodeMatch = OpCodeRegex.Match(lines[j]);
                        if (opCodeMatch.Success)
                        {
                            int instructionPointer = int.Parse(opCodeMatch.Groups[1].ToString());
                            Assert.AreEqual(methodEndAddrToName[instructionPointer], methodName);
                            i = j + 1;
                            break;
                        }
                    }
                }
            }
        }
    }
}
