using Neo.Compiler.MSIL.Utils;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    public class TestEngine : ApplicationEngine
    {
        public const long TestGas = 2000_000_000;

        static readonly IDictionary<string, BuildScript> scriptsAll = new Dictionary<string, BuildScript>();

        public readonly IDictionary<string, BuildScript> Scripts;

        public BuildScript ScriptEntry { get; private set; }

        public TestEngine(TriggerType trigger = TriggerType.Application, IVerifiable verificable = null, StoreView snapshot = null, Block persistingBlock = null)
            : base(trigger, verificable, snapshot ?? new TestSnapshot(), persistingBlock, TestGas)
        {
            Scripts = new Dictionary<string, BuildScript>();
        }

        public BuildScript Build(string filename, bool releaseMode = false, bool optimizer = true)
        {
            var contains = scriptsAll.ContainsKey(filename);

            if (!contains || (contains && scriptsAll[filename].UseOptimizer != optimizer))
            {
                if (Path.GetExtension(filename).ToLowerInvariant() == ".nef")
                {
                    var fileNameManifest = filename;
                    using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
                    {
                        NefFile neffile = new NefFile();
                        neffile.Deserialize(reader);
                        fileNameManifest = fileNameManifest.Replace(".nef", ".manifest.json");
                        string manifestFile = File.ReadAllText(fileNameManifest);
                        BuildScript buildScriptNef = new BuildNEF(neffile, manifestFile);
                        scriptsAll[filename] = buildScriptNef;
                    }
                }
                else
                {
                    scriptsAll[filename] = NeonTestTool.BuildScript(filename, releaseMode, optimizer);
                }
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
            while (this.ResultStack.Count > 0)
            {
                this.ResultStack.Pop();
            }
            if (ScriptEntry != null) this.LoadScript(ScriptEntry.finalNEFScript);
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
            var methods = this.ScriptEntry.finalABI["methods"] as JArray;
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
            var methods = this.ScriptEntry.finalABI["methods"] as JArray;
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
            var offset = GetMethodEntryOffset(methodname);
            if (offset == -1) throw new Exception("Can't find method : " + methodname);
            var rvcount = GetMethodReturnCount(methodname);
            if (rvcount == -1) throw new Exception("Can't find method return count : " + methodname);
            return ExecuteTestCaseStandard(offset, (ushort)rvcount, args);
        }

        public EvaluationStack ExecuteTestCaseStandard(int offset, ushort rvcount, params StackItem[] args)
        {
            var context = InvocationStack.Pop();
            context = CreateContext(context.Script, (ushort)args.Length, rvcount, offset);
            LoadContext(context);
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

        protected override void OnFault(Exception e)
        {
            base.OnFault(e);
            Console.WriteLine(e.ToString());
        }

        public EvaluationStack ExecuteTestCase(params StackItem[] args)
        {
            if (CurrentContext.InstructionPointer != 0)
            {
                var context = InvocationStack.Pop();
                LoadContext(context.Clone(0));
            }
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

        public void SendTestNotification(UInt160 hash, string eventName, VM.Types.Array state)
        {
            typeof(ApplicationEngine).GetMethod("SendNotification", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(this, new object[] { hash, eventName, state });
        }

        static Dictionary<uint, InteropDescriptor> callmethod;

        public void ClearNotifications()
        {
            typeof(ApplicationEngine).GetField("notifications", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, null);
        }

        protected override void OnSysCall(uint method)
        {
            if (callmethod == null)
            {
                callmethod = new Dictionary<uint, InteropDescriptor>();
                foreach (var m in Services)
                {
                    callmethod[m.Key] = m.Value;
                }
            }
            if (callmethod.ContainsKey(method) == false)
            {
                throw new Exception($"Syscall not found: {method:X2} (using base call)");
            }
            else
            {
                base.OnSysCall(method);
            }
        }
    }
}
