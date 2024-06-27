using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Array : TestBase<Contract_Array>
    {
        public UnitTest_Array() : base(Contract_Array.Nef, Contract_Array.Manifest) { }

        [TestMethod]
        public void Test_GetTreeByteLengthPrefix()
        {
            var result = Contract.GetTreeByteLengthPrefix();

            CollectionAssert.AreEqual(new byte[] { 0x01, 0x03 }, result);
        }

        [TestMethod]
        public void Test_GetTreeByteLengthPrefix2()
        {
            var result = Contract.GetTreeByteLengthPrefix2();

            CollectionAssert.AreEqual(new byte[] { 0x01, 0x03 }, result);
        }

        [TestMethod]
        public void Test_JaggedArray()
        {
            var arr = Contract.TestJaggedArray();

            Assert.AreEqual(4, arr?.Count);
            var element0 = (Array?)arr?[0];
            CollectionAssert.AreEqual(new Integer[] { 1, 2, 3, 4 }, element0?.ToArray());
        }

        [TestMethod]
        public void Test_JaggedByteArray()
        {
            var arr = Contract.TestJaggedByteArray();

            Assert.AreEqual(4, arr?.Count);
            var element0 = (byte[]?)arr?[0];
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, element0);
        }

        [TestMethod]
        public void Test_EmptyArray()
        {
            var arr = Contract.TestEmptyArray();

            Assert.AreEqual(0, arr?.Count);
        }

        [TestMethod]
        public void Test_IntArray()
        {
            var arr = Contract.TestIntArray();

            //test 0,1,2
            CollectionAssert.AreEqual(new BigInteger[] { 0, 1, 2 }, arr?.ToArray());
        }

        [TestMethod]
        public void Test_IntArrayInit()
        {
            //test 1,4,5
            var arr = Contract.TestIntArrayInit();
            CollectionAssert.AreEqual(new BigInteger[] { 1, 4, 5 }, arr?.ToArray());


            //test 1,4,5
            arr = Contract.TestIntArrayInit2();
            CollectionAssert.AreEqual(new BigInteger[] { 1, 4, 5 }, arr?.ToArray());

            //test 1,4,5
            arr = Contract.TestIntArrayInit3();
            CollectionAssert.AreEqual(new BigInteger[] { 1, 4, 5 }, arr?.ToArray());
        }

        [TestMethod]
        public void Test_StructArray()
        {
            var result = Contract.TestStructArray();

            //test (1+5)*7 == 42
            var bequal = result as Struct != null;
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_StructArrayInit()
        {
            var result = Contract.TestStructArrayInit();

            //test (1+5)*7 == 42
            var bequal = result as Struct != null;
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_ByteArrayOwner()
        {
            var bts = Contract.TestByteArrayOwner();

            CollectionAssert.AreEqual(new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe }, bts);
        }

        [TestMethod]
        public void Test_DynamicArrayInit()
        {
            var arr = Contract.TestDynamicArrayInit(3);

            Assert.AreEqual(3, arr?.Count);
            Assert.AreEqual(new BigInteger(0), arr?[0]);
            Assert.AreEqual(new BigInteger(1), arr?[1]);
            Assert.AreEqual(new BigInteger(2), arr?[2]);
        }

        [TestMethod]
        public void Test_DynamicArrayStringInit()
        {
            var arr = Contract.TestDynamicArrayStringInit("hello");

            Assert.AreEqual(5, arr?.Length);
            Assert.IsTrue(arr?.All(a => a == 0));
        }

        [TestMethod]
        public void Test_ByteArrayOwnerCall()
        {
            var bts = Contract.TestByteArrayOwnerCall();

            CollectionAssert.AreEqual(new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe }, bts);
        }

        [TestMethod]
        public void Test_StringArray()
        {
            var items = Contract.TestSupportedStandards();

            Assert.AreEqual((ByteString)"NEP-5", items?[0]);
            Assert.AreEqual((ByteString)"NEP-10", items?[1]);
        }

        [TestMethod]
        public void Test_Collectionexpressions()
        {
            var arr = Contract.TestCollectionexpressions();

            Assert.AreEqual(4, arr?.Count);

            var element0 = (Array?)arr?[0];
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 },
                element0?.Cast<PrimitiveType>().Select(u => (int)u.GetInteger()).ToArray());

            var element1 = (Array?)arr?[1];
            CollectionAssert.AreEqual(new[] { "one", "two", "three" },
                element1?.Cast<PrimitiveType>().Select(u => u.GetString()).ToArray());

            var element2 = (Array?)arr?[2];
            Assert.AreEqual(3, element2?.Count);
            var element2_0 = (Array?)element2?[0];
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 },
                element2_0?.Cast<PrimitiveType>().Select(u => (int)u.GetInteger()).ToArray());

            var element3 = (Array?)arr?[3];
            Assert.AreEqual(3, element3?.Count);
            var element3_0 = (Array?)element3?[0];
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 },
                element3_0?.Cast<PrimitiveType>().Select(u => (int)u.GetInteger()).ToArray());
        }
    }
}
