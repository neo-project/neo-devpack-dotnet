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
            return (BigInteger)notification[3];
        }

        private static bool Mint()
        {
            if (Runtime.InvocationCounter != 1)
                throw new Exception();

            var notifications = Runtime.GetNotifications();
            if (notifications.Length == 0)
                throw new Exception("Contribution transaction not found");

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

            StorageMap contract = Storage.CurrentContext.CreateMap(GetStoragePrefixContract());
            var supply = contract.Get("totalSupply");
            if (supply == null)
                throw new Exception("Contract not deployed");

            var current_supply = supply.ToBigInteger();
            var avaliable_supply = MaxSupply() - current_supply;

            var contribution = (neo * TokensPerNEO()) + (gas * TokensPerGAS());
            if (contribution <= 0)
                throw new Exception();
            if (contribution > avaliable_supply)
                throw new Exception();

            StorageMap balances = Storage.CurrentContext.CreateMap(GetStoragePrefixBalance());
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            var balance = balances.Get(tx.Sender)?.ToBigInteger() ?? 0;
            balances.Put(tx.Sender, balance + contribution);
            contract.Put("totalSupply", current_supply + contribution);

            OnTransfer(null, tx.Sender, balance + contribution);
            return true;
        }
    }
}
