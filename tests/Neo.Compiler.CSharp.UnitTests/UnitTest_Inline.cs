using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inline
    {
        [TestMethod]
        public void Test_Inline()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Inline.cs");

            var result1 = testengine.ExecuteTestCaseStandard("testInline", "inline").Pop();
            testengine.Reset();
            var result3 = testengine.ExecuteTestCaseStandard("testInline", "inline_with_one_parameters").Pop();
            testengine.Reset();
            var result4 = testengine.ExecuteTestCaseStandard("testInline", "inline_with_multi_parameters").Pop();

            // Test default

            Assert.AreEqual(result1.GetInteger(), 1);
            Assert.AreEqual(result3.GetInteger(), 3);
            Assert.AreEqual(result4.GetInteger(), 5);
            //op:[0000]INITSLOT
            // op:[0003]LDARG0
            // op:[0004]STLOC0
            // op:[0005]LDLOC0
            // op:[0006]PUSHDATA1
            // op:[000E]EQUAL
            // op:[000F]JMPIFNOT
            // op:[0011]PUSH1
            // op:[0012]JMP_L
            // op:[0114]RET
            // op:[0000]INITSLOT
            // op:[0003]LDARG0
            // op:[0004]STLOC0
            // op:[0005]LDLOC0
            // op:[0006]PUSHDATA1
            // op:[000E]EQUAL
            // op:[000F]JMPIFNOT
            // op:[001C]LDLOC0
            // op:[001D]PUSHDATA1
            // op:[0039]EQUAL
            // op:[003A]JMPIFNOT
            // op:[003C]PUSH3
            // op:[003D]JMP_L
            // op:[0114]RET
            // op:[0000]INITSLOT
            // op:[0003]LDARG0
            // op:[0004]STLOC0
            // op:[0005]LDLOC0
            // op:[0006]PUSHDATA1
            // op:[000E]EQUAL
            // op:[000F]JMPIFNOT
            // op:[001C]LDLOC0
            // op:[001D]PUSHDATA1
            // op:[0039]EQUAL
            // op:[003A]JMPIFNOT
            // op:[0047]LDLOC0
            // op:[0048]PUSHDATA1
            // op:[0066]EQUAL
            // op:[0067]JMPIFNOT
            // op:[0069]PUSH3
            // op:[006A]PUSH2
            // op:[006B]ADD
            // op:[006C]DUP
            // op:[006D]PUSHINT32
            // op:[0072]JMPGE
            // op:[0076]DUP
            // op:[0077]PUSHINT32
            // op:[007C]JMPLE
            // op:[009A]JMP
            // op:[0114]RET
        }

        [TestMethod]
        public void Test_NoInline()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Inline.cs");

            var result1 = testengine.ExecuteTestCaseStandard("testInline", "not_inline").Pop();
            testengine.Reset();
            var result3 = testengine.ExecuteTestCaseStandard("testInline", "not_inline_with_one_parameters").Pop();
            testengine.Reset();
            var result4 = testengine.ExecuteTestCaseStandard("testInline", "not_inline_with_multi_parameters").Pop();

            // Test default

            Assert.AreEqual(result1.GetInteger(), 1);
            Assert.AreEqual(result3.GetInteger(), 3);
            Assert.AreEqual(result4.GetInteger(), 5);

            //op:[0000]INITSLOT
            // op:[0003]LDARG0
            // op:[0004]STLOC0
            // op:[0005]LDLOC0
            // op:[0006]PUSHDATA1
            // op:[000E]EQUAL
            // op:[000F]JMPIFNOT
            // op:[001C]LDLOC0
            // op:[001D]PUSHDATA1
            // op:[0039]EQUAL
            // op:[003A]JMPIFNOT
            // op:[0047]LDLOC0
            // op:[0048]PUSHDATA1
            // op:[0066]EQUAL
            // op:[0067]JMPIFNOT
            // op:[009E]LDLOC0
            // op:[009F]PUSHDATA1
            // op:[00AB]EQUAL
            // op:[00AC]JMPIFNOT
            // op:[00AE]CALL
            // op:[0115]PUSH1
            // op:[0116]JMP
            // op:[0118]RET
            // op:[00B0]JMP
            // op:[0112]JMP
            // op:[0114]RET
            // op:[0000]INITSLOT
            // op:[0003]LDARG0
            // op:[0004]STLOC0
            // op:[0005]LDLOC0
            // op:[0006]PUSHDATA1
            // op:[000E]EQUAL
            // op:[000F]JMPIFNOT
            // op:[001C]LDLOC0
            // op:[001D]PUSHDATA1
            // op:[0039]EQUAL
            // op:[003A]JMPIFNOT
            // op:[0047]LDLOC0
            // op:[0048]PUSHDATA1
            // op:[0066]EQUAL
            // op:[0067]JMPIFNOT
            // op:[009E]LDLOC0
            // op:[009F]PUSHDATA1
            // op:[00AB]EQUAL
            // op:[00AC]JMPIFNOT
            // op:[00B2]LDLOC0
            // op:[00B3]PUSHDATA1
            // op:[00D3]EQUAL
            // op:[00D4]JMPIFNOT
            // op:[00D6]PUSH3
            // op:[00D7]CALL
            // op:[0119]INITSLOT
            // op:[011C]LDARG0
            // op:[011D]JMP
            // op:[011F]RET
            // op:[00D9]JMP
            // op:[0112]JMP
            // op:[0114]RET
            // op:[0000]INITSLOT
            // op:[0003]LDARG0
            // op:[0004]STLOC0
            // op:[0005]LDLOC0
            // op:[0006]PUSHDATA1
            // op:[000E]EQUAL
            // op:[000F]JMPIFNOT
            // op:[001C]LDLOC0
            // op:[001D]PUSHDATA1
            // op:[0039]EQUAL
            // op:[003A]JMPIFNOT
            // op:[0047]LDLOC0
            // op:[0048]PUSHDATA1
            // op:[0066]EQUAL
            // op:[0067]JMPIFNOT
            // op:[009E]LDLOC0
            // op:[009F]PUSHDATA1
            // op:[00AB]EQUAL
            // op:[00AC]JMPIFNOT
            // op:[00B2]LDLOC0
            // op:[00B3]PUSHDATA1
            // op:[00D3]EQUAL
            // op:[00D4]JMPIFNOT
            // op:[00DB]LDLOC0
            // op:[00DC]PUSHDATA1
            // op:[00FE]EQUAL
            // op:[00FF]JMPIFNOT
            // op:[0101]PUSH3
            // op:[0102]PUSH2
            // op:[0103]CALL
            // op:[0120]INITSLOT
            // op:[0123]LDARG0
            // op:[0124]LDARG1
            // op:[0125]ADD
            // op:[0126]DUP
            // op:[0127]PUSHINT32
            // op:[012C]JMPGE
            // op:[0130]DUP
            // op:[0131]PUSHINT32
            // op:[0136]JMPLE
            // op:[0154]JMP
            // op:[0156]RET
            // op:[0105]JMP
            // op:[0112]JMP
            // op:[0114]RET
        }

        [TestMethod]
        public void Test_NestedInline()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Inline.cs");

            var result1 = testengine.ExecuteTestCaseStandard("testInline", "inline_nested").Pop();
            Assert.AreEqual(result1.GetInteger(), 3);
        }
    }
}
