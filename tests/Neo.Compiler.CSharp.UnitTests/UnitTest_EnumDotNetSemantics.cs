// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_EnumDotNetSemantics.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_EnumDotNetSemantics
    {
        public abstract class EnumDotNetSemanticsContract(SmartContractInitialize initialize)
            : Neo.SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("values")] public abstract IList<object>? Values();
            [DisplayName("names")] public abstract IList<object>? Names();
            [DisplayName("genericValues")] public abstract IList<object>? GenericValues();
            [DisplayName("genericNames")] public abstract IList<object>? GenericNames();
            [DisplayName("negativeValues")] public abstract IList<object>? NegativeValues();
            [DisplayName("wideValues")] public abstract IList<object>? WideValues();
            [DisplayName("parse")] public abstract BigInteger? Parse(string value);
            [DisplayName("parseGeneric")] public abstract BigInteger? ParseGeneric(string value);
            [DisplayName("parseFlags")] public abstract BigInteger? ParseFlags(string value);
            [DisplayName("tryParse")] public abstract BigInteger? TryParse(string value);
            [DisplayName("tryParseGeneric")] public abstract BigInteger? TryParseGeneric(string value);
            [DisplayName("parseIgnoreCase")] public abstract BigInteger? ParseIgnoreCase(string value, bool ignoreCase);
            [DisplayName("tryParseIgnoreCase")] public abstract BigInteger? TryParseIgnoreCase(string value, bool ignoreCase);
            [DisplayName("tryParseGenericIgnoreCase")] public abstract BigInteger? TryParseGenericIgnoreCase(string value, bool ignoreCase);
        }

        [TestMethod]
        public void TestEnumGetValuesAndParseMatchDotNet()
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using System;
                using System.ComponentModel;

                public class Contract : SmartContract
                {
                    public enum Order { B = 2, A = 1, C = 3 }
                    public enum Negative { NegOne = -1, Zero = 0, One = 1 }
                    public enum Wide : ulong { Huge = ulong.MaxValue, Small = 1 }
                    [Flags] public enum Perms { Read = 1, Write = 2 }

                    [DisplayName("values")] public static Order[] Values() => (Order[])Enum.GetValues(typeof(Order));
                    [DisplayName("names")] public static string[] Names() => Enum.GetNames(typeof(Order));
                    [DisplayName("genericValues")] public static Order[] GenericValues() => Enum.GetValues<Order>();
                    [DisplayName("genericNames")] public static string[] GenericNames() => Enum.GetNames<Order>();
                    [DisplayName("negativeValues")] public static Negative[] NegativeValues() => Enum.GetValues<Negative>();
                    [DisplayName("wideValues")] public static Wide[] WideValues() => Enum.GetValues<Wide>();
                    [DisplayName("parse")] public static int Parse(string value) => (int)Enum.Parse(typeof(Order), value);
                    [DisplayName("parseGeneric")] public static int ParseGeneric(string value) => (int)Enum.Parse<Order>(value);
                    [DisplayName("parseFlags")] public static int ParseFlags(string value) => (int)Enum.Parse<Perms>(value);
                    [DisplayName("tryParse")] public static int TryParse(string value) => Enum.TryParse(typeof(Order), value, out object result) ? (int)result : -1;
                    [DisplayName("tryParseGeneric")] public static int TryParseGeneric(string value) => Enum.TryParse<Order>(value, out var result) ? (int)result : -1;
                    [DisplayName("parseIgnoreCase")] public static int ParseIgnoreCase(string value, bool ignoreCase) => (int)Enum.Parse(typeof(Order), value, ignoreCase);
                    [DisplayName("tryParseIgnoreCase")] public static int TryParseIgnoreCase(string value, bool ignoreCase) => Enum.TryParse(typeof(Order), value, ignoreCase, out object result) ? (int)result : -1;
                    [DisplayName("tryParseGenericIgnoreCase")] public static int TryParseGenericIgnoreCase(string value, bool ignoreCase) => Enum.TryParse<Order>(value, ignoreCase, out var result) ? (int)result : -1;
                }
                """;

            var context = TestHelper.CompileSingleContract(source);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
            var contract = new TestEngine(true).Deploy<EnumDotNetSemanticsContract>(context.CreateExecutable(), context.CreateManifest());

            CollectionAssert.AreEqual(new BigInteger[] { 1, 2, 3 }, contract.Values()!.Select(v => (BigInteger)v).ToArray());
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, contract.Names()!.Select(v => ((ByteString)v).GetString()).ToArray());
            CollectionAssert.AreEqual(new BigInteger[] { 1, 2, 3 }, contract.GenericValues()!.Select(v => (BigInteger)v).ToArray());
            CollectionAssert.AreEqual(new[] { "A", "B", "C" }, contract.GenericNames()!.Select(v => ((ByteString)v).GetString()).ToArray());
            CollectionAssert.AreEqual(new BigInteger[] { 0, 1, -1 }, contract.NegativeValues()!.Select(v => (BigInteger)v).ToArray());
            CollectionAssert.AreEqual(new BigInteger[] { 1, ulong.MaxValue }, contract.WideValues()!.Select(v => (BigInteger)v).ToArray());
            Assert.AreEqual(new BigInteger(1), contract.Parse(" A "));
            Assert.AreEqual(new BigInteger(1), contract.ParseGeneric("1"));
            Assert.AreEqual(new BigInteger(3), contract.ParseFlags("Read, Write"));
            Assert.AreEqual(new BigInteger(1), contract.TryParse("1"));
            Assert.AreEqual(new BigInteger(1), contract.TryParseGeneric(" A "));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseGeneric("unknown"));
            Assert.AreEqual(new BigInteger(2), contract.ParseIgnoreCase(" b ", true));
            Assert.AreEqual(new BigInteger(2), contract.TryParseIgnoreCase(" b ", true));
            Assert.AreEqual(new BigInteger(2), contract.TryParseGenericIgnoreCase(" b ", true));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseIgnoreCase(" b ", false));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseGeneric("2147483648"));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseGeneric("A,unknown"));

            // atoi can produce a 42-byte integer for this input, which faults the VM
            // before the enum's backing-type range check unless it is rejected first.
            var oversized = new string((char)57, 100);
            Assert.AreEqual(new BigInteger(-1), contract.TryParse(oversized));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseIgnoreCase(oversized, true));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseGeneric(oversized));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseGenericIgnoreCase(oversized, true));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseGeneric(new string('9', 21)));
            Assert.AreEqual(new BigInteger(1), contract.TryParseGeneric(new string('0', 100) + "1"));
            Assert.AreEqual(new BigInteger(int.MaxValue), contract.TryParseGeneric(int.MaxValue.ToString()));
            Assert.AreEqual(new BigInteger(-1), contract.TryParseGeneric("-2147483649"));
        }
    }
}
