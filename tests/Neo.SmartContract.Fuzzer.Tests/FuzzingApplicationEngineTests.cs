using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer;
using Neo.VM;
using System;
using System.Threading;

namespace Neo.SmartContract.Fuzzer.Tests
{
    [TestClass]
    public class FuzzingApplicationEngineTests
    {
        [TestMethod]
        public void TestInstructionCounting()
        {
            // Create a simple script that executes a few instructions
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush(1);
            sb.EmitPush(2);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            // Create a FuzzingApplicationEngine
            using var engine = FuzzingApplicationEngine.Create(
                TriggerType.Application,
                null,
                TestBlockchain.GetSnapshot(),
                null,
                null,
                10000);

            // Load the script
            engine.LoadScript(sb.ToArray());

            // Execute the script
            var state = engine.Execute();

            // Verify that the engine counted the instructions
            Assert.AreEqual(VMState.HALT, state);
            Assert.IsTrue(engine.InstructionCount > 0);
            Assert.IsFalse(engine.TimedOut);
        }

        [TestMethod]
        public void TestTimeout()
        {
            // Create a script with an infinite loop
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush(1);
            sb.Emit(OpCode.JMP, new byte[] { 0xFE }); // Jump back to the beginning

            // Create a FuzzingApplicationEngine with a short timeout
            using var engine = FuzzingApplicationEngine.Create(
                TriggerType.Application,
                null,
                TestBlockchain.GetSnapshot(),
                null,
                null,
                100); // 100ms timeout

            // Load the script
            engine.LoadScript(sb.ToArray());

            // Execute the script
            var state = engine.Execute();

            // Verify that the engine timed out
            Assert.AreEqual(VMState.HALT, state);
            Assert.IsTrue(engine.TimedOut);
            Assert.IsTrue(engine.InstructionCount > 0);
        }

        [TestMethod]
        public void TestWitnessCheckedEvent()
        {
            // Create a script that calls Runtime.CheckWitness
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush(new byte[20]); // Empty UInt160
            sb.EmitSysCall(ApplicationEngine.System_Runtime_CheckWitness);
            sb.Emit(OpCode.RET);

            // Create a FuzzingApplicationEngine
            using var engine = FuzzingApplicationEngine.Create(
                TriggerType.Application,
                null,
                TestBlockchain.GetSnapshot(),
                null,
                null,
                10000);

            // Set up event handler
            bool eventFired = false;
            string witnessAccount = null;
            engine.WitnessChecked += (sender, args) =>
            {
                eventFired = true;
                witnessAccount = args.Account;
            };

            // Load the script
            engine.LoadScript(sb.ToArray());

            // Execute the script
            var state = engine.Execute();

            // Verify that the event was fired
            Assert.IsTrue(eventFired);
            Assert.IsNotNull(witnessAccount);
        }

        [TestMethod]
        public void TestExternalCallEvent()
        {
            // Create a script that calls another contract
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush("method");
            sb.EmitPush(new byte[20]); // Contract hash
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
            sb.Emit(OpCode.RET);

            // Create a FuzzingApplicationEngine
            using var engine = FuzzingApplicationEngine.Create(
                TriggerType.Application,
                null,
                TestBlockchain.GetSnapshot(),
                null,
                null,
                10000);

            // Set up event handler
            bool eventFired = false;
            string targetContract = null;
            string methodName = null;
            engine.ExternalCallPerformed += (sender, args) =>
            {
                eventFired = true;
                targetContract = args.Target;
                methodName = args.Method;
            };

            // Load the script
            engine.LoadScript(sb.ToArray());

            // Execute the script
            try
            {
                var state = engine.Execute();
                // The call will likely fail since we're not setting up a real contract
                // but the event should still fire
            }
            catch
            {
                // Ignore execution errors
            }

            // Verify that the event was fired
            Assert.IsTrue(eventFired);
            Assert.IsNotNull(targetContract);
            Assert.AreEqual("method", methodName);
        }

        [TestMethod]
        public void TestGetExecutionTime()
        {
            // Create a simple script with a delay
            using ScriptBuilder sb = new ScriptBuilder();
            sb.EmitPush(1);
            sb.EmitPush(2);
            sb.Emit(OpCode.ADD);
            sb.Emit(OpCode.RET);

            // Create a FuzzingApplicationEngine
            using var engine = FuzzingApplicationEngine.Create(
                TriggerType.Application,
                null,
                TestBlockchain.GetSnapshot(),
                null,
                null,
                10000);

            // Load the script
            engine.LoadScript(sb.ToArray());

            // Add a small delay to ensure execution time is measurable
            Thread.Sleep(10);

            // Execute the script
            var state = engine.Execute();

            // Verify that the execution time is greater than zero
            var executionTime = engine.GetExecutionTime();
            Assert.IsTrue(executionTime.TotalMilliseconds > 0);
        }
    }
}
