using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Break : SmartContract.Framework.SmartContract
    {
        public static void BreakInTryCatch()
        {
            Storage.Put("\xff\x00", "\x00");
            foreach (object i in Storage.Find("\xff"))
                try { break; }
                finally { Storage.Put("\xff\x00", "\x01"); }
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x01");
            foreach (object i in Storage.Find("\xff"))
                try { throw new System.Exception(); }
                catch
                {
                    do { break; }
                    while (true);
                    Storage.Put("\xff\x00", "\x00");
                    break;  // break foreach; should execute finally
                }
                finally
                {
                    ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x00");
                    foreach (int _ in new int[] { 0, 1, 2 })
                    {
                        Storage.Put("\xff\x00", "\x02");
                        break;
                    }
                }
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x02");
            foreach (object i in Storage.Find("\xff"))
                try
                {
                    for (int j = 0; j < 3;)
                        break;
                    throw new System.Exception();
                }
                catch
                {
                    int j = 0;
                    while (j < 3)
                        break;
                    break;  // foreach; should execute finally
                }
                finally
                {
                    ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x02");
                    Storage.Put("\xff\x00", "\x03");
                }
            ExecutionEngine.Assert(Storage.Get("\xff\x00")! == "\x03");
        }
    }
}
