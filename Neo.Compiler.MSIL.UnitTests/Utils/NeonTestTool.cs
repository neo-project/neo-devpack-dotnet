using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Compiler.MSIL.Utils
{
    internal class NeonTestTool
    {
        private readonly ILModule modIL;
        private readonly ModuleConverter converterIL;
        private readonly byte[] finalAVM;

        public NeonTestTool(Stream fs, Stream fspdb)
        {
            var log = new DefLogger();
            this.modIL = new ILModule(log);
            try
            {
                modIL.LoadModule(fs, fspdb);
            }
            catch (Exception err)
            {
                log.Log("LoadModule Error:" + err.ToString());
                throw err;
            }

            converterIL = new ModuleConverter(log);
            ConvOption option = new ConvOption();
            try
            {
                converterIL.Convert(modIL, option);
                finalAVM = converterIL.outModule.Build();
            }
            catch (Exception err)
            {
                log.Log("Convert IL->ASM Error:" + err.ToString());
                throw err;
            }
        }

        public static NeonTestTool Build(string filename)
        {
            var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var srccode = File.ReadAllText(filename);
            var tree = CSharpSyntaxTree.ParseText(srccode);
            var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var comp = CSharpCompilation.Create("aaa.dll", new[] { tree }, new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(OpCodeAttribute).Assembly.Location)
            }, op);
            using (var streamDll = new MemoryStream())
            using (var streamPdb = new MemoryStream())
            {
                var result = comp.Emit(streamDll, streamPdb);
                Assert.IsTrue(result.Success);
                streamDll.Position = 0;
                streamPdb.Position = 0;
                return new NeonTestTool(streamDll, streamPdb);
            }
        }

        public string[] GetAllILFunction()
        {
            List<string> lists = new List<string>();
            foreach (var _class in modIL.mapType)
            {
                foreach (var method in _class.Value.methods)
                {
                    var name = method.Key;
                    lists.Add(name);
                }
            }
            return lists.ToArray();
        }

        public ILMethod FindMethod(string fromclass, string method)
        {
            foreach (var _class in modIL.mapType)
            {
                var indexbegin = _class.Key.LastIndexOf(".");
                var classname = _class.Key;
                if (indexbegin > 0)
                    classname = classname.Substring(indexbegin + 1);

                if (classname == fromclass)
                {
                    foreach (var _method in _class.Value.methods)
                    {
                        var indexmethodname = _method.Key.LastIndexOf("::");
                        var methodname = _method.Key.Substring(indexmethodname + 2);
                        var indexparams = methodname.IndexOf("(");
                        if (indexparams > 0)
                        {
                            methodname = methodname.Substring(0, indexparams);
                        }
                        if (methodname == method)
                            return _method.Value;
                    }
                }
            }
            return null;
        }

        public string GetFullMethodName(string fromclass, string method)
        {
            foreach (var _class in modIL.mapType)
            {
                var indexbegin = _class.Key.LastIndexOf("::");
                var classname = _class.Key.Substring(indexbegin + 2);
                if (classname == fromclass)
                {
                    foreach (var _method in _class.Value.methods)
                    {
                        var indexmethodname = _method.Key.LastIndexOf("::");
                        var methodname = _method.Key.Substring(indexmethodname + 2);
                        if (methodname == method)
                            return _method.Key;
                    }
                }
            }
            return null;
        }

        public NeoMethod GetNEOVMMethod(ILMethod method)
        {
            var neomethod = this.converterIL.methodLink[method];
            return neomethod;
        }

        public byte[] NeoMethodToBytes(NeoMethod method)
        {
            List<byte> bytes = new List<byte>();
            foreach (var c in method.body_Codes.Values)
            {
                bytes.Add((byte)c.code);
                if (c.bytes != null)
                    for (var i = 0; i < c.bytes.Length; i++)
                    {
                        bytes.Add(c.bytes[i]);
                    }
            }
            return bytes.ToArray();
        }

        private ExecutionEngine RunAVM(byte[] data, int addr = 0, StackItem[] _params = null)
        {
            var engine = new ExecutionEngine();
            engine.LoadScript(data);
            //從指定地址開始執行
            engine.InvocationStack.Peek().InstructionPointer = addr;
            if (_params != null)
            {
                for (var i = 0; i < _params.Length; i++)
                {
                    engine.CurrentContext.EvaluationStack.Push(_params[i]);
                }
            }
            engine.Execute();
            //while (((engine.State & VMState.FAULT) == 0) && ((engine.State & VMState.HALT) == 0))
            //{
            //    engine.ExecuteNext();
            //}
            return engine;
        }

        public ExecutionEngine RunScript(int addr, StackItem[] _params = null)
        {
            return RunAVM(finalAVM, addr, _params);
        }

        public ExecutionEngine RunMethodAsAStandaloneAVM(NeoMethod method, StackItem[] _params = null)
        {
            var bytes = NeoMethodToBytes(method);
            return RunAVM(bytes, 0, _params);
        }
    }
}
