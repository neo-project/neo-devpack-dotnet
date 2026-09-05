// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableNumericCasts.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableNumericCasts
{
    public static IEnumerable<object[]> ConversionCases()
    {
        (Type Source, Type Target)[] pairs =
        [
            (typeof(int), typeof(byte)), (typeof(long), typeof(sbyte)),
            (typeof(int), typeof(short)), (typeof(int), typeof(ushort)),
            (typeof(long), typeof(int)), (typeof(ulong), typeof(uint)),
            (typeof(int), typeof(uint)), (typeof(uint), typeof(int)),
            (typeof(long), typeof(ulong)), (typeof(ulong), typeof(long)),
            (typeof(int), typeof(char)), (typeof(char), typeof(sbyte)),
            (typeof(int), typeof(long)), (typeof(uint), typeof(ulong)),
            (typeof(int), typeof(int))
        ];
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.All })
            foreach (var (source, target) in pairs)
                foreach (bool checkedConversion in new[] { false, true })
                    foreach (var (nullableSource, nullableTarget) in new[] { (true, true), (false, true), (true, false) })
                        yield return [optimization, source, target, checkedConversion, nullableSource, nullableTarget];
    }

    [DataTestMethod]
    [DynamicData(nameof(ConversionCases))]
    public void NullableIntegralCastsMatchClr(CompilationOptions.OptimizationType optimization, Type source, Type target,
        bool checkedConversion, bool nullableSource, bool nullableTarget)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract($$"""
            using System;
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static {{target.Name}}? CheckedNullable({{source.Name}}? value) => checked(({{target.Name}}?)value);
                public static {{target.Name}}? UncheckedNullable({{source.Name}}? value) => unchecked(({{target.Name}}?)value);
                public static {{target.Name}}? CheckedValue({{source.Name}} value) => checked(({{target.Name}}?)value);
                public static {{target.Name}}? UncheckedValue({{source.Name}} value) => unchecked(({{target.Name}}?)value);
                public static {{target.Name}} CheckedUnwrap({{source.Name}}? value) => checked(({{target.Name}})value);
                public static {{target.Name}} UncheckedUnwrap({{source.Name}}? value) => unchecked(({{target.Name}})value);
            }
            """, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<NumericCastContract>(nef, manifest);

        (bool Checked, bool NullableSource, bool NullableTarget, Func<BigInteger?, BigInteger?> Invoke)[] conversions =
        [
            (false, true, true, contract.UncheckedNullable), (true, true, true, contract.CheckedNullable),
            (false, false, true, contract.UncheckedValue), (true, false, true, contract.CheckedValue),
            (false, true, false, contract.UncheckedUnwrap), (true, true, false, contract.CheckedUnwrap)
        ];
        foreach (var conversion in conversions.Where(conversion => conversion.Checked == checkedConversion
            && conversion.NullableSource == nullableSource && conversion.NullableTarget == nullableTarget))
        {
            Type inputType = conversion.NullableSource ? typeof(Nullable<>).MakeGenericType(source) : source;
            Type outputType = conversion.NullableTarget ? typeof(Nullable<>).MakeGenericType(target) : target;
            var input = Expression.Parameter(typeof(object));
            var unboxed = Expression.Convert(input, inputType);
            var cast = conversion.Checked ? Expression.ConvertChecked(unboxed, outputType) : Expression.Convert(unboxed, outputType);
            var oracle = Expression.Lambda<Func<object?, object?>>(Expression.Convert(cast, typeof(object)), input).Compile();

            foreach (BigInteger? value in BoundaryValues(source, target, conversion.NullableSource))
            {
                string label = $"{inputType} -> {outputType}, checked={conversion.Checked}, value={value}, {optimization}";
                object? argument = value is null ? null : source == typeof(char)
                    ? (char)(ushort)value.Value
                    : Convert.ChangeType(value.Value.ToString(CultureInfo.InvariantCulture), source, CultureInfo.InvariantCulture);
                object? expected;
                try
                {
                    expected = oracle(argument);
                }
                catch (Exception ex) when (ex is OverflowException or InvalidOperationException)
                {
                    var fault = Assert.ThrowsExactly<TestException>(() => conversion.Invoke(value), label);
                    Assert.IsInstanceOfType<VMUnhandledException>(fault.InnerException, label);
                    if (ex is OverflowException)
                        Assert.AreEqual(value, ((VMUnhandledException)fault.InnerException).ExceptionObject.GetInteger(), label);
                    else
                        Assert.IsTrue(((VMUnhandledException)fault.InnerException).ExceptionObject.IsNull, label);
                    continue;
                }

                BigInteger? expectedInteger = expected is null ? null : expected is char character
                    ? new BigInteger(character)
                    : BigInteger.Parse(Convert.ToString(expected, CultureInfo.InvariantCulture)!, CultureInfo.InvariantCulture);
                Assert.AreEqual(expectedInteger, conversion.Invoke(value), label);
            }
        }
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullableOperandIsEvaluatedOnce(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                private static int _calls;
                private static int? Evaluate(int? value) { _calls++; return value; }

                public static int CastOnce(int? value, bool check)
                {
                    _calls = 0;
                    byte? result = check ? checked((byte?)Evaluate(value)) : unchecked((byte?)Evaluate(value));
                    return _calls * 1000 + (result ?? 0);
                }
            }
            """, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<NumericCastContract>(nef, manifest);
        foreach (int? value in new int?[] { null, 0, 42, 255 })
        {
            Assert.AreEqual(new BigInteger(1000 + (value ?? 0)), contract.CastOnce(value, false));
            Assert.AreEqual(new BigInteger(1000 + (value ?? 0)), contract.CastOnce(value, true));
        }
        Assert.AreEqual(new BigInteger(1000), contract.CastOnce(256, false));
        Assert.AreEqual(new BigInteger(1255), contract.CastOnce(-1, false));
    }

    private static IEnumerable<BigInteger?> BoundaryValues(Type source, Type target, bool nullable)
    {
        if (nullable) yield return null;
        var (sourceMin, sourceMax) = Range(source);
        var (targetMin, targetMax) = Range(target);
        BigInteger[] values = [sourceMin, sourceMax, -1, 0, 1, targetMin - 1, targetMin, targetMax, targetMax + 1];
        foreach (var value in values.Distinct().Where(value => value >= sourceMin && value <= sourceMax))
            yield return value;
    }

    private static (BigInteger Min, BigInteger Max) Range(Type type)
    {
        BigInteger Read(string field) => type == typeof(char)
            ? new BigInteger((char)type.GetField(field)!.GetValue(null)!)
            : BigInteger.Parse(Convert.ToString(type.GetField(field)!.GetValue(null), CultureInfo.InvariantCulture)!, CultureInfo.InvariantCulture);
        return (Read("MinValue"), Read("MaxValue"));
    }

    public abstract class NumericCastContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("checkedNullable")] public abstract BigInteger? CheckedNullable(BigInteger? value);
        [DisplayName("uncheckedNullable")] public abstract BigInteger? UncheckedNullable(BigInteger? value);
        [DisplayName("checkedValue")] public abstract BigInteger? CheckedValue(BigInteger? value);
        [DisplayName("uncheckedValue")] public abstract BigInteger? UncheckedValue(BigInteger? value);
        [DisplayName("checkedUnwrap")] public abstract BigInteger? CheckedUnwrap(BigInteger? value);
        [DisplayName("uncheckedUnwrap")] public abstract BigInteger? UncheckedUnwrap(BigInteger? value);
        [DisplayName("castOnce")] public abstract BigInteger? CastOnce(BigInteger? value, bool check);
    }
}
