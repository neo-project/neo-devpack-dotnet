// Copyright (C) 2015-2024 The Neo Project.
//
// UnitTest_NEP17.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NEP17 : Nep17Tests<Contract_NEP17>
    {
        #region Expected values in base tests

        public override BigInteger ExpectedTotalSupply => 0;
        public override string ExpectedSymbol => "TEST";
        public override byte ExpectedDecimals => 8;

        #endregion

        /// <summary>
        /// Initialize Test
        /// </summary>
        public UnitTest_NEP17() : base(Contract_NEP17.Nef, Contract_NEP17.Manifest)
        {
            _ = TestCleanup.TestInitialize(typeof(Contract_NEP17));
        }

        [TestMethod]
        public override void TestTransfer()
        {
            // Contract has no mint
        }
    }
}
