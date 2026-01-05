// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Record.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    record Student(string name, int age);

    public record StudentR
    {
        public string Name { get; set; }
        public int Age { get; init; }

        public StudentR(string n)
        {
            Name = n;
        }
    }

    public record StudentWithExtras(string Name)
    {
        public int EnrollmentYear { get; init; } = 2025;
        public string? Tag { get; set; }
    }

    public record Customer(string Name, int Tier)
    {
        public string Identifier => $"{Name}:{Tier}";
    }

    public record PremiumCustomer(string Name, int Tier, string Level) : Customer(Name, Tier)
    {
        public int Score { get; init; }
    }

    public record struct Position(int X, int Y)
    {
        public int Z { get; init; }
    }

    public class Contract_Record : SmartContract.Framework.SmartContract
    {
        public static object Test_CreateRecord(string n, int a)
        {
            var p = new Student(n, a);
            return p;
        }

        public static object Test_CreateRecord2(string n, int a)
        {
            var p = new StudentR(n) { Age = a };
            return p;
        }

        public static object Test_UpdateRecord(string n, int a)
        {
            var p = new Student(n, a);
            var p2 = p with { age = a + 1 };
            return p;
        }

        public static object Test_UpdateRecord2(string n, int a)
        {
            var p = new Student(n, a);
            var p2 = p with { age = a + 1, name = "0" + n };
            return p2;
        }

        public static string Test_DeconstructRecord(string n, int a)
        {
            var p = new Student(n, a);
            var (name, age) = p;
            return name;
        }

        public static object Test_CreateRecordWithExtras(string name, string? tag)
        {
            var record = new StudentWithExtras(name) { Tag = tag };
            return record;
        }

        public static object Test_WithRecordExtras(string name, string tag, int yearIncrement)
        {
            var record = new StudentWithExtras(name) { Tag = tag };
            var updated = record with
            {
                Tag = tag + ":updated",
                EnrollmentYear = record.EnrollmentYear + yearIncrement
            };
            return updated;
        }

        public static object Test_RecordStructWith(int x, int y, int z, int deltaY)
        {
            var position = new Position(x, y) { Z = z };
            var updated = position with { Y = y + deltaY };
            return updated;
        }

        public static object Test_DerivedRecordWith(string name, int tier, string level, int bonus)
        {
            var customer = new PremiumCustomer(name, tier, level);
            var updated = customer with
            {
                Tier = tier + bonus,
                Level = level + "-VIP",
                Score = (tier + bonus) * 10
            };
            return updated;
        }

        public static bool Test_RecordEquality(string name, int age)
        {
            var a = new Student(name, age);
            var b = new Student(name, age);
            return a == b;
        }

        public static bool Test_RecordStructIsolation(int x, int y, int z)
        {
            var original = new Position(x, y) { Z = z };
            var clone = original with { Y = y + 10 };
            return original.Y == y && clone.Y == y + 10 && original.Z == clone.Z;
        }
    }
}
