// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Attribute.cs file belongs to the neo project and is free
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
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Attribute : DebugAndTestBase<Contract_AttributeChanged>
    {
        [TestMethod]
        public void SingleParameterInitialValueInfersFieldType()
        {
            var context = TestCleanup.TestInitialize(typeof(Contract_InitialValue));
            Assert.IsNotNull(context);
            TestSingleContractBasicBlockStartEnd(context);
            var engine = new TestEngine(true);
            var contract = engine.Deploy<Contract_InitialValue>(context.CreateExecutable(), context.CreateManifest());

            Assert.AreEqual(new BigInteger(42), contract.GetInferredInteger());
            Assert.AreEqual("DefaultName", contract.GetDefaultName());
            CollectionAssert.AreEqual(new byte[] { 0x00, 0x11, 0x22, 0x33 }, contract.GetInferredBytes());
        }

        [TestMethod]
        public void ExplicitInitialValueTypesArePreserved()
        {
            var context = TestCleanup.TestInitialize(typeof(Contract_InitialValue));
            Assert.IsNotNull(context);
            var engine = new TestEngine(true);
            var contract = engine.Deploy<Contract_InitialValue>(context.CreateExecutable(), context.CreateManifest());

            Assert.AreEqual(new BigInteger(18), contract.GetDefaultAge());
            Assert.AreEqual(UInt160.Parse("0x71a87191aef3fcf5e4441d791ded67ebab1aee7e"), contract.GetDefaultOwner());
        }

        [TestMethod]
        public void UnsupportedInferredInitialValueTypeReportsDiagnostic()
        {
            var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
public class Contract : SmartContract
{
    [InitialValue("true")] private static readonly bool Value;
    public static bool Get() => Value;
}
""");
            var diagnostics = string.Join(Environment.NewLine, context.Diagnostics);
            Assert.IsFalse(context.Success, diagnostics);
            Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidInitialValueType
                && d.GetMessage().Contains("Boolean")), diagnostics);
            Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
        }

        [TestMethod]
        public void AttributeTest()
        {
            Assert.AreEqual(Contract_AttributeChanged.Manifest.Name, "Contract_AttributeChanged");
            Assert.IsTrue(Contract.Test());
            AssertGasConsumed(984060);
        }
    }
}
