using Neo.Consensus;
using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.Wallets;
using System.Collections.Generic;
using System.Linq;

namespace Neo.TestingEngine
{
    public class TestConsensusContext : ConsensusContext
    {
        public TestConsensusContext(ECPoint[] validators, Wallet wallet, IStore store) : base(wallet, store)
        {
            this.Validators = validators;
            this.CommitPayloads = new ConsensusPayload[validators.Length];
        }

        public void SetBlock(Block block)
        {
            this.Block = block;
        }

        public Block CreateBlock()
        {
            EnsureHeader();
            Contract contract = Contract.CreateMultiSigContract(M, Validators);
            ContractParametersContext sc = new ContractParametersContext(Block);

            var witness = new Witness();
            witness.InvocationScript = contract.Script;
            witness.VerificationScript = new byte[0];
            Block.Witness = witness;
            Block.Transactions = TransactionHashes.Select(p => Transactions[p]).ToArray();
            return Block;
        }

    }
}
