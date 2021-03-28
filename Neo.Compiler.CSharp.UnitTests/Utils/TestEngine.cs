using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests.Utils
{
    class TestEngine : ApplicationEngine
    {
        public const long TestGas = 2000_00000000;
        public readonly IDictionary<string, BuildScript> Scripts;

        public BuildScript ScriptEntry { get; private set; }
        public TestEngine(TriggerType trigger = TriggerType.Application, IVerifiable verificable = null, DataCache snapshot = null, Block persistingBlock = null)
             : base(trigger, verificable, snapshot, persistingBlock, ProtocolSettings.Default, TestGas)
        {
            Scripts = new Dictionary<string, BuildScript>();
        }
        public void AddEntryScript(string filename)
        {
            ScriptEntry = new BuildScript();
            ScriptEntry.Build(filename);
            Reset();
        }
        public void Reset()
        {
            this.State = VMState.BREAK; // Required for allow to reuse the same TestEngine
            this.InvocationStack.Clear();
            while (this.ResultStack.Count > 0)
            {
                this.ResultStack.Pop();
            }
            if (ScriptEntry != null)
            {
                this.LoadScript(ScriptEntry.nef.Script);
                // Mock contract
                var contextState = CurrentContext.GetState<ExecutionContextState>();
                contextState.Contract ??= new ContractState()
                {
                    Nef = ScriptEntry.nef
                };
            }
        }
        public int GetMethodEntryOffset(string methodname)
        {
            if (this.ScriptEntry is null) return -1;
            var methods = this.ScriptEntry.manifest["abi"]["methods"] as JArray;
            foreach (var item in methods)
            {
                var method = item as JObject;
                if (method["name"].AsString() == methodname)
                    return int.Parse(method["offset"].AsString());
            }

            return -1;
        }

        public int GetMethodReturnCount(string methodname)
        {
            if (this.ScriptEntry is null) return -1;
            var methods = this.ScriptEntry.manifest["abi"]["methods"] as JArray;
            foreach (var item in methods)
            {
                var method = item as JObject;
                if (method["name"].AsString() == methodname)
                {
                    var returntype = method["returntype"].AsString();
                    if (returntype == "Null" || returntype == "Void")
                        return 0;
                    else
                        return 1;
                }
            }
            return -1;
        }
        public EvaluationStack ExecuteTestCaseStandard(string methodname, params StackItem[] args)
        {
            return ExecuteTestCaseStandard(methodname, ScriptEntry.nef, args);
        }
        public EvaluationStack ExecuteTestCaseStandard(string methodname, NefFile contract, params StackItem[] args)
        {
            var offset = GetMethodEntryOffset(methodname);
            if (offset == -1) throw new Exception("Can't find method : " + methodname);
            var rvcount = GetMethodReturnCount(methodname);
            if (rvcount == -1) throw new Exception("Can't find method return count : " + methodname);
            return ExecuteTestCaseStandard(offset, (ushort)rvcount, contract, args);
        }
        public EvaluationStack ExecuteTestCaseStandard(int offset, ushort rvcount, NefFile contract, params StackItem[] args)
        {
            var context = InvocationStack.Pop();
            context = CreateContext(context.Script, rvcount, offset);
            LoadContext(context);
            // Mock contract
            var contextState = CurrentContext.GetState<ExecutionContextState>();
            contextState.Contract ??= new ContractState() { Nef = contract };
            for (var i = args.Length - 1; i >= 0; i--)
                this.Push(args[i]);
            var initializeOffset = GetMethodEntryOffset("_initialize");
            if (initializeOffset != -1)
            {
                LoadContext(CurrentContext.Clone(initializeOffset));
            }
            while (true)
            {
                var bfault = (this.State & VMState.FAULT) > 0;
                var bhalt = (this.State & VMState.HALT) > 0;
                if (bfault || bhalt) break;
                Console.WriteLine("op:[" +
                    this.CurrentContext.InstructionPointer.ToString("X04") +
                    "]" +
                this.CurrentContext.CurrentInstruction.OpCode);
                this.ExecuteNext();
            }
            return this.ResultStack;
        }
    }
}
