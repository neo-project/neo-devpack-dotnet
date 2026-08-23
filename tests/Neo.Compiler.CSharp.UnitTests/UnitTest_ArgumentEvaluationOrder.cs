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

                [DisplayName("emptyExpandedParams")]
                public static int EmptyExpandedParams()
                {
                    _counter = 0;
                    return Combine(Next());
                }

                [DisplayName("explicitParamsArray")]
                public static int ExplicitParamsArray()
                {
                    _counter = 0;
                    return CombineOptional(remaining: new[] { Next(), Next() });
                }

                [DisplayName("paramsOnly")]
                public static int ParamsOnly()
                {
                    _counter = 0;
                    return CombineParamsOnly(Next(), Next(), Next());
                }

                private static int Next()
                {
                    _counter++;
                    return _counter;
                }

                private static int Combine(int first, params int[] remaining)
                {
                    return first * 100 +
                        (remaining.Length > 0 ? remaining[0] * 10 : 0) +
                        (remaining.Length > 1 ? remaining[1] : 0);
                }

                private static int CombineOptional(int first = 9, params int[] remaining)
                {
                    return first * 100 + remaining[0] * 10 + remaining[1];
                }

                private static int CombineParamsOnly(params int[] values)
                {
                    return values[0] * 100 + values[1] * 10 + values[2];
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<ParamsOrderContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(123), contract.ExpandedParams());
        Assert.AreEqual(new BigInteger(123), contract.NamedExpandedParams());
        Assert.AreEqual(new BigInteger(100), contract.EmptyExpandedParams());
        Assert.AreEqual(new BigInteger(912), contract.ExplicitParamsArray());
        Assert.AreEqual(new BigInteger(123), contract.ParamsOnly());
    }

    [TestMethod]
    public void AssertArgumentsEvaluateLeftToRightWhenConditionIsTrue()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static int _counter;

                [DisplayName("assertMessage")]
                public static int AssertMessage()
                {
                    _counter = 0;
                    ExecutionEngine.Assert(Condition(), Message());
                    return _counter;
                }

                private static bool Condition()
                {
                    _counter = _counter * 10 + 1;
                    return true;
                }

                private static string Message()
                {
                    _counter = _counter * 10 + 2;
                    return "message";
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<AssertOrderContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(12), contract.AssertMessage());
    }

    [TestMethod]
    public void InstanceReceiverEvaluatesBeforeArguments()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static int _counter;

                [DisplayName("receiverBeforeArgument")]
                public static int ReceiverBeforeArgument()
                {
                    _counter = 0;
                    return NextReceiver().Combine(NextArgument());
                }

                private static Recorder NextReceiver()
                {
                    _counter++;
                    return new Recorder(_counter);
                }

                private static int NextArgument()
                {
                    _counter++;
                    return _counter;
                }

                private class Recorder
                {
                    private readonly int _order;

                    public Recorder(int order)
                    {
                        _order = order;
                    }

                    public int Combine(int argument)
                    {
                        return _order * 10 + argument;
                    }
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<ReceiverOrderContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(12), contract.ReceiverBeforeArgument());
    }

    [TestMethod]
    public void NamedArgumentBeforeExpandedParamsBindsToCorrectParameter()
    {
        const string source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            private static int _counter;

            [DisplayName("namedThenPositional")]
            public static int NamedThenPositional()
            {
                // "a" is named but occupies its correct positional slot (ordinal 0),
                // followed by plain positional arguments for "b" and the params array.
                // Equivalent to calling Combine(1, 2, 3).
                return Combine(a: 1, 2, 3);
            }

            [DisplayName("namedThenPositionalWithSideEffects")]
            public static int NamedThenPositionalWithSideEffects()
            {
                _counter = 0;
                return Combine(a: Next(), Next(), Next());
            }

            private static int Next()
            {
                _counter++;
                return _counter;
            }

            private static int Combine(int a, int b, params int[] c)
            {
                // Expected: a=1, b=2, c=[3] => 1*1000 + 2*100 + (c.Length > 0 ? c[0] : 0) = 1203
                return a * 1000 + b * 100 + (c.Length > 0 ? c[0] : 0);
            }
        }
        """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<NamedThenPositionalContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(1203), contract.NamedThenPositional());
        Assert.AreEqual(new BigInteger(1203), contract.NamedThenPositionalWithSideEffects());
    }

    public abstract class NamedThenPositionalContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("namedThenPositional")]
        public abstract BigInteger? NamedThenPositional();

        [DisplayName("namedThenPositionalWithSideEffects")]
        public abstract BigInteger? NamedThenPositionalWithSideEffects();
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

        [DisplayName("emptyExpandedParams")]
        public abstract BigInteger? EmptyExpandedParams();

        [DisplayName("explicitParamsArray")]
        public abstract BigInteger? ExplicitParamsArray();

        [DisplayName("paramsOnly")]
        public abstract BigInteger? ParamsOnly();
    }

    public abstract class AssertOrderContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("assertMessage")]
        public abstract BigInteger? AssertMessage();
    }

    public abstract class ReceiverOrderContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("receiverBeforeArgument")]
        public abstract BigInteger? ReceiverBeforeArgument();
    }
}
