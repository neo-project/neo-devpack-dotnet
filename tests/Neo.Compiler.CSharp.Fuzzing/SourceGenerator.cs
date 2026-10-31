// Copyright (C) 2015-2026 The Neo Project.
//
// SourceGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;
using System.Text;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Generates syntactically valid C# smart-contract sources.
/// </summary>
/// <remarks>
/// <para>
/// A pure random-byte fuzzer only exercises the parser and produces mostly
/// <c>CS1xxx</c> syntax diagnostics, which the compiler handles by design. To reach
/// the interesting layers (semantic binding, <c>MethodConvert</c>, the optimizer) the
/// generator has to emit well-formed programs.
/// </para>
/// <para>
/// The generator therefore works on a typed expression grammar: every generated
/// expression carries the static type it will have in C#, so the emitted program is
/// almost always legal C# that binds successfully and reaches code generation.
/// </para>
/// </remarks>
public sealed class SourceGenerator
{
    private static readonly string[] NumericTypes = ["int", "long", "short", "byte", "sbyte", "uint", "ulong", "ushort"];

    private static readonly string[] BinaryOperators =
    [
        "+", "-", "*", "/", "%", "&", "|", "^", "<<", ">>",
    ];

    private static readonly string[] ComparisonOperators = ["<", ">", "<=", ">=", "==", "!="];

    private readonly Random _random;

    /// <summary>
    /// Creates a generator with an explicit seed.
    /// </summary>
    /// <param name="seed">Seed for the random number generator.</param>
    public SourceGenerator(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>
    /// Generates a complete smart-contract source file.
    /// </summary>
    /// <returns>The generated C# source.</returns>
    public string GenerateContract()
    {
        int methodCount = _random.Next(1, 4);
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using System.Numerics;");
        sb.AppendLine("using Neo.SmartContract.Framework;");
        sb.AppendLine("using Neo.SmartContract.Framework.Services;");
        sb.AppendLine();
        sb.AppendLine("public class FuzzContract : SmartContract");
        sb.AppendLine("{");

        for (int i = 0; i < methodCount; i++)
        {
            GenerateMethod(sb, i);
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Generates a pair of sources that implement the same methods: one as a Neo smart
    /// contract and one as a plain .NET class.
    /// </summary>
    /// <remarks>
    /// The two sources share the method bodies verbatim. Compiling the contract with the
    /// Neo compiler and the reference with Roslyn gives two independent implementations
    /// of the same specification, which is what the cross-language oracle compares.
    /// </remarks>
    /// <param name="feature">The feature family to target.</param>
    /// <returns>The contract source and the reference source.</returns>
    public (string Contract, string Reference) GeneratePair(FeatureFamily feature)
    {
        var methods = new StringBuilder();

        switch (feature)
        {
            case FeatureFamily.Arithmetic:
                GenerateArithmeticMethods(methods);
                break;
            case FeatureFamily.ControlFlow:
                GenerateControlFlowMethods(methods);
                break;
            case FeatureFamily.Collections:
                GenerateCollectionMethods(methods);
                break;
            case FeatureFamily.Strings:
                GenerateStringMethods(methods);
                break;
            case FeatureFamily.Conversions:
                GenerateConversionMethods(methods);
                break;
            case FeatureFamily.ExceptionHandling:
                GenerateExceptionMethods(methods);
                break;
            case FeatureFamily.Lambdas:
                GenerateLambdaMethods(methods);
                break;
            case FeatureFamily.PatternMatching:
                GeneratePatternMethods(methods);
                break;
            case FeatureFamily.AsyncAwait:
                GenerateAsyncMethods(methods);
                break;
            case FeatureFamily.StaticState:
                GenerateStaticStateMethods(methods);
                break;
            case FeatureFamily.NarrowTypes:
                GenerateNarrowTypeMethods(methods);
                break;
            case FeatureFamily.LongJumps:
                GenerateLongJumpMethods(methods);
                break;
            case FeatureFamily.Generics:
                GenerateGenericsMethods(methods);
                break;
            case FeatureFamily.Interfaces:
                GenerateInterfaceMethods(methods);
                break;
            case FeatureFamily.Enums:
                GenerateEnumMethods(methods);
                break;
            case FeatureFamily.Events:
                GenerateEventMethods(methods);
                break;
            case FeatureFamily.OutRef:
                GenerateOutRefMethods(methods);
                break;
            case FeatureFamily.EvaluationOrder:
                GenerateEvaluationOrderMethods(methods);
                break;
            case FeatureFamily.CallBinding:
                GenerateCallBindingMethods(methods);
                break;
            case FeatureFamily.RefAliases:
                GenerateRefAliasMethods(methods);
                break;
            case FeatureFamily.TupleAssignment:
                GenerateTupleAssignmentMethods(methods);
                break;
            case FeatureFamily.NullPropagation:
                GenerateNullPropagationMethods(methods);
                break;
            case FeatureFamily.Nullable:
                GenerateNullableMethods(methods);
                break;
            case FeatureFamily.RandomExpressions:
                GenerateRandomExpressionMethods(methods);
                break;
            case FeatureFamily.InterContract:
                GenerateInterContractMethods(methods);
                break;
            case FeatureFamily.StorageMap:
                GenerateStorageMapMethods(methods);
                break;

            case FeatureFamily.Structs:
                GenerateStructMethods(methods);
                break;
            case FeatureFamily.MultiDimArrays:
                GenerateMultiDimArrayMethods(methods);
                break;
            case FeatureFamily.OperatorOverloads:
                GenerateOperatorOverloadMethods(methods);
                break;
            case FeatureFamily.Records:
                GenerateRecordMethods(methods);
                break;
            case FeatureFamily.Indexers:
                GenerateIndexerMethods(methods);
                break;
            case FeatureFamily.ControlFlowInteractions:
                GenerateControlFlowInteractionMethods(methods);
                break;
            case FeatureFamily.SwitchForms:
                GenerateSwitchFormMethods(methods);
                break;
            case FeatureFamily.BigMath:
                GenerateBigMathMethods(methods);
                break;
            case FeatureFamily.StringApis:
                GenerateStringApiMethods(methods);
                break;
            case FeatureFamily.DelegateCapture:
                GenerateDelegateCaptureMethods(methods);
                break;
            case FeatureFamily.EnumSwitch:
                GenerateEnumSwitchMethods(methods);
                break;
            case FeatureFamily.DeepLoops:
                GenerateDeepLoopMethods(methods);
                break;
            case FeatureFamily.RecursiveCalls:
                GenerateRecursiveMethods(methods);
                break;
            case FeatureFamily.CrossFamily:
                GenerateCrossFamilyMethods(methods);
                break;
            case FeatureFamily.Storage:
                GenerateStorageMethods(methods);
                break;
        }

        // A family that makes a contract call needs the manifest to grant permission;
        // without it the VM refuses the call and both builds fail identically.
        string classAttributes = feature is FeatureFamily.InterContract
            ? "[ContractPermission(\"*\", \"*\")]\n"
            : string.Empty;

        string contract =
            "using System;\nusing System.Linq;\nusing System.Numerics;\nusing Neo.SmartContract.Framework;\n" +
            "using Neo.SmartContract.Framework.Services;\n" +
            "using Neo.SmartContract.Framework.Attributes;\n\n" +
            classAttributes +
            "public class FuzzContract : SmartContract\n{\n" + methods + "}\n";

        // The plain C# reference is only produced for families that have a CLR
        // equivalent. Stateful families use storage or the contract-call syscall, neither
        // of which exists outside the VM, so they can only be checked by the
        // optimizer-equivalence and storage oracles.
        string reference = HasClrEquivalent(feature)
            ? "using System;\nusing System.Linq;\nusing System.Numerics;\n\n" +
              "public static class FuzzReference\n{\n" + methods + "}\n"
            : string.Empty;

        return (contract, reference);
    }

    /// <summary>
    /// Determines whether a feature family has a plain C# equivalent.
    /// </summary>
    /// <param name="feature">The family to check.</param>
    /// <returns><c>true</c> when a plain C# reference can be compiled for the family.</returns>
    /// <remarks>
    /// A family has no CLR equivalent when it uses storage, a prefixed storage map, or
    /// <c>System.Contract.Call</c>. Producing a reference for those would fail to compile,
    /// which the cross-language oracle treats as "skip" — silently removing the family
    /// from coverage, so the decision has to be explicit and complete.
    /// </remarks>
    private static bool HasClrEquivalent(FeatureFamily feature) => feature switch
    {
        FeatureFamily.Storage => false,
        FeatureFamily.StorageMap => false,
        FeatureFamily.InterContract => false,
        _ => true,
    };

    /// <summary>
    /// Generates a source file that exercises a specific language feature family.
    /// </summary>
    /// <param name="feature">The feature family to target.</param>
    /// <returns>The generated C# source.</returns>
    public string GenerateForFeature(FeatureFamily feature)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using System.Numerics;");
        sb.AppendLine("using Neo.SmartContract.Framework;");
        sb.AppendLine("using Neo.SmartContract.Framework.Services;");
        sb.AppendLine("using Neo.SmartContract.Framework.Attributes;");
        sb.AppendLine();
        if (feature is FeatureFamily.InterContract)
        {
            // A contract call needs the manifest to grant permission.
            sb.AppendLine("[ContractPermission(\"*\", \"*\")]");
        }
        sb.AppendLine("public class FuzzContract : SmartContract");
        sb.AppendLine("{");

        switch (feature)
        {
            case FeatureFamily.Arithmetic:
                GenerateArithmeticMethods(sb);
                break;
            case FeatureFamily.ControlFlow:
                GenerateControlFlowMethods(sb);
                break;
            case FeatureFamily.Collections:
                GenerateCollectionMethods(sb);
                break;
            case FeatureFamily.Strings:
                GenerateStringMethods(sb);
                break;
            case FeatureFamily.Conversions:
                GenerateConversionMethods(sb);
                break;
            case FeatureFamily.ExceptionHandling:
                GenerateExceptionMethods(sb);
                break;
            case FeatureFamily.Lambdas:
                GenerateLambdaMethods(sb);
                break;
            case FeatureFamily.PatternMatching:
                GeneratePatternMethods(sb);
                break;
            case FeatureFamily.AsyncAwait:
                GenerateAsyncMethods(sb);
                break;
            case FeatureFamily.StaticState:
                GenerateStaticStateMethods(sb);
                break;
            case FeatureFamily.NarrowTypes:
                GenerateNarrowTypeMethods(sb);
                break;
            case FeatureFamily.LongJumps:
                GenerateLongJumpMethods(sb);
                break;
            case FeatureFamily.Generics:
                GenerateGenericsMethods(sb);
                break;
            case FeatureFamily.Interfaces:
                GenerateInterfaceMethods(sb);
                break;
            case FeatureFamily.Enums:
                GenerateEnumMethods(sb);
                break;
            case FeatureFamily.Events:
                GenerateEventMethods(sb);
                break;
            case FeatureFamily.OutRef:
                GenerateOutRefMethods(sb);
                break;
            case FeatureFamily.EvaluationOrder:
                GenerateEvaluationOrderMethods(sb);
                break;
            case FeatureFamily.CallBinding:
                GenerateCallBindingMethods(sb);
                break;
            case FeatureFamily.RefAliases:
                GenerateRefAliasMethods(sb);
                break;
            case FeatureFamily.TupleAssignment:
                GenerateTupleAssignmentMethods(sb);
                break;
            case FeatureFamily.NullPropagation:
                GenerateNullPropagationMethods(sb);
                break;
            case FeatureFamily.Nullable:
                GenerateNullableMethods(sb);
                break;
            case FeatureFamily.RandomExpressions:
                GenerateRandomExpressionMethods(sb);
                break;
            case FeatureFamily.InterContract:
                GenerateInterContractMethods(sb);
                break;
            case FeatureFamily.StorageMap:
                GenerateStorageMapMethods(sb);
                break;

            case FeatureFamily.Structs:
                GenerateStructMethods(sb);
                break;
            case FeatureFamily.MultiDimArrays:
                GenerateMultiDimArrayMethods(sb);
                break;
            case FeatureFamily.OperatorOverloads:
                GenerateOperatorOverloadMethods(sb);
                break;
            case FeatureFamily.Records:
                GenerateRecordMethods(sb);
                break;
            case FeatureFamily.Indexers:
                GenerateIndexerMethods(sb);
                break;
            case FeatureFamily.ControlFlowInteractions:
                GenerateControlFlowInteractionMethods(sb);
                break;
            case FeatureFamily.SwitchForms:
                GenerateSwitchFormMethods(sb);
                break;
            case FeatureFamily.BigMath:
                GenerateBigMathMethods(sb);
                break;
            case FeatureFamily.StringApis:
                GenerateStringApiMethods(sb);
                break;
            case FeatureFamily.DelegateCapture:
                GenerateDelegateCaptureMethods(sb);
                break;
            case FeatureFamily.EnumSwitch:
                GenerateEnumSwitchMethods(sb);
                break;
            case FeatureFamily.DeepLoops:
                GenerateDeepLoopMethods(sb);
                break;
            case FeatureFamily.RecursiveCalls:
                GenerateRecursiveMethods(sb);
                break;
            case FeatureFamily.CrossFamily:
                GenerateCrossFamilyMethods(sb);
                break;
            case FeatureFamily.Storage:
                GenerateStorageMethods(sb);
                break;
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private void GenerateMethod(StringBuilder sb, int index)
    {
        string type = NumericTypes[_random.Next(NumericTypes.Length)];
        string expr = GenerateExpression(type, depth: 3);
        sb.AppendLine($"    public static {type} Method{index}({type} a, {type} b)");
        sb.AppendLine("    {");
        // The expression is evaluated in the promoted type (int for the narrow types),
        // so it is converted back explicitly.
        sb.AppendLine($"        return ({type})({expr});");
        sb.AppendLine("    }");
    }

    private void GenerateArithmeticMethods(StringBuilder sb)
    {
        for (int i = 0; i < 3; i++)
        {
            string type = NumericTypes[_random.Next(NumericTypes.Length)];
            string op = BinaryOperators[_random.Next(BinaryOperators.Length)];
            sb.AppendLine($"    public static {type} Arith{i}({type} a, {type} b)");
            sb.AppendLine("    {");
            // The arithmetic is performed in int (the C# promotion rule) and then
            // converted back, so an explicit cast is required for the narrow types.
            // The shift count has to be an int: uint and ulong reject a shift by their own type.
            string right = op is "<<" or ">>" ? "(int)b" : "b";
            sb.AppendLine($"        return ({type})(a {op} {right});");
            sb.AppendLine("    }");
        }
        // checked / unchecked contexts are a rich source of divergence.
        sb.AppendLine("    public static int Checked(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return checked(a + b);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Unchecked(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return unchecked(a + b);");
        sb.AppendLine("    }");
        // BigInteger arithmetic goes through the dedicated system-call handlers.
        sb.AppendLine("    public static BigInteger BigMath(BigInteger a, BigInteger b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return BigInteger.Add(BigInteger.Multiply(a, b), BigInteger.Remainder(a, b));");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int MathFns(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return Math.Abs(a) + Math.Max(b, 1) + Math.Min(a, b) + Math.Sign(a);");
        sb.AppendLine("    }");
    }

    private void GenerateControlFlowMethods(StringBuilder sb)
    {
        for (int i = 0; i < 2; i++)
        {
            sb.AppendLine($"    public static int Flow{i}(int a, int b)");
            sb.AppendLine("    {");
            sb.AppendLine("        int total = 0;");
            // The loop bound is derived from the parameter but capped, so that an
            // argument such as int.MaxValue cannot turn the contract into a
            // practically infinite loop on the VM.
            sb.AppendLine("        int count = a % 8; if (count < 0) count = -count;");
            sb.AppendLine("        for (int i = 0; i < count; i++) { total += i * b; }");
            sb.AppendLine("        while (total > 1000) { total /= 2; }");
            sb.AppendLine("        switch (a % 5)");
            sb.AppendLine("        {");
            sb.AppendLine("            case 0: total += 1; break;");
            sb.AppendLine("            case 1: total -= 2; break;");
            sb.AppendLine("            case 2: total ^= 3; break;");
            sb.AppendLine("            default: total = -total; break;");
            sb.AppendLine("        }");
            sb.AppendLine("        return total;");
            sb.AppendLine("    }");
        }
    }

    private void GenerateCollectionMethods(StringBuilder sb)
    {
        // Arrays are supported by the framework; List<T>.Add is not (NC1002), so the
        // generator sticks to arrays and to the array helpers the framework does support.
        for (int i = 0; i < 2; i++)
        {
            sb.AppendLine($"    public static int Coll{i}(int n)");
            sb.AppendLine("    {");
            sb.AppendLine("        var a = new int[Math.Abs(n) % 8];");
            sb.AppendLine("        for (int i = 0; i < a.Length; i++) a[i] = i * i;");
            sb.AppendLine("        int sum = 0;");
            sb.AppendLine("        for (int i = 0; i < a.Length; i++) sum += a[i];");
            sb.AppendLine("        return sum + a.Length;");
            sb.AppendLine("    }");
        }
        sb.AppendLine("    public static int Arr(int n)");
        sb.AppendLine("    {");
        sb.AppendLine("        var a = new int[Math.Abs(n) % 8];");
        sb.AppendLine("        for (int i = 0; i < a.Length; i++) a[i] = i;");
        sb.AppendLine("        int s = 0;");
        sb.AppendLine("        for (int i = 0; i < a.Length; i++) s += a[i];");
        sb.AppendLine("        return s;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ArrCopy(int n)");
        sb.AppendLine("    {");
        sb.AppendLine("        var src = new int[4] { 1, 2, 3, 4 };");
        sb.AppendLine("        var dst = new int[4];");
        sb.AppendLine("        for (int i = 0; i < src.Length; i++) dst[i] = src[i];");
        sb.AppendLine("        return dst[0] + dst[3] + n;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ArrReverse(int n)");
        sb.AppendLine("    {");
        sb.AppendLine("        var a = new int[4] { 1, 2, 3, 4 };");
        sb.AppendLine("        for (int i = 0; i < a.Length / 2; i++) { int t = a[i]; a[i] = a[a.Length - 1 - i]; a[a.Length - 1 - i] = t; }");
        sb.AppendLine("        return a[0] + a[3] + n;");
        sb.AppendLine("    }");
    }

    private void GenerateStringMethods(StringBuilder sb)
    {
        sb.AppendLine("    public static int Str(string s, int n)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (s == null) return n;");
        sb.AppendLine("        int total = s.Length;");
        sb.AppendLine("        for (int i = 0; i < s.Length; i++) total += s[i];");
        sb.AppendLine("        return total + n;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static string Concat(string a, string b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a + b;");
        sb.AppendLine("    }");
        // Interpolated strings. The compiler concatenates the segments with CAT and
        // stringifies each hole, so the shape of the segments and the hole types both
        // matter. Alignment and format clauses are rejected (NCxxxx), so only bare
        // holes are generated.
        sb.AppendLine("    public static string InterpSimple(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return $\"a={a},b={b}\";");
        sb.AppendLine("    }");
        sb.AppendLine("    public static string InterpNested(int a, string s)");
        sb.AppendLine("    {");
        sb.AppendLine("        return $\"{s}:({a + 1})\";");
        sb.AppendLine("    }");
        sb.AppendLine("    public static string InterpChain(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        string x = $\"v={a}\";");
        sb.AppendLine("        string y = $\"{x}!\";");
        sb.AppendLine("        return y;");
        sb.AppendLine("    }");
    }

    private void GenerateConversionMethods(StringBuilder sb)
    {
        sb.AppendLine("    public static long Conv(long a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (long)a + b;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Narrow(long a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (int)(a + b);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static BigInteger Big(long a, BigInteger b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return b + a;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static byte ToByte(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (byte)(a + b);");
        sb.AppendLine("    }");
    }

    private void GenerateExceptionMethods(StringBuilder sb)
    {
        // The compiler supports exactly one catch clause per try (NC2006).
        sb.AppendLine("    public static int Try(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        try { return a / b; }");
        sb.AppendLine("        catch (System.Exception) { return -1; }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Fin(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int x = 0;");
        sb.AppendLine("        try { x = a; return x; }");
        sb.AppendLine("        finally { x = 1; }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Nested(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine("            try { return a / b; }");
        sb.AppendLine("            finally { a = a + 1; }");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (System.Exception) { return -2; }");
        sb.AppendLine("    }");
    }

    private void GenerateLambdaMethods(StringBuilder sb)
    {
        sb.AppendLine("    public static int Lam(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Func<int,int,int> f = (x, y) => x * y + a;");
        sb.AppendLine("        return f(a, b);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int LamBlock(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Func<int,int> g = x => { int t = x * 2; return t + 1; };");
        sb.AppendLine("        return g(a);");
        sb.AppendLine("    }");
        // LINQ query operators are not part of the contract ABI (NC1002), but the
        // delegate shapes they build on are, so the generator uses explicit delegates.
        sb.AppendLine("    public static int Linq(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var xs = new int[] { 1, 2, 3, 4 };");
        sb.AppendLine("        Func<int, bool> pred = x => x > 1;");
        sb.AppendLine("        Func<int, int> proj = x => x * a;");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < xs.Length; i++) { if (pred(xs[i])) total += proj(xs[i]); }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
    }

    private void GeneratePatternMethods(StringBuilder sb)
    {
        sb.AppendLine("    public static int Pat(object o, int n)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (o is int i) return i + n;");
        sb.AppendLine("        if (o is string s) return s.Length + n;");
        sb.AppendLine("        return n;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Switch(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a switch { 0 => b, 1 => a + b, _ => a - b };");
        sb.AppendLine("    }");
        // Relational and logical patterns lower through the recursive-pattern machinery,
        // which builds the comparison chain from the pattern combinators.
        sb.AppendLine("    public static int Relational(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a switch");
        sb.AppendLine("        {");
        sb.AppendLine("            < 0 => -1,");
        sb.AppendLine("            >= 100 => 2,");
        sb.AppendLine("            >= 10 => 1,");
        sb.AppendLine("            _ => 0");
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Logical(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (a is > 5 and < 50) return 1;");
        sb.AppendLine("        if (a is 7 or 9) return 2;");
        sb.AppendLine("        if (b is not (0 or 1)) return 3;");
        sb.AppendLine("        return 0;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Guard(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        switch (a)");
        sb.AppendLine("        {");
        sb.AppendLine("            case int i when i > b:");
        sb.AppendLine("                return 1;");
        sb.AppendLine("            case int i when i < 0:");
        sb.AppendLine("                return 2;");
        sb.AppendLine("            default:");
        sb.AppendLine("                return 3;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Property(string s)");
        sb.AppendLine("    {");
        // Recursive property patterns only support constant sub-patterns (NC2001), so
        // the relational variant is intentionally left out.
        sb.AppendLine("        if (s is { Length: 0 }) return 2;");
        sb.AppendLine("        if (s is { Length: 4 }) return 4;");
        sb.AppendLine("        return 0;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Declaration(object o)");
        sb.AppendLine("    {");
        // Array types are not supported pattern input types (NC2001).
        sb.AppendLine("        if (o is string { Length: 2 } text) return text.Length + 10;");
        sb.AppendLine("        if (o is string rest) return rest.Length * 10;");
        sb.AppendLine("        return -1;");
        sb.AppendLine("    }");
        // Collection expressions target-typed to arrays; the spread element is rejected
        // by design, so only literal elements are generated.
        sb.AppendLine("    public static int CollectionLiteral(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int[] values = [a, a + 1, 5];");
        sb.AppendLine("        return values[0] + values[1] * 10 + values[2];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CollectionLiteralEmpty(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int[] empty = [];");
        sb.AppendLine("        int[] one = [a];");
        sb.AppendLine("        return empty.Length * 100 + one.Length + one[0];");
        sb.AppendLine("    }");
    }

    private void GenerateAsyncMethods(StringBuilder sb)
    {
        // The compiler rejects the await expression (NC2001) and Task helpers are not
        // callable from a contract (NC1002), so this family exercises the lowering
        // machinery that async would otherwise sit on: delegates, generic helpers,
        // and static state.
        sb.AppendLine("    private static int counter = 0;");
        sb.AppendLine("    public static int Delegate(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Func<int, int, int> op = (x, y) => x + y;");
        sb.AppendLine("        return op(a, 1);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int GenericHelper(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        return Identity(a) + Identity(a + 1);");
        sb.AppendLine("    }");
        sb.AppendLine("    private static T Identity<T>(T value) { return value; }");
        sb.AppendLine("    public static int StaticState(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        counter = counter + a;");
        sb.AppendLine("        return counter;");
        sb.AppendLine("    }");
    }

    private void GenerateStaticStateMethods(StringBuilder sb)
    {
        // Static fields and explicit static constructors drive the two mechanisms that
        // remove _initialize from the manifest, so this family exercises them directly.
        // The field initializers are what matter; the static constructor below is
        // written without naming the enclosing type, so the same body can be emitted
        // into both the contract and the plain C# reference.
        sb.AppendLine("    private static int counter = 7;");
        sb.AppendLine("    private static int other = 9;");
        sb.AppendLine("    private static readonly string label = \"fuzz\";");
        sb.AppendLine();
        sb.AppendLine("    public static int StaticFields(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a + counter + other;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StaticFieldsNoCtor(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a + counter;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static string StaticLabel(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        return label + a;");
        sb.AppendLine("    }");
    }

    private void GenerateGenericsMethods(StringBuilder sb)
    {
        // Generic method and generic class instantiation. The repository has no generic
        // contract at all, so the compiler's specialisation of type parameters is
        // completely untested.
        sb.AppendLine("    public static int GenericMethod(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return Identity(a) + Identity(b);");
        sb.AppendLine("    }");
        sb.AppendLine("    private static T Identity<T>(T value) { return value; }");
        sb.AppendLine("    private static T Pick<T>(T first, T second, bool takeFirst) { return takeFirst ? first : second; }");
        sb.AppendLine("    public static int GenericClass(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var holder = new Holder<int>();");
        sb.AppendLine("        holder.Value = a;");
        sb.AppendLine("        return holder.Value + Pick(a, b, a > b);");
        sb.AppendLine("    }");
        sb.AppendLine("    private class Holder<T>");
        sb.AppendLine("    {");
        sb.AppendLine("        public T Value { get; set; }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int GenericConstraint(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return Sum(a) + Sum(b);");
        sb.AppendLine("    }");
        sb.AppendLine("    private static int Sum<T>(T value) where T : struct");
        sb.AppendLine("    {");
        sb.AppendLine("        return (int)(object)value;");
        sb.AppendLine("    }");
    }

    private void GenerateInterfaceMethods(StringBuilder sb)
    {
        // Virtual dispatch through inheritance. Interfaces themselves are rejected by
        // the compiler (NC2009, "Interfaces are not supported"), so the vtable path is
        // reached through a base class and an override.
        sb.AppendLine("    public class BaseShape");
        sb.AppendLine("    {");
        sb.AppendLine("        public virtual int Area(int a) { return a * a; }");
        sb.AppendLine("        public virtual int BumpTwice(ref int first, ref int second)");
        sb.AppendLine("        {");
        sb.AppendLine("            first += 2;");
        sb.AppendLine("            second += 3;");
        sb.AppendLine("            return first * 10 + second;");
        sb.AppendLine("        }");
        sb.AppendLine("        public virtual int SetValues(out int first, out int second)");
        sb.AppendLine("        {");
        sb.AppendLine("            first = 2;");
        sb.AppendLine("            second = 3;");
        sb.AppendLine("            return first * 10 + second;");
        sb.AppendLine("        }");
        sb.AppendLine("        public virtual int Mix(ref int first, ref int second, ref int third)");
        sb.AppendLine("        {");
        sb.AppendLine("            first += 2;");
        sb.AppendLine("            second += 3;");
        sb.AppendLine("            third += 5;");
        sb.AppendLine("            return first * 100 + second * 10 + third;");
        sb.AppendLine("        }");
        sb.AppendLine("        public virtual int BumpThroughBase(ref int first, ref int second)");
        sb.AppendLine("        {");
        sb.AppendLine("            first += 2;");
        sb.AppendLine("            second += 3;");
        sb.AppendLine("            return first * 10 + second;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("    public class DerivedShape : BaseShape");
        sb.AppendLine("    {");
        sb.AppendLine("        public override int Area(int a) { return a * a * a; }");
        sb.AppendLine("        public override int BumpTwice(ref int first, ref int second)");
        sb.AppendLine("        {");
        sb.AppendLine("            first += 5;");
        sb.AppendLine("            second += 7;");
        sb.AppendLine("            return first * 10 + second;");
        sb.AppendLine("        }");
        sb.AppendLine("        public override int SetValues(out int first, out int second)");
        sb.AppendLine("        {");
        sb.AppendLine("            first = 5;");
        sb.AppendLine("            second = 7;");
        sb.AppendLine("            return first * 10 + second;");
        sb.AppendLine("        }");
        sb.AppendLine("        public override int Mix(ref int first, ref int second, ref int third)");
        sb.AppendLine("        {");
        sb.AppendLine("            first += 5;");
        sb.AppendLine("            second += 7;");
        sb.AppendLine("            third += 11;");
        sb.AppendLine("            return first * 100 + second * 10 + third;");
        sb.AppendLine("        }");
        sb.AppendLine("        public override int BumpThroughBase(ref int first, ref int second)");
        sb.AppendLine("        {");
        sb.AppendLine("            int result = base.BumpThroughBase(ref first, ref second);");
        sb.AppendLine("            first += 5;");
        sb.AppendLine("            second += 7;");
        sb.AppendLine("            return result * 100 + first * 10 + second;");
        sb.AppendLine("        }");
        sb.AppendLine("        public virtual int Extra() { return 9; }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Dispatch(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = a > b ? new DerivedShape() : new BaseShape();");
        sb.AppendLine("        return shape.Area(a);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchStatic(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return new BaseShape().Area(a) + new DerivedShape().Area(b);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchVirtual(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = new DerivedShape();");
        sb.AppendLine("        return shape.Area(a) + shape.Area(b);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchRefAlias(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = new DerivedShape();");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int result = shape.BumpTwice(ref state, ref state);");
        sb.AppendLine("        return result * 100 + state;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchDistinctRefs(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = new DerivedShape();");
        sb.AppendLine("        int first = a;");
        sb.AppendLine("        int second = b;");
        sb.AppendLine("        int result = shape.BumpTwice(ref first, ref second);");
        sb.AppendLine("        return result * 1000 + first * 100 + second;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchMixedRefs(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = new DerivedShape();");
        sb.AppendLine("        int alias = a;");
        sb.AppendLine("        int first = a;");
        sb.AppendLine("        int second = b;");
        sb.AppendLine("        int aliasResult = shape.BumpTwice(ref alias, ref alias);");
        sb.AppendLine("        int distinctResult = shape.BumpTwice(ref first, ref second);");
        sb.AppendLine("        return aliasResult * 1000000 + alias * 10000 + distinctResult * 1000 + first * 100 + second;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchRefAliasSignatures(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = new DerivedShape();");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int other = b;");
        sb.AppendLine("        int firstResult = shape.Mix(ref state, ref state, ref other);");
        sb.AppendLine("        int secondResult = shape.Mix(ref state, ref other, ref other);");
        sb.AppendLine("        return firstResult * 1000000 + secondResult * 1000 + state * 100 + other;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchRefAliasThroughBase(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = new DerivedShape();");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int result = shape.BumpThroughBase(ref state, ref state);");
        sb.AppendLine("        return result * 100 + state;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DispatchOut(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        BaseShape shape = new DerivedShape();");
        sb.AppendLine("        int result = shape.SetValues(out int first, out int second);");
        sb.AppendLine("        return result * 100 + first * 10 + second;");
        sb.AppendLine("    }");
    }

    private void GenerateEnumMethods(StringBuilder sb)
    {
        // Enums, including the underlying type conversion. Enum operations are lowered
        // by dedicated system calls, so they take a different path from plain integers.
        sb.AppendLine("    public enum FuzzEnum : int");
        sb.AppendLine("    {");
        sb.AppendLine("        Zero = 0,");
        sb.AppendLine("        One = 1,");
        sb.AppendLine("        Large = 1000000,");
        sb.AppendLine("        Negative = -5");
        sb.AppendLine("    }");
        sb.AppendLine("    public enum FuzzByteEnum : byte");
        sb.AppendLine("    {");
        sb.AppendLine("        Low = 0,");
        sb.AppendLine("        High = 255");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int EnumToInt(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        FuzzEnum e = (FuzzEnum)a;");
        sb.AppendLine("        return (int)e;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int EnumCompare(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        FuzzEnum e = (FuzzEnum)a;");
        sb.AppendLine("        FuzzEnum f = (FuzzEnum)b;");
        sb.AppendLine("        return e == f ? 1 : (e < f ? -1 : 2);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int EnumUnderlying(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        FuzzByteEnum e = (FuzzByteEnum)(a & 0xFF);");
        sb.AppendLine("        return (int)e;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int EnumArithmetic(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        FuzzEnum e = (FuzzEnum)a;");
        sb.AppendLine("        return (int)e + b;");
        sb.AppendLine("    }");
    }

    private void GenerateEventMethods(StringBuilder sb)
    {
        // Event declaration and invocation. The compiler rejects the event symbol
        // itself (NC2001), so the notification is driven through a delegate that the
        // event is initialised with.
        sb.AppendLine("    public static event System.Action<int, int> Changed = (x, y) => { };");
        sb.AppendLine("    public static int Raise(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Changed(a, b);");
        sb.AppendLine("        return a + b;");
        sb.AppendLine("    }");
        // The payload is derived from the return value, so the fuzzer can check that the
        // notification carries the same values without knowing what the method computes.
        sb.AppendLine("    public static int EchoEvent(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int result = a + b;");
        sb.AppendLine("        Changed(result, result);");
        sb.AppendLine("        return result;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RaiseTwice(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Changed(a, b);");
        sb.AppendLine("        Changed(b, a);");
        sb.AppendLine("        return a - b;");
        sb.AppendLine("    }");
    }

    private void GenerateOutRefMethods(StringBuilder sb)
    {
        // out and ref parameters are lowered through captured slots and ref bindings, which is
        // one of the more intricate parts of the converter. The bodies only use the parameter
        // values, so the same code compiles as plain C# and the cross-language oracle applies.
        sb.AppendLine("    private static void Split(int value, out int first, out int second)");
        sb.AppendLine("    {");
        sb.AppendLine("        first = value + 1;");
        sb.AppendLine("        second = value * 2;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static void Bump(ref int value, int delta)");
        sb.AppendLine("    {");
        sb.AppendLine("        value = value + delta;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static void BumpTwice(ref int first, ref int second)");
        sb.AppendLine("    {");
        sb.AppendLine("        first++;");
        sb.AppendLine("        second++;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static void SetTwice(out int first, out int second)");
        sb.AppendLine("    {");
        sb.AppendLine("        first = 1;");
        sb.AppendLine("        second = first + 1;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static bool TryDouble(int value, out int doubled)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (value > 1000)");
        sb.AppendLine("        {");
        sb.AppendLine("            doubled = 0;");
        sb.AppendLine("            return false;");
        sb.AppendLine("        }");
        sb.AppendLine("        doubled = value * 2;");
        sb.AppendLine("        return true;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int OutTwo(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Split(a, out int first, out int second);");
        sb.AppendLine("        return first * 1000 + second;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int OutOverwrite(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int value = b;");
        sb.AppendLine("        Split(a, out value, out value);");
        sb.AppendLine("        return value;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RefAccumulate(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = a;");
        sb.AppendLine("        Bump(ref total, 3);");
        sb.AppendLine("        Bump(ref total, -1);");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int TryPattern(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (TryDouble(a, out int doubled)) return doubled;");
        sb.AppendLine("        return -1;");
        sb.AppendLine("    }");
        // ref on an array element is not accepted by the compiler (NC2001), so the family only
        // covers ref on locals. The rejection probe fails a family that does not compile, which
        // is what caught this.
        sb.AppendLine("    public static int OutInLoop(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < 3; i++)");
        sb.AppendLine("        {");
        sb.AppendLine("            Split(a + i, out int first, out int second);");
        sb.AppendLine("            total = total + first + second;");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int OutAliasSequence(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int alias = a;");
        sb.AppendLine("        int first = a;");
        sb.AppendLine("        int second = 10;");
        sb.AppendLine("        SetTwice(out alias, out alias);");
        sb.AppendLine("        SetTwice(out first, out second);");
        sb.AppendLine("        return alias * 10000 + first * 100 + second;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RefAliasSequence(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int alias = 0;");
        sb.AppendLine("        int first = a;");
        sb.AppendLine("        int second = 10;");
        sb.AppendLine("        BumpTwice(ref alias, ref alias);");
        sb.AppendLine("        BumpTwice(ref first, ref second);");
        sb.AppendLine("        return alias * 10000 + first * 100 + second;");
        sb.AppendLine("    }");
    }

    private void GenerateEvaluationOrderMethods(StringBuilder sb)
    {
        sb.AppendLine("    public struct MutablePair { public int X; public int Y; }");
        // These shapes make evaluation order observable. A converter that reuses a stale
        // slot, evaluates an assignment target twice, or reverses arguments can otherwise
        // return the same result as the CLR on side-effect-free expressions.
        sb.AppendLine("    public static int PrePost(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int x = a;");
        sb.AppendLine("        int y = x++ + ++x * 10;");
        sb.AppendLine("        return x * 1000 + y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Compound(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int x = a;");
        sb.AppendLine("        x += b;");
        sb.AppendLine("        x *= 3;");
        sb.AppendLine("        x -= ++b;");
        sb.AppendLine("        x ^= b << 1;");
        sb.AppendLine("        return x * 100 + b;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ArraySideEffects(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var values = new int[] { a, a + 1, a + 2 };");
        sb.AppendLine("        int i = 0;");
        sb.AppendLine("        values[i++] += ++i;");
        sb.AppendLine("        return values[0] * 10000 + values[1] * 100 + values[2] + i;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static int Next(ref int state, int delta)");
        sb.AppendLine("    {");
        sb.AppendLine("        state += delta;");
        sb.AppendLine("        return state;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static int Combine(int x, int y, int z) { return x + y * 10 + z * 100; }");
        sb.AppendLine("    public static int ArgumentOrder(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int result = Combine(Next(ref state, 1), Next(ref state, 10), Next(ref state, 100));");
        sb.AppendLine("        return result * 1000 + state;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ShortCircuit(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int x = a;");
        sb.AppendLine("        bool first = a > 0 && (x += b) > 0;");
        sb.AppendLine("        bool second = a < 0 || (x += b * 2) > 0;");
        sb.AppendLine("        return x * 10 + (first ? 2 : 0) + (second ? 1 : 0);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NestedAssignment(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int x = 0;");
        sb.AppendLine("        int y = 0;");
        sb.AppendLine("        x = y = a;");
        sb.AppendLine("        x += y += b;");
        sb.AppendLine("        return x * 100 + y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StructIncrement(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var pair = new MutablePair { X = a, Y = a + 1 };");
        sb.AppendLine("        int before = pair.X++;");
        sb.AppendLine("        int after = ++pair.Y;");
        sb.AppendLine("        pair.X += after;");
        sb.AppendLine("        return pair.X * 10000 + pair.Y * 100 + before;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StructClosure(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var pair = new MutablePair { X = a, Y = a + 1 };");
        sb.AppendLine("        Func<int> read = () => pair.X + pair.Y;");
        sb.AppendLine("        Action<int> write = value => pair.X = value;");
        sb.AppendLine("        int before = read();");
        sb.AppendLine("        write(99);");
        sb.AppendLine("        return before * 10000 + read() * 100 + pair.X;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ConditionalStructCopy(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var first = new MutablePair { X = a, Y = 1 };");
        sb.AppendLine("        var second = new MutablePair { X = a + 1, Y = 2 };");
        sb.AppendLine("        var copy = a > 0 ? first : second;");
        sb.AppendLine("        copy.X = 99;");
        sb.AppendLine("        return first.X * 10000 + second.X * 100 + copy.X;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VoidCompound(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int captured = a;");
        sb.AppendLine("        Action<int> update = value => captured += value * 2;");
        sb.AppendLine("        update(b);");
        sb.AppendLine("        return captured;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VoidIncrement(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int captured = a;");
        sb.AppendLine("        Action increment = () => captured++;");
        sb.AppendLine("        Action decrement = () => --captured;");
        sb.AppendLine("        increment();");
        sb.AppendLine("        increment();");
        sb.AppendLine("        decrement();");
        sb.AppendLine("        return captured;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VoidNestedAssignment(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int captured = a;");
        sb.AppendLine("        int other = b;");
        sb.AppendLine("        Action<int> update = value => captured = other = value;");
        sb.AppendLine("        update(a + b);");
        sb.AppendLine("        return captured * 100 + other;");
        sb.AppendLine("    }");
    }

    private void GenerateCallBindingMethods(StringBuilder sb)
    {
        // Evaluate arguments in source order, then marshal them in parameter order. The
        // state and ref result expose both reordering and stale temporary slots.
        sb.AppendLine("    private static int NextBinding(ref int state)");
        sb.AppendLine("    {");
        sb.AppendLine("        return ++state;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static int Bind(int first, ref int middle, int last = 7, params int[] rest)");
        sb.AppendLine("    {");
        sb.AppendLine("        middle += 100;");
        sb.AppendLine("        return first * 100000 + middle * 1000 + last * 100 + rest[0] * 10 + rest[1];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NamedOptionalParams(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int value = 5;");
        sb.AppendLine("        int result = Bind(rest: new[] { NextBinding(ref state), NextBinding(ref state) }, middle: ref value, first: NextBinding(ref state));");
        sb.AppendLine("        return result * 1000 + value * 10 + state;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ParamsExpanded(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int value = 3;");
        sb.AppendLine("        int result = Bind(NextBinding(ref state), ref value, 11, NextBinding(ref state), NextBinding(ref state));");
        sb.AppendLine("        return result * 1000 + value * 10 + state;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NestedBinding(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int value = 1;");
        sb.AppendLine("        int first = Bind(NextBinding(ref state), ref value, rest: new[] { NextBinding(ref state), NextBinding(ref state) });");
        sb.AppendLine("        int second = Bind(first: NextBinding(ref state), middle: ref value, last: 2, rest: new[] { first, NextBinding(ref state) });");
        sb.AppendLine("        return second * 1000 + first + value;");
        sb.AppendLine("    }");
    }

    private void GenerateRefAliasMethods(StringBuilder sb)
    {
        // Ref bindings must retain the selected array element across alias copies,
        // rebind only the assigned local, and release temporaries at control-flow joins.
        sb.AppendLine("    public static int RefAliasElements(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int[] values = { a, a + 1, a + 2 };");
        sb.AppendLine("        int index = 1;");
        sb.AppendLine("        int selected = index++;");
        sb.AppendLine("        ref int alias = ref values[selected];");
        sb.AppendLine("        ref int snapshot = ref values[selected];");
        sb.AppendLine("        alias += 10;");
        sb.AppendLine("        index = 0;");
        sb.AppendLine("        alias = ref values[index];");
        sb.AppendLine("        alias += 20;");
        sb.AppendLine("        snapshot++;");
        sb.AppendLine("        return values[0] * 10000 + values[1] * 100 + values[2] + index;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RefAliasChain(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int[] values = { a, a + 1, a + 2 };");
        sb.AppendLine("        ref int first = ref values[0];");
        sb.AppendLine("        ref int second = ref values[0];");
        sb.AppendLine("        first = ref values[2];");
        sb.AppendLine("        first += 3;");
        sb.AppendLine("        second += 5;");
        sb.AppendLine("        return values[0] * 100 + values[1] * 10 + values[2];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RefLoopControl(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int[] values = { a, a + 1, a + 2, a + 3 };");
        sb.AppendLine("        for (int index = 0; index < values.Length; index++)");
        sb.AppendLine("        {");
        sb.AppendLine("            ref int value = ref values[index];");
        sb.AppendLine("            if (index == 1) continue;");
        sb.AppendLine("            value += index + 1;");
        sb.AppendLine("            if (index == 2) break;");
        sb.AppendLine("        }");
        sb.AppendLine("        return values[0] * 1000 + values[1] * 100 + values[2] * 10 + values[3];");
        sb.AppendLine("    }");
        sb.AppendLine("    private static void AliasBump(ref int first, ref int second)");
        sb.AppendLine("    {");
        sb.AppendLine("        first++;");
        sb.AppendLine("        second++;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RefParameterAliases(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int alias = a;");
        sb.AppendLine("        int first = a;");
        sb.AppendLine("        int second = 10;");
        sb.AppendLine("        AliasBump(ref alias, ref alias);");
        sb.AppendLine("        AliasBump(ref first, ref second);");
        sb.AppendLine("        return alias * 10000 + first * 100 + second;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static int RecurseRefAliases(ref int first, ref int second, int depth)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (depth == 0) return first * 10 + second;");
        sb.AppendLine("        first += 2;");
        sb.AppendLine("        second += 3;");
        sb.AppendLine("        return RecurseRefAliases(ref first, ref first, depth - 1) + 1;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RefRecursiveAliases(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        int result = RecurseRefAliases(ref state, ref state, 2);");
        sb.AppendLine("        return result * 100 + state;");
        sb.AppendLine("    }");
    }

    private void GenerateTupleAssignmentMethods(StringBuilder sb)
    {
        // Tuple lowering must preserve RHS evaluation, receiver evaluation, and final
        // left-to-right writes. Discards and repeated targets expose extra reads/writes.
        sb.AppendLine("    private sealed class TupleBox { public int X; public int Y; }");
        sb.AppendLine("    private static int TupleNext(ref int state) { return ++state; }");
        sb.AppendLine("    private static TupleBox TuplePick(TupleBox value, ref int state)");
        sb.AppendLine("    {");
        sb.AppendLine("        state++;");
        sb.AppendLine("        return value;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int TupleSideEffects(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var left = new TupleBox();");
        sb.AppendLine("        var right = new TupleBox();");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        (TuplePick(left, ref state).X, _, TuplePick(right, ref state).Y) = (TupleNext(ref state), TupleNext(ref state), TupleNext(ref state));");
        sb.AppendLine("        return left.X * 100000 + right.Y * 10000 + state;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int TupleRepeatedTarget(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int x = 0;");
        sb.AppendLine("        (x, x) = (a, b);");
        sb.AppendLine("        return x;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int TupleForeach(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var pairs = new (int X, int Y)[] { (a, b), (b, a) };");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        foreach (var (x, y) in pairs) total = total * 10 + x - y;");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
    }

    private void GenerateNullPropagationMethods(StringBuilder sb)
    {
        // Null-conditional chains must preserve receivers, defer indexes/arguments/RHSs,
        // and leave exactly one result on every path. The call counter makes duplicated
        // or prematurely evaluated operands observable to the CLR differential oracle.
        sb.AppendLine("    private sealed class NullNode");
        sb.AppendLine("    {");
        sb.AppendLine("        public int Value;");
        sb.AppendLine("        public string Text;");
        sb.AppendLine("        public string TextProperty { get; set; }");
        sb.AppendLine("        public NullNode Child;");
        sb.AppendLine("    }");
        sb.AppendLine("    private static int NullNext(ref int state) { return ++state; }");
        sb.AppendLine("    private static int NullIndex(ref int state) { state++; return 0; }");
        sb.AppendLine("    private static string NullText(ref int state) { state++; return \"X\"; }");
        sb.AppendLine("    private static NullNode NullPick(NullNode value, ref int state)");
        sb.AppendLine("    {");
        sb.AppendLine("        state++;");
        sb.AppendLine("        return value;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullChainReceiver(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var root = a == 0 ? null : new NullNode { Value = a, Child = a == 1 ? null : new NullNode { Value = a + 10 } };");
        sb.AppendLine("        int calls = 0;");
        sb.AppendLine("        int value = NullPick(root, ref calls)?.Child?.Value ?? NullNext(ref calls);");
        sb.AppendLine("        return value * 100 + calls;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullElementShortCircuit(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int[][] rows = a == 0 ? null : new[] { new[] { a, a + 1 } };");
        sb.AppendLine("        int calls = 0;");
        sb.AppendLine("        int value = rows?[NullIndex(ref calls)]?[1] ?? NullNext(ref calls);");
        sb.AppendLine("        return value * 100 + calls;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullDelegateArguments(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Func<int, int, int> f = a == 0 ? null : (left, right) => left * 10 + right + a;");
        sb.AppendLine("        int calls = 0;");
        sb.AppendLine("        int value = f?.Invoke(NullNext(ref calls), NullNext(ref calls)) ?? NullNext(ref calls);");
        sb.AppendLine("        return value * 100 + calls;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullCoalesceTargets(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        string local = a == 0 ? null : \"Local\";");
        sb.AppendLine("        string[] values = { a == 1 ? null : \"Array\", \"unused\" };");
        sb.AppendLine("        var box = new NullNode { Text = a == 2 ? null : \"Field\", TextProperty = a == 3 ? null : \"Property\" };");
        sb.AppendLine("        int calls = 0;");
        sb.AppendLine("        local ??= NullText(ref calls);");
        sb.AppendLine("        values[NullIndex(ref calls)] ??= NullText(ref calls);");
        sb.AppendLine("        box.Text ??= NullText(ref calls);");
        sb.AppendLine("        box.TextProperty ??= NullText(ref calls);");
        sb.AppendLine("        return local.Length * 1000000 + values[0].Length * 100000 + box.Text.Length * 10000 + box.TextProperty.Length * 1000 + calls;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullConditionalCompound(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var root = a == 0 ? null : new NullNode { Child = a == 1 ? null : new NullNode { Value = a } };");
        sb.AppendLine("        int calls = 0;");
        sb.AppendLine("        root?.Child?.Value += NullNext(ref calls);");
        sb.AppendLine("        return (root?.Child?.Value ?? -1) * 100 + calls;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullConditionalCoalesce(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var root = a == 0 ? null : new NullNode { Child = a == 1 ? null : new NullNode { Text = a == 2 ? null : \"Child\" } };");
        sb.AppendLine("        int calls = 0;");
        sb.AppendLine("        root?.Child?.Text ??= NullText(ref calls);");
        sb.AppendLine("        string value = root?.Child?.Text ?? \"Missing\";");
        sb.AppendLine("        return value.Length * 100 + calls;");
        sb.AppendLine("    }");
    }

    private void GenerateNullableMethods(StringBuilder sb)
    {
        // Nullable value types: lifting, unboxing, and the null-coalescing operators.
        sb.AppendLine("    public static int NullableLift(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int? x = a;");
        sb.AppendLine("        int? y = b;");
        sb.AppendLine("        int? sum = x + y;");
        sb.AppendLine("        return sum ?? -1;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullableCoalesce(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int? x = a > 0 ? a : null;");
        sb.AppendLine("        return (x ?? b) + (x ?? 0);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static bool NullableCompare(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int? x = a;");
        sb.AppendLine("        int? y = b;");
        sb.AppendLine("        return x == y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NullableUnbox(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int? x = a;");
        sb.AppendLine("        return x.HasValue ? x.Value : b;");
        sb.AppendLine("    }");
    }

    private void GenerateRandomExpressionMethods(StringBuilder sb)
    {
        // Type-driven random expressions. Unlike the template families, the structure of
        // this code differs on every generation, which is what lets the search reach
        // compiler output that a fixed template never produces.
        var generator = new ExpressionGenerator(_random.Next());
        int methodCount = _random.Next(2, 6);

        for (int i = 0; i < methodCount; i++)
        {
            FuzzType resultType = (FuzzType)_random.Next(0, 5); // Int, Long, BigInt, Bool, Str
            ExprNode body = generator.Generate(resultType, _random.Next(2, 7));
            string parameters = string.Join(
                ", ",
                ExpressionGenerator.Parameters.Select(p => p.Declaration));

            sb.AppendLine($"    public static {ExpressionRenderer.TypeName(resultType)} Random{i}({parameters})");
            sb.AppendLine("    {");
            sb.AppendLine($"        return {ExpressionRenderer.Render(body)};");
            sb.AppendLine("    }");
        }
    }

    /// <summary>
    /// Generates a two-contract source where one contract calls the other.
    /// </summary>
    /// <remarks>
    /// The caller receives the callee's hash as a parameter rather than embedding it,
    /// because the hash only exists after deployment. This is the shape a real
    /// cross-contract call takes, and it exercises the method-token table, argument
    /// marshalling, and entry-point dispatch across two separately compiled artifacts.
    /// </remarks>
    public string GenerateMultiContract()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Numerics;");
        sb.AppendLine("using Neo.SmartContract.Framework;");
        sb.AppendLine("using Neo.SmartContract.Framework.Services;");
        sb.AppendLine("using Neo.SmartContract.Framework.Attributes;");
        sb.AppendLine();
        sb.AppendLine("public class ContractB : SmartContract");
        sb.AppendLine("{");
        sb.AppendLine("    public static int Compute(int a, int b)");
        sb.AppendLine("    {");
        // The body has to stay within the parameters this signature declares. The random
        // expression generator draws from the full parameter pool, which this method does
        // not have, so it is not used here.
        sb.AppendLine("        return a * b + 1;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Accumulate(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var existing = Storage.Get(new byte[] { 0x01 });");
        sb.AppendLine("        int current = existing == null ? 0 : (int)(System.Numerics.BigInteger)existing;");
        sb.AppendLine("        int next = current + a;");
        sb.AppendLine("        Storage.Put(new byte[] { 0x01 }, next);");
        sb.AppendLine("        return next;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        // Without an explicit permission the VM refuses the cross-contract call with
        // "Cannot Call Method ... From Contract ...", so the caller has to declare that
        // it may call the callee.
        sb.AppendLine("[ContractPermission(\"*\", \"*\")]");
        sb.AppendLine("public class ContractA : SmartContract");
        sb.AppendLine("{");
        sb.AppendLine("    public static int Forward(int a, int b, UInt160 target)");
        sb.AppendLine("    {");
        sb.AppendLine("        var result = Contract.Call(target, \"compute\", CallFlags.All,");
        sb.AppendLine("            new object[] { a, b });");
        sb.AppendLine("        return (int)result;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ForwardTwice(int a, int b, UInt160 target)");
        sb.AppendLine("    {");
        sb.AppendLine("        var first = Contract.Call(target, \"compute\", CallFlags.All,");
        sb.AppendLine("            new object[] { a, b });");
        sb.AppendLine("        var second = Contract.Call(target, \"accumulate\", CallFlags.All,");
        sb.AppendLine("            new object[] { (int)first });");
        sb.AppendLine("        return (int)second;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ForwardAndStore(int a, int b, UInt160 target)");
        sb.AppendLine("    {");
        sb.AppendLine("        var result = Contract.Call(target, \"accumulate\", CallFlags.All,");
        sb.AppendLine("            new object[] { a + b });");
        sb.AppendLine("        Storage.Put(new byte[] { 0x02 }, (int)result);");
        sb.AppendLine("        return (int)result * 2;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private void GenerateInterContractMethods(StringBuilder sb)
    {
        // Cross-contract dispatch. Calling through System.Contract.Call exercises the
        // method-token table, the argument marshalling, and the entry-point dispatch,
        // none of which a direct call touches. The call target is the contract itself,
        // which keeps the input self-contained.
        sb.AppendLine("    public static int Target(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a * b + 1;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CallSelf(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var result = Contract.Call(Runtime.ExecutingScriptHash, \"target\",");
        sb.AppendLine("            CallFlags.All, new object[] { a, b });");
        sb.AppendLine("        return (int)result;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CallSelfTwice(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var first = Contract.Call(Runtime.ExecutingScriptHash, \"target\",");
        sb.AppendLine("            CallFlags.All, new object[] { a, b });");
        sb.AppendLine("        var second = Contract.Call(Runtime.ExecutingScriptHash, \"target\",");
        sb.AppendLine("            CallFlags.All, new object[] { (int)first, b });");
        sb.AppendLine("        return (int)second;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CallWithCurrency(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int local = a + b;");
        sb.AppendLine("        var result = Contract.Call(Runtime.ExecutingScriptHash, \"target\",");
        sb.AppendLine("            CallFlags.All, new object[] { local, b });");
        sb.AppendLine("        return (int)result + local;");
        sb.AppendLine("    }");
    }

    private void GenerateStorageMapMethods(StringBuilder sb)
    {
        // StorageMap adds a key prefix on top of the raw storage API, so it exercises a
        // different key-encoding path from Storage.Put/Get.
        sb.AppendLine("    public static int MapWriteRead(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x10 });");
        sb.AppendLine("        map.Put(new byte[] { 0x01 }, a);");
        sb.AppendLine("        map.Put(new byte[] { 0x02 }, b);");
        sb.AppendLine("        var value = map.Get(new byte[] { 0x01 });");
        sb.AppendLine("        return value == null ? -1 : (int)(System.Numerics.BigInteger)value;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int MapOverwrite(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x11 });");
        sb.AppendLine("        map.Put(new byte[] { 0x01 }, a);");
        sb.AppendLine("        map.Put(new byte[] { 0x01 }, b);");
        sb.AppendLine("        var value = map.Get(new byte[] { 0x01 });");
        sb.AppendLine("        return value == null ? -1 : (int)(System.Numerics.BigInteger)value;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int MapDelete(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x12 });");
        sb.AppendLine("        map.Put(new byte[] { 0x01 }, a);");
        sb.AppendLine("        map.Delete(new byte[] { 0x01 });");
        sb.AppendLine("        var value = map.Get(new byte[] { 0x01 });");
        sb.AppendLine("        return value == null ? b : (int)(System.Numerics.BigInteger)value;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int MapTwoPrefixes(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var first = new StorageMap(Storage.CurrentContext, new byte[] { 0x13 });");
        sb.AppendLine("        var second = new StorageMap(Storage.CurrentContext, new byte[] { 0x14 });");
        sb.AppendLine("        first.Put(new byte[] { 0x01 }, a);");
        sb.AppendLine("        second.Put(new byte[] { 0x01 }, b);");
        sb.AppendLine("        var x = first.Get(new byte[] { 0x01 });");
        sb.AppendLine("        var y = second.Get(new byte[] { 0x01 });");
        sb.AppendLine("        if (x == null || y == null) return -1;");
        sb.AppendLine("        return (int)(System.Numerics.BigInteger)x + (int)(System.Numerics.BigInteger)y;");
        sb.AppendLine("    }");
    }

    private void GenerateStructMethods(StringBuilder sb)
    {
        // Value types. A struct is copied on assignment, so a mutation through one copy
        // must not be visible through the other; the compiler has to reproduce that
        // semantics on a VM whose only composite is a mutable array.
        sb.AppendLine("    public struct Point");
        sb.AppendLine("    {");
        sb.AppendLine("        public int X;");
        sb.AppendLine("        public int Y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public struct Nested");
        sb.AppendLine("    {");
        sb.AppendLine("        public Point Origin;");
        sb.AppendLine("        public int Scale;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static int StructFields(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var p = new Point();");
        sb.AppendLine("        p.X = a;");
        sb.AppendLine("        p.Y = b;");
        sb.AppendLine("        return p.X * 2 + p.Y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StructCopyIsIndependent(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var p = new Point { X = a, Y = a + 1 };");
        sb.AppendLine("        var q = p;");
        sb.AppendLine("        q.X = 99;");
        sb.AppendLine("        return p.X + q.X;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StructNested(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var n = new Nested();");
        sb.AppendLine("        n.Origin.X = a;");
        sb.AppendLine("        n.Origin.Y = b;");
        sb.AppendLine("        n.Scale = 2;");
        sb.AppendLine("        return n.Origin.X * n.Scale + n.Origin.Y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StructProperty(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var p = new Point { X = a, Y = b };");
        sb.AppendLine("        return Add(p, b);");
        sb.AppendLine("    }");
        sb.AppendLine("    private static int Add(Point p, int delta) { return p.X + p.Y + delta; }");
    }

    private void GenerateMultiDimArrayMethods(StringBuilder sb)
    {
        // Multi-dimensional arrays. The compiler has to lower the rank into a flat
        // representation and recompute the index arithmetic, so the stride and the bounds
        // are both worth checking against the same code running on the CLR.
        sb.AppendLine("    public static int MatrixSized(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var matrix = new int[2, 3];");
        sb.AppendLine("        matrix[0, 1] = a;");
        sb.AppendLine("        matrix[1, 2] = b;");
        sb.AppendLine("        return matrix[0, 1] + matrix[1, 2] + matrix[0, 0];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int MatrixInitializer(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var matrix = new int[,] { { 1, 2 }, { 3, 4 } };");
        sb.AppendLine("        return matrix[0, 0] + matrix[1, 1] + a;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int MatrixCompound(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var matrix = new int[2, 2];");
        sb.AppendLine("        matrix[0, 0] = a;");
        sb.AppendLine("        matrix[1, 1] = matrix[0, 0] + 2;");
        sb.AppendLine("        matrix[1, 1] += 5;");
        sb.AppendLine("        matrix[1, 1]--;");
        sb.AppendLine("        ++matrix[1, 1];");
        sb.AppendLine("        return matrix[1, 1];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Jagged(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var rows = new int[2][];");
        sb.AppendLine("        rows[0] = new int[] { a, a + 1 };");
        sb.AppendLine("        rows[1] = new int[] { b, b + 1 };");
        sb.AppendLine("        return rows[0][0] + rows[0][1] + rows[1][0] + rows[1][1];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ArrayLength(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var matrix = new int[2, 3];");
        sb.AppendLine("        return matrix.Length + a;");
        sb.AppendLine("    }");
    }

    private void GenerateRecordMethods(StringBuilder sb)
    {
        // Records. A `with` expression clones the record and applies the listed changes,
        // so the original must be untouched; positional deconstruction and the synthesized
        // equality take separate lowering paths.
        sb.AppendLine("    public record Point(int X, int Y);");
        sb.AppendLine("    public record NamedPoint(string Name, int X, int Y) : Point(X, Y);");
        sb.AppendLine("    public record struct Rect(int W, int H);");
        sb.AppendLine("    public static int WithExpression(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Point p = new Point(a, b);");
        sb.AppendLine("        Point q = p with { X = 99 };");
        sb.AppendLine("        return p.X * 1000 + p.Y * 10 + q.X;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RecordEquality(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Point p = new Point(a, b);");
        sb.AppendLine("        Point q = new Point(a, b);");
        sb.AppendLine("        Point r = new Point(a, b + 1);");
        sb.AppendLine("        return (p == q ? 1 : 0) + (p != r ? 2 : 0);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int Deconstruct(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        var (x, y) = new Point(a, b);");
        sb.AppendLine("        return x * 100 + y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RecordStructWith(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Rect r1 = new Rect(a, 5);");
        sb.AppendLine("        Rect r2 = r1 with { W = 9 };");
        sb.AppendLine("        return r1.W * 1000 + r1.H * 10 + r2.W + r2.H;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DerivedWith(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        NamedPoint n = new NamedPoint(\"src\", a, 7);");
        sb.AppendLine("        NamedPoint m = n with { Name = \"dst\" };");
        sb.AppendLine("        return (m.Name == \"dst\" ? 1000 : 0) + m.X * 10 + m.Y;");
        sb.AppendLine("    }");
    }

    private void GenerateCrossFamilyMethods(StringBuilder sb)
    {
        // Cross-feature interaction probe: methods from three families that declare no
        // conflicting types are composed into one contract. Interaction defects surface
        // only when shapes co-reside (the by-ref x initializer case), and the random
        // template pick plus the families' own internal randomness keep producing
        // novel combinations across iterations.
        (string Label, Action<StringBuilder> Generate)[] templates =
        [
            ("arith+conv+flow", s =>
            {
                GenerateArithmeticMethods(s);
                GenerateConversionMethods(s);
                GenerateControlFlowMethods(s);
            }),
            ("strings+lambda+exc", s =>
            {
                GenerateStringMethods(s);
                GenerateLambdaMethods(s);
                GenerateExceptionMethods(s);
            }),
            ("structs+multiarray+static", s =>
            {
                GenerateStructMethods(s);
                GenerateMultiDimArrayMethods(s);
                GenerateStaticStateMethods(s);
            }),
            ("evalorder+callbind+narrow", s =>
            {
                GenerateEvaluationOrderMethods(s);
                GenerateCallBindingMethods(s);
                GenerateNarrowTypeMethods(s);
            }),
            ("enumswitch+deeploop+bigmath", s =>
            {
                GenerateEnumSwitchMethods(s);
                GenerateDeepLoopMethods(s);
                GenerateBigMathMethods(s);
            }),
            ("operators+indexer+strings", s =>
            {
                GenerateOperatorOverloadMethods(s);
                GenerateIndexerMethods(s);
                GenerateStringMethods(s);
            }),
            ("records+recursion+events", s =>
            {
                GenerateRecordMethods(s);
                GenerateRecursiveMethods(s);
                GenerateEventMethods(s);
            }),
            ("patterns+nullable+outref", s =>
            {
                GeneratePatternMethods(s);
                GenerateNullableMethods(s);
                GenerateOutRefMethods(s);
            }),
        ];

        var template = templates[_random.Next(templates.Length)];
        template.Generate(sb);
    }

    private void GenerateRecursiveMethods(StringBuilder sb)
    {
        // Recursion: every frame needs its own parameter slots and INITSLOT, the call is
        // to the method currently being converted, and the base case must terminate before
        // the VM invocation-stack limit. Depths are kept small and derived from the
        // arguments so arbitrary fuzz inputs converge.
        sb.AppendLine("    public static int RecFactorial(int n) { return n <= 1 ? 1 : n * RecFactorial(n - 1); }");
        sb.AppendLine("    public static int RecSum(int n) { return n <= 0 ? 0 : n + RecSum(n - 1); }");
        sb.AppendLine("    public static int RecEven(int n) { return n == 0 ? 1 : RecOdd(n < 0 ? n + 1 : n - 1); }");
        sb.AppendLine("    public static int RecOdd(int n) { return n == 0 ? 0 : RecEven(n < 0 ? n + 1 : n - 1); }");
        sb.AppendLine("    public static int RecAccumulate(int n, int acc) { return n <= 0 ? acc : RecAccumulate(n - 1, acc + n); }");
        sb.AppendLine("    public static int RecArraySum(int[] values, int index)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (values == null || index >= values.Length) return 0;");
        sb.AppendLine("        return values[index] + RecArraySum(values, index + 1);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RecEarlyExit(int n)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (n >= 100) return -1;");
        sb.AppendLine("        if (n % 7 == 0 && n > 0) return n;");
        sb.AppendLine("        return RecEarlyExit(n + 1);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int RecWrapper(int n) { return RecFactorial(n < 0 ? 0 : n % 6); }");
    }

    private void GenerateEnumSwitchMethods(StringBuilder sb)
    {
        // Enum-typed switches: the jump table keys off the enum's underlying value, and
        // the arms must not leak into each other when values are non-contiguous.
        sb.AppendLine("    public enum State : int { Idle = 0, Running = 5, Done = 9 }");
        sb.AppendLine("    public static int EnumSwitchInt(State s, int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        switch (s)");
        sb.AppendLine("        {");
        sb.AppendLine("            case State.Idle: return a + 1;");
        sb.AppendLine("            case State.Running: return a * 2;");
        sb.AppendLine("            case State.Done: return a - 3;");
        sb.AppendLine("            default: return -1;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int EnumCastRoundTrip(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        State s = (State)(a % 10);");
        sb.AppendLine("        return s switch");
        sb.AppendLine("        {");
        sb.AppendLine("            State.Idle => 1,");
        sb.AppendLine("            State.Running => 2,");
        sb.AppendLine("            State.Done => 3,");
        sb.AppendLine("            _ => 0");
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int EnumIfChain(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        State s = State.Running;");
        sb.AppendLine("        if ((int)s == a) return a;");
        sb.AppendLine("        return (int)s + a;");
        sb.AppendLine("    }");
    }

    private void GenerateDeepLoopMethods(StringBuilder sb)
    {
        // Nested loops with cross-level control flow: the compiler has to unwind the
        // right number of loop frames for an inner break/continue and keep the outer
        // condition evaluated with the mutated variable.
        sb.AppendLine("    public static int NestedBreakContinue(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < 4; i++)");
        sb.AppendLine("        {");
        sb.AppendLine("            for (int j = 0; j < 4; j++)");
        sb.AppendLine("            {");
        sb.AppendLine("                if (j == 2) break;");
        sb.AppendLine("                if (i == a) continue;");
        sb.AppendLine("                total += i * 10 + j;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int WhileInsideFor(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        int i = 0;");
        sb.AppendLine("        while (i < 5)");
        sb.AppendLine("        {");
        sb.AppendLine("            int j = 0;");
        sb.AppendLine("            while (j < a)");
        sb.AppendLine("            {");
        sb.AppendLine("                total += j;");
        sb.AppendLine("                j++;");
        sb.AppendLine("            }");
        sb.AppendLine("            i++;");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DoWhileCombined(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        int k = 0;");
        sb.AppendLine("        do");
        sb.AppendLine("        {");
        sb.AppendLine("            k++;");
        sb.AppendLine("            if (k % 2 == 0) continue;");
        sb.AppendLine("            total += k;");
        sb.AppendLine("        } while (k < 6);");
        sb.AppendLine("        return total * (a + 1);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int TripleNestedSum(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < 3; i++)");
        sb.AppendLine("            for (int j = 0; j < 3; j++)");
        sb.AppendLine("                for (int k = 0; k < 3; k++)");
        sb.AppendLine("                    total += (i + j + k) % 2 == 0 ? 1 : -1;");
        sb.AppendLine("        return total * a;");
        sb.AppendLine("    }");
    }

    private void GenerateDelegateCaptureMethods(StringBuilder sb)
    {
        // Delegates capturing locals, parameters, fields, and mutations. The compiler has
        // to hoist the captured variables, route the invoke through the closure, and keep
        // later reads of the same variable seeing the delegate's writes.
        sb.AppendLine("    private static int _shared;");
        sb.AppendLine("    public static int CaptureLocal(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int captured = a;");
        sb.AppendLine("        Func<int> get = () => captured;");
        sb.AppendLine("        captured += 5;");
        sb.AppendLine("        return get() * 10 + captured;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CaptureAndMutate(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int state = a;");
        sb.AppendLine("        Action bump = () => state += 3;");
        sb.AppendLine("        bump();");
        sb.AppendLine("        bump();");
        sb.AppendLine("        return state;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CaptureField(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        _shared = a;");
        sb.AppendLine("        Func<int, int> add = x => x + _shared;");
        sb.AppendLine("        _shared = a * 2;");
        sb.AppendLine("        return add(a) + _shared;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CaptureParameter(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Func<int, int> scale = k => k * a;");
        sb.AppendLine("        return scale(b) + scale(b + 1);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DelegateChaining(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Func<int, int> f = x => x + 1;");
        sb.AppendLine("        Func<int, int> g = x => f(x) * 2;");
        sb.AppendLine("        return g(a);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int DelegateInLoop(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < 3; i++)");
        sb.AppendLine("        {");
        sb.AppendLine("            int local = i * a;");
        sb.AppendLine("            Func<int> get = () => local;");
        sb.AppendLine("            total += get();");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StaticDelegate(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Func<int, int, int> op = static (x, y) => x * y + 1;");
        sb.AppendLine("        return op(a, a + 1);");
        sb.AppendLine("    }");
    }

    private void GenerateBigMathMethods(StringBuilder sb)
    {
        // BigInteger instance and static math beyond the raw operators. NeoVM integers
        // are arbitrary precision already, so these methods exercise the lowering of the
        // helper calls and their argument shapes rather than the arithmetic itself.
        sb.AppendLine("    public static BigInteger BigAdd(BigInteger a, BigInteger b) { return BigInteger.Add(a, b); }");
        sb.AppendLine("    public static BigInteger BigSub(BigInteger a, BigInteger b) { return BigInteger.Subtract(a, b); }");
        sb.AppendLine("    public static BigInteger BigMul(BigInteger a, BigInteger b) { return BigInteger.Multiply(a, b); }");
        sb.AppendLine("    public static BigInteger BigDiv(BigInteger a, BigInteger b) { return b != 0 ? BigInteger.Divide(a, b) : BigInteger.MinusOne; }");
        sb.AppendLine("    public static BigInteger BigRem(BigInteger a, BigInteger b) { return b != 0 ? BigInteger.Remainder(a, b) : BigInteger.MinusOne; }");
        sb.AppendLine("    public static BigInteger BigNeg(BigInteger a) { return BigInteger.Negate(a); }");
        sb.AppendLine("    public static int BigSign(BigInteger a) { return a.Sign; }");
        sb.AppendLine("    public static bool BigIsZero(BigInteger a) { return a.IsZero; }");
        sb.AppendLine("    public static bool BigIsOne(BigInteger a) { return a.IsOne; }");
        sb.AppendLine("    public static BigInteger BigMin(BigInteger a, BigInteger b) { return BigInteger.Min(a, b); }");
        sb.AppendLine("    public static BigInteger BigMax(BigInteger a, BigInteger b) { return BigInteger.Max(a, b); }");
        sb.AppendLine("    public static BigInteger BigAbs(BigInteger a) { return a < 0 ? -a : a; }");
        sb.AppendLine("    public static BigInteger BigPow(BigInteger a, int e) { return e >= 0 && e <= 8 ? BigInteger.Pow(a, e) : BigInteger.Zero; }");
        sb.AppendLine("    public static BigInteger BigModPow(BigInteger b, BigInteger e, BigInteger m)");
        sb.AppendLine("    {");
        sb.AppendLine("        return m != 0 ? BigInteger.ModPow(b, e, m) : BigInteger.Zero;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static BigInteger BigGcd(BigInteger a, BigInteger b) { return BigInteger.GreatestCommonDivisor(a, b); }");
        sb.AppendLine("    public static bool BigEquals(BigInteger a, BigInteger b) { return a.Equals(b); }");
        sb.AppendLine("    public static string BigToString(BigInteger a) { return a.ToString(); }");
        sb.AppendLine("    public static BigInteger BigParse(string s) { try { return BigInteger.Parse(s); } catch (System.Exception) { return BigInteger.Zero; } }");
    }

    private void GenerateStringApiMethods(StringBuilder sb)
    {
        // String instance APIs. Strings are UTF-8 ByteStrings on the VM, so slicing and
        // searching lower to byte arithmetic whose results must line up with C#'s
        // UTF-16 semantics on the generated inputs.
        sb.AppendLine("    public static string StrSubstring(string s, int start, int len) { return start >= 0 && len >= 0 && start + len <= s.Length ? s.Substring(start, len) : \"\"; }");
        sb.AppendLine("    public static int StrIndexOf(string s, string t) { return s.IndexOf(t); }");
        sb.AppendLine("    public static int StrLastIndexOf(string s, string t) { return s.LastIndexOf(t); }");
        sb.AppendLine("    public static bool StrStartsWith(string s, string t) { return s.StartsWith(t); }");
        sb.AppendLine("    public static bool StrEndsWith(string s, string t) { return s.EndsWith(t); }");
        sb.AppendLine("    public static string StrRemove(string s, int start, int len) { return start >= 0 && len >= 0 && start + len <= s.Length ? s.Remove(start, len) : s; }");
        sb.AppendLine("    public static string StrToUpper(string s) { return s.ToUpper(); }");
        sb.AppendLine("    public static string StrToLower(string s) { return s.ToLower(); }");
        sb.AppendLine("    public static string StrTrim(string s) { return s.Trim(); }");
        sb.AppendLine("    public static string StrTrimStart(string s) { return s.TrimStart(); }");
        sb.AppendLine("    public static string StrTrimEnd(string s) { return s.TrimEnd(); }");
        sb.AppendLine("    public static int StrSplitCount(string s, char sep) { return s.Split(sep).Length; }");
        sb.AppendLine("    public static string StrJoinFirst(string s, char sep)");
        sb.AppendLine("    {");
        sb.AppendLine("        var parts = s.Split(sep);");
        sb.AppendLine("        return parts.Length > 0 ? parts[0] : \"\";");
        sb.AppendLine("    }");
    }

    private void GenerateSwitchFormMethods(StringBuilder sb)
    {
        // Switch forms beyond the constant-int one: a string switch lowers through
        // sequential comparisons, goto case threads control between switch arms, and the
        // collection/index initializer shapes exercise the initializer converters.
        sb.AppendLine("    public static int StringSwitch(string s, int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        switch (s)");
        sb.AppendLine("        {");
        sb.AppendLine("            case \"add\": return a + 1;");
        sb.AppendLine("            case \"sub\": return a - 1;");
        sb.AppendLine("            case null: return 0;");
        sb.AppendLine("            default: return -1;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int GotoCase(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        switch (a)");
        sb.AppendLine("        {");
        sb.AppendLine("            case 0:");
        sb.AppendLine("                total += 1;");
        sb.AppendLine("                goto case 2;");
        sb.AppendLine("            case 1:");
        sb.AppendLine("                total += 10;");
        sb.AppendLine("                goto default;");
        sb.AppendLine("            case 2:");
        sb.AppendLine("                total += 100;");
        sb.AppendLine("                break;");
        sb.AppendLine("            default:");
        sb.AppendLine("                total += 1000;");
        sb.AppendLine("                break;");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int IndexInitializer(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var table = new Table");
        sb.AppendLine("        {");
        sb.AppendLine("            [1] = a,");
        sb.AppendLine("            [2] = a + 1");
        sb.AppendLine("        };");
        sb.AppendLine("        return table[1] * 10 + table[2];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NameOfAndDefault(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int b = default;");
        sb.AppendLine("        string n = nameof(a);");
        sb.AppendLine("        return b + a + n.Length;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ObjectInitializerNested(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        var outer = new Wrapper");
        sb.AppendLine("        {");
        sb.AppendLine("            Tag = \"t\",");
        sb.AppendLine("            Inner = new Point { X = a, Y = a + 1 }");
        sb.AppendLine("        };");
        sb.AppendLine("        return outer.Inner.X * 100 + outer.Inner.Y + outer.Tag.Length;");
        sb.AppendLine("    }");
        sb.AppendLine("    public class Table");
        sb.AppendLine("    {");
        sb.AppendLine("        private int[] _data = new int[8];");
        sb.AppendLine("        public int this[int i]");
        sb.AppendLine("        {");
        sb.AppendLine("            get { return _data[i]; }");
        sb.AppendLine("            set { _data[i] = value; }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("    public class Wrapper");
        sb.AppendLine("    {");
        sb.AppendLine("        public string Tag;");
        sb.AppendLine("        public Point Inner;");
        sb.AppendLine("    }");
        sb.AppendLine("    public struct Point");
        sb.AppendLine("    {");
        sb.AppendLine("        public int X;");
        sb.AppendLine("        public int Y;");
        sb.AppendLine("    }");
    }

    private void GenerateControlFlowInteractionMethods(StringBuilder sb)
    {
        // Control-flow interactions: the compiler lowers break/continue/return out of a
        // protected region by rerouting them through the finally blocks, so the shapes
        // where several of them combine are the interesting ones.
        sb.AppendLine("    public static int BreakOutOfTryInLoop(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < 10; i++)");
        sb.AppendLine("        {");
        sb.AppendLine("            try");
        sb.AppendLine("            {");
        sb.AppendLine("                if (i == a) break;");
        sb.AppendLine("                total += i;");
        sb.AppendLine("            }");
        sb.AppendLine("            catch (System.Exception)");
        sb.AppendLine("            {");
        sb.AppendLine("                total -= 100;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ReturnThroughFinally(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int tracked = 0;");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine("            tracked += 1;");
        sb.AppendLine("            return a + tracked;");
        sb.AppendLine("        }");
        sb.AppendLine("        finally");
        sb.AppendLine("        {");
        sb.AppendLine("            tracked += 50;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ContinueThroughFinally(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < 6; i++)");
        sb.AppendLine("        {");
        sb.AppendLine("            try");
        sb.AppendLine("            {");
        sb.AppendLine("                if (i == a) continue;");
        sb.AppendLine("                total += i;");
        sb.AppendLine("            }");
        sb.AppendLine("            finally");
        sb.AppendLine("            {");
        sb.AppendLine("                total += 1000;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CatchInLoop(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = -2; i < 3; i++)");
        sb.AppendLine("        {");
        sb.AppendLine("            try");
        sb.AppendLine("            {");
        sb.AppendLine("                total += a / i;");
        sb.AppendLine("            }");
        sb.AppendLine("            catch (System.Exception)");
        sb.AppendLine("            {");
        sb.AppendLine("                total += 7;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int NestedFinallyBreak(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        for (int i = 0; i < 5; i++)");
        sb.AppendLine("        {");
        sb.AppendLine("            try");
        sb.AppendLine("            {");
        sb.AppendLine("                try");
        sb.AppendLine("                {");
        sb.AppendLine("                    if (i == a) break;");
        sb.AppendLine("                    total += 1;");
        sb.AppendLine("                }");
        sb.AppendLine("                finally");
        sb.AppendLine("                {");
        sb.AppendLine("                    total += 10;");
        sb.AppendLine("                }");
        sb.AppendLine("            }");
        sb.AppendLine("            finally");
        sb.AppendLine("            {");
        sb.AppendLine("                total += 100;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("        return total;");
        sb.AppendLine("    }");
    }

    private void GenerateIndexerMethods(StringBuilder sb)
    {
        // Custom indexers on a user class. The compound form routes through the property
        // get/set pair of the indexer, and the increment through the same machinery as a
        // field, so both interop paths get exercised.
        sb.AppendLine("    public class Table");
        sb.AppendLine("    {");
        sb.AppendLine("        private int[] _data = new int[8];");
        sb.AppendLine("        public int this[int i]");
        sb.AppendLine("        {");
        sb.AppendLine("            get { return _data[i]; }");
        sb.AppendLine("            set { _data[i] = value; }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int IndexerGetSet(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Table t = new Table();");
        sb.AppendLine("        t[3] = a;");
        sb.AppendLine("        t[4] = b;");
        sb.AppendLine("        return t[3] * 10 + t[4];");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int IndexerCompound(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Table t = new Table();");
        sb.AppendLine("        t[2] = a;");
        sb.AppendLine("        t[2] += 5;");
        sb.AppendLine("        t[2]++;");
        sb.AppendLine("        return t[2];");
        sb.AppendLine("    }");
    }

    private void GenerateOperatorOverloadMethods(StringBuilder sb)
    {
        // User-defined operators. Roslyn binds `a == b` on a struct to a static
        // op_Equality method, so the compiled contract has to call that method; emitting
        // a raw VM comparison instead silently disagrees with C# whenever the operator's
        // semantics differ from structural equality. Arithmetic operator uses are
        // rejected outright by the compiler today, so this family only exercises the
        // shapes that compile: comparison operators, the implicit conversion, and the
        // explicit conversion (which is routed to the operator method correctly).
        sb.AppendLine("    public struct Vec");
        sb.AppendLine("    {");
        sb.AppendLine("        public int X;");
        sb.AppendLine("        public int Y;");
        sb.AppendLine("        public Vec(int x, int y) { X = x; Y = y; }");
        sb.AppendLine("        public static Vec operator +(Vec a, Vec b) { return new Vec(a.X + b.X, a.Y + b.Y); }");
        sb.AppendLine("        public static Vec operator -(Vec a, Vec b) { return new Vec(a.X - b.X, a.Y - b.Y); }");
        sb.AppendLine("        public static Vec operator *(Vec a, int k) { return new Vec(a.X * k, a.Y * k); }");
        sb.AppendLine("        public static Vec operator *(int k, Vec a) { return new Vec(a.X * k, a.Y * k); }");
        sb.AppendLine("        public static Vec operator ++(Vec a) { return new Vec(a.X + 1, a.Y + 10); }");
        sb.AppendLine("        public static Vec operator --(Vec a) { return new Vec(a.X - 1, a.Y - 5); }");
        sb.AppendLine("        public static bool operator ==(Vec a, Vec b) { return a.X == b.X; }");
        sb.AppendLine("        public static bool operator !=(Vec a, Vec b) { return a.X != b.X; }");
        sb.AppendLine("        public static bool operator <(Vec a, Vec b) { return a.X < b.X; }");
        sb.AppendLine("        public static bool operator >(Vec a, Vec b) { return a.X > b.X; }");
        sb.AppendLine("        public static implicit operator Vec(int x) { return new Vec(x, x + 1); }");
        sb.AppendLine("        public static explicit operator int(Vec v) { return v.X * 1000 + v.Y; }");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecEqPartial(int a, int b, int c)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec p = new Vec(a, b);");
        sb.AppendLine("        Vec q = new Vec(a, c);");
        sb.AppendLine("        return (p == q ? 1 : 0) + (p != q ? 2 : 0);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecEqSelf(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec p = new Vec(a, b);");
        sb.AppendLine("        return p == p ? (int)p : -1;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecRelational(int a, int b, int c)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec p = new Vec(a, b);");
        sb.AppendLine("        Vec q = new Vec(c, b);");
        sb.AppendLine("        return p < q ? 1 : (p > q ? 2 : 3);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecImplicit(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec v = a;");
        sb.AppendLine("        return v.X * 100 + v.Y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecExplicit(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec v = new Vec(a, b);");
        sb.AppendLine("        return (int)v;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecMixed(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec p = new Vec(a, b);");
        sb.AppendLine("        Vec q = new Vec(b, a);");
        sb.AppendLine("        int code = 0;");
        sb.AppendLine("        if (p == q) code += 1;");
        sb.AppendLine("        if (p < q) code += 10;");
        sb.AppendLine("        return code + (int)p;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecAddSub(int a, int b, int c, int d)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec left = new Vec(a, b);");
        sb.AppendLine("        Vec right = new Vec(c, d);");
        sb.AppendLine("        Vec sum = left + right;");
        sb.AppendLine("        Vec diff = sum - right;");
        sb.AppendLine("        return sum.X * 100 + sum.Y + diff.X;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecScalar(int a, int k)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec v = new Vec(a, a + 1);");
        sb.AppendLine("        Vec right = v * k;");
        sb.AppendLine("        Vec left = k * v;");
        sb.AppendLine("        return right.X * 100 + right.Y + left.Y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecChained(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec v = a;");
        sb.AppendLine("        v = v * 2 + new Vec(1, 1);");
        sb.AppendLine("        return (int)v;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecCompound(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec v = new Vec(a, a + 1);");
        sb.AppendLine("        v += new Vec(1, 2);");
        sb.AppendLine("        v *= 2;");
        sb.AppendLine("        return v.X * 100 + v.Y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecIncrement(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec v = new Vec(a, a + 2);");
        sb.AppendLine("        Vec old = v++;");
        sb.AppendLine("        Vec now = --v;");
        sb.AppendLine("        return old.X * 10000 + old.Y * 100 + now.X * 10 + now.Y;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int VecCompoundOnElement(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        Vec[] arr = new Vec[] { new Vec(a, a + 1), new Vec(1, 1) };");
        sb.AppendLine("        arr[0] += new Vec(1, 2);");
        sb.AppendLine("        return arr[0].X * 100 + arr[0].Y + arr[1].X;");
        sb.AppendLine("    }");
    }

    private void GenerateStorageMethods(StringBuilder sb)
    {
        // Stateful contracts. The cross-language oracle cannot cover these - the CLR
        // has no storage - but the optimizer-equivalence oracle can, and it must compare
        // the stored values, not just the return value.
        sb.AppendLine("    public static int StoreAndRead(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Neo.SmartContract.Framework.Services.Storage.Put(new byte[] { 0x01 }, a);");
        sb.AppendLine("        Neo.SmartContract.Framework.Services.Storage.Put(new byte[] { 0x02 }, b);");
        sb.AppendLine("        var first = Neo.SmartContract.Framework.Services.Storage.Get(new byte[] { 0x01 });");
        sb.AppendLine("        if (first == null) return -1;");
        sb.AppendLine("        return (int)(System.Numerics.BigInteger)first + a;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StoreThenOverwrite(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Neo.SmartContract.Framework.Services.Storage.Put(new byte[] { 0x03 }, a);");
        sb.AppendLine("        Neo.SmartContract.Framework.Services.Storage.Put(new byte[] { 0x03 }, b);");
        sb.AppendLine("        var v = Neo.SmartContract.Framework.Services.Storage.Get(new byte[] { 0x03 });");
        sb.AppendLine("        return v == null ? -1 : (int)(System.Numerics.BigInteger)v;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StoreAndDelete(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        Neo.SmartContract.Framework.Services.Storage.Put(new byte[] { 0x04 }, a);");
        sb.AppendLine("        Neo.SmartContract.Framework.Services.Storage.Delete(new byte[] { 0x04 });");
        sb.AppendLine("        var v = Neo.SmartContract.Framework.Services.Storage.Get(new byte[] { 0x04 });");
        sb.AppendLine("        return v == null ? b : (int)(System.Numerics.BigInteger)v;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int StoreComputed(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        // The stored value is computed, so a rewrite that changes the");
        sb.AppendLine("        // arithmetic would change the storage even though the return");
        sb.AppendLine("        // value is a constant.");
        sb.AppendLine("        Neo.SmartContract.Framework.Services.Storage.Put(new byte[] { 0x05 }, a * b + a);");
        sb.AppendLine("        return 42;");
        sb.AppendLine("    }");
    }

    private void GenerateNarrowTypeMethods(StringBuilder sb)
    {
        // Narrow integer types are where the optimizer's INC/DEC and NZ rewrites can
        // change behaviour, because ADD converts its operands to numbers while INC
        // requires an integer.
        sb.AppendLine("    public static int ByteArith(byte a, byte b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (a + b) + 1;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int ShortArith(short a, short b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (a - b) + 2;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int CharArith(char a, char b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a + b;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int UnsignedArith(uint a, uint b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (int)(a + b);");
        sb.AppendLine("    }");
        sb.AppendLine("    public static bool ZeroCompare(int a)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a == 0;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static bool ZeroCompareBig(BigInteger a)");
        sb.AppendLine("    {");
        sb.AppendLine("        return a == 0;");
        sb.AppendLine("    }");
        sb.AppendLine("    public static int BigArith(BigInteger a, BigInteger b)");
        sb.AppendLine("    {");
        sb.AppendLine("        return (int)(a + b);");
        sb.AppendLine("    }");
    }

    private void GenerateLongJumpMethods(StringBuilder sb)
    {
        // A try/catch whose handler sits far away forces the optimizer to compress a
        // long-form jump, including the dual-operand TRY form.
        sb.AppendLine("    public static int LongJump(int a, int b)");
        sb.AppendLine("    {");
        sb.AppendLine("        int total = 0;");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        for (int i = 0; i < 12; i++)
        {
            sb.AppendLine($"            total += {i} * a;");
            sb.AppendLine($"            total ^= b + {i};");
        }
        sb.AppendLine("            return total;");
        sb.AppendLine("        }");
        sb.AppendLine("        catch (System.Exception)");
        sb.AppendLine("        {");
        for (int i = 0; i < 6; i++)
        {
            sb.AppendLine($"            total += {i} * b;");
        }
        sb.AppendLine("            return total;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
    }

    /// <summary>
    /// Generates a typed expression of the requested type.
    /// </summary>
    /// <param name="type">The static type the expression must have.</param>
    /// <param name="depth">Remaining nesting depth.</param>
    /// <returns>A C# expression text.</returns>
    private string GenerateExpression(string type, int depth)
    {
        if (depth <= 0)
        {
            return GenerateLeaf(type);
        }

        int choice = _random.Next(10);

        if (choice < 3)
        {
            // Binary arithmetic.
            string op = BinaryOperators[_random.Next(BinaryOperators.Length)];
            if (op is "<<" or ">>")
            {
                return $"({GenerateExpression(type, depth - 1)} {op} {_random.Next(1, 16)})";
            }
            return $"({GenerateExpression(type, depth - 1)} {op} {GenerateExpression(type, depth - 1)})";
        }

        if (choice < 4)
        {
            // Comparison producing bool.
            string cmp = ComparisonOperators[_random.Next(ComparisonOperators.Length)];
            return $"({GenerateExpression(type, depth - 1)} {cmp} {GenerateExpression(type, depth - 1)})";
        }

        if (choice < 5)
        {
            // Conditional expression.
            string cond = GenerateExpression("bool", depth - 1);
            return $"({cond} ? {GenerateExpression(type, depth - 1)} : {GenerateExpression(type, depth - 1)})";
        }

        if (choice < 6)
        {
            // Cast.
            return $"(({type}){GenerateExpression("long", depth - 1)})";
        }

        if (choice < 7)
        {
            // Unary.
            string operand = GenerateExpression(type, depth - 1);
            return _random.Next(2) == 0 ? $"(-{operand})" : $"(~{operand})";
        }

        if (choice < 8)
        {
            // Math helper.
            string operand = GenerateExpression(type, depth - 1);
            return $"Math.Abs({operand})";
        }

        if (choice < 9)
        {
            // Explicit checked / unchecked.
            string operand = GenerateExpression(type, depth - 1);
            return _random.Next(2) == 0 ? $"checked({operand})" : $"unchecked({operand})";
        }

        return GenerateLeaf(type);
    }

    private string GenerateLeaf(string type)
    {
        if (type == "bool")
        {
            return _random.Next(2) == 0 ? "true" : "false";
        }

        if (type == "string")
        {
            return "\"fuzz\"";
        }

        if (type == "BigInteger")
        {
            return $"{_random.Next(int.MinValue, int.MaxValue)}";
        }

        // Numeric leaf: prefer literals but occasionally reference a parameter so that
        // the expression is not fully constant-foldable.
        if (_random.Next(3) == 0)
        {
            return _random.Next(2) == 0 ? "a" : "b";
        }

        int value = _random.Next(int.MinValue / 2, int.MaxValue / 2);
        return $"{value}";
    }
}

/// <summary>
/// Feature families targeted by the source generator.
/// </summary>
public enum FeatureFamily
{
    /// <summary>Arithmetic and overflow semantics.</summary>
    Arithmetic,

    /// <summary>Loops, switch, and branching.</summary>
    ControlFlow,

    /// <summary>Arrays and generic collections.</summary>
    Collections,

    /// <summary>String manipulation.</summary>
    Strings,

    /// <summary>Numeric conversions and narrowing.</summary>
    Conversions,

    /// <summary>try/catch/finally semantics.</summary>
    ExceptionHandling,

    /// <summary>Delegates, lambdas, and LINQ.</summary>
    Lambdas,

    /// <summary>Pattern matching and switch expressions.</summary>
    PatternMatching,

    /// <summary>Generic helpers, delegates, local functions, and static state.</summary>
    AsyncAwait,

    /// <summary>Static fields and explicit static constructors.</summary>
    StaticState,

    /// <summary>Narrow integer types, where the optimizer's INC/DEC and NZ rewrites can bite.</summary>
    NarrowTypes,

    /// <summary>Try/catch with a distant handler, forcing long-form jump compression.</summary>
    LongJumps,

    /// <summary>Stateful contracts that write to and read from storage.</summary>
    Storage,

    /// <summary>Generic methods, generic classes, and generic constraints.</summary>
    Generics,

    /// <summary>Interface declaration and virtual dispatch through the vtable.</summary>
    Interfaces,

    /// <summary>Enum declaration, comparison, and underlying type conversion.</summary>
    Enums,

    /// <summary>Event declaration and invocation.</summary>
    Events,

    /// <summary>Nullable value type lifting, unboxing, and coalescing.</summary>
    Nullable,

    /// <summary>Type-driven random expressions, structurally different on every run.</summary>
    RandomExpressions,

    /// <summary>out and ref parameters, including structs and array elements.</summary>
    OutRef,

    /// <summary>Side effects, compound assignment, and left-to-right evaluation.</summary>
    EvaluationOrder,

    /// <summary>Named, optional, params, and side-effecting argument binding.</summary>
    CallBinding,

    /// <summary>Array ref aliases, rebinding, and loop control flow.</summary>
    RefAliases,

    /// <summary>Tuple RHS evaluation, member writes, discards, and repeated targets.</summary>
    TupleAssignment,

    /// <summary>Null-conditional access, element binding, and coalescing assignments.</summary>
    NullPropagation,

    /// <summary>Struct field access, value-copy semantics, and nested structs.</summary>
    Structs,

    /// <summary>Multi-dimensional and jagged arrays.</summary>
    MultiDimArrays,

    /// <summary>User-defined operator overloads, compound assignment through operators, and user-defined conversions.</summary>
    OperatorOverloads,

    /// <summary>Records: positional construction, non-destructive with, deconstruction, and record equality.</summary>
    Records,

    /// <summary>Custom indexers on user classes, including compound assignment through the indexer.</summary>
    Indexers,

    /// <summary>try/finally interacting with loops, break, continue, and return.</summary>
    ControlFlowInteractions,

    /// <summary>String switches, goto case/default, index initializers, nameof, and default literals.</summary>
    SwitchForms,

    /// <summary>BigInteger instance and static math beyond the raw operators.</summary>
    BigMath,

    /// <summary>String instance APIs: slicing, search, trim, split, and removal.</summary>
    StringApis,

    /// <summary>Delegates capturing locals and fields, chained through helper methods.</summary>
    DelegateCapture,

    /// <summary>Enum-typed switches and jump tables over enum underlying values.</summary>
    EnumSwitch,

    /// <summary>Nested loops with labels, breaks across levels, and combined continue.</summary>
    DeepLoops,

    /// <summary>Direct and mutual recursion with accumulating parameters and early exits.</summary>
    RecursiveCalls,

    /// <summary>Methods from several families combined into one contract, probing cross-feature interactions.</summary>
    CrossFamily,

    /// <summary>Cross-contract dispatch through System.Contract.Call.</summary>
    InterContract,

    /// <summary>Storage accessed through a prefixed StorageMap.</summary>
    StorageMap,
}
