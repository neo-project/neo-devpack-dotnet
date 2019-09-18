using Neo.Ledger;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MSIL.Utils
{
    class TestEngine : ExecutionEngine
    {
        public const int MaxStorageKeySize = 64;
        public const int MaxStorageValueSize = ushort.MaxValue;

        static IDictionary<string, BuildScript> scriptsAll = new Dictionary<string, BuildScript>();

        public readonly IDictionary<string, BuildScript> Scripts;

        public readonly IDictionary<StorageKey, StorageItem> Storages;

        public BuildScript ScriptEntry
        {
            get;
            private set;
        }

        public TestEngine()
        {
            Scripts = new Dictionary<string, BuildScript>();
            Storages = new Dictionary<StorageKey, StorageItem>();
        }

        public void AddAppcallScript(string filename, string specScriptID)
        {
            byte[] hex = NeonTestTool.HexString2Bytes(specScriptID);
            if (hex.Length != 20)
                throw new Exception("fail Script ID");

            if (scriptsAll.ContainsKey(filename) == false)
            {
                scriptsAll[filename] = NeonTestTool.BuildScript(filename);
            }

            Scripts[specScriptID.ToLower()] = scriptsAll[filename];
        }

        public void AddEntryScript(string filename)
        {
            if (scriptsAll.ContainsKey(filename) == false)
            {
                scriptsAll[filename] = NeonTestTool.BuildScript(filename);
            }

            ScriptEntry = scriptsAll[filename];
        }

        public class ContractMethod
        {
            TestEngine engine;
            string methodname;
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

        public RandomAccessStack<StackItem> ExecuteTestCaseStandard(string methodname, params StackItem[] _params)
        {
            //var engine = new ExecutionEngine();
            this.State = VMState.BREAK; // Required for allow to reuse the same TestEngine
            this.LoadScript(ScriptEntry.finalNEF);
            this.InvocationStack.Peek().InstructionPointer = 0;
            this.CurrentContext.EvaluationStack.Push(_params);
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

        public RandomAccessStack<StackItem> ExecuteTestCase(StackItem[] _params)
        {
            //var engine = new ExecutionEngine();
            this.LoadScript(ScriptEntry.finalNEF);
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
            if (method == InteropService.System_Contract_Call)
            {
                //a appcall
                return Contract_Call();
            }
            else if (method == InteropService.System_Runtime_Log)
            {
                return Contract_Log();
            }
            else if (method == InteropService.System_Runtime_Notify)
            {
                return Contract_Log();
            }
            // Storages
            else if (method == InteropService.System_Storage_GetContext)
            {
                return Contract_Storage_GetContext();
            }
            else if (method == InteropService.System_Storage_GetReadOnlyContext)
            {
                return Contract_Storage_GetReadOnlyContext();
            }
            else if (method == InteropService.System_Storage_Get)
            {
                return Contract_Storage_Get();
            }
            else if (method == InteropService.System_Storage_Delete)
            {
                return Contract_Storage_Delete();
            }
            else if (method == InteropService.System_Storage_Put)
            {
                return Contract_Storage_Put();
            }
            else if (method == InteropService.System_Runtime_GetInvocationCounter)
            {
                return Runtime_GetInvocationCounter();
            }
            else if (method == InteropService.System_Runtime_GetNotifications)
            {
                return Runtime_GetNotifications();
            }

            return base.OnSysCall(method);
        }

        private bool Runtime_GetNotifications()
        {
            byte[] data = CurrentContext.EvaluationStack.Pop().GetByteArray();
            if ((data.Length != 0) && (data.Length != UInt160.Length)) return false;

            IEnumerable<NotifyEventArgs> notifications = new NotifyEventArgs[]
            {
                new NotifyEventArgs(null, UInt160.Zero, new Integer(0x01)),
                new NotifyEventArgs(null, UInt160.Parse("0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"), new Integer(0x02))
            };

            if (data.Length == UInt160.Length) // must filter by scriptHash
            {
                var hash = new UInt160(data);
                notifications = notifications.Where(p => p.ScriptHash == hash);
            }

            CurrentContext.EvaluationStack.Push(notifications.Select(u => new VM.Types.Array(new StackItem[] { u.ScriptHash.ToArray(), u.State })).ToArray());
            return true;
        }

        private bool Runtime_GetInvocationCounter()
        {
            CurrentContext.EvaluationStack.Push(0x01);
            return true;
        }

        #region Storage

        private bool Contract_Storage_GetContext()
        {
            CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new TestStorageContext
            {
                ScriptHash = CurrentContext.ScriptHash(),
                IsReadOnly = false
            }));
            return true;
        }

        private bool Contract_Storage_GetReadOnlyContext()
        {
            CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new TestStorageContext
            {
                ScriptHash = CurrentContext.ScriptHash(),
                IsReadOnly = true
            }));
            return true;
        }

        private bool Contract_Storage_Delete()
        {
            if (CurrentContext.EvaluationStack.Pop() is InteropInterface _interface)
            {
                TestStorageContext context = _interface.GetInterface<TestStorageContext>();
                if (context.IsReadOnly) return false;

                StorageKey key = new StorageKey
                {
                    ScriptHash = context.ScriptHash,
                    Key = CurrentContext.EvaluationStack.Pop().GetByteArray()
                };
                if (Storages.TryGetValue(key, out var item) && item.IsConstant == true) return false;
                Storages.Remove(key);
                return true;
            }
            return false;
        }

        private bool Contract_Storage_Get()
        {
            if (CurrentContext.EvaluationStack.Pop() is InteropInterface _interface)
            {
                TestStorageContext context = _interface.GetInterface<TestStorageContext>();
                byte[] key = CurrentContext.EvaluationStack.Pop().GetByteArray();

                if (Storages.TryGetValue(new StorageKey
                {
                    ScriptHash = context.ScriptHash,
                    Key = key
                }, out var item))
                {
                    CurrentContext.EvaluationStack.Push(item.Value);
                }
                else
                {
                    CurrentContext.EvaluationStack.Push(new byte[0]);
                }
                return true;
            }
            return false;
        }

        private bool Contract_Storage_Put()
        {
            if (!(CurrentContext.EvaluationStack.Pop() is InteropInterface _interface))
                return false;
            TestStorageContext context = _interface.GetInterface<TestStorageContext>();
            byte[] key = CurrentContext.EvaluationStack.Pop().GetByteArray();
            byte[] value = CurrentContext.EvaluationStack.Pop().GetByteArray();
            return PutEx(context, key, value, StorageFlags.None);
        }

        private bool PutEx(TestStorageContext context, byte[] key, byte[] value, StorageFlags flags)
        {
            if (key.Length > MaxStorageKeySize) return false;
            if (value.Length > MaxStorageValueSize) return false;
            if (context.IsReadOnly) return false;

            StorageKey skey = new StorageKey
            {
                ScriptHash = context.ScriptHash,
                Key = key
            };

            if (Storages.TryGetValue(skey, out var item) && item.IsConstant == true) return false;

            if (value.Length == 0 && !flags.HasFlag(StorageFlags.Constant))
            {
                // If put 'value' is empty (and non-const), we remove it (implicit `Storage.Delete`)
                Storages.Remove(skey);
            }
            else
            {
                item = Storages[skey] = new StorageItem();
                item.Value = value;
                item.IsConstant = flags.HasFlag(StorageFlags.Constant);
            }
            return true;
        }

        #endregion

        private bool Contract_Call()
        {
            StackItem item0 = this.CurrentContext.EvaluationStack.Pop();
            var contractid = item0.GetByteArray();
            var contractkey = NeonTestTool.Bytes2HexString(contractid.Reverse().ToArray()).ToLower();
            var contract = Scripts[contractkey];

            if (contract is null) return false;
            StackItem item1 = this.CurrentContext.EvaluationStack.Pop();
            StackItem item2 = this.CurrentContext.EvaluationStack.Pop();
            ExecutionContext context_new = this.LoadScript(contract.finalNEF, 1);
            context_new.EvaluationStack.Push(item2);
            context_new.EvaluationStack.Push(item1);
            return true;
        }

        private bool Contract_Log()
        {
            StackItem item0 = this.CurrentContext.EvaluationStack.Pop();
            DumpItem(item0);
            return true;
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
