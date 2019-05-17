using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    internal class TestInteropService : IInteropService
    {
        private readonly Dictionary<uint, Func<ExecutionEngine, bool>> methods = new Dictionary<uint, Func<ExecutionEngine, bool>>();
        private readonly Dictionary<uint, long> prices = new Dictionary<uint, long>();

        public TestInteropService()
        {
            Register("System.Blockchain.GetContract", Blockchain_GetContract);
            Register("Neo.Blockchain.GetContract", Blockchain_GetContract);

            Register("AntShares.Transaction.GetInputs", Transaction_GetInputs);
            Register("AntShares.Input.GetHash", Input_GetHash);
            Register("AntShares.Input.GetIndex", Input_GetIndex);
            Register("AntShares.Output.GetIndex", Output_GetIndex);
            Register("AntShares.Output.GetHash", Output_GetHash);
            Register("AntShares.Blockchain.GetHeight", Blockchain_GetHeight);
            Register("AntShares.Blockchain.GetBlock", Blockchain_GetBlock);
            Register("AntShares.Header.GetTimestamp", Header_GetTimestamp);
            Register("AntShares.Storage.Put", Storage_Put);
            Register("AntShares.Storage.GetContext", Storage_GetContext);
            Register("AntShares.Storage.Get", Storage_Get);
            Register("AntShares.Storage.CurrentContext", Storage_GetContext);
            Register("AntShares.Storage.Delete", Storage_Delete);
            Register("Neo.Runtime.CheckWitness", Runtime_CheckWitness);
            Register("Neo.Transaction.GetInputs", Transaction_GetInputs);
            Register("Neo.Transaction.GetHash", Transaction_GetHash);
            Register("Neo.Input.GetHash", Input_GetHash);
            Register("Neo.Input.GetIndex", Input_GetIndex);
            Register("Neo.Output.GetIndex", Output_GetIndex);
            Register("Neo.Output.GetHash", Output_GetHash);
            Register("Neo.Blockchain.GetHeight", Blockchain_GetHeight);
            Register("Neo.Blockchain.GetBlock", Blockchain_GetBlock);
            Register("Neo.Blockchain.GetHeader", Blockchain_GetHeader);
            Register("Neo.Blockchain.GetTransaction", Blockchain_GetTransaction);

            Register("Neo.Header.GetHash", Header_GetHash);
            Register("Neo.Header.GetTimestamp", Header_GetTimestamp);
            Register("Neo.Storage.Put", Storage_Put);
            Register("Neo.Storage.GetContext", Storage_GetContext);
            Register("Neo.Storage.Get", Storage_Get);
            Register("Neo.Storage.CurrentContext", Storage_GetContext);
            Register("Neo.Storage.Delete", Storage_Delete);

            Register("Neo.Runtime.Notify", Runtime_Notify);
            Register("Neo.Runtime.Log", Runtime_Log);

            Register("Neo.Block.GetTransaction", Block_GetTransaction);
            Register("Neo.Block.GetTransactionCount", Block_GetTransactionCount);
            Register("Neo.Runtime.GetTrigger", Runtime_GetTrigger);

            Register("Neo.Runtime.Serialize", Runtime_Serialize);
            Register("Neo.Runtime.Deserialize", Runtime_Deserialize);
        }

        public bool Invoke(byte[] method, ExecutionEngine engine)
        {
            uint hash = method.Length == 4
                ? BitConverter.ToUInt32(method, 0)
                : Encoding.ASCII.GetString(method).ToInteropMethodHash();
            if (!methods.TryGetValue(hash, out Func<ExecutionEngine, bool> func)) return false;
            return func(engine);
        }

        protected void Register(string method, Func<ExecutionEngine, bool> handler)
        {
            methods.Add(method.ToInteropMethodHash(), handler);
        }

        protected void Register(string method, Func<ExecutionEngine, bool> handler, long price)
        {
            Register(method, handler);
            prices.Add(method.ToInteropMethodHash(), price);
        }

        internal enum StackItemType : byte
        {
            ByteArray = 0x00,
            Boolean = 0x01,
            Integer = 0x02,
            InteropInterface = 0x40,
            Array = 0x80,
            Struct = 0x81,
        }

        private void SerializeStackItem(StackItem item, BinaryWriter writer)
        {
            //switch (item)
            //{
            //    case ByteArray _:
            //        writer.Write((byte)StackItemType.ByteArray);
            //        writer.WriteVarBytes(item.GetByteArray());
            //        break;
            //    case VMBoolean _:
            //        writer.Write((byte)StackItemType.Boolean);
            //        writer.Write(item.GetBoolean());
            //        break;
            //    case Integer _:
            //        writer.Write((byte)StackItemType.Integer);
            //        writer.WriteVarBytes(item.GetByteArray());
            //        break;
            //    case InteropInterface _:
            //        throw new NotSupportedException();
            //    case VMArray array:
            //        if (array is Struct)
            //            writer.Write((byte)StackItemType.Struct);
            //        else
            //            writer.Write((byte)StackItemType.Array);
            //        writer.WriteVarInt(array.Count);
            //        foreach (StackItem subitem in array)
            //            SerializeStackItem(subitem, writer);
            //        break;
            //}
        }

        protected virtual bool Runtime_Serialize(ExecutionEngine engine)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                try
                {
                    SerializeStackItem(engine.CurrentContext.EvaluationStack.Pop(), writer);
                }
                catch (NotSupportedException)
                {
                    return false;
                }
                writer.Flush();
                engine.CurrentContext.EvaluationStack.Push(ms.ToArray());
            }
            return true;
        }

        private StackItem DeserializeStackItem(BinaryReader reader)
        {
            //StackItemType type = (StackItemType)reader.ReadByte();
            //switch (type)
            //{
            //    case StackItemType.ByteArray:
            //        return new ByteArray(reader.ReadVarBytes());
            //    case StackItemType.Boolean:
            //        return new VMBoolean(reader.ReadBoolean());
            //    case StackItemType.Integer:
            //        return new Integer(new BigInteger(reader.ReadVarBytes()));
            //    case StackItemType.Array:
            //    case StackItemType.Struct:
            //        VMArray array = type == StackItemType.Struct ? new Struct() : new VMArray();
            //        ulong count = reader.ReadVarInt();
            //        while (count-- > 0)
            //            array.Add(DeserializeStackItem(reader));
            //        return array;
            //    default:
            //        return null;
            //}
            return null;
        }

        protected virtual bool Runtime_Deserialize(ExecutionEngine engine)
        {
            byte[] data = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            using (MemoryStream ms = new MemoryStream(data, false))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                StackItem item = DeserializeStackItem(reader);
                if (item == null) return false;
                engine.CurrentContext.EvaluationStack.Push(item);
            }
            return true;
        }

        protected virtual bool Runtime_CheckWitness(ExecutionEngine engine)
        {
            _ = engine.CurrentContext.EvaluationStack.Pop();
            engine.CurrentContext.EvaluationStack.Push(true);
            return true;
        }

        //easy add for test
        protected virtual bool Runtime_Notify(ExecutionEngine engine)
        {
            var array = engine.CurrentContext.EvaluationStack.Pop() as VM.Types.Array;
            _ = Encoding.UTF8.GetString(array[0].GetByteArray());
            //var s2 = System.Text.Encoding.UTF8.GetString(array[1].GetByteArray());
            return true;
        }

        protected virtual bool Runtime_Log(ExecutionEngine engine)
        {
            var str = engine.CurrentContext.EvaluationStack.Pop().GetString();
            Console.WriteLine("log:" + str);
            return true;
        }

        protected virtual bool Runtime_GetTrigger(ExecutionEngine engine)
        {
            //var br = System.Windows.Forms.MessageBox.Show("选择入口", "yes =  0x00 no ==0x10 cancel==0x11", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
            StackItem item = 0;
            //if (br == System.Windows.Forms.DialogResult.Yes)
            //{
            //    item = 0;

            //}
            //if (br == System.Windows.Forms.DialogResult.No)
            //{
            //    item = 0x10;

            //}
            //if (br == System.Windows.Forms.DialogResult.Cancel)
            //{
            //    item = 0x11;

            //}
            engine.CurrentContext.EvaluationStack.Push(item);
            return true;
        }

        private static bool Transaction_GetInputs(ExecutionEngine engine)
        {
            TestTransaction tx = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestTransaction>();
            if (tx == null) return false;

            List<StackItem> array = new List<StackItem>();

            for (int i = 0; i < tx.Inputs.Count; i++)
            {
                array.Add(StackItem.FromInterface(tx.Inputs[i]));
            }
            StackItem _array = array.ToArray();

            engine.CurrentContext.EvaluationStack.Push(_array);


            return true;
        }

        private static bool Input_GetHash(ExecutionEngine engine)
        {
            var input = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestTxInput>();
            if (input == null) return false;
            engine.CurrentContext.EvaluationStack.Push(input.PrevHash);
            return true;
        }

        private static bool Output_GetHash(ExecutionEngine engine)
        {
            var v = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestTxOutput>();
            if (v == null) return false;
            engine.CurrentContext.EvaluationStack.Push(v.PrevHash);
            return true;
        }

        private static bool Input_GetIndex(ExecutionEngine engine)
        {
            var input = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestTxInput>();
            if (input == null) return false;
            engine.CurrentContext.EvaluationStack.Push((int)input.PrevIndex);
            return true;
        }

        private static bool Output_GetIndex(ExecutionEngine engine)
        {
            var v = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestTxOutput>();
            if (v == null) return false;
            engine.CurrentContext.EvaluationStack.Push((int)v.PrevIndex);
            return true;
        }

        private static bool Blockchain_GetHeight(ExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.Push((int)365);
            return true;
        }

        private static bool Blockchain_GetBlock(ExecutionEngine engine)
        {
            var b = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            _ = Encoding.UTF8.GetString(b);
            //var v = engine.EvaluationStack.Pop().GetBigInteger();

            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new TestBlock()));
            return true;
        }

        private static bool Blockchain_GetHeader(ExecutionEngine engine)
        {
            _ = engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            //string text = System.Text.Encoding.UTF8.GetString(b);
            //var v = engine.EvaluationStack.Pop().GetBigInteger();

            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new TestHeader()));
            return true;
        }

        private static bool Blockchain_GetTransaction(ExecutionEngine engine)
        {
            _ = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            //string text = System.Text.Encoding.UTF8.GetString(b);
            //var v = engine.EvaluationStack.Pop().GetBigInteger();

            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new TestTransaction()));
            return true;
        }

        private static bool Blockchain_GetContract(ExecutionEngine engine)
        {
            _ = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            engine.CurrentContext.EvaluationStack.Push(new byte[0]);
            return true;
        }

        private static bool Header_GetHash(ExecutionEngine engine)
        {
            var header = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestHeader>();
            //string text = System.Text.Encoding.UTF8.GetString(b);
            //var v = engine.EvaluationStack.Pop().GetBigInteger();

            engine.CurrentContext.EvaluationStack.Push(header.Hash);
            return true;
        }

        private static bool Transaction_GetHash(ExecutionEngine engine)
        {
            var header = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestTransaction>();
            //string text = System.Text.Encoding.UTF8.GetString(b);
            //var v = engine.EvaluationStack.Pop().GetBigInteger();

            engine.CurrentContext.EvaluationStack.Push(header.Hash);
            return true;
        }

        private static bool Block_GetTransaction(ExecutionEngine engine)
        {
            _ = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestBlock>();
            _ = (int)engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            engine.CurrentContext.EvaluationStack.Push(StackItem.FromInterface(new TestTransaction()));
            return true;
        }

        private static bool Block_GetTransactionCount(ExecutionEngine engine)
        {
            var block = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestBlock>();
            engine.CurrentContext.EvaluationStack.Push(block.TxCount);
            return true;
        }

        private static bool Header_GetTimestamp(ExecutionEngine engine)
        {
            _ = (engine.CurrentContext.EvaluationStack.Pop() as VM.Types.InteropInterface).GetInterface<TestBlock>();

            engine.CurrentContext.EvaluationStack.Push((uint)3655);
            return true;
        }

        private static bool Storage_Put(ExecutionEngine engine)
        {
            _ = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            _ = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            _ = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            //engine.EvaluationStack.Push((uint)3655);
            return true;
        }

        private static bool Storage_Get(ExecutionEngine engine)
        {
            _ = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();
            _ = engine.CurrentContext.EvaluationStack.Pop().GetByteArray();

            engine.CurrentContext.EvaluationStack.Push((uint)3655);
            return true;
        }

        private static bool Storage_Delete(ExecutionEngine engine)
        {
            _ = engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            _ = engine.CurrentContext.EvaluationStack.Pop().GetBigInteger();
            return true;
        }

        private static bool Storage_GetContext(ExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.Push((uint)3655);

            //engine.EvaluationStack.Push((uint)3655);
            return true;
        }
    }
}
