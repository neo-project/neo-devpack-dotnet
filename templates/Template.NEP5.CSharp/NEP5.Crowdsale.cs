using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        private static BigInteger GetTransactionAmount(object state)
        {
            var notification = (object[])state;
            // Checks notification format
            if (notification.Length != 4) return 0;
            // Only allow Transfer notifications
            if ((string)notification[0] != "Transfer") return 0;
            // Check dest
            if ((byte[])notification[2] != ExecutionEngine.ExecutingScriptHash) return 0;
            // Amount
            var amount = (BigInteger)notification[3];
            if (amount < 0) return 0;
            return amount;
        }

        public static bool Mint()
        {
            if (Runtime.InvocationCounter != 1)
            {
                Error("InvocationCounter must be 1.");
                return false;
            }

            var notifications = Runtime.GetNotifications();
            if (notifications.Length == 0)
            {
                Error("Contribution transaction not found.");
                return false;
            }

            BigInteger neo = 0;
            BigInteger gas = 0;

            for (int i = 0; i < notifications.Length; i++)
            {
                var notification = notifications[i];

                if (notification.ScriptHash == NeoToken)
                {
                    neo += GetTransactionAmount(notification.State);
                }
                else if (notification.ScriptHash == GasToken)
                {
                    gas += GetTransactionAmount(notification.State);
                }
            }

            var totalSupply = TotalSupplyStorage.Get();
            if (totalSupply <= 0)
            {
                Error("Contract not deployed.");
                return false;
            }

            var avaliable_supply = MaxSupply - totalSupply;

            var contribution = (neo * TokensPerNEO) + (gas * TokensPerGAS);
            if (contribution <= 0)
            {
                Error("Contribution cannot be zero.");
                return false;
            }
            if (contribution > avaliable_supply)
            {
                Error("Insufficient supply for mint tokens.");
                return false;
            }

            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            AssetStorage.Increase(tx.Sender, contribution);
            TotalSupplyStorage.Increase(contribution);

            OnTransfer(null, tx.Sender, contribution);
            return true;
        }
    }
}
