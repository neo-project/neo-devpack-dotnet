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
        public static void OnPayment(UInt160 from, BigInteger amount, object data)
        {
            if (AssetStorage.GetPaymentStatus())
            {
                if (ExecutionEngine.CallingScriptHash == NEO.Hash)
                    Mint(amount * TokensPerNEO);
                else if (ExecutionEngine.CallingScriptHash == GAS.Hash)
                    Mint(amount * TokensPerGAS);
                else
                    throw new Exception("Wrong calling script hash");
            }
            else
            {
                throw new Exception("Payment is disable on this contract!");
            }
        }

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

        private static bool Mint(BigInteger amount)
        {
            var totalSupply = TotalSupplyStorage.Get();
            if (totalSupply <= 0) throw new Exception("Contract not deployed.");

            var avaliable_supply = MaxSupply - totalSupply;

            if (amount <= 0) throw new Exception("Amount cannot be zero.");
            if (amount > avaliable_supply) throw new Exception("Insufficient supply for mint tokens.");

            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            AssetStorage.Increase(tx.Sender, amount);
            TotalSupplyStorage.Increase(amount);

            OnTransfer(null, tx.Sender, amount);
            return true;
        }
    }
}
