using System;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;
using Neo.Compiler.MIR;
using Neo.Compiler.MiddleEnd.Lowering;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public class HirLoweringTests
{
    [TestMethod]
    public void ArrayInitializer_Is_Lowered_To_ArraySet()
    {
        const string source = @"
using System;

public sealed class C
{
    public int[] M()
    {
        var data = new[] { 10, 20, 30 };
        return data;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);

        var entry = function.Entry;
        Assert.AreEqual("entry", entry.Label);
        Assert.IsInstanceOfType(entry.Terminator, typeof(HirBranch), "Entry block should branch to the first body block.");

        var bodyBlock = function.Blocks.First(b => b != entry);

        var arrayNew = bodyBlock.Instructions.OfType<HirArrayNew>().SingleOrDefault();
        Assert.IsNotNull(arrayNew, "Array initializer should create array via HirArrayNew.");
        Assert.AreEqual(3, (arrayNew!.Length as HirConstInt)?.Value, "Array length should match initializer element count.");

        var arraySets = bodyBlock.Instructions.OfType<HirArraySet>().ToList();
        Assert.AreEqual(3, arraySets.Count, "Each initializer element should become a HirArraySet.");

        for (int i = 0; i < arraySets.Count; i++)
        {
            var set = arraySets[i];
            var indexConst = set.Index as HirConstInt;
            Assert.IsNotNull(indexConst, "Array index should be a constant.");
            Assert.AreEqual(i, (int)indexConst!.Value, $"Initializer index mismatch at position {i}.");

            var valueConst = set.Value as HirConstInt;
            Assert.IsNotNull(valueConst, "Array initializer value should be a constant int.");
            Assert.AreEqual((i + 1) * 10, (int)valueConst!.Value, $"Initializer value mismatch at position {i}.");
        }

        var bodyBlockWithReturn = function.Blocks.First(b => b.Terminator is HirReturn);
        Assert.IsInstanceOfType(bodyBlockWithReturn.Terminator, typeof(HirReturn), "Function should return array.");
    }

    [TestMethod]
    public void ForLoop_AssignsPhiForLoopCarriedState()
    {
        const string source = @"
using System;

public sealed class C
{
    public int M(int n)
    {
        var total = 0;
        for (var i = 0; i < n; i++)
        {
            total += i;
        }

        return total;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);

        var exitBlock = function.Blocks.Single(b => b.Label.StartsWith("for_exit", StringComparison.Ordinal));
        var phi = exitBlock.Phis.SingleOrDefault();
        Assert.IsNotNull(phi, "Loop-carried variable should produce a phi node at exit block.");
        Assert.AreEqual(2, phi!.Inputs.Count, "Phi should merge pre-loop and loop body values.");
        CollectionAssert.AreEquivalent(new[] { "for_body", "for_cond" }, phi.Inputs.Select(i => i.Block.Label).ToArray());
    }

    [TestMethod]
    public void TryFinally_PreservesAssignmentsAcrossBlocks()
    {
        const string source = @"
using System;

public sealed class C
{
    public int M(int flag)
    {
        var value = 1;
        try
        {
            value = flag;
        }
        finally
        {
            value = value + 1;
        }

        return value;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);

        var mergeBlock = function.Blocks.Single(b => b.Label.StartsWith("try_merge", StringComparison.Ordinal));
        var phi = mergeBlock.Phis.SingleOrDefault();
        Assert.IsNotNull(phi, "Try/finally should merge local state via phi at merge block.");
        Assert.AreEqual(2, phi!.Inputs.Count, "Phi should merge values from try body and finally block.");
    }

    [TestMethod]
    public void TryFinally_ArrayAccess_Is_Localised_For_Scope_State()
    {
        const string source = @"
using System;

public sealed class C
{
    public static int M(int[] data, int index)
    {
        var value = -1;
        try
        {
            value = data[index];
        }
        finally
        {
            value = value + 1;
        }

        return value;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var instructions = function.Blocks.SelectMany(b => b.Instructions).ToList();

        var arrayCaptureStore = instructions
            .OfType<HirStoreLocal>()
            .FirstOrDefault(store => store.Local.Name.StartsWith("arrget_capture", StringComparison.Ordinal));
        Assert.IsNotNull(arrayCaptureStore, "Importer should capture array element access into a synthetic local.");
        Assert.IsInstanceOfType(arrayCaptureStore!.Value, typeof(HirArrayGet), "Captured store should wrap the array access.");

        var scopeStateStore = instructions
            .OfType<HirStoreLocal>()
            .FirstOrDefault(store => store.Local.Name.StartsWith("try_state_value", StringComparison.Ordinal));
        Assert.IsNotNull(scopeStateStore, "Try scope state should persist locals via synthetic slots.");
        Assert.IsInstanceOfType(scopeStateStore!.Value, typeof(HirLoadLocal), "Scope state store should load from the captured local.");

        var loaded = (HirLoadLocal)scopeStateStore.Value;
        Assert.IsTrue(loaded.Local.Name.StartsWith("arrget_capture", StringComparison.Ordinal),
            "Scope state should reference the captured array access.");
    }

    [TestMethod]
    public void NoReentrantAttribute_Is_Surfaced_As_HirAttribute()
    {
        const string source = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

public sealed class C
{
    [NoReentrant]
    public static void M()
    {
        return;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var attribute = function.Signature.Attributes.OfType<HirNoReentrantAttribute>().SingleOrDefault();
        Assert.IsNotNull(attribute, "NoReentrant attribute should be attached to the HIR signature.");
        Assert.AreEqual((byte)0xFF, attribute!.Prefix, "Default prefix should be preserved.");
        Assert.AreEqual("noReentrant", attribute.Key, "Default key should be preserved.");
    }

    [TestMethod]
    public void AggressiveInlining_Is_Recorded_As_HirAttribute()
    {
        const string source = @"
using System.Runtime.CompilerServices;

public sealed class C
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Inline(int value)
    {
        return value;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var inlineAttr = function.Signature.Attributes.OfType<HirInlineAttribute>().SingleOrDefault();
        Assert.IsNotNull(inlineAttr, "AggressiveInlining should be captured as HirInlineAttribute.");
        Assert.IsTrue(inlineAttr!.Aggressive, "Inline attribute should mark aggressive hint.");
    }

    [TestMethod]
    public void NativeContractCall_Lowers_To_PointerCall_With_CallTableIndex()
    {
        const string source = @"
using Neo.SmartContract.Framework.Native;

public sealed class C
{
    public static string Format(int value)
    {
        return StdLib.Itoa(value);
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var pointerCall = function.Blocks.SelectMany(b => b.Instructions).OfType<HirPointerCall>().SingleOrDefault();

        Assert.IsNotNull(pointerCall, "Native contract call should lower to HirPointerCall.");
        Assert.IsNull(pointerCall!.Pointer, "Call-table based pointer call should not require a pointer operand.");
        Assert.IsTrue(pointerCall.IsTailCall, "Native contract calls should surface as tail calls (CALLT).");
        Assert.IsTrue(pointerCall.CallTableIndex.HasValue, "Pointer call should carry a call-table index.");
        Assert.AreEqual(1, pointerCall.Arguments.Count, "Expected exactly one argument passed to StdLib.Itoa.");
    }

    [TestMethod]
    public void TryFinally_Lowers_To_HirLeave_And_EndFinally()
    {
        const string source = @"
public sealed class C
{
    public static int M(int value)
    {
        var result = value;
        try
        {
            result = value + 1;
        }
        finally
        {
            var temp = value;
        }

        return result;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);

        var scope = function.Blocks.SelectMany(b => b.Instructions).OfType<HirTryFinallyScope>().SingleOrDefault();
        Assert.IsNotNull(scope, "Try/finally should produce a HirTryFinallyScope marker.");

        var tryBlock = scope!.TryBlock;
        var finallyBlock = scope.FinallyBlock;
        var mergeBlock = scope.MergeBlock;

        Assert.IsInstanceOfType(tryBlock.Terminator, typeof(HirLeave), "Try block should terminate with HirLeave.");
        var leave = (HirLeave)tryBlock.Terminator!;
        Assert.AreSame(scope, leave.Scope, "HirLeave should reference the enclosing try scope.");
        Assert.AreSame(mergeBlock, leave.Target, "Leave target should be the merge block.");

        Assert.IsInstanceOfType(finallyBlock.Terminator, typeof(HirEndFinally), "Finally block should terminate with HirEndFinally.");
        var endFinally = (HirEndFinally)finallyBlock.Terminator!;
        Assert.AreSame(scope, endFinally.Scope, "HirEndFinally should reference the enclosing try scope.");
        Assert.AreSame(mergeBlock, endFinally.Target, "EndFinally target should be the merge block.");

        Assert.IsInstanceOfType(mergeBlock.Terminator, typeof(HirReturn), "Merge block should return the computed result.");
    }

    [TestMethod]
    public void TryFinally_Lowers_To_MirLeave_And_EndFinally()
    {
        const string source = @"
public sealed class C
{
    public static int M(int value)
    {
        var result = value;
        try
        {
            result = value + 1;
        }
        finally
        {
            var temp = value;
        }

        return result;
    }
}
";

        var hirFunction = HirLoweringTestBase.LowerMethod(source);
        var module = new MirModule();
        var lowerer = new HirToMirLowerer();
        var mirFunction = lowerer.Lower(hirFunction, module);

        var scope = mirFunction.Blocks.SelectMany(b => b.Instructions).OfType<MirTry>().SingleOrDefault();
        Assert.IsNotNull(scope, "Try/finally lowering should create a MirTry instruction.");

        var tryBlock = scope!.TryBlock;
        Assert.IsInstanceOfType(tryBlock.Terminator, typeof(MirLeave), "Try block should terminate with MirLeave.");
        var leave = (MirLeave)tryBlock.Terminator!;
        Assert.AreSame(scope, leave.Scope, "MirLeave should reference the enclosing MirTry scope.");
        Assert.AreSame(scope.MergeBlock, leave.Target, "MirLeave target should be the merge block.");

        var finallyBlock = scope.FinallyBlock;
        Assert.IsInstanceOfType(finallyBlock.Terminator, typeof(MirEndFinally), "Finally block should terminate with MirEndFinally.");
        var endFinally = (MirEndFinally)finallyBlock.Terminator!;
        Assert.AreSame(scope, endFinally.Scope, "MirEndFinally should reference the enclosing MirTry scope.");
        Assert.AreSame(scope.MergeBlock, endFinally.Target, "MirEndFinally target should be the merge block.");
    }

    [TestMethod]
    public void TryFinally_InLoop_Emits_HirLeave_For_Break_And_Continue()
    {
        const string source = @"
public sealed class C
{
    public static int M(int value)
    {
        var result = value;
        while (value < 3)
        {
            try
            {
                if (value == 0)
                    continue;
                if (value == 1)
                    break;

                result = value;
            }
            finally
            {
            }
        }

        return result;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);

        var scope = function.Blocks.SelectMany(b => b.Instructions).OfType<HirTryFinallyScope>().SingleOrDefault();
        Assert.IsNotNull(scope, "Try/finally should produce a HirTryFinallyScope marker.");

        var leaves = function.Blocks
            .Select(b => b.Terminator)
            .OfType<HirLeave>()
            .ToArray();

        Assert.IsTrue(leaves.Length >= 2, "Expected HirLeave terminators for both break and continue paths.");

        Assert.IsTrue(
            leaves.Any(l => l.Scope == scope && l.Target.Label.StartsWith("while_cond", StringComparison.Ordinal)),
            "Continue inside try should lower to HirLeave targeting the loop condition block.");

        Assert.IsTrue(
            leaves.Any(l => l.Scope == scope && l.Target.Label.StartsWith("while_exit", StringComparison.Ordinal)),
            "Break inside try should lower to HirLeave targeting the loop exit block.");
    }

    [TestMethod]
    public void TryFinally_Nested_Scopes_Produce_Distinct_HirLeave()
    {
        const string source = @"
public sealed class C
{
    public static int M(int value)
    {
        var result = value;
        try
        {
            try
            {
                if (value == 0)
                    return 1;

                result = value + 2;
            }
            finally
            {
                result++;
            }
        }
        finally
        {
            result++;
        }

        return result;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);

        var scopes = function.Blocks.SelectMany(b => b.Instructions).OfType<HirTryFinallyScope>().ToArray();
        Assert.AreEqual(2, scopes.Length, "Nested try/finally should produce two HirTryFinallyScope markers.");

        var leaves = function.Blocks.Select(b => b.Terminator).OfType<HirLeave>().ToArray();
        Assert.IsTrue(leaves.Length >= 2, "Expected HirLeave terminators for each nested scope.");
        foreach (var scope in scopes)
        {
            Assert.IsTrue(leaves.Any(l => l.Scope == scope), "Each nested try scope should have at least one HirLeave terminator.");
        }

        var endFinallys = function.Blocks.Select(b => b.Terminator).OfType<HirEndFinally>().ToArray();
        Assert.IsTrue(endFinallys.Length >= 2, "Expected HirEndFinally terminators for nested finally blocks.");
        foreach (var scope in scopes)
        {
            Assert.IsTrue(endFinallys.Any(e => e.Scope == scope), "Each nested try scope should have an associated HirEndFinally terminator.");
        }
    }

    [TestMethod]
    public void TryFinally_InLoop_Lowers_To_MirLeave_Dispatch()
    {
        const string source = @"
public sealed class C
{
    public static int M(int value)
    {
        var result = value;
        while (value < 3)
        {
            try
            {
                if (value == 0)
                    continue;
                if (value == 1)
                    break;

                result = value;
            }
            finally
            {
            }
        }

        return result;
    }
}
";

        var hirFunction = HirLoweringTestBase.LowerMethod(source);
        var module = new MirModule();
        var lowerer = new HirToMirLowerer();
        var mirFunction = lowerer.Lower(hirFunction, module);

        var scope = mirFunction.Blocks.SelectMany(b => b.Instructions).OfType<MirTry>().SingleOrDefault();
        Assert.IsNotNull(scope, "Lowering should produce a MirTry node.");

        var leaves = mirFunction.Blocks
            .Select(b => b.Terminator)
            .OfType<MirLeave>()
            .ToArray();

        Assert.IsTrue(leaves.Length >= 2, "Expected MirLeave terminators for both break and continue paths.");

        Assert.IsTrue(
            leaves.Any(l => l.Scope == scope && l.Target.Label.StartsWith("while_cond", StringComparison.Ordinal)),
            "Continue inside try should lower to MirLeave targeting the loop condition block.");

        Assert.IsTrue(
            leaves.Any(l => l.Scope == scope && l.Target.Label.StartsWith("while_exit", StringComparison.Ordinal)),
            "Break inside try should lower to MirLeave targeting the loop exit block.");
    }

    [TestMethod]
    public void AggressiveInlining_Flows_To_MirFunction()
    {
        const string source = @"
using System.Runtime.CompilerServices;

public sealed class C
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Inline(int value)
    {
        return value;
    }
}
";

        var hirFunction = HirLoweringTestBase.LowerMethod(source);
        var module = new MirModule();
        var lowerer = new HirToMirLowerer();
        var mirFunction = lowerer.Lower(hirFunction, module);

        Assert.IsTrue(mirFunction.AggressiveInlineHint, "MirFunction should reflect aggressive inline hint.");
    }

    [TestMethod]
    public void FloatingPointType_Is_Rejected()
    {
        const string source = @"
public sealed class C
{
    public float M(float value)
    {
        return value;
    }
}
";

        Assert.ThrowsException<NotSupportedException>(() => HirLoweringTestBase.LowerMethod(source));
    }

    [TestMethod]
    public void BigInteger_Types_Map_To_HirInt()
    {
        const string source = @"
using System.Numerics;

public sealed class C
{
    public static BigInteger Increment(BigInteger value)
    {
        return value + 1;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var signature = function.Signature;
        Assert.AreEqual(1, signature.ParameterTypes.Count, "Static method should only expose declared parameters.");
        var paramType = signature.ParameterTypes[0] as HirIntType;
        Assert.IsNotNull(paramType, "BigInteger parameters should map to HirIntType.");
        var returnType = signature.ReturnType as HirIntType;
        Assert.IsNotNull(returnType, "BigInteger return type should map to HirIntType.");
    }

    [TestMethod]
    public void SwitchStatement_Lowers_To_HirSwitch()
    {
        const string source = @"
public sealed class C
{
    public static int Select(int value)
    {
        switch (value)
        {
            case 0:
                return 10;
            case 1:
                return 20;
            default:
                return -1;
        }
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var switchBlock = function.Blocks.Single(b => b.Terminator is HirSwitch);
        var terminator = (HirSwitch)switchBlock.Terminator!;

        Assert.AreEqual(2, terminator.Cases.Count, "Switch should surface two explicit case targets.");
        CollectionAssert.AreEquivalent(new[] { BigInteger.Zero, BigInteger.One }, terminator.Cases.Select(c => c.Case).ToArray());
        Assert.IsTrue(function.Blocks.Any(b => b == terminator.DefaultTarget), "Switch should expose default target block.");

        foreach (var (constant, target) in terminator.Cases)
        {
            var returnInst = target.Terminator as HirReturn;
            Assert.IsNotNull(returnInst, "Each switch case should terminate with return.");
            var constValue = returnInst!.Value as HirConstInt;
            Assert.IsNotNull(constValue, "Return value for switch case should be constant.");
            Assert.AreEqual(constant == BigInteger.Zero ? 10 : 20, (int)constValue!.Value);
        }

        var defaultReturn = terminator.DefaultTarget.Terminator as HirReturn;
        Assert.IsNotNull(defaultReturn, "Default switch arm should return.");
        Assert.AreEqual(-1, (int)((HirConstInt)defaultReturn!.Value!).Value);
    }

    [TestMethod]
    public void Foreach_WithBreak_And_Continue_ProducesLoopBlocks()
    {
        const string source = @"
using System;

public sealed class C
{
    public static int Sum(int[] values)
    {
        var total = 0;
        foreach (var item in values)
        {
            if (item == 0)
                continue;
            total += item;
            if (item > 5)
                break;
        }

        return total;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        Assert.IsTrue(function.Blocks.Any(b => b.Label.StartsWith("foreach_cond", StringComparison.Ordinal)), "Foreach should emit condition block.");
        Assert.IsTrue(function.Blocks.Any(b => b.Label.StartsWith("foreach_body", StringComparison.Ordinal)), "Foreach should emit body block.");
        Assert.IsTrue(function.Blocks.Any(b => b.Label.StartsWith("foreach_exit", StringComparison.Ordinal)), "Foreach should emit exit block.");

        var exitBlock = function.Blocks.Single(b => b.Label.StartsWith("foreach_exit", StringComparison.Ordinal));
        Assert.IsTrue(exitBlock.Phis.Any(), "Loop-carried variable should merge via phi in exit block.");
    }

    [TestMethod]
    public void UsingStatement_EmitsDisposeInvocation()
    {
        const string source = @"
using System;

public sealed class C
{
    private sealed class Tracker : IDisposable
    {
        public void Dispose() { }
    }

    public static int Run()
    {
        using var tracker = new Tracker();
        return 5;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        Assert.IsTrue(function.Blocks.Any(b => b.Label.StartsWith("using_body", StringComparison.Ordinal)), "Using should generate body block.");
        Assert.IsTrue(function.Blocks.Any(b => b.Label.StartsWith("using_dispose", StringComparison.Ordinal)), "Using should generate dispose block.");

        var disposeBlock = function.Blocks.Single(b => b.Label.StartsWith("using_dispose", StringComparison.Ordinal));
        var call = disposeBlock.Instructions.OfType<HirCall>().SingleOrDefault();
        Assert.IsNotNull(call, "Dispose block should issue a call.");
        StringAssert.Contains(call!.Callee, "Tracker.Dispose", "Dispose invocation should target resource dispose method.");
    }

    [TestMethod]
    public void ThrowStatement_Lowers_To_HirThrow()
    {
        const string source = @"
using System;

public sealed class C
{
    public static void Fail()
    {
        throw new InvalidOperationException();
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var throwBlock = function.Blocks.Single(b => b.Terminator is HirThrow);
        var terminator = (HirThrow)throwBlock.Terminator!;
        Assert.IsNotNull(terminator.Exception, "Throw should carry exception value.");
        Assert.IsInstanceOfType(terminator.Exception, typeof(HirNewObject));
    }

    [TestMethod]
    public void NullableReceiver_EmitsNullCheckGuard()
    {
        const string source = @"
public sealed class C
{
    public static int Length(string? value)
    {
        return value.Length;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var guard = function.Blocks.SelectMany(b => b.Instructions).OfType<HirNullCheck>().SingleOrDefault();
        Assert.IsNotNull(guard, "Nullable receiver access should inject a null guard.");
        Assert.AreEqual(HirFailPolicy.Abort, guard!.Policy, "Guard policy should abort on null.");
    }

    [TestMethod]
    public void DoWhileLoop_FormsBackEdgeBlocks()
    {
        const string source = @"
public sealed class C
{
    public static int Count(int limit)
    {
        var value = 0;
        do
        {
            value++;
        } while (value < limit);
        return value;
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        Assert.IsTrue(function.Blocks.Any(b => b.Label.StartsWith("do_body", StringComparison.Ordinal)), "Do loop should emit body block.");
        Assert.IsTrue(function.Blocks.Any(b => b.Label.StartsWith("do_cond", StringComparison.Ordinal)), "Do loop should emit condition block.");
        var condBlock = function.Blocks.Single(b => b.Label.StartsWith("do_cond", StringComparison.Ordinal));
        Assert.IsInstanceOfType(condBlock.Terminator, typeof(HirConditionalBranch), "Do condition should branch back to body.");
    }
}
