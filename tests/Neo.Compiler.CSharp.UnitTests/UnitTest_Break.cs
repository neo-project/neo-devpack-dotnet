// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Break.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Break : DebugAndTestBase<Contract_Break>
    {
        [TestMethod]
        public void TestBreakInTry()
        {
            Contract.BreakInTryCatch(true);
            Contract.BreakInTryCatch(false);
        }
    }
}
