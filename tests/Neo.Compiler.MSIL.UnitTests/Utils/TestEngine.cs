using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.MSIL.Utils
{
    public class TestEngine : ApplicationEngine
    {
        public const int MaxStorageKeySize = 64;
        public const int MaxStorageValueSize = ushort.MaxValue;

        static IDictionary<string, BuildScript> scriptsAll = new Dictionary<string, BuildScript>();

        public readonly IDictionary<string, BuildScript> Scripts;

        public BuildScript ScriptEntry { get; private set; }

        public TestEngine(TriggerType trigger = TriggerType.Application, IVerifiable verificable = null, Snapshot snapshot = null)
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

<<<<<<< HEAD
=======
            ScriptEntry = scriptsAll[filename];
            Reset();
        }
<<<<<<< HEAD
>>>>>>> 9129587... UT optimization (#117)
=======

>>>>>>> 6d44ce5... Update TestEngine.cs
        public void Reset()
        {
            this.State = VMState.BREAK; // Required for allow to reuse the same TestEngine
            this.InvocationStack.Clear();
            this.LoadScript(ScriptEntry.finalNEF);
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

        public RandomAccessStack<StackItem> ExecuteTestCaseStandard(string methodname, params StackItem[] args)
        {
<<<<<<< HEAD
            //var engine = new ExecutionEngine();
=======
>>>>>>> 9129587... UT optimization (#117)
            this.InvocationStack.Peek().InstructionPointer = 0;
            this.CurrentContext.EvaluationStack.Push(args);
            this.CurrentContext.EvaluationStack.Push(methodname);
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

        public RandomAccessStack<StackItem> ExecuteTestCase(params StackItem[] args)
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
            var stack = this.ResultStack;
            return stack;
        }

        protected override bool OnSysCall(uint method)
        {
            if (
               // Native
               method == InteropService.Neo_Native_Deploy ||
               // Account
               method == InteropService.Neo_Account_IsStandard ||
               // Storages
               method == InteropService.System_Storage_GetContext ||
               method == InteropService.System_Storage_GetReadOnlyContext ||
               method == InteropService.System_Storage_GetReadOnlyContext ||
               method == InteropService.System_StorageContext_AsReadOnly ||
               method == InteropService.System_Storage_Get ||
               method == InteropService.System_Storage_Delete ||
               method == InteropService.System_Storage_Put ||
               // Enumerator
               method == InteropService.Neo_Enumerator_Concat ||
               method == InteropService.Neo_Enumerator_Create ||
               method == InteropService.Neo_Enumerator_Next ||
               method == InteropService.Neo_Enumerator_Value ||
               // Iterator
               method == InteropService.Neo_Iterator_Concat ||
               method == InteropService.Neo_Iterator_Create ||
               method == InteropService.Neo_Iterator_Key ||
               method == InteropService.Neo_Iterator_Keys ||
               method == InteropService.Neo_Iterator_Values ||
               // ExecutionEngine
               method == InteropService.System_ExecutionEngine_GetCallingScriptHash ||
               method == InteropService.System_ExecutionEngine_GetEntryScriptHash ||
               method == InteropService.System_ExecutionEngine_GetExecutingScriptHash ||
               method == InteropService.System_ExecutionEngine_GetScriptContainer ||
               // Runtime
               method == InteropService.System_Runtime_CheckWitness ||
               method == InteropService.System_Runtime_GetNotifications ||
               method == InteropService.System_Runtime_GetInvocationCounter ||
               method == InteropService.System_Runtime_GetTrigger ||
               method == InteropService.System_Runtime_GetTime ||
               method == InteropService.System_Runtime_Platform ||
               method == InteropService.System_Runtime_Log ||
               method == InteropService.System_Runtime_Notify ||
               // Json
               method == InteropService.Neo_Json_Deserialize ||
               method == InteropService.Neo_Json_Serialize ||
               // Crypto
               method == InteropService.Neo_Crypto_ECDsaVerify ||
               method == InteropService.Neo_Crypto_ECDsaCheckMultiSig ||
               // Blockchain
               method == InteropService.System_Blockchain_GetHeight ||
               method == InteropService.System_Blockchain_GetBlock ||
               method == InteropService.System_Blockchain_GetContract ||
               method == InteropService.System_Blockchain_GetTransaction ||
               method == InteropService.System_Blockchain_GetTransactionHeight ||
               method == InteropService.System_Blockchain_GetTransactionFromBlock ||
               // Native
               method == NativeContract.NEO.ServiceName.ToInteropMethodHash() ||
               method == NativeContract.GAS.ServiceName.ToInteropMethodHash() ||
               method == NativeContract.Policy.ServiceName.ToInteropMethodHash() ||
               // Contract
               method == InteropService.System_Contract_Call ||
               method == InteropService.System_Contract_Destroy ||
               method == InteropService.Neo_Contract_Create ||
               method == InteropService.Neo_Contract_Update
               )
            {
                return base.OnSysCall(method);
            }

            throw new Exception($"Syscall not found: {method.ToString("X2")} (using base call)");
        }

        public bool CheckAsciiChar(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToLower().ToCharArray()[i];
                if ((!((c >= 97 && c <= 123) || (c >= 48 && c <= 57) || c == 45 || c == 46 || c == 64 || c == 95)))
                    return false;
            }
            return true;
        }

        private void DumpItemShort(StackItem item, int space = 0)
        {
            var spacestr = "";
            for (var i = 0; i < space; i++) spacestr += "    ";
            var line = NeonTestTool.Bytes2HexString(item.GetByteArray());

            if (item is Neo.VM.Types.ByteArray)
            {
                var str = item.GetString();
                if (CheckAsciiChar(str))
                {
                    line += "|" + str;
                }
            }
            Console.WriteLine(spacestr + line);
        }

        private void DumpItem(StackItem item, int space = 0)
        {
            var spacestr = "";
            for (var i = 0; i < space; i++) spacestr += "    ";
            Console.WriteLine(spacestr + "got Param:" + item.GetType().ToString());

            if (item is Neo.VM.Types.Array || item is Neo.VM.Types.Struct)
            {
                var array = item as Neo.VM.Types.Array;
                for (var i = 0; i < array.Count; i++)
                {
                    var subitem = array[i];
                    DumpItem(subitem, space + 1);
                }
            }
            else if (item is Neo.VM.Types.Map)
            {
                var map = item as Neo.VM.Types.Map;
                foreach (var subitem in map)
                {
                    Console.WriteLine("---Key---");
                    DumpItemShort(subitem.Key, space + 1);
                    Console.WriteLine("---Value---");
                    DumpItem(subitem.Value, space + 1);
                }
            }
            else
            {
                Console.WriteLine(spacestr + "--as num:" + item.GetBigInteger());

                Console.WriteLine(spacestr + "--as bin:" + NeonTestTool.Bytes2HexString(item.GetByteArray()));
                if (item is Neo.VM.Types.ByteArray)
                {
                    var str = item.GetString();
                    if (CheckAsciiChar(str))
                    {
                        Console.WriteLine(spacestr + "--as str:" + item.GetString());
                    }
                }
            }
        }
    }
}
