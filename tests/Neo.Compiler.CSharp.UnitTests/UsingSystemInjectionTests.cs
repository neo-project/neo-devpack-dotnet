// Copyright (C) 2015-2025 The Neo Project.
//
// UsingSystemInjectionTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UsingSystemInjectionTests
    {
        [TestMethod]
        public void EnsureUsingSystemExists_WithoutUsingSystem_ShouldAddIt()
        {
            // Arrange
            string sourceCode = @"using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static void TestMethod()
    {
        throw new Exception(""Test"");
    }
}";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.IsTrue(result.Contains("using System;"));
            Assert.IsTrue(result.IndexOf("using System;") < result.IndexOf("using Neo.SmartContract.Framework;"));
        }

        [TestMethod]
        public void EnsureUsingSystemExists_WithExistingUsingSystem_ShouldRemainUnchanged()
        {
            // Arrange
            string sourceCode = @"using System;
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static void TestMethod()
    {
        throw new Exception(""Test"");
    }
}";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.AreEqual(sourceCode, result); // Should be unchanged
            Assert.AreEqual(1, CountOccurrences(result, "using System;"));
        }

        [TestMethod]
        public void EnsureUsingSystemExists_WithGlobalUsing_ShouldRemainUnchanged()
        {
            // Arrange
            string sourceCode = @"global using System;
using Neo.SmartContract.Framework;

public class TestContract : SmartContract
{
    public static void TestMethod()
    {
        throw new Exception(""Test"");
    }
}";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.AreEqual(sourceCode, result); // Should be unchanged
        }

        [TestMethod]
        public void EnsureUsingSystemExists_EmptyFile_ShouldAddUsingSystem()
        {
            // Arrange
            string sourceCode = "";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.IsTrue(result.StartsWith("using System;"));
        }

        [TestMethod]
        public void EnsureUsingSystemExists_WithComments_ShouldInsertAfterComments()
        {
            // Arrange
            string sourceCode = @"// Copyright notice
// Some comments
/* Block comment */
using Neo.SmartContract.Framework;

public class TestContract
{
    public static void TestMethod()
    {
        throw new Exception(""Test"");
    }
}";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.IsTrue(result.Contains("using System;"));
            Assert.IsTrue(result.IndexOf("// Copyright notice") < result.IndexOf("using System;"));
            Assert.IsTrue(result.IndexOf("/* Block comment */") < result.IndexOf("using System;"));
            Assert.IsTrue(result.IndexOf("using System;") < result.IndexOf("using Neo.SmartContract.Framework;"));
        }

        [TestMethod]
        public void EnsureUsingSystemExists_WithNamespaceOnly_ShouldInsertAtBeginning()
        {
            // Arrange
            string sourceCode = @"namespace MyNamespace
{
    public class TestContract
    {
        public static void TestMethod()
        {
            throw new Exception(""Test"");
        }
    }
}";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.IsTrue(result.StartsWith("using System;"));
            Assert.IsTrue(result.IndexOf("using System;") < result.IndexOf("namespace MyNamespace"));
        }

        [TestMethod]
        public void EnsureUsingSystemExists_WithCompilerDirectives_ShouldInsertAfterDirectives()
        {
            // Arrange
            string sourceCode = @"#pragma warning disable CS0162
#define DEBUG
using Neo.SmartContract.Framework;

public class TestContract
{
}";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.IsTrue(result.Contains("using System;"));
            Assert.IsTrue(result.IndexOf("#pragma warning disable CS0162") < result.IndexOf("using System;"));
            Assert.IsTrue(result.IndexOf("#define DEBUG") < result.IndexOf("using System;"));
            Assert.IsTrue(result.IndexOf("using System;") < result.IndexOf("using Neo.SmartContract.Framework;"));
        }

        [TestMethod]
        public void HasUsingSystem_VariousValidFormats_ShouldReturnTrue()
        {
            // Test exact match
            Assert.IsTrue(Helper.HasUsingSystem("using System;"));

            // Test with space instead of semicolon
            Assert.IsTrue(Helper.HasUsingSystem("using System ;"));

            // Test global using
            Assert.IsTrue(Helper.HasUsingSystem("global using System;"));

            // Test case insensitive
            Assert.IsTrue(Helper.HasUsingSystem("USING SYSTEM;"));
            Assert.IsTrue(Helper.HasUsingSystem("Using System;"));

            // Test with other usings
            Assert.IsTrue(Helper.HasUsingSystem(@"
using System;
using Neo.SmartContract.Framework;"));

            // Test with comments before
            Assert.IsTrue(Helper.HasUsingSystem(@"
// Comment
using System;
using Other;"));
        }

        [TestMethod]
        public void HasUsingSystem_InvalidFormats_ShouldReturnFalse()
        {
            // Test partial matches that should not be detected
            Assert.IsFalse(Helper.HasUsingSystem("using SystemSomething;"));
            Assert.IsFalse(Helper.HasUsingSystem("using Neo.System;"));
            Assert.IsFalse(Helper.HasUsingSystem("using MySystem;"));

            // Test commented out using
            Assert.IsFalse(Helper.HasUsingSystem("// using System;"));
            Assert.IsFalse(Helper.HasUsingSystem("/* using System; */"));

            // Test empty or null
            Assert.IsFalse(Helper.HasUsingSystem(""));
            Assert.IsFalse(Helper.HasUsingSystem(null!));

            // Test without using System
            Assert.IsFalse(Helper.HasUsingSystem(@"
using Neo.SmartContract.Framework;
using System.Collections.Generic;"));
        }

        [TestMethod]
        public void InjectUsingSystem_AtCorrectPosition_ShouldMaintainStructure()
        {
            // Arrange
            string sourceCode = @"// Header comment
using Neo.SmartContract.Framework;
using System.Collections.Generic;

namespace Test
{
    public class TestClass
    {
    }
}";

            // Act
            string result = Helper.InjectUsingSystem(sourceCode);

            // Assert
            string[] lines = result.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // Find the indices
            int headerCommentIndex = System.Array.FindIndex(lines, l => l.Trim() == "// Header comment");
            int usingSystemIndex = System.Array.FindIndex(lines, l => l.Trim() == "using System;");
            int usingFrameworkIndex = System.Array.FindIndex(lines, l => l.Trim() == "using Neo.SmartContract.Framework;");

            Assert.IsTrue(headerCommentIndex < usingSystemIndex);
            Assert.IsTrue(usingSystemIndex < usingFrameworkIndex);
        }

        [TestMethod]
        public void InjectUsingSystem_WithExistingUsingSystemVariant_ShouldNotDuplicate()
        {
            // Arrange
            string sourceCode = @"using System ;
using Neo.SmartContract.Framework;";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert - should remain unchanged since "using System " is detected
            Assert.AreEqual(sourceCode, result);
        }

        [TestMethod]
        public void EnsureUsingSystemExists_NullInput_ShouldHandleGracefully()
        {
            // Act & Assert
            string result = Helper.EnsureUsingSystemExists(null!);
            Assert.IsTrue(result.StartsWith("using System;"));
        }

        [TestMethod]
        public void EnsureUsingSystemExists_WhitespaceOnly_ShouldAddUsingSystem()
        {
            // Arrange
            string sourceCode = "   \n\n  \t  \n";

            // Act
            string result = Helper.EnsureUsingSystemExists(sourceCode);

            // Assert
            Assert.IsTrue(result.Contains("using System;"));
        }

        private static int CountOccurrences(string text, string pattern)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(pattern, index)) != -1)
            {
                count++;
                index += pattern.Length;
            }
            return count;
        }
    }
}
