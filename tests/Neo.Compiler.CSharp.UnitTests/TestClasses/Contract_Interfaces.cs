using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    interface IStorageMap<TKey, TValue>
    {
        TValue this[TKey account] { get; set; }
    }

    class AccountStorage : IStorageMap<UInt160, BigInteger>
    {
        readonly StorageMap map;

        public AccountStorage()
        {
            map = new StorageMap(Storage.CurrentContext, nameof(AccountStorage));
        }

        public BigInteger this[UInt160 account]
        {
            get => (BigInteger)map.Get(account);
            set => map.Put(account, value);
        }
    }

    public class Contract_Interfaces : SmartContract.Framework.SmartContract
    {
        AccountStorage accountStorage = new();
        IStorageMap<UInt160, BigInteger> Accounts => accountStorage;

        public void TransferAll(UInt160 from, UInt160 to)
        {
            Accounts[to] = Accounts[from] + Accounts[to];
            Accounts[from] = 0;
        }

        public void SetValue(UInt160 addr, BigInteger amount)
        {
            Accounts[addr] = amount;
        }

        public BigInteger GetValue(UInt160 addr)
        {
            return Accounts[addr];
        }
    }
}
