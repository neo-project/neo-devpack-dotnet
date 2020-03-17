using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        private static bool Deploy()
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            StorageMap contract = Storage.CurrentContext.CreateMap(GetStoragePrefixContract());
            if (contract.Get("totalSupply") != null)
                throw new Exception("Contract already deployed");

            StorageMap balances = Storage.CurrentContext.CreateMap(GetStoragePrefixBalance());
            balances.Put(Owner, InitialSupply());
            contract.Put("totalSupply", InitialSupply());

            OnTransfer(null, Owner, InitialSupply());
            return true;
        }

        public static bool Migrate(byte[] script, string manifest)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }
            if (script.Length == 0 || manifest.Length == 0)
            {
                return false;
            }
            Contract.Update(script, manifest);
            return true;
        }

        public static bool Destroy()
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            Contract.Destroy();
            return true;
        }
    }
}
