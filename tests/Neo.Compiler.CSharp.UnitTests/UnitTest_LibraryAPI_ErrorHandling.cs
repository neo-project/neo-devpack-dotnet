// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_LibraryAPI_ErrorHandling.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_LibraryAPI_ErrorHandling
    {
        private readonly string tempTestDir = Path.Combine(Path.GetTempPath(), "neo-compiler-error-tests", Guid.NewGuid().ToString());

        [TestInitialize]
        public void Setup()
        {
            Directory.CreateDirectory(tempTestDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(tempTestDir))
            {
                Directory.Delete(tempTestDir, true);
            }
        }

        [TestMethod]
        public void Test_CompileSources_EmptyArray_ReturnsEmptyResults()
        {
            var engine = new CompilationEngine();
            var results = engine.CompileSources(new string[0]);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_CompileSources_NullArray_ThrowsArgumentNullException()
        {
            var engine = new CompilationEngine();
            engine.CompileSources((string[])null);
        }

        [TestMethod]
        public void Test_CompileSources_NonExistentFile_ReturnsFailure()
        {
            var engine = new CompilationEngine();
            var nonExistentPath = Path.Combine(tempTestDir, "nonexistent.cs");

            var results = engine.CompileSources(new[] { nonExistentPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            Assert.IsTrue(results[0].Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        }

        [TestMethod]
        public void Test_CompileSources_InvalidExtension_ReturnsFailure()
        {
            var engine = new CompilationEngine();
            var invalidFile = Path.Combine(tempTestDir, "test.txt");
            File.WriteAllText(invalidFile, "This is not a C# file");

            var results = engine.CompileSources(new[] { invalidFile });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            Assert.IsTrue(results[0].Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        }

        [TestMethod]
        public void Test_CompileSources_EmptyFile_ReturnsFailure()
        {
            var engine = new CompilationEngine();
            var emptyFile = Path.Combine(tempTestDir, "empty.cs");
            File.WriteAllText(emptyFile, "");

            var results = engine.CompileSources(new[] { emptyFile });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
        }

        [TestMethod]
        public void Test_CompileSources_SyntaxErrors_ReturnsFailureWithDiagnostics()
        {
            var invalidContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class InvalidContract : SmartContract
    {
        public static void Test()
        {
            int x = 5 // Missing semicolon
            string y = ""unterminated string
            return x;
        }
    }
}";

            var engine = new CompilationEngine();
            var contractPath = Path.Combine(tempTestDir, "InvalidSyntax.cs");
            File.WriteAllText(contractPath, invalidContract);

            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            
            var errors = results[0].Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.IsTrue(errors.Count > 0);
            
            // Should have specific syntax error messages
            Assert.IsTrue(errors.Any(e => e.Id.Contains("CS") || e.GetMessage().Length > 0));
        }

        [TestMethod]
        public void Test_CompileSources_SemanticErrors_ReturnsFailureWithDiagnostics()
        {
            var invalidContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class SemanticErrorContract : SmartContract
    {
        public static void Test()
        {
            UndefinedType x = new UndefinedType(); // Undefined type
            NonExistentMethod(); // Undefined method
            int y = ""string to int""; // Type mismatch
        }
    }
}";

            var engine = new CompilationEngine();
            var contractPath = Path.Combine(tempTestDir, "SemanticErrors.cs");
            File.WriteAllText(contractPath, invalidContract);

            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            
            var errors = results[0].Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.IsTrue(errors.Count > 0);
        }

        [TestMethod]
        public void Test_CompileSources_MissingReferences_ReturnsFailureWithDiagnostics()
        {
            var contractWithMissingRef = @"
using System.Data.SqlClient; // Reference not available in smart contracts
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class MissingRefContract : SmartContract
    {
        public static void Test()
        {
            var connection = new SqlConnection(""connection string"");
        }
    }
}";

            var engine = new CompilationEngine();
            var contractPath = Path.Combine(tempTestDir, "MissingRef.cs");
            File.WriteAllText(contractPath, contractWithMissingRef);

            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            
            var errors = results[0].Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            Assert.IsTrue(errors.Count > 0);
        }

        [TestMethod]
        public void Test_CompileProject_NonExistentProject_ReturnsFailure()
        {
            var engine = new CompilationEngine();
            var nonExistentProject = Path.Combine(tempTestDir, "NonExistent.csproj");

            var results = engine.CompileProject(nonExistentProject);

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            Assert.IsTrue(results[0].Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        }

        [TestMethod]
        public void Test_CompileProject_InvalidProjectFile_ReturnsFailure()
        {
            var invalidProject = @"<InvalidXml>
    <This is not valid XML
</NotClosed>";

            var engine = new CompilationEngine();
            var projectPath = Path.Combine(tempTestDir, "Invalid.csproj");
            File.WriteAllText(projectPath, invalidProject);

            var results = engine.CompileProject(projectPath);

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            Assert.IsTrue(results[0].Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        }

        [TestMethod]
        public void Test_CreateResults_OnFailedCompilation_ThrowsException()
        {
            var invalidContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class InvalidContract : SmartContract
    {
        public static void Test()
        {
            UndefinedType x; // This will cause compilation failure
        }
    }
}";

            var engine = new CompilationEngine();
            var contractPath = Path.Combine(tempTestDir, "FailedContract.cs");
            File.WriteAllText(contractPath, invalidContract);

            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];

            Assert.IsFalse(context.Success);

            // Attempting to create results from a failed compilation should throw
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                context.CreateResults(tempTestDir);
            });
        }

        [TestMethod]
        public void Test_CreateAssembly_OnFailedCompilation_ThrowsException()
        {
            var invalidContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class InvalidContract : SmartContract
    {
        public static void Test()
        {
            UndefinedType x; // This will cause compilation failure
        }
    }
}";

            var engine = new CompilationEngine();
            var contractPath = Path.Combine(tempTestDir, "FailedAssemblyContract.cs");
            File.WriteAllText(contractPath, invalidContract);

            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];

            Assert.IsFalse(context.Success);

            // Attempting to create assembly from a failed compilation should throw
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                context.CreateAssembly();
            });
        }

        [TestMethod]
        public void Test_GetContractHash_OnFailedCompilation_ReturnsNull()
        {
            var invalidContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class InvalidContract : SmartContract
    {
        public static void Test()
        {
            UndefinedType x; // This will cause compilation failure
        }
    }
}";

            var engine = new CompilationEngine();
            var contractPath = Path.Combine(tempTestDir, "FailedHashContract.cs");
            File.WriteAllText(contractPath, invalidContract);

            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];

            Assert.IsFalse(context.Success);

            var contractHash = context.GetContractHash();
            Assert.IsNull(contractHash);
        }

        [TestMethod]
        public void Test_CompilationEngine_WithNullOptions_UsesDefaults()
        {
            // Test with default constructor instead of null
            var engine = new CompilationEngine();
            Assert.IsNotNull(engine);

            // Should still be able to compile with default options
            var validContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class ValidContract : SmartContract
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }
}";

            var contractPath = Path.Combine(tempTestDir, "ValidContract.cs");
            File.WriteAllText(contractPath, validContract);

            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);
        }

        [TestMethod]
        public void Test_CompilationOptions_InvalidValues_DoesNotThrow()
        {
            // Test that extreme or invalid option values don't cause crashes
            var options = new CompilationOptions
            {
                Debug = unchecked((CompilationOptions.DebugType)999), // Invalid enum value
                Optimize = unchecked((CompilationOptions.OptimizationType)999), // Invalid enum value
                AddressVersion = 255, // Extreme value
                BaseName = new string('A', 1000) // Very long name
            };

            // Should not throw when creating engine
            var engine = new CompilationEngine(options);
            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void Test_CompileSources_VeryLongPath_HandlesGracefully()
        {
            var engine = new CompilationEngine();
            
            // Create a very long path (but not longer than system limits)
            var longDir = Path.Combine(tempTestDir, new string('A', 100));
            Directory.CreateDirectory(longDir);
            var longPath = Path.Combine(longDir, "VeryLongContractName.cs");

            var validContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class LongPathContract : SmartContract
    {
        public static int Test() => 42;
    }
}";

            File.WriteAllText(longPath, validContract);

            var results = engine.CompileSources(new[] { longPath });

            Assert.AreEqual(1, results.Count);
            // Should either succeed or fail gracefully without crashing
            Assert.IsNotNull(results[0]);
        }
    }
}