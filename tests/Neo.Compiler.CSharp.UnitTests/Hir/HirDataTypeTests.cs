using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirDataTypeTests
{
    [TestMethod]
    public void Struct_Fields_Are_Ordered_Alphabetically()
    {
        const string source = @"
public sealed class C
{
    private struct Payload
    {
        public int B;
        public int A;
    }

    public static object Create()
    {
        return new Payload { B = 2, A = 1 };
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var newStruct = function.Blocks.SelectMany(b => b.Instructions).OfType<HirNewStruct>().Single();
        var structType = (HirStructType)newStruct.Type;

        CollectionAssert.AreEqual(new[] { "A", "B" }, structType.Fields.Select(f => f.Name).ToArray(), "Struct fields should be canonicalised alphabetically.");
    }

    [TestMethod]
    public void StaticFieldAssignment_EmitsStoreInstruction()
    {
        const string source = @"
public sealed class C
{
    private static int Counter;

    public static void Set(int value)
    {
        Counter = value;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        Assert.IsTrue(function.Blocks.SelectMany(b => b.Instructions).OfType<HirStoreStaticField>().Any(), "Static field assignment should use HirStoreStaticField.");
    }

    [TestMethod]
    public void TupleReturn_Is_Lowered_To_HirNewStruct()
    {
        const string source = @"
public sealed class C
{
    public static (int,int) Pair(int value)
    {
        return (value, value + 1);
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var newStruct = function.Blocks.SelectMany(b => b.Instructions).OfType<HirNewStruct>().Single();
        var structType = (HirStructType)newStruct.Type;

        CollectionAssert.AreEqual(new[] { "Item1", "Item2" }, structType.Fields.Select(f => f.Name).ToArray());
        Assert.AreEqual(2, newStruct.Fields.Count, "Tuple construction should supply two operands.");
    }

    [TestMethod]
    public void InlineCollectionExpression_Lowers_To_ArrayNew()
    {
        const string source = @"
public sealed class C
{
    public static int[] Build()
    {
        int[] values = [1, 2, 3];
        return values;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var arrayNew = function.Blocks.SelectMany(b => b.Instructions).OfType<HirArrayNew>().First();
        var lengthConst = arrayNew.Length as HirConstInt;
        Assert.IsNotNull(lengthConst);
        Assert.AreEqual(3, (int)lengthConst!.Value);

        var sets = function.Blocks.SelectMany(b => b.Instructions).OfType<HirArraySet>().ToArray();
        Assert.AreEqual(3, sets.Length, "Inline initializer should expand to array set instructions.");
    }

    [TestMethod]
    public void JaggedArray_Produces_Nested_ArrayNew()
    {
        const string source = @"
public sealed class C
{
    public static int[][] Jagged()
    {
        return new int[][] { new[] { 1 }, new[] { 2 } };
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var arrayNews = function.Blocks.SelectMany(b => b.Instructions).OfType<HirArrayNew>().ToList();
        Assert.IsTrue(arrayNews.Count >= 3, "Jagged arrays should allocate outer and inner arrays.");
        Assert.IsTrue(arrayNews.Any(a => a.Type is HirArrayType { ElementType: HirArrayType }), "Outer array should have array element type.");
    }

    [TestMethod]
    public void EnumParameters_Map_To_IntType()
    {
        const string source = @"
public sealed class C
{
    private enum Color { Red, Blue }

    public static int Identity(Color c) => (int)c;
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var signature = function.Signature;
        Assert.IsInstanceOfType(signature.ParameterTypes.Single(), typeof(HirIntType));
    }
}
