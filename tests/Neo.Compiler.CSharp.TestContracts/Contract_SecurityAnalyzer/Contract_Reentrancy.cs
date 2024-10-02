using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Reentrancy : SmartContract.Framework.SmartContract
    {
        public static void HasReentrancy()
        {
            try
            {
                Contract.Call(NEO.Hash, "transfer", CallFlags.All, [UInt160.Zero, UInt160.Zero, 0, null]);
            }
            catch
            {
                Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
            }
        }
        public static void NoReentrancy()
        {
            Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
            Contract.Call(NEO.Hash, "transfer", CallFlags.All, [UInt160.Zero, UInt160.Zero, 0, null]);
        }
    }
}
