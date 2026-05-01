using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_OutAlias
{
    [TestMethod]
    public void SequentialOutByteArrayDeclarations_UseDistinctCapturedSlots()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.Numerics;

public class Contract : SmartContract
{
    private static bool TryEncodeBigIntegerScalar(BigInteger value, out byte[] scalar)
    {
        scalar = new byte[] { (byte)value };
        return true;
    }

    public static int Main(BigInteger amountWithdraw, BigInteger fee)
    {
        if (!TryEncodeBigIntegerScalar(amountWithdraw, out byte[] amountWithdrawScalar))
            return -1;
        if (!TryEncodeBigIntegerScalar(fee, out byte[] feeScalar))
            return -2;
        return amountWithdrawScalar[0] * 256 + feeScalar[0];
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var methodBlock = ExtractMethodBlock(context.CreateAssembly(), "Contract.Main(System.Numerics.BigInteger, System.Numerics.BigInteger)");
        var ldsfldMatches = Regex.Matches(methodBlock, @"\bLDSFLD\s+(\d+)\b");
        var distinctCapturedSlots = ldsfldMatches
            .Cast<Match>()
            .Select(m => int.Parse(m.Groups[1].Value))
            .Distinct()
            .ToArray();

        Assert.IsTrue(distinctCapturedSlots.Length >= 2,
            $"Expected at least two distinct captured slots for sequential out byte[] values, but found [{string.Join(", ", distinctCapturedSlots)}].{Environment.NewLine}{methodBlock}");
    }

    private static string ExtractMethodBlock(string assembly, string methodSignature)
    {
        var normalized = assembly.Replace("\r\n", "\n", StringComparison.Ordinal);
        var marker = $"// {methodSignature}";
        var start = normalized.IndexOf(marker, StringComparison.Ordinal);
        Assert.IsTrue(start >= 0, $"Method section '{methodSignature}' was not found in generated assembly.\n{assembly}");

        var next = normalized.IndexOf("\n// ", start + marker.Length, StringComparison.Ordinal);
        if (next < 0) next = normalized.Length;

        return normalized[start..next];
    }
}
