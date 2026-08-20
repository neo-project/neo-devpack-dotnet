using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ArgumentEvaluationOrder
{
    [TestMethod]
    public void MethodCallArgumentsEvaluateLeftToRight()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static int _counter;

                [DisplayName("positional")]
                public static int Positional()
                {
                    _counter = 0;
                    return Combine(Next(), Next());
                }

                [DisplayName("named")]
                public static int Named()
                {
                    _counter = 0;
                    return Combine(first: Next(), second: Next());
                }

                [DisplayName("namedOutOfOrder")]
                public static int NamedOutOfOrder()
                {
                    _counter = 0;
                    return Combine(second: Next(), first: Next());
                }

                public static int Main()
                {
                    _counter = 0;
                    return Combine(Next(), Next());
                }

                private static int Next()
                {
                    _counter++;
                    return _counter;
                }

                private static int Combine(int first, int second)
                {
                    return first * 10 + second;
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<ArgumentOrderContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(12), contract.Positional());
        Assert.AreEqual(new BigInteger(12), contract.Named());
        Assert.AreEqual(new BigInteger(21), contract.NamedOutOfOrder());
    }

    [TestMethod]
    public void ExpandedParamsArgumentsEvaluateLeftToRight()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static int _counter;

                [DisplayName("expandedParams")]
                public static int ExpandedParams()
                {
                    _counter = 0;
                    return Combine(Next(), Next(), Next());
                }

                [DisplayName("namedExpandedParams")]
                public static int NamedExpandedParams()
                {
                    _counter = 0;
                    return Combine(first: Next(), Next(), Next());
                }

                private static int Next()
                {
                    _counter++;
                    return _counter;
                }

                private static int Combine(int first, params int[] remaining)
                {
                    return first * 100 + remaining[0] * 10 + remaining[1];
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<ParamsOrderContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(123), contract.ExpandedParams());
        Assert.AreEqual(new BigInteger(123), contract.NamedExpandedParams());
    }

    public abstract class ArgumentOrderContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("positional")]
        public abstract BigInteger? Positional();

        [DisplayName("named")]
        public abstract BigInteger? Named();

        [DisplayName("namedOutOfOrder")]
        public abstract BigInteger? NamedOutOfOrder();
    }

    public abstract class ParamsOrderContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("expandedParams")]
        public abstract BigInteger? ExpandedParams();

        [DisplayName("namedExpandedParams")]
        public abstract BigInteger? NamedExpandedParams();
    }
}
