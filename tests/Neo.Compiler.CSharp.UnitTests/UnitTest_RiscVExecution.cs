// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RiscVExecution.cs file belongs to the neo project and is free
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
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

/// <summary>
/// Tests that compile C# contracts to RISC-V (.polkavm) binaries and
/// execute them directly via the native neo_riscv_host library, using
/// <see cref="RiscVExecutionBridge"/> for P/Invoke.
///
/// These tests validate the full C#-to-RISC-V pipeline:
///   1. C# contract -> NeoVM bytecode (compiler frontend)
///   2. NeoVM bytecode -> Rust source (NeoVmToRustTranslator)
///   3. Rust source -> .polkavm binary (cargo + polkatool)
///   4. .polkavm binary -> native execution (neo_riscv_host)
/// </summary>
[TestClass]
[TestCategory("RiscV")]
public class UnitTest_RiscVExecution
{
    private static bool s_runtimeAvailable;
    private static bool s_contractsBuilt;

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        // Step 1: Load the native library.
        try
        {
            RiscVExecutionBridge.Initialize();
            s_runtimeAvailable = true;
        }
        catch (Exception ex) when (ex is FileNotFoundException or DllNotFoundException)
        {
            Console.Error.WriteLine($"[RiscV] Native library not available: {ex.Message}");
            return;
        }

        // Step 2: Compile all test contracts to RISC-V and build .polkavm binaries.
        try
        {
            RiscVTestHelper.Initialize();
            s_contractsBuilt = true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[RiscV] Contract compilation failed: {ex.Message}");
        }
    }

    private static byte[] LoadBinary(string contractName)
    {
        var path = RiscVTestHelper.GetPolkaVmBinary(contractName);
        Assert.IsNotNull(path, $"Failed to build .polkavm binary for {contractName}");
        return File.ReadAllBytes(path);
    }

    private byte[] LoadBinaryOrSkip(string contractName)
    {
        RequireRuntime();
        var path = RiscVTestHelper.GetPolkaVmBinary(contractName);
        if (path == null)
        {
            Assert.Inconclusive($"{contractName} .polkavm binary not available.");
            return Array.Empty<byte>(); // unreachable
        }
        return File.ReadAllBytes(path);
    }

    private void RequireRuntime()
    {
        if (!s_runtimeAvailable)
            Assert.Inconclusive("RISC-V native library (libneo_riscv_host.so) not available.");
        if (!s_contractsBuilt)
            Assert.Inconclusive("RISC-V contract compilation toolchain not available.");
    }

    // -----------------------------------------------------------
    //  Contract_Assignment: pure computation, no syscalls needed
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Assignment_TestAssignment()
    {
        RequireRuntime();
        var binary = LoadBinary("Contract_Assignment");
        var result = RiscVExecutionBridge.Execute(binary, "testAssignment");
        Assert.IsTrue(result.IsHalt,
            $"Expected HALT but got state={result.State}. Error: {result.Error}");
    }

    [TestMethod]
    public void Contract_Assignment_TestCoalesceAssignment()
    {
        RequireRuntime();
        var binary = LoadBinary("Contract_Assignment");
        var result = RiscVExecutionBridge.Execute(binary, "testCoalesceAssignment");
        // Note: Nullable coalescing (??=) generates complex control flow with
        // ISNULL/JMPIFNOT/DUP opcodes. If this fails with "invalid pc", it
        // indicates a translation gap in NeoVmToRustTranslator for that pattern.
        Assert.IsTrue(result.IsHalt,
            $"Expected HALT but got state={result.State}. Error: {result.Error}");
    }

    // -----------------------------------------------------------
    //  Contract_Boolean: boolean operations
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Boolean_TestBooleanOr()
    {
        RequireRuntime();
        var path = RiscVTestHelper.GetPolkaVmBinary("Contract_Boolean");
        if (path == null)
        {
            Assert.Inconclusive("Contract_Boolean .polkavm binary not available.");
            return;
        }

        var binary = File.ReadAllBytes(path);

        // testBooleanOr() returns true (true || false)
        var result = RiscVExecutionBridge.Execute(binary, "testBooleanOr");
        Assert.IsTrue(result.IsHalt,
            $"testBooleanOr: Expected HALT but got state={result.State}. Error: {result.Error}");
        // Should return a boolean true
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be a Boolean (kind=3).");
        Assert.AreEqual(1L, result.Stack[0].IntegerValue, "Result should be true (1).");
    }

    // -----------------------------------------------------------
    //  Contract_Types: various primitive return types
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Types_CheckBoolTrue()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Types");
        var result = RiscVExecutionBridge.Execute(binary, "checkBoolTrue");
        Assert.IsTrue(result.IsHalt,
            $"checkBoolTrue: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
        Assert.AreEqual(1L, result.Stack[0].IntegerValue, "Should be true.");
    }

    [TestMethod]
    public void Contract_Types_CheckBoolFalse()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Types");
        var result = RiscVExecutionBridge.Execute(binary, "checkBoolFalse");
        Assert.IsTrue(result.IsHalt,
            $"checkBoolFalse: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
        Assert.AreEqual(0L, result.Stack[0].IntegerValue, "Should be false.");
    }

    [TestMethod]
    public void Contract_Types_CheckInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Types");
        var result = RiscVExecutionBridge.Execute(binary, "checkInt");
        Assert.IsTrue(result.IsHalt,
            $"checkInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
        Assert.AreEqual(5L, result.Stack[0].IntegerValue, "Should return 5.");
    }

    [TestMethod]
    public void Contract_Types_CheckString()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Types");
        var result = RiscVExecutionBridge.Execute(binary, "checkString");
        Assert.IsTrue(result.IsHalt,
            $"checkString: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(1u, result.Stack[0].Kind, "Result should be ByteString (kind=1).");
        var text = System.Text.Encoding.UTF8.GetString(result.Stack[0].Bytes!);
        Assert.AreEqual("neo", text, "Should return 'neo'.");
    }

    [TestMethod]
    public void Contract_Types_CheckNull()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Types");
        var result = RiscVExecutionBridge.Execute(binary, "checkNull");
        Assert.IsTrue(result.IsHalt,
            $"checkNull: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(2u, result.Stack[0].Kind, "Result should be Null (kind=2).");
    }

    [TestMethod]
    public void Contract_Types_CheckLong()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Types");
        var result = RiscVExecutionBridge.Execute(binary, "checkLong");
        Assert.IsTrue(result.IsHalt,
            $"checkLong: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
        Assert.AreEqual(5L, result.Stack[0].IntegerValue, "Should return 5.");
    }

    // -----------------------------------------------------------
    //  Contract_Math: simple math operations
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Math_Max()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Math");
        var result = RiscVExecutionBridge.Execute(binary, "max");
        Assert.IsTrue(result.IsHalt,
            $"max: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Math_Min()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Math");
        var result = RiscVExecutionBridge.Execute(binary, "min");
        Assert.IsTrue(result.IsHalt,
            $"min: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Math_Abs()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Math");
        var result = RiscVExecutionBridge.Execute(binary, "abs");
        Assert.IsTrue(result.IsHalt,
            $"abs: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Math_Sign()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Math");
        var result = RiscVExecutionBridge.Execute(binary, "sign");
        Assert.IsTrue(result.IsHalt,
            $"sign: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Math_BigMul()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Math");
        var result = RiscVExecutionBridge.Execute(binary, "bigMul");
        Assert.IsTrue(result.IsHalt,
            $"bigMul: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Math_DivRemInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Math");
        var result = RiscVExecutionBridge.Execute(binary, "DivRemInt");
        Assert.IsTrue(result.IsHalt,
            $"DivRemInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Math_ClampInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Math");
        var result = RiscVExecutionBridge.Execute(binary, "ClampInt");
        Assert.IsTrue(result.IsHalt,
            $"ClampInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    // -----------------------------------------------------------
    //  Contract_Concat: string concatenation
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Concat_TestStringAdd1()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Concat");
        var result = RiscVExecutionBridge.Execute(binary, "TestStringAdd1");
        Assert.IsTrue(result.IsHalt,
            $"TestStringAdd1: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(1u, result.Stack[0].Kind, "Result should be ByteString (kind=1).");
    }

    [TestMethod]
    public void Contract_Concat_TestStringAdd2()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Concat");
        var result = RiscVExecutionBridge.Execute(binary, "TestStringAdd2");
        Assert.IsTrue(result.IsHalt,
            $"TestStringAdd2: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(1u, result.Stack[0].Kind, "Result should be ByteString (kind=1).");
    }

    [TestMethod]
    public void Contract_Concat_TestStringAdd3()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Concat");
        var result = RiscVExecutionBridge.Execute(binary, "TestStringAdd3");
        Assert.IsTrue(result.IsHalt,
            $"TestStringAdd3: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(1u, result.Stack[0].Kind, "Result should be ByteString (kind=1).");
    }

    [TestMethod]
    public void Contract_Concat_TestStringAdd4()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Concat");
        var result = RiscVExecutionBridge.Execute(binary, "TestStringAdd4");
        Assert.IsTrue(result.IsHalt,
            $"TestStringAdd4: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(1u, result.Stack[0].Kind, "Result should be ByteString (kind=1).");
    }

    // -----------------------------------------------------------
    //  Contract_Switch: switch statement control flow
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Switch_SwitchLong()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Switch");
        var result = RiscVExecutionBridge.Execute(binary, "SwitchLong");
        Assert.IsTrue(result.IsHalt,
            $"SwitchLong: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Switch_Switch6()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Switch");
        var result = RiscVExecutionBridge.Execute(binary, "Switch6");
        Assert.IsTrue(result.IsHalt,
            $"Switch6: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Switch_SwitchInteger()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Switch");
        var result = RiscVExecutionBridge.Execute(binary, "SwitchInteger");
        Assert.IsTrue(result.IsHalt,
            $"SwitchInteger: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Switch_SwitchLongLong()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Switch");
        var result = RiscVExecutionBridge.Execute(binary, "SwitchLongLong");
        Assert.IsTrue(result.IsHalt,
            $"SwitchLongLong: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Switch_Switch6Inline()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Switch");
        var result = RiscVExecutionBridge.Execute(binary, "Switch6Inline");
        Assert.IsTrue(result.IsHalt,
            $"Switch6Inline: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    // -----------------------------------------------------------
    //  Contract_Integer: integer utility methods
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Integer_IsEvenIntegerInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Integer");
        var result = RiscVExecutionBridge.Execute(binary, "IsEvenIntegerInt");
        Assert.IsTrue(result.IsHalt,
            $"IsEvenIntegerInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_Integer_IsPow2Int()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Integer");
        var result = RiscVExecutionBridge.Execute(binary, "IsPow2Int");
        Assert.IsTrue(result.IsHalt,
            $"IsPow2Int: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_Integer_IsNegativeInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Integer");
        var result = RiscVExecutionBridge.Execute(binary, "IsNegativeInt");
        Assert.IsTrue(result.IsHalt,
            $"IsNegativeInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_Integer_IsPositiveInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Integer");
        var result = RiscVExecutionBridge.Execute(binary, "IsPositiveInt");
        Assert.IsTrue(result.IsHalt,
            $"IsPositiveInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_Integer_ClampInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Integer");
        var result = RiscVExecutionBridge.Execute(binary, "ClampInt");
        Assert.IsTrue(result.IsHalt,
            $"ClampInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Integer_LeadingZeroCountInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Integer");
        var result = RiscVExecutionBridge.Execute(binary, "LeadingZeroCountInt");
        Assert.IsTrue(result.IsHalt,
            $"LeadingZeroCountInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_Integer_PopCountInt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Integer");
        var result = RiscVExecutionBridge.Execute(binary, "PopCountInt");
        Assert.IsTrue(result.IsHalt,
            $"PopCountInt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    // -----------------------------------------------------------
    //  Contract_BigInteger: BigInteger operations
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_BigInteger_TestPow()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "TestPow");
        Assert.IsTrue(result.IsHalt,
            $"TestPow: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestSqrt()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "TestSqrt");
        Assert.IsTrue(result.IsHalt,
            $"TestSqrt: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestIsZero()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "testIsZero");
        Assert.IsTrue(result.IsHalt,
            $"testIsZero: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestIsOne()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "testIsOne");
        Assert.IsTrue(result.IsHalt,
            $"testIsOne: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestSubtract()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "TestSubtract");
        Assert.IsTrue(result.IsHalt,
            $"TestSubtract: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestMultiply()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "TestMultiply");
        Assert.IsTrue(result.IsHalt,
            $"TestMultiply: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestDivide()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "TestDivide");
        Assert.IsTrue(result.IsHalt,
            $"TestDivide: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestModPow()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "TestModPow");
        Assert.IsTrue(result.IsHalt,
            $"TestModPow: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    // -----------------------------------------------------------
    //  Contract_BinaryExpression: binary operations
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_BinaryExpression_BinaryIs()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BinaryExpression");
        var result = RiscVExecutionBridge.Execute(binary, "BinaryIs");
        Assert.IsTrue(result.IsHalt,
            $"BinaryIs: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(0, result.Stack.Length, "void method should produce empty result stack.");
    }

    [TestMethod]
    public void Contract_BinaryExpression_BinaryAs()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BinaryExpression");
        var result = RiscVExecutionBridge.Execute(binary, "BinaryAs");
        Assert.IsTrue(result.IsHalt,
            $"BinaryAs: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(0, result.Stack.Length, "void method should produce empty result stack.");
    }

    // -----------------------------------------------------------
    //  Contract_Array: array construction and indexing
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Array_TestIntArray()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Array");
        var result = RiscVExecutionBridge.Execute(binary, "TestIntArray");
        Assert.IsTrue(result.IsHalt,
            $"TestIntArray: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Array_TestDefaultArray()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Array");
        var result = RiscVExecutionBridge.Execute(binary, "TestDefaultArray");
        Assert.IsTrue(result.IsHalt,
            $"TestDefaultArray: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_Array_TestIntArrayInit()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Array");
        var result = RiscVExecutionBridge.Execute(binary, "TestIntArrayInit");
        Assert.IsTrue(result.IsHalt,
            $"TestIntArrayInit: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Array_TestEmptyArray()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Array");
        var result = RiscVExecutionBridge.Execute(binary, "TestEmptyArray");
        Assert.IsTrue(result.IsHalt,
            $"TestEmptyArray: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Array_TestJaggedArray()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Array");
        var result = RiscVExecutionBridge.Execute(binary, "TestJaggedArray");
        Assert.IsTrue(result.IsHalt,
            $"TestJaggedArray: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    // -----------------------------------------------------------
    //  Contract_Enum: enum value mapping
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Enum_TestEnumGetNames()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Enum");
        var result = RiscVExecutionBridge.Execute(binary, "TestEnumGetNames");
        Assert.IsTrue(result.IsHalt,
            $"TestEnumGetNames: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Enum_TestEnumGetValues()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Enum");
        var result = RiscVExecutionBridge.Execute(binary, "TestEnumGetValues");
        Assert.IsTrue(result.IsHalt,
            $"TestEnumGetValues: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
    }

    [TestMethod]
    public void Contract_Enum_TestEnumIsDefined()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Enum");
        var result = RiscVExecutionBridge.Execute(binary, "TestEnumIsDefined");
        Assert.IsTrue(result.IsHalt,
            $"TestEnumIsDefined: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_Enum_TestEnumGetName()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Enum");
        var result = RiscVExecutionBridge.Execute(binary, "TestEnumGetName");
        Assert.IsTrue(result.IsHalt,
            $"TestEnumGetName: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(1u, result.Stack[0].Kind, "Result should be ByteString (kind=1).");
    }

    [TestMethod]
    public void Contract_Enum_TestEnumToString()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_Enum");
        var result = RiscVExecutionBridge.Execute(binary, "TestEnumToString");
        Assert.IsTrue(result.IsHalt,
            $"TestEnumToString: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(1u, result.Stack[0].Kind, "Result should be ByteString (kind=1).");
    }

    // -----------------------------------------------------------
    //  Contract_MissingCheckWitness: Storage.Put via TestHostCallback
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_BigInteger_TestIsEven()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "testIsEven");
        Assert.IsTrue(result.IsHalt,
            $"testIsEven: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(3u, result.Stack[0].Kind, "Result should be Boolean (kind=3).");
    }

    [TestMethod]
    public void Contract_BigInteger_TestAdd()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_BigInteger");
        var result = RiscVExecutionBridge.Execute(binary, "TestAdd");
        Assert.IsTrue(result.IsHalt,
            $"TestAdd: Expected HALT but got state={result.State}. Error: {result.Error}");
        Assert.AreEqual(1, result.Stack.Length, "Should return one item.");
        Assert.AreEqual(0u, result.Stack[0].Kind, "Result should be Integer (kind=0).");
    }

    // -----------------------------------------------------------
    //  Contract_MissingCheckWitness: Storage.Put via TestHostCallback
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_MissingCheckWitness_UnsafeUpdate()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_MissingCheckWitness");

        // UnsafeUpdate(byte[] key, byte[] value) does:
        //   Storage.Put(context, key, value)
        // Exercises Storage.GetContext + Storage.Put syscalls via TestHostCallback.
        var host = new RiscVExecutionBridge.TestHostCallback();
        var key = System.Text.Encoding.UTF8.GetBytes("mykey");
        var value = System.Text.Encoding.UTF8.GetBytes("myvalue");
        var initialArgs = new[]
        {
            new RiscVExecutionBridge.ResultStackItem { Kind = 1, Bytes = key },
            new RiscVExecutionBridge.ResultStackItem { Kind = 1, Bytes = value },
        };
        var result = host.Execute(binary, "unsafeUpdate", initialArgs);

        // Debug: dump called APIs
        var apiDump = string.Join(", ", host.CalledApis.Select(a => $"0x{a:X8}"));
        Console.WriteLine($"UnsafeUpdate: CalledApis=[{apiDump}], Storage.Count={host.Storage.Count}, IsHalt={result.IsHalt}, State={result.State}, Error={result.Error}");

        Assert.IsTrue(result.IsHalt,
            $"UnsafeUpdate: Expected HALT but got state={result.State}. Error: {result.Error}");

        // Verify storage was populated by the Put call
        Assert.AreEqual(1, host.Storage.Count, "Storage should have exactly one entry.");
        Assert.IsTrue(host.Storage.ContainsKey(key), "Storage should contain the key.");
        Assert.AreEqual("myvalue",
            System.Text.Encoding.UTF8.GetString(host.Storage[key]),
            "Storage value should match.");
    }

    [TestMethod]
    public void Contract_MissingCheckWitness_SafeUpdate_WithWitness()
    {
        RequireRuntime();
        var binary = LoadBinaryOrSkip("Contract_MissingCheckWitness");

        // SafeUpdate(UInt160 owner, byte[] key, byte[] value) does:
        //   ExecutionEngine.Assert(Runtime.CheckWitness(owner));
        //   Storage.Put(context, key, value)
        // TestHostCallback returns true for CheckWitness, so this should succeed.
        var host = new RiscVExecutionBridge.TestHostCallback();
        var owner = new byte[20]; // zero address, matches Runtime.GetExecutingScriptHash
        var key = System.Text.Encoding.UTF8.GetBytes("safekey");
        var value = System.Text.Encoding.UTF8.GetBytes("safevalue");
        var initialArgs = new[]
        {
            new RiscVExecutionBridge.ResultStackItem { Kind = 1, Bytes = owner },
            new RiscVExecutionBridge.ResultStackItem { Kind = 1, Bytes = key },
            new RiscVExecutionBridge.ResultStackItem { Kind = 1, Bytes = value },
        };
        var result = host.Execute(binary, "safeUpdate", initialArgs);

        Assert.IsTrue(result.IsHalt,
            $"SafeUpdate: Expected HALT but got state={result.State}. Error: {result.Error}");

        // Verify storage was populated
        Assert.AreEqual(1, host.Storage.Count, "Storage should have one entry after SafeUpdate.");
        Assert.IsTrue(host.Storage.ContainsKey(key), "Storage should contain the safekey.");
    }

    // -----------------------------------------------------------
    //  Negative test: invalid method name should fault
    // -----------------------------------------------------------

    [TestMethod]
    public void Execute_InvalidMethod_Faults()
    {
        RequireRuntime();
        var binary = LoadBinary("Contract_Assignment");
        var result = RiscVExecutionBridge.Execute(binary, "nonExistentMethod");
        Assert.IsTrue(result.IsFault,
            "Expected FAULT for non-existent method.");
        Assert.IsNotNull(result.Error,
            "Expected an error message for non-existent method.");
    }

    // -----------------------------------------------------------
    //  Negative test: garbage binary should fail gracefully
    // -----------------------------------------------------------

    [TestMethod]
    public void Execute_GarbageBinary_DoesNotCrash()
    {
        RequireRuntime();
        var garbage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0x01, 0x02, 0x03 };
        var result = RiscVExecutionBridge.Execute(garbage, "test");
        // Should fault (or at least not crash the process).
        Assert.IsTrue(result.IsFault,
            "Expected FAULT for garbage binary.");
    }

    // -----------------------------------------------------------
    //  Test result stack reading
    // -----------------------------------------------------------

    [TestMethod]
    public void Contract_Assignment_ResultStackIsEmpty()
    {
        RequireRuntime();
        var binary = LoadBinary("Contract_Assignment");
        var result = RiscVExecutionBridge.Execute(binary, "testAssignment");
        Assert.IsTrue(result.IsHalt,
            $"Expected HALT. Error: {result.Error}");
        // testAssignment() returns void, so result stack should be empty.
        Assert.AreEqual(0, result.Stack.Length,
            "void method should produce empty result stack.");
    }
}
