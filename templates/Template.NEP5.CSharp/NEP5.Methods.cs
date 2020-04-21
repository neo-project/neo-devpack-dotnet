using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        public static BigInteger TotalSupply()
        {
            StorageMap contract = Storage.CurrentContext.CreateMap(StoragePrefixContract);
            return contract.Get("totalSupply")?.ToBigInteger() ?? 0;
        }

        public static BigInteger BalanceOf(byte[] account)
        {
            if (!ValidateAddress(account)) throw new FormatException("The parameter 'account' SHOULD be 20-byte addresses.");

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            return balances.Get(account)?.ToBigInteger() ?? 0;
        }

        public static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            if (!ValidateAddress(from)) throw new FormatException("The parameter 'from' SHOULD be 20-byte addresses.");
            if (!ValidateAddress(to)) throw new FormatException("The parameters 'to' SHOULD be 20-byte addresses.");
            if (!IsPayable(to)) return false;
            if (amount <= 0) throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
            if (!Runtime.CheckWitness(from)) return false;

            StorageMap balances = Storage.CurrentContext.CreateMap(StoragePrefixBalance);
            BigInteger fromAmount = balances.Get(from).ToBigInteger();

            if (fromAmount < amount) return false;
            if (amount == 0 || from == to) return true;

            if (fromAmount == amount)
            {
                balances.Delete(from);
            }
            else
            {
                balances.Put(from, fromAmount - amount);
            }

            BigInteger toAmount = balances.Get(to)?.ToBigInteger() ?? 0;
            balances.Put(to, toAmount + amount);

            OnTransfer(from, to, amount);
            return true;
        }
    }
}
