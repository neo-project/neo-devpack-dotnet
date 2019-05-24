using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    class TestEngine : ExecutionEngine
    {
        IDictionary<string, BuildScript> contracts;
        public TestEngine(IDictionary<string, BuildScript> contracts)
        {
            this.contracts = contracts;
        }
        public RandomAccessStack<StackItem> ExecuteTestCase(BuildScript entryscript, StackItem[] _params)
        {
            //var engine = new ExecutionEngine();
            this.LoadScript(entryscript.finalAVM);
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
            var contractkey = JsonTestTool.Bytes2HexString(contractid.Reverse().ToArray());
            var contract = contracts[contractkey];

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
