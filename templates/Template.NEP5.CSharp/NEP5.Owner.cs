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
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }

            if (TotalSupplyStorage.Get() > 0)
            {
                Error("Contract has been deployed.");
                return false;
            }

            TotalSupplyStorage.Increase(InitialSupply);

            OnTransfer(null, Owner, InitialSupply);
            return true;
        }

        public static bool Migrate(byte[] script, string manifest)
        {
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }
            if (script.Length == 0 || manifest.Length == 0) return false;
            if (script != null && script.Equals(Blockchain.GetContract(ExecutionEngine.ExecutingScriptHash))) return true;
            Contract.Update(script, manifest);
            return true;
        }

        public static bool Destroy()
        {
            if (!IsOwner())
            {
                Error("No authorization.");
                return false;
            }

            Contract.Destroy();
            return true;
        }

        private static bool IsOwner() => Runtime.CheckWitness(Owner);
    }
}
