// Copyright (C) 2015-2026 The Neo Project.
//
// StdLibTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class StdLibTest : DebugAndTestBase<Contract_StdLib>
    {
        [TestMethod]
        public void AtoiTest()
        {
            Assert.AreEqual(-1, Contract.Atoi("-1", 10));
        }

        [TestMethod]
        public void ItoaTest()
        {
            Assert.AreEqual("-1", Contract.Itoa(-1, 10));
        }

        [TestMethod]
        public void Base64DecodeTest()
        {
            Assert.AreEqual("test", Encoding.UTF8.GetString(Contract.Base64Decode("dGVzdA==")!));
        }

        [TestMethod]
        public void Base64EncodeTest()
        {
            Assert.AreEqual("dGVzdA==", Contract.Base64Encode(Encoding.UTF8.GetBytes("test")));
        }

        [TestMethod]
        public void Base64Url()
        {
            Assert.AreEqual("test", Encoding.UTF8.GetString(Contract.Base64UrlDecode("dGVzdA")!));
            Assert.AreEqual("dGVzdA", Contract.Base64UrlEncode(Encoding.UTF8.GetBytes("test")));
            Assert.AreEqual("MDA_", Contract.Base64UrlEncode(Encoding.UTF8.GetBytes("00?")));
            Assert.AreEqual("00?", Encoding.UTF8.GetString(Contract.Base64UrlDecode("MDA_")!));
        }

        [TestMethod]
        public void Base58DecodeTest()
        {
            Assert.AreEqual("test", Encoding.UTF8.GetString(Contract.Base58Decode("3yZe7d")!));
        }

        [TestMethod]
        public void Base58EncodeTest()
        {
            Assert.AreEqual("3yZe7d", Contract.Base58Encode(Encoding.UTF8.GetBytes("test")));
        }

        [TestMethod]
        public void Base58CheckEncodeTest()
        {
            Assert.AreEqual("LUC1eAJa5jW", Contract.Base58CheckEncode(Encoding.UTF8.GetBytes("test")));
        }

        [TestMethod]
        public void Base58CheckDecodeTest()
        {
            Assert.AreEqual("test", Encoding.UTF8.GetString(Contract.Base58CheckDecode("LUC1eAJa5jW")!));
        }

        [TestMethod]
        public void MemoryCompareTest()
        {
            Assert.AreEqual(-1, Contract.MemoryCompare(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c")));
            Assert.AreEqual(-1, Contract.MemoryCompare(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("d")));
            Assert.AreEqual(0, Contract.MemoryCompare(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("abc")));
            Assert.AreEqual(-1, Contract.MemoryCompare(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("abcd")));
        }

        [TestMethod]
        public void StringSplitTest()
        {
            CollectionAssert.AreEqual(new ByteString[] { "a", "b", "c" }, Contract.StringSplit1("a,b,c", ",")!.ToArray());
            CollectionAssert.AreEqual(new ByteString[] { "a", "", "c" }, Contract.StringSplit2("a,,c", ",", false)!.ToArray());
            CollectionAssert.AreEqual(new ByteString[] { "a", "c" }, Contract.StringSplit2("a,,c", ",", true)!.ToArray());
        }

        [TestMethod]
        public void ByteStringSplitTest()
        {
            var abc = Encoding.UTF8.GetBytes("a,b,c");
            var ac = Encoding.UTF8.GetBytes("a,,c");
            var separator = Encoding.UTF8.GetBytes(",");
            CollectionAssert.AreEqual(new ByteString[] { "a", "b", "c" }, Contract.ByteStringSplit(abc, separator, false)!.ToArray());
            CollectionAssert.AreEqual(new ByteString[] { "a", "", "c" }, Contract.ByteStringSplit(ac, separator, false)!.ToArray());
            CollectionAssert.AreEqual(new ByteString[] { "a", "c" }, Contract.ByteStringSplit(ac, separator, true)!.ToArray());
        }

        [TestMethod]
        public void MemorySearchTest()
        {
            Assert.AreEqual(0, Contract.MemorySearch1(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("a")));
            Assert.AreEqual(1, Contract.MemorySearch1(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("b")));
            Assert.AreEqual(2, Contract.MemorySearch1(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c")));
            Assert.AreEqual(-1, Contract.MemorySearch1(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("d")));

            Assert.AreEqual(2, Contract.MemorySearch2(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 0));
            Assert.AreEqual(2, Contract.MemorySearch2(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 1));
            Assert.AreEqual(2, Contract.MemorySearch2(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 2));
            Assert.AreEqual(-1, Contract.MemorySearch2(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 3));
            Assert.AreEqual(-1, Contract.MemorySearch2(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("d"), 0));

            Assert.AreEqual(2, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 0, false));
            Assert.AreEqual(2, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 1, false));
            Assert.AreEqual(2, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 2, false));
            Assert.AreEqual(-1, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 3, false));
            Assert.AreEqual(-1, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("d"), 0, false));

            Assert.AreEqual(-1, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 0, true));
            Assert.AreEqual(-1, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 1, true));
            Assert.AreEqual(-1, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 2, true));
            Assert.AreEqual(2, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("c"), 3, true));
            Assert.AreEqual(-1, Contract.MemorySearch3(Encoding.UTF8.GetBytes("abc"), Encoding.UTF8.GetBytes("d"), 0, true));
        }
    }
}
