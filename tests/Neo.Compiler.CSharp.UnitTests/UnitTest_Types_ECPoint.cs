// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Types_ECPoint.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Interpreters;
using Neo.SmartContract.Testing.InvalidTypes;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types_ECPoint : DebugAndTestBase<Contract_Types_ECPoint>
    {
        [TestMethod]
        public void TestToECPoint()
        {
            Assert.IsTrue(Contract.ToECPoint(new byte[33]));
            Assert.IsFalse(Contract.ToECPoint(new byte[32]));
            Assert.IsFalse(Contract.ToECPoint(new byte[34]));
            Assert.IsFalse(Contract.ToECPoint(null));
        }

        [TestMethod]
        public void ECPoint_test()
        {
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.InvalidLength));
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.InvalidType));
            Assert.IsFalse(Contract.IsValid(InvalidECPoint.Null));
            Assert.IsTrue(Contract.IsValid(Cryptography.ECC.ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Cryptography.ECC.ECCurve.Secp256r1)));

            Engine.StringInterpreter = new HexStringInterpreter();

            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.Ecpoint2String());
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.EcpointReturn()?.ToString());
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", (Contract.Ecpoint2ByteArray() as VM.Types.Buffer)!.GetSpan().ToHexString());
        }
    }
}
