using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        private static bool Deploy()
        {
            if (!Runtime.CheckWitness(Owner()))
            {
                return false;
            }

            StorageMap contract = Storage.CurrentContext.CreateMap(GetStoragePrefixContract());
            if (contract.Get("totalSupply").Length != 0)
                throw new Exception ("Contract already deployed");

            StorageMap balances = Storage.CurrentContext.CreateMap(GetStoragePrefixBalance());
            balances.Put(Owner(), InitialSupply());
            contract.Put("totalSupply", InitialSupply());

            OnTransfer(null, Owner(), InitialSupply());
            return true;
        }

        public static bool Migrate(byte[] script, ContractPropertyState manifest)
        {
            if (!Runtime.CheckWitness(Owner()))
            {
                return false;
            }
            // TODO Fix Contract.Migrate
            Contract.Migrate(script, manifest);
            return true;
        }

        public static bool Destroy()
        {
            if (!Runtime.CheckWitness(Owner()))
            {
                return false;
            }

            Contract.Destroy();
            return true;
        }
    }
}
