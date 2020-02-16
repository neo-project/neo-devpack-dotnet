using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.MSIL.Utils
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

            Native_Deploy = (InteropDescriptor)typeof(InteropService)
                    .GetNestedType("Native", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .GetField("Deploy")
                    .GetValue(null);
        }


        public const int MaxStorageKeySize = 64;
        public const int MaxStorageValueSize = ushort.MaxValue;

        static IDictionary<string, BuildScript> scriptsAll = new Dictionary<string, BuildScript>();

        public readonly IDictionary<string, BuildScript> Scripts;

        public BuildScript ScriptEntry { get; private set; }

        public TestEngine(TriggerType trigger = TriggerType.Application, IVerifiable verificable = null, StoreView snapshot = null)
            : base(trigger, verificable, snapshot == null ? new TestSnapshot() : snapshot, 0, true)
        {
            Scripts = new Dictionary<string, BuildScript>();
        }

        public BuildScript Build(string filename, bool releaseMode = false)
        {
            if (scriptsAll.ContainsKey(filename) == false)
            {
                scriptsAll[filename] = NeonTestTool.BuildScript(filename, releaseMode);
            }

            return scriptsAll[filename];
        }

        public void AddEntryScript(string filename, bool releaseMode = false)
        {
            ScriptEntry = Build(filename, releaseMode);
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
        }

        public ContractMethod GetMethod(string methodname)
        {
            return new ContractMethod(this, methodname);
        }

        public EvaluationStack ExecuteTestCaseStandard(string methodname, params StackItem[] args)
        {
            this.InvocationStack.Peek().InstructionPointer = 0;
            this.Push(new VM.Types.Array(this.ReferenceCounter, args));
            this.Push(methodname);
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
                foreach (var m in InteropService.SupportedMethods())
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
