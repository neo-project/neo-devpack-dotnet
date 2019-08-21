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
            balances.Put(Owner(), MaxSupply());
            contract.Put("totalSupply", MaxSupply());

            // TODO event Transfer

            return true;
        }
    }
}