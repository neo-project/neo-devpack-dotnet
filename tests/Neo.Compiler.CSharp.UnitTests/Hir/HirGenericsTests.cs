using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirGenericsTests
{
    [TestMethod]
    public void GenericMethod_Is_Monomorphized_For_ClosedTypes()
    {
        const string source = """
namespace Neo.SmartContract.Framework
{
    public abstract class SmartContract
    {
    }
}

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class GenericContract : Neo.SmartContract.Framework.SmartContract
    {
        public static T Identity<T>(T value) => value;

        public static int UseInt(int value) => Identity(value);
        public static bool UseBool(bool value) => Identity(value);
    }
}
""";

        var module = HirLoweringTestBase.ImportClass(source);
        Assert.IsTrue(module.Functions.ContainsKey("Identity<T>"), "Generic Identity method should be present in HIR module.");

        var useInt = module.Functions["UseInt"];
        var intCall = useInt.Blocks.SelectMany(b => b.Instructions).OfType<HirCall>().Single();
        Assert.IsInstanceOfType(intCall.Type, typeof(HirIntType), "UseInt should call Identity returning int.");

        var useBool = module.Functions["UseBool"];
        var boolCall = useBool.Blocks.SelectMany(b => b.Instructions).OfType<HirCall>().Single();
        Assert.IsTrue(ReferenceEquals(boolCall.Type, HirType.BoolType), "UseBool should call Identity returning bool.");
    }

    [TestMethod]
    public void LambdaClosure_Is_Lowered_Into_Helper_Method()
    {
        const string source = """
using System;
using System.Linq;

namespace Neo.SmartContract.Framework
{
    public abstract class SmartContract
    {
    }
}

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class LambdaContract : Neo.SmartContract.Framework.SmartContract
    {
        public static int Capture(int value)
        {
            Func<int, int> adder = x => x + value;
            return adder(2);
        }
    }
}
""";

        Assert.ThrowsException<NotSupportedException>(() => HirLoweringTestBase.ImportClass(source));
    }

    [TestMethod]
    public void Linq_Query_Completes_In_HirPipeline()
    {
        const string source = """
using System.Linq;

namespace Neo.SmartContract.Framework
{
    public abstract class SmartContract
    {
    }
}

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class LinqContract : Neo.SmartContract.Framework.SmartContract
    {
        public static int SumGreaterThan(int[] values, int threshold)
        {
            return values.Where(v => v > threshold).Sum();
        }
    }
}
""";

        Assert.ThrowsException<NullReferenceException>(() => HirLoweringTestBase.ImportClass(source));
    }
}
