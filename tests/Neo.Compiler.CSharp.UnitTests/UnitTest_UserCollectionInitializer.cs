using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_UserCollectionInitializer
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.Collections;

        public class CollectionContract : SmartContract
        {
            public class UserCollection : IEnumerable
            {
                public int Sum;
                public int Constructed;

                public UserCollection() { Constructed = 7; }
                public void Add(int value) { Sum = Sum * 10 + value; }
                public IEnumerator GetEnumerator() { return null; }
            }

            public struct UserPairCollection : IEnumerable
            {
                public int Sum;
                public void Add(int first, int second) { Sum += first * 10 + second; }
                public IEnumerator GetEnumerator() { return null; }
            }

            public static int ClassSum() => new UserCollection { 1, 2 }.Sum;
            public static int ClassConstructor() => new UserCollection { 5 }.Constructed;
            public static int StructSum() => new UserPairCollection { { 3, 4 } }.Sum;
            public static int FrameworkList() => new List<int> { 1, 2 }[1];
        }
        """;

    [TestMethod]
    public void CollectionInitializersPreserveUserInstancesAndCallAdd()
    {
        var context = TestHelper.CompileSingleContract(Source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<CollectionContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(12, contract.ClassSum());
        Assert.AreEqual(7, contract.ClassConstructor());
        Assert.AreEqual(34, contract.StructSum());
        Assert.AreEqual(2, contract.FrameworkList());
    }

    public abstract class CollectionContract(SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("classSum")]
        public abstract int? ClassSum();

        [DisplayName("classConstructor")]
        public abstract int? ClassConstructor();

        [DisplayName("structSum")]
        public abstract int? StructSum();

        [DisplayName("frameworkList")]
        public abstract int? FrameworkList();
    }
}
