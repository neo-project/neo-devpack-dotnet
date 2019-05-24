using Neo.VM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    class TestEngine : ExecutionEngine
    {
        public TestEngine(IEnumerable<BuildScript> aboutscripts)
        {

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
            var state = this.Execute();
            var stack = this.ResultStack;
            return stack;
        }
        protected override bool OnSysCall(uint method)
        {
            return base.OnSysCall(method);
        }
    }
}
