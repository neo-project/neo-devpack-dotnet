// Copyright (C) 2015-2025 The Neo Project.
//
// SequencePointInserterTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class SequencePointInserterTest : DebugAndTestBase<Contract_SequencePointInserter>
    {
        [TestMethod]
        public void Test_SequencePointInserter()
        {
            var debug = TestCleanup.CachedContracts[typeof(Contract_SequencePointInserter)].DbgInfo;
            Assert.IsNotNull(debug);

            var points = debug.Methods[0].SequencePoints.Select(u => u.Address).ToArray();

            // Ensure that all the instructions have sequence point

            var ip = 0;
            Script script = NefFile.Script;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);

                if (ip != 0) // Avoid INITSLOT
                {
                    Assert.IsTrue(points.Contains(ip), $"Offset {ip} with '{instruction.OpCode}' is not in sequence points.");
                }

                ip += instruction.Size;
            }
        }

        [TestMethod]
        public void Test_If()
        {
            Assert.AreEqual(23, Contract.Test(1));
            Assert.AreEqual(45, Contract.Test(0));
        }
    }
}
