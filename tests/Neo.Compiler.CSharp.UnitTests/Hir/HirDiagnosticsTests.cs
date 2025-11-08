using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.HIR;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirDiagnosticsTests
{
    [TestMethod]
    public void MultiDimensionalArrays_AreRejected()
    {
        const string source = @"
public sealed class C
{
    public static int[,] Create()
    {
        return new int[2,2];
    }
}
";

        Assert.ThrowsException<NotSupportedException>(() => HirLoweringTestBase.LowerMethod(source));
    }

    [TestMethod]
    public void HirVerifier_Flags_Leave_With_Unknown_Scope()
    {
        var signature = new HirSignature(Array.Empty<HirType>(), HirType.VoidType, Array.Empty<HirAttribute>());
        var function = new HirFunction("invalid_leave", signature);

        var tryBlock = function.AddBlock("try_body");
        var finallyBlock = function.AddBlock("finally_body");
        var mergeBlock = function.AddBlock("merge_body");

        tryBlock.SetTerminator(new HirReturn(null));
        finallyBlock.SetTerminator(new HirReturn(null));
        mergeBlock.SetTerminator(new HirReturn(null));

        var orphanScope = new HirTryFinallyScope(tryBlock, finallyBlock, mergeBlock);
        function.Entry.SetTerminator(new HirLeave(orphanScope, mergeBlock));

        var verifier = new HirVerifier();
        var errors = verifier.Verify(function);

        Assert.IsTrue(
            errors.Any(e => e.Contains("unknown try scope", StringComparison.OrdinalIgnoreCase)),
            "Verifier should report HirLeave terminators that reference unknown try scopes.");
    }

    [TestMethod]
    public void AsyncMethods_AreRejected()
    {
        const string source = @"
using System.Threading.Tasks;

public sealed class C
{
    public static async Task Run()
    {
        await Task.CompletedTask;
    }
}
";

        Assert.ThrowsException<NotSupportedException>(() => HirLoweringTestBase.LowerMethod(source));
    }

    [TestMethod]
    public void IteratorMethods_AreRejected()
    {
        const string source = @"
using System.Collections.Generic;

public sealed class C
{
    public static IEnumerable<int> Generate()
    {
        yield return 1;
    }
}
";

        Assert.ThrowsException<NotSupportedException>(() => HirLoweringTestBase.LowerMethod(source));
    }

    [TestMethod]
    public void TryCatch_IsRejected()
    {
        const string source = @"
using System;

public sealed class C
{
    public static void Work()
    {
        try
        {
            var value = 1;
        }
        catch (Exception)
        {
        }
    }
}
";

        var function = HirLoweringTestBase.LowerMethod(source);
        var tryScope = function.Blocks.SelectMany(b => b.Instructions).OfType<HirTryFinallyScope>().SingleOrDefault();
        Assert.IsNotNull(tryScope, "Try statement should materialise a HirTryFinallyScope.");

        var catchScopes = function.Blocks.SelectMany(b => b.Instructions).OfType<HirCatchScope>().ToList();
        Assert.IsTrue(catchScopes.Count > 0, "Try/catch should emit a HirCatchScope for each handler.");

        var leaveTerms = function.Blocks.Select(b => b.Terminator).OfType<HirLeave>().ToList();
        Assert.IsTrue(leaveTerms.Count > 0, "Try/catch should route control flow via HirLeave terminators.");
    }
}
