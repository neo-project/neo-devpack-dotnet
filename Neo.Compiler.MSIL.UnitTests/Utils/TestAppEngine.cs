using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    partial class TestAppEngine : ExecutionEngine
    {

        static TestAppEngine()
        {
            methods = new Dictionary<uint, Func<TestAppEngine, bool>>();
            scripts = new Dictionary<string, byte[]>();
            InitInteropService();
        }
        private static Dictionary<string, byte[]> scripts;
        static string bytes2hexstr(byte[] data)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("X02"));
            }
            return sb.ToString();
        }


        //when got a syscall
        protected override bool OnSysCall(uint method)
        {
            if (!methods.TryGetValue(method, out Func<TestAppEngine, bool> func))
                return false;
            return func(this);
        }
        //after run opcode
        protected override bool PostExecuteInstruction(Instruction instruction)
        {
            return true;
        }
        //before run opcode
        protected override bool PreExecuteInstruction()
        {
            return true;
        }
    }
}
