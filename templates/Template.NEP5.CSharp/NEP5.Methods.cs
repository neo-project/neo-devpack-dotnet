using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        public static BigInteger TotalSupply() => TotalSupplyStorage.Get();

        public static BigInteger BalanceOf(byte[] account) => AssetStorage.Get(account);

        public static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(from) || !ValidateAddress(to))
            {
                Error("The parameters from and to SHOULD be 20-byte addresses.");
                return false;
            }
            if (amount <= 0)
            {
                Error("The parameter amount MUST be greater than 0.");
                return false;
            }
            if (!IsPayable(to))
            {
                Error("Receiver cannot receive.");
                return false;
            }
            if (!Runtime.CheckWitness(from) && !from.Equals(ExecutionEngine.CallingScriptHash))
            {
                Error("No authorization.");
                return false;
            }
            if (AssetStorage.Get(from) < amount)
            {
                Error("Insufficient balance.");
                return false;
            }
            if (amount == 0 || from == to) return true;

            AssetStorage.Reduce(from, amount);

            AssetStorage.Increase(to, amount);

            OnTransfer(from, to, amount);
            return true;
        }
    }
}
