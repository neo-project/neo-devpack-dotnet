using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        public static bool Deploy()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            if (TotalSupplyStorage.Get() > 0) throw new Exception("Contract has been deployed.");

            TotalSupplyStorage.Increase(InitialSupply);
            AssetStorage.Increase(Owner, InitialSupply);

            OnTransfer(null, Owner, InitialSupply);
            return true;
        }

        public static bool Update(byte[] script, string manifest)
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            if (script.Length == 0 || manifest.Length == 0) return false;
            if (script != null && script.Equals(Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash))) return true;
            Contract.Update(script, manifest);
            return true;
        }

        public static bool Destroy()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            Contract.Destroy();
            return true;
        }

        private static bool IsOwner() => Runtime.CheckWitness(Owner);
    }
}
