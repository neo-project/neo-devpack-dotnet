using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inline_Validation
    {
        [TestMethod]
        public void Test_RecursiveInline_ShouldFail()
        {
            // This test verifies that recursive inline methods are rejected at compile time
            var exception = Assert.ThrowsException<Exception>(() =>
            {
                var contract = TestingArtifacts.Contract_Inline_Invalid.CreateEngine();
            });

            // Check that the exception message contains information about recursive inline
            Assert.IsTrue(exception.Message.Contains("Recursive methods cannot be inlined") || 
                         exception.InnerException?.Message.Contains("Recursive methods cannot be inlined") == true,
                         $"Expected error about recursive inline, but got: {exception.Message}");
        }

        [TestMethod]
        public void Test_OutParameter_ShouldFail()
        {
            // This test verifies that inline methods with out parameters are rejected
            var exception = Assert.ThrowsException<Exception>(() =>
            {
                var contract = TestingArtifacts.Contract_Inline_Invalid.CreateEngine();
            });

            // Check that the exception message contains information about ref/out parameters
            Assert.IsTrue(exception.Message.Contains("ref/out parameters cannot be inlined") || 
                         exception.InnerException?.Message.Contains("ref/out parameters cannot be inlined") == true,
                         $"Expected error about ref/out parameters, but got: {exception.Message}");
        }

        [TestMethod]
        public void Test_RefParameter_ShouldFail()
        {
            // This test verifies that inline methods with ref parameters are rejected
            var exception = Assert.ThrowsException<Exception>(() =>
            {
                var contract = TestingArtifacts.Contract_Inline_Invalid.CreateEngine();
            });

            // Check that the exception message contains information about ref/out parameters
            Assert.IsTrue(exception.Message.Contains("ref/out parameters cannot be inlined") || 
                         exception.InnerException?.Message.Contains("ref/out parameters cannot be inlined") == true,
                         $"Expected error about ref/out parameters, but got: {exception.Message}");
        }

        [TestMethod]
        public void Test_CompilationWithInlineValidation()
        {
            // Try to compile a contract with invalid inline methods
            string sourceCode = @"
using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RecursiveMethod(int n)
    {
        if (n <= 0) return 1;
        return n * RecursiveMethod(n - 1);
    }
    
    public static int Main()
    {
        return RecursiveMethod(5);
    }
}";

            try
            {
                // This should throw during compilation
                var result = new Neo.Compiler.CSharp.CompilationEngine(new CompilationOptions()).CompileSources(sourceCode);
                Assert.Fail("Expected compilation to fail for recursive inline method");
            }
            catch (Exception ex)
            {
                // Verify the error message
                Assert.IsTrue(ex.Message.Contains("Recursive") || ex.Message.Contains("cannot be inlined"),
                             $"Unexpected error message: {ex.Message}");
            }
        }

        [TestMethod]
        public void Test_ValidInline_ShouldPass()
        {
            // This should compile successfully as it's a valid inline method
            string sourceCode = @"
using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SimpleInline(int a, int b)
    {
        return a + b;
    }
    
    public static int Main()
    {
        return SimpleInline(5, 3);
    }
}";

            try
            {
                var result = new Neo.Compiler.CSharp.CompilationEngine(new CompilationOptions()).CompileSources(sourceCode);
                Assert.IsNotNull(result);
                // If we get here, compilation succeeded which is what we want
            }
            catch (Exception ex)
            {
                Assert.Fail($"Valid inline method should compile successfully, but got error: {ex.Message}");
            }
        }
    }
}