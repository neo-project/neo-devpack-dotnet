// Copyright (C) 2015-2025 The Neo Project.
//
// ReturnValueTypeAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<
    Neo.SmartContract.Analyzer.ReturnValueTypeAnalyzer>;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class ReturnValueTypeAnalyzerUnitTests
{
    private const string FrameworkStub = """
namespace Neo.SmartContract.Framework
{
    public abstract class SmartContract { }
    public abstract class UInt160
    {
        public static extern implicit operator UInt160(string value);
        public static extern explicit operator UInt160(byte[] value);
        public static UInt160 Parse(string value) => throw null!;
    }
    public abstract class UInt256
    {
        public static extern implicit operator UInt256(string value);
        public static UInt256 Parse(string value) => throw null!;
    }
    public abstract class ECPoint
    {
        public static extern implicit operator ECPoint(string value);
        public static ECPoint Parse(string value) => throw null!;
    }
}
""";

    private static string WithFrameworkStub(string source) =>
        "#line 1\n" + source + "\n" + FrameworkStub;

    [TestMethod]
    public async Task ReturnStringFromUInt160Method_ReportsDiagnostic()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt160 Owner()
    {
        return "0xd2a4cff31913016155e38e474a2c06d08be276cf";
    }
}
""");

        var expected = VerifyCS.Diagnostic(ReturnValueTypeAnalyzer.DiagnosticId)
            .WithSpan(8, 16, 8, 60)
            .WithArguments("UInt160", "string");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task ExpressionBodiedMethodReturningString_ReportsDiagnostic()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt160 Owner() => "0xd2a4cff31913016155e38e474a2c06d08be276cf";
}
""");

        var expected = VerifyCS.Diagnostic(ReturnValueTypeAnalyzer.DiagnosticId)
            .WithSpan(6, 38, 6, 82)
            .WithArguments("UInt160", "string");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task ReturningStringVariable_ReportsDiagnostic()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt160 Owner()
    {
        string hash = "0xd2a4cff31913016155e38e474a2c06d08be276cf";
        return hash;
    }
}
""");

        var expected = VerifyCS.Diagnostic(ReturnValueTypeAnalyzer.DiagnosticId)
            .WithSpan(9, 16, 9, 20)
            .WithArguments("UInt160", "string");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task UInt256ReturnWithString_ReportsDiagnostic()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt256 Hash() => "0xedcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";
}
""");

        var expected = VerifyCS.Diagnostic(ReturnValueTypeAnalyzer.DiagnosticId)
            .WithSpan(6, 37, 6, 105)
            .WithArguments("UInt256", "string");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task ExplicitCast_AllowsReturn()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt160 Owner()
    {
        return (UInt160)"0xd2a4cff31913016155e38e474a2c06d08be276cf";
    }
}
""");

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task UInt160Parse_AllowsReturn()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt160 Owner(string input)
    {
        return UInt160.Parse(input);
    }
}
""");

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task UInt256Parse_AllowsReturn()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt256 Hash(string input)
    {
        return UInt256.Parse(input);
    }
}
""");

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task ECPointReturnWithString_ReportsDiagnostic()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static ECPoint PubKey() => "02728d7f58f62ae087f0f1c0da31f3380d5ce24df854132d668163ad7e9d2d8d9b";
}
""");

        var expected = VerifyCS.Diagnostic(ReturnValueTypeAnalyzer.DiagnosticId)
            .WithSpan(6, 39, 6, 107)
            .WithArguments("ECPoint", "string");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }

    [TestMethod]
    public async Task ExplicitConversionFromByteArray_AllowsReturn()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt160 Owner()
    {
        var bytes = new byte[20];
        return (UInt160)bytes;
    }
}
""");

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task ECPointParse_AllowsReturn()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static ECPoint PubKey(string input)
    {
        return ECPoint.Parse(input);
    }
}
""");

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [TestMethod]
    public async Task NullReturn_DoesNotReportDiagnostic()
    {
        var code = WithFrameworkStub("""
using Neo.SmartContract.Framework;

class Contract : SmartContract
{
    public static UInt160 Owner()
    {
        return null;
    }
}
""");

        await VerifyCS.VerifyAnalyzerAsync(code);
    }
}
