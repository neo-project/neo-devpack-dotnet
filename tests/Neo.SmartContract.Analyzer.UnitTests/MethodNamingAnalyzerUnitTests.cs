// Copyright (C) 2015-2026 The Neo Project.
//
// MethodNamingAnalyzerUnitTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.MSTest.AnalyzerVerifier<Neo.SmartContract.Analyzer.SmartContractMethodNamingAnalyzer>;

namespace Neo.SmartContract.Analyzer.Test
{
    [TestClass]
    public class MethodNamingAnalyzerUnitTests
    {
        [TestMethod]
        public async Task PartialContract_ReportsEachConflictOnce()
        {
            const string source = """
                namespace Neo.SmartContract.Framework { public class SmartContract { } }
                public partial class Contract : Neo.SmartContract.Framework.SmartContract
                {
                    {|#0:public static void Transfer(int value) { }|}
                }
                public partial class Contract
                {
                    {|#1:public static void Transfer(string value) { }|}
                }
                """;
            await VerifyCS.VerifyAnalyzerAsync(source,
                VerifyCS.Diagnostic().WithLocation(0).WithArguments("Transfer"),
                VerifyCS.Diagnostic().WithLocation(1).WithArguments("Transfer"));
        }

        [TestMethod]
        [DataRow("public static int Read() => 1;", "public static int read() => 2;", "Read", "read")]
        [DataRow("[System.ComponentModel.DisplayName(\"value\")] public static int First() => 1;", "[System.ComponentModel.DisplayName(\"value\")] public static int Second() => 2;", "First", "Second")]
        [DataRow("public static int _Read() => 1;", "public static int _read() => 2;", "_Read", "_read")]
        public async Task ExportedNamesCollide_ReportsBothMethods(string first, string second, string firstName, string secondName)
        {
            var source = $$"""
                namespace Neo.SmartContract.Framework { public class SmartContract { } }
                public class Contract : Neo.SmartContract.Framework.SmartContract
                {
                    {|#0:{{first}}|}
                    {|#1:{{second}}|}
                }
                """;
            await VerifyCS.VerifyAnalyzerAsync(source,
                VerifyCS.Diagnostic().WithLocation(0).WithArguments(firstName),
                VerifyCS.Diagnostic().WithLocation(1).WithArguments(secondName));
        }

        [TestMethod]
        [DataRow("public static int Value", "get_Value")]
        [DataRow("[System.ComponentModel.DisplayName(\"value\")] public static int Item", "get_Item")]
        public async Task PropertyGetterNameCollides_WithMethod(string declaration, string getterName)
        {
            var source = $$"""
                namespace Neo.SmartContract.Framework { public class SmartContract { } }
                public class Contract : Neo.SmartContract.Framework.SmartContract
                {
                    {{declaration}} {|#0:=> 1|};
                    {|#1:public static int value() => 2;|}
                }
                """;
            await VerifyCS.VerifyAnalyzerAsync(source,
                VerifyCS.Diagnostic().WithLocation(0).WithArguments(getterName),
                VerifyCS.Diagnostic().WithLocation(1).WithArguments("value"));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("[System.ComponentModel.DisplayName(\"renamed\")]")]
        public async Task PropertySetterNameCollides_WithMethod(string attribute)
        {
            var source = $$"""
                namespace Neo.SmartContract.Framework { public class SmartContract { } }
                public class Contract : Neo.SmartContract.Framework.SmartContract
                {
                    {{attribute}}
                    public static int Value { get => 1; {|#0:set { }|} }
                    {|#1:public static void setValue(int value) { }|}
                }
                """;
            await VerifyCS.VerifyAnalyzerAsync(source,
                VerifyCS.Diagnostic().WithLocation(0).WithArguments("set_Value"),
                VerifyCS.Diagnostic().WithLocation(1).WithArguments("setValue"));
        }

        [TestMethod]
        public async Task AbstractBaseAbiNames_CanBeDisambiguatedByAConcreteContract()
        {
            const string source = """
                namespace Neo.SmartContract.Framework { public class SmartContract { } }
                public abstract class BaseContract : Neo.SmartContract.Framework.SmartContract
                {
                    public static int Read() => 1;
                    public static int read() => 2;
                }
                public class Contract : BaseContract
                {
                    [System.ComponentModel.DisplayName("other")]
                    public new static int Read() => 3;
                }
                """;
            await VerifyCS.VerifyAnalyzerAsync(source);
        }

        [TestMethod]
        public async Task DistinctExportNamesAndArities_DoNotConflict()
        {
            const string source = """
                namespace Neo.SmartContract.Framework { public class SmartContract { } }
                public partial class Contract : Neo.SmartContract.Framework.SmartContract
                {
                    [System.ComponentModel.DisplayName("first")] public static int Read() => 1;
                    public static int read() => 2;
                    public static int Read(int value) => value;
                    private static int read(int value) => value;
                }
                public partial class Contract { }
                """;
            await VerifyCS.VerifyAnalyzerAsync(source);
        }

        [TestMethod]
        public async Task MethodsWithSameNameAndParamCount_ShouldReportDiagnostic()
        {
            var test = """
                       namespace Neo.SmartContract.Framework
                       {
                           public class SmartContract { }
                       }

                       public class TestContract : Neo.SmartContract.Framework.SmartContract
                       {
                           {|#0:public void Transfer(int value) { }|}
                           {|#1:public void Transfer(string value) { }|}
                       }
                       """;

            var expected = new[]
            {
                VerifyCS.Diagnostic(SmartContractMethodNamingAnalyzer.DiagnosticId).WithLocation(0).WithArguments("Transfer"),
                VerifyCS.Diagnostic(SmartContractMethodNamingAnalyzer.DiagnosticId).WithLocation(1).WithArguments("Transfer"),
            };

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task MethodsWithSameNameButDifferentParamCount_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Transfer(byte[] to) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodsWithDifferentNames_ShouldNotReportDiagnostic()
        {
            var test = """
                       using System.Numerics;
                       public class TestContract
                       {
                           public void Transfer(byte[] from, byte[] to, BigInteger amount) { }
                           public void Withdraw(byte[] to, BigInteger amount) { }
                       }
                       """;

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public void MethodNamingDiagnostics_ShouldNotOfferAutomaticCodeFixes()
        {
            var fixes = typeof(SmartContractMethodNamingAnalyzer).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(CodeFixProvider).IsAssignableFrom(type))
                .Select(type => (CodeFixProvider)Activator.CreateInstance(type)!)
                .SelectMany(provider => provider.FixableDiagnosticIds
                    .Where(id => id is SmartContractMethodNamingAnalyzer.DiagnosticId or SmartContractMethodNamingAnalyzerUnderline.DiagnosticId)
                    .Select(id => $"{provider.GetType().Name}: {id}"))
                .OrderBy(fix => fix, StringComparer.Ordinal)
                .ToArray();

            Assert.AreEqual(0, fixes.Length, string.Join(Environment.NewLine, fixes));
        }
    }
}
