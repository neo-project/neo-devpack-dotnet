using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    public class TestEngine : ApplicationEngine
    {
        protected override bool PreExecuteInstruction()
        {
            return true;
        }

        public static InteropDescriptor Native_Deploy;

        static TestEngine()
        {
            // Extract Native deploy syscall

            Native_Deploy = Neo_Native_Deploy;
        }


        public const int MaxStorageKeySize = 64;
        public const int MaxStorageValueSize = ushort.MaxValue;

        static readonly IDictionary<string, BuildScript> scriptsAll = new Dictionary<string, BuildScript>();

        public readonly IDictionary<string, BuildScript> Scripts;

        public BuildScript ScriptEntry { get; private set; }

        public TestEngine(TriggerType trigger = TriggerType.Application, IVerifiable verificable = null, StoreView snapshot = null)
            : base(trigger, verificable, snapshot ?? new TestSnapshot(), 0, true)
        {
            Scripts = new Dictionary<string, BuildScript>();
        }

        public BuildScript Build(string filename, bool releaseMode = false, bool optimizer = true)
        {
            var contains = scriptsAll.ContainsKey(filename);

            if (!contains || (contains && scriptsAll[filename].UseOptimizer != optimizer))
            {
                scriptsAll[filename] = NeonTestTool.BuildScript(filename, releaseMode, optimizer);
            }

            return scriptsAll[filename];
        }

        public BuildScript Build(string[] filenames, bool releaseMode = false, bool optimizer = true)
        {
            var key = string.Join("\n", filenames);

            if (scriptsAll.ContainsKey(key) == false)
            {
                return NeonTestTool.BuildScript(filenames, releaseMode, optimizer);
            }

            return scriptsAll[key];
        }

        public void AddEntryScript(string filename, bool releaseMode = false, bool optimizer = true)
        {
            ScriptEntry = Build(filename, releaseMode, optimizer);
            Reset();
        }


        public void AddEntryScript(string[] filenames, bool releaseMode = false, bool optimizer = true)
        {
            ScriptEntry = Build(filenames, releaseMode, optimizer);
            Reset();
        }

        public void Reset()
        {
            this.State = VMState.BREAK; // Required for allow to reuse the same TestEngine
            this.InvocationStack.Clear();
            if (ScriptEntry != null) this.LoadScript(ScriptEntry.finalNEF);
        }

        public class ContractMethod
        {
            readonly TestEngine engine;
            readonly string methodname;

            public ContractMethod(TestEngine engine, string methodname)
            {
                this.engine = engine;
                this.methodname = methodname;
            }

            public StackItem Run(params StackItem[] _params)
            {
                return this.engine.ExecuteTestCaseStandard(methodname, _params).Pop();
            }

            public EvaluationStack RunEx(params StackItem[] _params)
            {
                return this.engine.ExecuteTestCaseStandard(methodname, _params);
            }
        }

        public ContractMethod GetMethod(string methodname)
        {
            return new ContractMethod(this, methodname);
        }

        public int GetMethodEntryOffset(string methodname)
        {
            if (this.ScriptEntry is null) return -1;
            var methods = this.ScriptEntry.finialABI.GetDictItem("methods") as MyJson.JsonNode_Array;
            foreach (var item in methods)
            {
                var method = item as MyJson.JsonNode_Object;
                if (method.GetDictItem("name").ToString() == methodname)
                    return int.Parse(method.GetDictItem("offset").ToString());
            }
            return -1;
        }

        public EvaluationStack ExecuteTestCaseStandard(string methodname, params StackItem[] args)
        {
            var offset = GetMethodEntryOffset(methodname);
            if (offset == -1) throw new Exception("Can't find method : " + methodname);
            return ExecuteTestCaseStandard(offset, args);
        }

        public EvaluationStack ExecuteTestCaseStandard(int offset, params StackItem[] args)
        {
            this.InvocationStack.Peek().InstructionPointer = offset;
            for (var i = args.Length - 1; i >= 0; i--)
                this.Push(args[i]);
            var initializeOffset = GetMethodEntryOffset("_initialize");
            if (initializeOffset != -1)
                this.LoadClonedContext(initializeOffset);
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

        public EvaluationStack ExecuteTestCase(params StackItem[] args)
        {
            //var engine = new ExecutionEngine();
            this.InvocationStack.Peek().InstructionPointer = 0;
            if (args != null)
            {
                for (var i = args.Length - 1; i >= 0; i--)
                {
                    this.CurrentContext.EvaluationStack.Push(args[i]);
                }
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

        static Dictionary<uint, InteropDescriptor> callmethod;

        protected override bool OnSysCall(uint method)
        {
            if (callmethod == null)
            {
                callmethod = new Dictionary<uint, InteropDescriptor>()
                {
                    { Native_Deploy.Hash , Native_Deploy }
                };
                foreach (var m in ApplicationEngine.Services)
                {
                    callmethod[m] = m;
                }
            }
            if (callmethod.ContainsKey(method) == false)
            {
                throw new Exception($"Syscall not found: {method.ToString("X2")} (using base call)");
            }
            else
            {
                return base.OnSysCall(method);
            }
        }
    }
}
