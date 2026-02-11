// Copyright (C) 2015-2026 The Neo Project.
//
// InitialValueAnalyzerUnitTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.InitialValueAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class InitialValueAnalyzerUnitTest
    {
        [TestMethod]
        public async Task FieldWithoutAttribute_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private static readonly string _address = ""hello"";
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FieldWithRegularInitializer_ShouldNotReportDiagnostic()
        {
            var test = @"
class TestClass
{
    private static readonly int _value = 42;
    private static readonly string _name = ""test"";
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FieldWithNonTargetAttribute_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

[AttributeUsage(AttributeTargets.Field)]
class MyCustomAttribute : Attribute
{
    public MyCustomAttribute(string value) { }
}

class TestClass
{
    [MyCustom(""some_value"")]
    private static readonly string _address = default!;
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task FieldWithInitialValueAttributeAndNonDefaultInit_ShouldNotReportDiagnostic()
        {
            var test = @"
using System;

[AttributeUsage(AttributeTargets.Field)]
class InitialValueAttribute : Attribute
{
    public InitialValueAttribute(string value) { }
}

class TestClass
{
    [InitialValue(""some_value"")]
    private static readonly string _address = ""actual_value"";
}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
