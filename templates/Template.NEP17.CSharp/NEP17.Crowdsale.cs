using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace Template.NEP17.CSharp
{
    public partial class NEP17 : SmartContract
    {
        private static BigInteger GetTransactionAmount(Notification notification)
        {
            // Only allow Transfer notifications
            if (notification.EventName != "Transfer") return 0;
            var state = notification.State;
            // Checks notification format
            if (state.Length != 3) return 0;
            // Check dest
            if ((Neo.UInt160)state[1] != ExecutionEngine.ExecutingScriptHash) return 0;
            // Amount
            var amount = (BigInteger)state[2];
            if (amount < 0) return 0;
            return amount;
        }

        private static bool _mint(BigInteger neo, BigInteger gas)
        {
            var totalSupply = TotalSupplyStorage.Get();
            if (totalSupply <= 0) throw new Exception("Contract not deployed.");

            var avaliable_supply = MaxSupply - totalSupply;

            var contribution = (neo * TokensPerNEO) + (gas * TokensPerGAS);
            if (contribution <= 0) throw new Exception("Contribution cannot be zero.");
            if (contribution > avaliable_supply) throw new Exception("Insufficient supply for mint tokens.");

            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            AssetStorage.Increase(tx.Sender, contribution);
            TotalSupplyStorage.Increase(contribution);

            OnTransfer(null, tx.Sender, contribution);
            return true;
        }
    }
}
