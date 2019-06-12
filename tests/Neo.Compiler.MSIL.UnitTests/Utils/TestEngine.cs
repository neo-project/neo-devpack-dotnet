using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    class TestEngine : ExecutionEngine
    {
        static IDictionary<string, BuildScript> scriptsAll= new Dictionary<string, BuildScript>();

        IDictionary<string, BuildScript> scripts = new Dictionary<string, BuildScript>();
        BuildScript scriptEntry =null;


        public void AddAppcallScript(string filename,string specScriptID)
        {
            byte[] hex = NeonTestTool.HexString2Bytes(specScriptID);
            if (hex.Length != 20)
                throw new Exception("fail Script ID");

            if(scriptsAll.ContainsKey(filename)==false)
            {
                scriptsAll[filename]= NeonTestTool.BuildScript(filename);
            }

            scripts[specScriptID.ToLower()] = scriptsAll[filename];
        }
        
        public void AddEntryScript(string filename)
        {
            if (scriptsAll.ContainsKey(filename) == false)
            {
                scriptsAll[filename] = NeonTestTool.BuildScript(filename);
            }

            scriptEntry = scriptsAll[filename];
        }
        public RandomAccessStack<StackItem> ExecuteTestCase(StackItem[] _params)
        {
            //var engine = new ExecutionEngine();
            this.LoadScript(scriptEntry.finalAVM);
            this.InvocationStack.Peek().InstructionPointer = 0;
            if (_params != null)
            {
                for (var i = _params.Length - 1; i >= 0; i--)
                {
                    this.CurrentContext.EvaluationStack.Push(_params[i]);
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
            var stack = this.ResultStack;
            return stack;
        }
        protected override bool OnSysCall(uint method)
        {
            if (method == Neo.SmartContract.InteropService.System_Contract_Call)
            {
                //a appcall
                return Contract_Call();
            }
            return base.OnSysCall(method);
        }

        private bool Contract_Call()
        {
            StackItem item0 = this.CurrentContext.EvaluationStack.Pop();
            var contractid = item0.GetByteArray();
            var contractkey = NeonTestTool.Bytes2HexString(contractid.Reverse().ToArray()).ToLower();
            var contract = scripts[contractkey];

            if (contract is null) return false;
            StackItem item1 = this.CurrentContext.EvaluationStack.Pop();
            StackItem item2 = this.CurrentContext.EvaluationStack.Pop();
            ExecutionContext context_new = this.LoadScript(contract.finalAVM, 1);
            context_new.EvaluationStack.Push(item2);
            context_new.EvaluationStack.Push(item1);
            return true;
        }
    }
}
