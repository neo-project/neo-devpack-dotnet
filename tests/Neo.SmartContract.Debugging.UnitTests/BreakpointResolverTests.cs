// Copyright (C) 2015-2026 The Neo Project.
//
// BreakpointResolverTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Debugging.UnitTests
{
    [TestClass]
    public class BreakpointResolverTests
    {
        private const string DocA = "/repo/src/Contract.cs";
        private const string DocB = "/repo/src/Helper.cs";

        // A synthetic source map:
        //   Contract.cs (doc 0): line 10 -> addr 0; line 11 -> addr 5 and addr 9; line 13 -> addr 16; line 15 -> addr 37
        //   Helper.cs   (doc 1): line 4  -> addr 60
        private static NeoDebugInfo BuildDebugInfo()
        {
            var documents = new List<string> { DocA, DocB };

            var main = new NeoDebugInfo.Method("0", "Test", "main", (0, 50),
                new List<NeoDebugInfo.Parameter>(),
                new List<NeoDebugInfo.SequencePoint>
                {
                    new(0, 0, (10, 9), (10, 20)),
                    new(5, 0, (11, 13), (11, 25)),
                    new(9, 0, (11, 27), (11, 40)),
                    new(16, 0, (13, 9), (13, 30)),
                    new(37, 0, (15, 9), (15, 18)),
                });

            var helper = new NeoDebugInfo.Method("1", "Test", "helper", (51, 80),
                new List<NeoDebugInfo.Parameter>(),
                new List<NeoDebugInfo.SequencePoint>
                {
                    new(60, 1, (4, 5), (4, 22)),
                });

            return new NeoDebugInfo(UInt160.Zero, "/repo", documents, new List<NeoDebugInfo.Method> { main, helper });
        }

        [TestMethod]
        public void ExactLine_ResolvesToFirstInstruction()
        {
            var bp = BuildDebugInfo().ResolveBreakpoint(DocA, 10);
            Assert.IsNotNull(bp);
            Assert.AreEqual(0, bp!.Value.Address);
            Assert.AreEqual(10, bp.Value.Line);
            Assert.AreEqual(9, bp.Value.Column);
        }

        [TestMethod]
        public void MultipleSequencePointsOnLine_PicksLowestAddress()
        {
            var bp = BuildDebugInfo().ResolveBreakpoint(DocA, 11);
            Assert.IsNotNull(bp);
            Assert.AreEqual(5, bp!.Value.Address);
            Assert.AreEqual(11, bp.Value.Line);
        }

        [TestMethod]
        public void LineWithoutCode_BindsToNextExecutableLine()
        {
            // Line 12 has no code; the breakpoint binds to line 13.
            var bp = BuildDebugInfo().ResolveBreakpoint(DocA, 12);
            Assert.IsNotNull(bp);
            Assert.AreEqual(13, bp!.Value.Line);
            Assert.AreEqual(16, bp.Value.Address);
        }

        [TestMethod]
        public void LineBeyondAllCode_ReturnsNull()
        {
            Assert.IsNull(BuildDebugInfo().ResolveBreakpoint(DocA, 16));
        }

        [TestMethod]
        public void FileNameMatch_BindsAcrossDifferentDirectory()
        {
            // Same file name, different path -> still resolves via the file-name fallback.
            var bp = BuildDebugInfo().ResolveBreakpoint("/somewhere/else/Contract.cs", 10);
            Assert.IsNotNull(bp);
            Assert.AreEqual(0, bp!.Value.Address);
        }

        [TestMethod]
        public void UnknownFile_ReturnsNull()
        {
            Assert.IsNull(BuildDebugInfo().ResolveBreakpoint("/repo/src/Missing.cs", 10));
        }

        [TestMethod]
        public void ResolvesInSecondMethodAndDocument()
        {
            var bp = BuildDebugInfo().ResolveBreakpoint(DocB, 4);
            Assert.IsNotNull(bp);
            Assert.AreEqual(60, bp!.Value.Address);
            Assert.AreEqual(DocB, bp.Value.Document);
        }

        [TestMethod]
        public void RequestedLineBelowFirst_BindsToFirstLine()
        {
            // A breakpoint requested before any executable line binds to the first one.
            var bp = BuildDebugInfo().ResolveBreakpoint(DocA, 1);
            Assert.IsNotNull(bp);
            Assert.AreEqual(10, bp!.Value.Line);
            Assert.AreEqual(0, bp.Value.Address);
        }

        [TestMethod]
        public void NullArguments_Throw()
        {
            var info = BuildDebugInfo();
            Assert.ThrowsException<ArgumentNullException>(() => ((NeoDebugInfo)null!).ResolveBreakpoint(DocA, 10));
            Assert.ThrowsException<ArgumentNullException>(() => info.ResolveBreakpoint(null!, 10));
        }
    }
}
