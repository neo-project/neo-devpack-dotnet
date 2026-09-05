// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_OverrideDispatch.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_OverrideDispatch
{
    private const string Source = """
        using Neo.SmartContract.Framework;

        public class Contract : SmartContract
        {
            private class Base
            {
                public virtual int Evaluate() => 5;
            }

            private class Derived : Base
            {
                public override int Evaluate() => 12;
            }

            private sealed class SealedDerived : Base
            {
                public sealed override int Evaluate() => 19;
            }

            private class Middle : Base
            {
                public override int Evaluate() => 23;
            }

            private class Leaf : Middle
            {
                public override int Evaluate() => 31;
            }

            private class Inherited : Middle { }

            private class CallsBase : Base
            {
                public override int Evaluate() => base.Evaluate() + 40;
            }

            private class FieldBase
            {
                public int Seed = 7;
                public virtual int Evaluate() => Seed;
            }

            private class FieldDerived : FieldBase
            {
                public override int Evaluate() => Seed * 2;
            }

            private class ExtraFieldDerived : FieldBase
            {
                public int Offset = 11;
                public override int Evaluate() => Seed + Offset;
            }

            private class PropertyBase
            {
                public virtual int Value => 5;
            }

            private class PropertyDerived : PropertyBase
            {
                public override int Value => base.Value + 40;
            }

            private record RecordBase(int Value)
            {
                public virtual int Evaluate() => Value;
            }

            private record RecordDerived(int Value) : RecordBase(Value)
            {
                public override int Evaluate() => Value * 2;
            }

            private struct ValueType
            {
                public int Value;
                public override int GetHashCode() => Value * 2;
            }

            public static int DirectRecordOverrideCall() => new RecordDerived(7).Evaluate();

            public static int DirectStructOverrideCall() => new ValueType { Value = 7 }.GetHashCode();

            public static object StructLayout() => new ValueType { Value = 7 };

            public static int WithDerivedFields()
            {
                FieldBase receiver = new ExtraFieldDerived();
                return receiver.Evaluate();
            }

            public static int WithDerivedFieldInitializer()
            {
                FieldBase receiver = new ExtraFieldDerived { Seed = 9, Offset = 13 };
                return receiver.Evaluate();
            }

            public static int OverrideProperty()
            {
                PropertyBase receiver = new PropertyDerived();
                return receiver.Value;
            }

            public static int NoFields()
            {
                Base receiver = new Derived();
                return receiver.Evaluate();
            }

            public static int SealedOverride()
            {
                Base receiver = new SealedDerived();
                return receiver.Evaluate();
            }

            public static int MultipleLevels()
            {
                Base receiver = new Leaf();
                return receiver.Evaluate();
            }

            public static int InheritedOverride()
            {
                Base receiver = new Inherited();
                return receiver.Evaluate();
            }

            public static int ExplicitBaseCall()
            {
                Base receiver = new CallsBase();
                return receiver.Evaluate();
            }

            public static int WithInheritedFields()
            {
                FieldBase receiver = new FieldDerived();
                return receiver.Evaluate();
            }

            public static int WithObjectInitializer()
            {
                FieldBase receiver = new FieldDerived { Seed = 9 };
                return receiver.Evaluate();
            }

            public static int DirectOverrideCall() => new Derived().Evaluate();
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, "NoFields", 12)]
    [DataRow(CompilationOptions.OptimizationType.All, "NoFields", 12)]
    [DataRow(CompilationOptions.OptimizationType.None, "SealedOverride", 19)]
    [DataRow(CompilationOptions.OptimizationType.All, "SealedOverride", 19)]
    [DataRow(CompilationOptions.OptimizationType.None, "MultipleLevels", 31)]
    [DataRow(CompilationOptions.OptimizationType.All, "MultipleLevels", 31)]
    [DataRow(CompilationOptions.OptimizationType.None, "InheritedOverride", 23)]
    [DataRow(CompilationOptions.OptimizationType.All, "InheritedOverride", 23)]
    [DataRow(CompilationOptions.OptimizationType.None, "ExplicitBaseCall", 45)]
    [DataRow(CompilationOptions.OptimizationType.All, "ExplicitBaseCall", 45)]
    [DataRow(CompilationOptions.OptimizationType.None, "WithInheritedFields", 14)]
    [DataRow(CompilationOptions.OptimizationType.All, "WithInheritedFields", 14)]
    [DataRow(CompilationOptions.OptimizationType.None, "WithObjectInitializer", 18)]
    [DataRow(CompilationOptions.OptimizationType.All, "WithObjectInitializer", 18)]
    [DataRow(CompilationOptions.OptimizationType.None, "DirectOverrideCall", 12)]
    [DataRow(CompilationOptions.OptimizationType.All, "DirectOverrideCall", 12)]
    [DataRow(CompilationOptions.OptimizationType.None, "WithDerivedFields", 18)]
    [DataRow(CompilationOptions.OptimizationType.All, "WithDerivedFields", 18)]
    [DataRow(CompilationOptions.OptimizationType.None, "WithDerivedFieldInitializer", 22)]
    [DataRow(CompilationOptions.OptimizationType.All, "WithDerivedFieldInitializer", 22)]
    [DataRow(CompilationOptions.OptimizationType.None, "OverrideProperty", 45)]
    [DataRow(CompilationOptions.OptimizationType.All, "OverrideProperty", 45)]
    [DataRow(CompilationOptions.OptimizationType.None, "DirectRecordOverrideCall", 14)]
    [DataRow(CompilationOptions.OptimizationType.All, "DirectRecordOverrideCall", 14)]
    [DataRow(CompilationOptions.OptimizationType.None, "DirectStructOverrideCall", 14)]
    [DataRow(CompilationOptions.OptimizationType.All, "DirectStructOverrideCall", 14)]
    public void OverridesRemainInVirtualTables(CompilationOptions.OptimizationType optimization, string method, int expected)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var contract = new TestEngine(true).Deploy<OverrideContract>(nef, manifest);
        BigInteger? actual = method switch
        {
            "NoFields" => contract.NoFields(),
            "SealedOverride" => contract.SealedOverride(),
            "MultipleLevels" => contract.MultipleLevels(),
            "InheritedOverride" => contract.InheritedOverride(),
            "ExplicitBaseCall" => contract.ExplicitBaseCall(),
            "WithInheritedFields" => contract.WithInheritedFields(),
            "WithObjectInitializer" => contract.WithObjectInitializer(),
            "DirectOverrideCall" => contract.DirectOverrideCall(),
            "WithDerivedFields" => contract.WithDerivedFields(),
            "WithDerivedFieldInitializer" => contract.WithDerivedFieldInitializer(),
            "OverrideProperty" => contract.OverrideProperty(),
            "DirectRecordOverrideCall" => contract.DirectRecordOverrideCall(),
            "DirectStructOverrideCall" => contract.DirectStructOverrideCall(),
            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };
        Assert.AreEqual(new BigInteger(expected), actual);
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void StructOverridesPreserveFieldLayout(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var contract = new TestEngine(true).Deploy<OverrideContract>(nef, manifest);
        var value = contract.StructLayout();
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.Count);
        Assert.AreEqual(new BigInteger(7), value[0]);
    }

    public abstract class OverrideContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("noFields")]
        public abstract BigInteger? NoFields();
        [DisplayName("sealedOverride")]
        public abstract BigInteger? SealedOverride();
        [DisplayName("multipleLevels")]
        public abstract BigInteger? MultipleLevels();
        [DisplayName("inheritedOverride")]
        public abstract BigInteger? InheritedOverride();
        [DisplayName("explicitBaseCall")]
        public abstract BigInteger? ExplicitBaseCall();
        [DisplayName("withInheritedFields")]
        public abstract BigInteger? WithInheritedFields();
        [DisplayName("withObjectInitializer")]
        public abstract BigInteger? WithObjectInitializer();
        [DisplayName("directOverrideCall")]
        public abstract BigInteger? DirectOverrideCall();
        [DisplayName("withDerivedFields")]
        public abstract BigInteger? WithDerivedFields();
        [DisplayName("withDerivedFieldInitializer")]
        public abstract BigInteger? WithDerivedFieldInitializer();
        [DisplayName("overrideProperty")]
        public abstract BigInteger? OverrideProperty();
        [DisplayName("directRecordOverrideCall")]
        public abstract BigInteger? DirectRecordOverrideCall();
        [DisplayName("directStructOverrideCall")]
        public abstract BigInteger? DirectStructOverrideCall();
        [DisplayName("structLayout")]
        public abstract IList<object>? StructLayout();
    }
}
