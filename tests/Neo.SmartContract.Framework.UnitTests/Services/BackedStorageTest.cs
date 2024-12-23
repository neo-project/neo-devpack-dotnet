using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class BackedStorageTest : DebugAndTestBase<Contract_Stored>
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(0, Engine.Storage.Snapshot.GetChangeSet().Count(u => u.Key.Id == Contract.Storage.Id));

            Test_Kind(Contract.GetWithoutConstructor, () => Contract.WithoutConstructor, i => Contract.PutWithoutConstructor(i));
            Test_Kind(Contract.GetWithKey, () => Contract.WithKey, i => Contract.PutWithKey(i));
            Test_Kind(Contract.GetWithString, () => Contract.WithString, i => Contract.PutWithString(i));

            Assert.AreEqual(3, Engine.Storage.Snapshot.GetChangeSet().Count(u => u.Key.Id == Contract.Storage.Id));
        }

        [TestMethod]
        public void Test_Private_Getter_Public_Setter()
        {
            // Read initial value & Test private getter

            Assert.AreEqual(0, Contract.PrivateGetterPublicSetter);

            // Test public setter
            Contract.PrivateGetterPublicSetter = 123;

            // check public setter

            Assert.AreEqual(123, Contract.PrivateGetterPublicSetter);
        }

        [TestMethod]
        public void Test_Non_Static_Private_Getter_Public_Setter()
        {
            // Read initial value & Test private getter

            Assert.AreEqual(0, Contract.NonStaticPrivateGetterPublicSetter);

            // Test public setter
            Contract.NonStaticPrivateGetterPublicSetter = 123;

            // check public setter

            Assert.AreEqual(123, Contract.NonStaticPrivateGetterPublicSetter);
        }

        [TestMethod]
        public void Test_StaticMultiSet()
        {
            var res = Contract.TestStaticMultiGet();
            Assert.AreEqual(0, res);
            res = Contract.TestStaticMultiSet();
            Assert.AreEqual(3, res);
            res = Contract.TestStaticMultiGet();
            Assert.AreEqual(3, res);
        }

        [TestMethod]
        public void Test_MultiSet()
        {
            var res = Contract.TestMultiGet();
            res = Contract.TestMultiSet();
            res = Contract.TestMultiGet();
            Assert.AreEqual(3, res);
        }

        public void Test_Kind(Func<BigInteger?> getter, Func<BigInteger?> publicGetter, Action<BigInteger> put)
        {
            // Read initial value

            Assert.AreEqual(0, getter());

            // Test public getter

            Assert.AreEqual(0, publicGetter());

            // Put

            put(123);

            // Get

            Assert.AreEqual(123, getter());

            // Test public getter

            Assert.AreEqual(123, publicGetter());
        }
    }
}
