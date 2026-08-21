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
    public void AssertMessageIsEvaluatedWhenConditionIsTrue()
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
                    _counter++;
                    return true;
                }

                private static string Message()
                {
                    _counter++;
                    return "message";
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<AssertOrderContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(2), contract.AssertMessage());
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
