// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_PrintAbi.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_PrintAbi
    {
        private static string CaptureReport(SmartContract.NefFile nef, SmartContract.Manifest.ContractManifest manifest)
        {
            var writer = new StringWriter();
            AbiReporter.Print(nef, manifest, writer);
            return writer.ToString();
        }

        [TestMethod]
        public void Test_PrintAbi_ContainsContractName()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            Assert.IsTrue(output.Contains("Contract:"), "Report must contain 'Contract:' header");
        }

        [TestMethod]
        public void Test_PrintAbi_ContainsStaticHeader()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            // The word "Static" must appear in the report header so readers understand
            // this is a compile-time analysis, not a runtime measurement.
            Assert.IsTrue(output.Contains("Static ABI Report"), "Report header must read 'Static ABI Report' to make its nature explicit");
        }

        [TestMethod]
        public void Test_PrintAbi_ContainsScriptSize()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            Assert.IsTrue(output.Contains("Script size:"), "Report must contain script size");
            // Script must be non-zero
            Assert.IsTrue(output.Contains("bytes"), "Script size must include 'bytes' unit");
        }

        [TestMethod]
        public void Test_PrintAbi_ContainsInstructionCount()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            Assert.IsTrue(output.Contains("Instruction count:"), "Report must contain instruction count");
        }

        [TestMethod]
        public void Test_PrintAbi_InstructionCount_IsNumericOrNA()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            // Extract the value after "Instruction count:"
            int labelIndex = output.IndexOf("Instruction count:", StringComparison.Ordinal);
            Assert.IsTrue(labelIndex >= 0);
            string afterLabel = output[(labelIndex + "Instruction count:".Length)..].TrimStart();
            string value = afterLabel.Split('\n')[0].Trim();
            bool isNumeric = int.TryParse(value, out int count) && count > 0;
            bool isNA = value == "N/A";
            Assert.IsTrue(isNumeric || isNA, $"Instruction count must be a positive integer or N/A, got: '{value}'");
        }

        [TestMethod]
        public void Test_PrintAbi_ContainsAbiMethods()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            Assert.IsTrue(output.Contains("ABI methods:"), "Report must list ABI method count");
        }

        [TestMethod]
        public void Test_PrintAbi_DoesNotMentionGasEstimation()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            Assert.IsFalse(output.Contains("Gas estimation:"), "ABI output must not include a gas estimation section");
            Assert.IsFalse(output.Contains("Engine.GasConsumed"), "ABI output must not reference runtime gas measurement APIs");
        }

        [TestMethod]
        public void Test_PrintAbi_ContainsMethodTable()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            // NEP-17 standard methods should appear
            Assert.IsTrue(output.Contains("transfer"), "Report must list 'transfer' method");
            Assert.IsTrue(output.Contains("balanceOf"), "Report must list 'balanceOf' method");
            Assert.IsTrue(output.Contains("symbol"), "Report must list 'symbol' method");
        }

        [TestMethod]
        public void Test_PrintAbi_SafeFlag()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            // The method table header must contain the "Safe" column label.
            // We look for it surrounded by whitespace as produced by the fixed-width format
            // so we do not accidentally match substrings in method names or the disclaimer.
            Assert.IsTrue(output.Contains("Safe     Return"), "Report must include the Safe column header");
            // At least one method must be flagged safe=yes (NEP-17 has several safe methods)
            Assert.IsTrue(output.Contains("yes      "), "Report must show at least one safe method");
        }

        [TestMethod]
        public void Test_PrintAbi_NullNef_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => AbiReporter.Print(null!, Contract_NEP17.Manifest, Console.Out));
        }

        [TestMethod]
        public void Test_PrintAbi_NullManifest_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => AbiReporter.Print(Contract_NEP17.Nef, null!, Console.Out));
        }

        [TestMethod]
        public void Test_PrintAbi_NullWriter_Throws()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => AbiReporter.Print(Contract_NEP17.Nef, Contract_NEP17.Manifest, null!));
        }

        [TestMethod]
        public void Test_PrintAbi_ScriptSizeMatchesNef()
        {
            var nef = Contract_NEP17.Nef;
            var manifest = Contract_NEP17.Manifest;
            int expectedSize = nef.Script.Length;

            var output = CaptureReport(nef, manifest);

            Assert.IsTrue(output.Contains($"{expectedSize} bytes"), $"Report must show correct script size: {expectedSize} bytes");
        }

        [TestMethod]
        public void Test_PrintAbi_EventsSection_WhenPresent()
        {
            var output = CaptureReport(Contract_Event.Nef, Contract_Event.Manifest);
            Assert.IsTrue(output.Contains("Events:"), "Report must contain events count");
        }

        [TestMethod]
        public void Test_PrintAbi_DoesNotWriteToStdErr()
        {
            var stderrCapture = new StringWriter();
            var originalErr = Console.Error;
            try
            {
                Console.SetError(stderrCapture);
                CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            }
            finally
            {
                Console.SetError(originalErr);
            }
            Assert.AreEqual(string.Empty, stderrCapture.ToString(), "AbiReporter.Print must not write anything to stderr for a valid contract");
        }

        [TestMethod]
        public void Test_PrintAbi_SafeMethodCount_MatchesManifest()
        {
            var manifest = Contract_NEP17.Manifest;
            var expectedSafe = 0;
            foreach (var m in manifest.Abi.Methods)
                if (m.Safe) expectedSafe++;

            var output = CaptureReport(Contract_NEP17.Nef, manifest);

            var idx = output.IndexOf("Safe methods:", StringComparison.Ordinal);
            Assert.IsTrue(idx >= 0);
            var afterLabel = output[(idx + "Safe methods:".Length)..].TrimStart();
            var value = afterLabel.Split('\n')[0].Trim();
            Assert.IsTrue(int.TryParse(value, out int reported), $"Safe methods count must be an integer, got: '{value}'");
            Assert.AreEqual(expectedSafe, reported, "Reported safe method count must match the manifest ABI");
        }

        [TestMethod]
        public void Test_PrintAbi_InstructionCount_NA_WhenScriptUndecodable()
        {
            var badScript = new byte[] { (byte)OpCode.PUSHDATA1 };
            var nef = new NefFile
            {
                Compiler = "test",
                Source = "test.cs",
                Tokens = [],
                Script = badScript
            };
            nef.CheckSum = NefFile.ComputeChecksum(nef);

            var manifest = BuildMinimalManifest("BadScript");
            var output = CaptureReport(nef, manifest);

            Assert.IsTrue(output.Contains("Instruction count:"), "Report must contain the instruction count label");
            var idx = output.IndexOf("Instruction count:", StringComparison.Ordinal);
            var afterLabel = output[(idx + "Instruction count:".Length)..].TrimStart();
            var value = afterLabel.Split('\n')[0].Trim();
            Assert.AreEqual("N/A", value, "Instruction count must be N/A when the script cannot be decoded");
        }

        [TestMethod]
        public void Test_PrintAbi_NoEventsSection_WhenContractHasNoEvents()
        {
            var output = CaptureReport(Contract_ABISafe.Nef, Contract_ABISafe.Manifest);

            // The events block header is the column label "Event" followed by "Parameters".
            // It must NOT appear when the contract has no events.
            Assert.IsFalse(output.Contains("Event Parameters"), "Events table header must be omitted when there are no events");
        }

        [TestMethod]
        public void Test_PrintAbi_Event_WithNoParameters_ShowsDash()
        {
            var nef = Contract_ABISafe.Nef;
            var manifest = BuildManifestWithEvents(
            [
                new ContractEventDescriptor
                {
                    Name = "NoParamEvent",
                    Parameters = []
                }
            ]);

            var output = CaptureReport(nef, manifest);

            Assert.IsTrue(output.Contains("NoParamEvent"), "Report must list the event name");
            var evIdx = output.IndexOf("NoParamEvent", StringComparison.Ordinal);
            var evRow = output[evIdx..].Split('\n')[0];
            Assert.IsTrue(evRow.TrimEnd().EndsWith("-"), $"Event with no parameters must show '-', row was: '{evRow}'");
        }

        [TestMethod]
        public void Test_PrintAbi_UnsafeMethod_ShowsNo()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            var idx = output.IndexOf("transfer", StringComparison.Ordinal);
            Assert.IsTrue(idx >= 0, "transfer method must appear in the report");
            string row = output[idx..].Split('\n')[0];
            Assert.IsTrue(row.Contains("no"), $"transfer must be marked unsafe (no), row was: '{row}'");
        }

        [TestMethod]
        public void Test_PrintAbi_Method_WithParameters_ShowsTypes()
        {
            var output = CaptureReport(Contract_NEP17.Nef, Contract_NEP17.Manifest);
            var idx = output.IndexOf("transfer", StringComparison.Ordinal);
            Assert.IsTrue(idx >= 0);
            string row = output[idx..].Split('\n')[0];
            Assert.IsTrue(row.Contains("Hash160"), $"transfer row must list parameter types, row was: '{row}'");
        }

        [TestMethod]
        public void Test_PrintAbi_Event_WithParameters_ShowsTypes()
        {
            var output = CaptureReport(Contract_Event.Nef, Contract_Event.Manifest);

            var evSectionIdx = output.IndexOf("Event", StringComparison.Ordinal);
            Assert.IsTrue(evSectionIdx >= 0, "Events section must be present");
            string evSection = output[evSectionIdx..];
            Assert.IsTrue(evSection.Contains("ByteArray"), "Event parameters must list their types (ByteArray expected)");
        }

        [TestMethod]
        public void Test_PrintAbi_CLI_Flag_PrintsReportToStdout()
        {
            // Find the Contract_NEP17 source file by navigating from the test binary.
            var baseDir = Directory.GetCurrentDirectory();
            var contractPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "tests", "Neo.Compiler.CSharp.TestContracts", "Contract_NEP17.cs"));

            if (!File.Exists(contractPath))
            {
                Assert.Inconclusive($"Test contract not found at {contractPath}; skipping CLI test.");
                return;
            }

            var stdoutCapture = new StringWriter();
            var stderrCapture = new StringWriter();
            int exitCode;

            var originalOut = Console.Out;
            var originalErr = Console.Error;
            try
            {
                Console.SetOut(stdoutCapture);
                Console.SetError(stderrCapture);
                exitCode = Program.Main(new[] { contractPath, "--print-abi" });
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalErr);
            }

            Assert.AreEqual(0, exitCode, $"Compiler must succeed. stderr: {stderrCapture}");
            var stdout = stdoutCapture.ToString();
            Assert.IsTrue(stdout.Contains("Static ABI Report"), "stdout must contain the ABI report when --print-abi is passed");
            Assert.IsTrue(stdout.Contains("Script size:"), "stdout must contain script size line");
            Assert.IsFalse(stdout.Contains("Gas estimation:"), "stdout must not contain a gas estimation section");
        }

        [TestMethod]
        public void Test_PrintAbi_CLI_WithoutFlag_NoReportInOutput()
        {
            var baseDir = Directory.GetCurrentDirectory();
            var contractPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "tests", "Neo.Compiler.CSharp.TestContracts", "Contract_NEP17.cs"));

            if (!File.Exists(contractPath))
            {
                Assert.Inconclusive($"Test contract not found at {contractPath}; skipping CLI test.");
                return;
            }

            var stdoutCapture = new StringWriter();
            var stderrCapture = new StringWriter();
            int exitCode;

            var originalOut = Console.Out;
            var originalErr = Console.Error;
            try
            {
                Console.SetOut(stdoutCapture);
                Console.SetError(stderrCapture);
                exitCode = Program.Main([contractPath]);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalErr);
            }

            Assert.AreEqual(0, exitCode, $"Compiler must succeed. stderr: {stderrCapture}");
            Assert.IsFalse(stdoutCapture.ToString().Contains("Static ABI Report"), "stdout must NOT contain the ABI report when --print-abi is absent");
        }

        // We redirect Console.Out to a writer that throws on WriteLine to simulate
        // an unexpected failure inside Print, then verify that the compiler still
        // exits with code 0 and logs the error to stderr.
        [TestMethod]
        public void Test_PrintAbi_CLI_PrintException_LoggedToStderr_CompilationStillSucceeds()
        {
            var baseDir = Directory.GetCurrentDirectory();
            var contractPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "tests", "Neo.Compiler.CSharp.TestContracts", "Contract_NEP17.cs"));

            if (!File.Exists(contractPath))
            {
                Assert.Inconclusive($"Test contract not found at {contractPath}; skipping CLI test.");
                return;
            }

            var stderrCapture = new StringWriter();
            int exitCode;

            var originalOut = Console.Out;
            var originalErr = Console.Error;
            try
            {
                Console.SetOut(new ThrowingAfterNWritesWriter(originalOut, throwAfter: 5));
                Console.SetError(stderrCapture);
                exitCode = Program.Main([contractPath, "--print-abi"]);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalErr);
            }

            Assert.AreEqual(0, exitCode, "Compilation exit code must be 0 even when the ABI report throws");
            Assert.IsTrue(stderrCapture.ToString().Contains("ABI report error:"), $"stderr must contain 'ABI report error:' when Print throws, stderr was: {stderrCapture}");
        }

        /// <summary>
        /// A <see cref="TextWriter"/> that delegates to an inner writer for the first
        /// <paramref name="throwAfter"/> calls to <see cref="WriteLine()"/> / <see cref="WriteLine(string)"/>,
        /// then throws <see cref="IOException"/> on subsequent calls.
        /// </summary>
        private sealed class ThrowingAfterNWritesWriter : TextWriter
        {
            private readonly TextWriter _inner;
            private readonly int _throwAfter;
            private int _writeCount;

            public ThrowingAfterNWritesWriter(TextWriter inner, int throwAfter)
            {
                _inner = inner;
                _throwAfter = throwAfter;
            }

            public override System.Text.Encoding Encoding => _inner.Encoding;

            public override void WriteLine(string? value)
            {
                if (++_writeCount > _throwAfter)
                    throw new IOException("Simulated write failure for test coverage");
                _inner.WriteLine(value);
            }

            public override void WriteLine()
            {
                if (++_writeCount > _throwAfter)
                    throw new IOException("Simulated write failure for test coverage");
                _inner.WriteLine();
            }

            public override void Write(char value) => _inner.Write(value);
        }

        private static ContractManifest BuildMinimalManifest(string name)
        {
            return new ContractManifest
            {
                Name = name,
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods = [],
                    Events = []
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }

        private static ContractManifest BuildManifestWithEvents(ContractEventDescriptor[] events)
        {
            return new ContractManifest
            {
                Name = "TestWithEvents",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods = [],
                    Events = events
                },
                Permissions = [],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };
        }
    }
}
