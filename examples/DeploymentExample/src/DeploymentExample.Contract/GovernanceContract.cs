using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace DeploymentExample.Contract
{
    [DisplayName("DeploymentExample.GovernanceContract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [ManifestExtra("Author", "Neo Community")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Governance Contract for managing the ecosystem")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractPermission("*", "*")]
    public class GovernanceContract : SmartContract
    {
        // Storage keys
        private const byte PREFIX_OWNER = 0x30;
        private const byte PREFIX_MANAGED_CONTRACTS = 0x31;
        private const byte PREFIX_PROPOSAL = 0x32;
        private const byte PREFIX_VOTE = 0x33;
        private const byte PREFIX_NEXT_PROPOSAL_ID = 0x34;
        private const byte PREFIX_TOKEN_CONTRACT = 0x35;
        private const byte PREFIX_VOTING_THRESHOLD = 0x36;
        private const byte PREFIX_VOTING_PERIOD = 0x37;

        // Default values
        private const BigInteger DEFAULT_VOTING_THRESHOLD = 51; // 51%
        private const ulong DEFAULT_VOTING_PERIOD = 7 * 24 * 3600; // 7 days in seconds

        // Proposal types
        private const byte PROPOSAL_TYPE_ADD_CONTRACT = 1;
        private const byte PROPOSAL_TYPE_REMOVE_CONTRACT = 2;
        private const byte PROPOSAL_TYPE_UPDATE_THRESHOLD = 3;
        private const byte PROPOSAL_TYPE_UPDATE_PERIOD = 4;
        private const byte PROPOSAL_TYPE_EXECUTE_ACTION = 5;

        // Events
        [DisplayName("ProposalCreated")]
        public static event Action<BigInteger, UInt160, byte, object> OnProposalCreated;

        [DisplayName("Voted")]
        public static event Action<UInt160, BigInteger, bool> OnVoted;

        [DisplayName("ProposalExecuted")]
        public static event Action<BigInteger, bool> OnProposalExecuted;

        [DisplayName("ContractManaged")]
        public static event Action<UInt160, bool> OnContractManaged;

        /// <summary>
        /// Deploy the governance contract
        /// </summary>
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (!update)
            {
                var args = (object[])data;
                var owner = (UInt160)args[0];
                var tokenContract = args.Length > 1 ? (UInt160)args[1] : UInt160.Zero;
                
                if (!owner.IsValid || owner.IsZero)
                {
                    throw new Exception("Invalid owner address");
                }
                
                // Set contract owner
                Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_OWNER }, owner);
                
                // Set token contract for voting power
                if (tokenContract.IsValid && !tokenContract.IsZero)
                {
                    Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_TOKEN_CONTRACT }, tokenContract);
                }
                
                // Set default voting parameters
                Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_VOTING_THRESHOLD }, DEFAULT_VOTING_THRESHOLD);
                Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_VOTING_PERIOD }, DEFAULT_VOTING_PERIOD);
                
                // Initialize proposal ID counter
                Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_NEXT_PROPOSAL_ID }, 1);
            }
        }

        /// <summary>
        /// Create a new proposal
        /// </summary>
        [DisplayName("createProposal")]
        public static BigInteger CreateProposal(byte proposalType, object data, string description)
        {
            var proposer = Runtime.CallingScriptHash;
            if (proposer == null || !Runtime.CheckWitness(proposer))
            {
                throw new Exception("Unauthorized");
            }

            // Check proposer has voting power
            var votingPower = GetVotingPower(proposer);
            if (votingPower == 0)
            {
                throw new Exception("No voting power");
            }

            // Validate proposal type
            if (proposalType < 1 || proposalType > 5)
            {
                throw new Exception("Invalid proposal type");
            }

            // Get next proposal ID
            var nextIdKey = new byte[] { PREFIX_NEXT_PROPOSAL_ID };
            var nextId = Storage.Get(Storage.CurrentContext, nextIdKey);
            var proposalId = nextId?.Length > 0 ? (BigInteger)nextId : 1;

            // Create proposal
            var proposal = new Map<string, object>();
            proposal["id"] = proposalId;
            proposal["proposer"] = proposer;
            proposal["type"] = proposalType;
            proposal["data"] = StdLib.Serialize(data);
            proposal["description"] = description;
            proposal["createdAt"] = Runtime.Time;
            proposal["endTime"] = Runtime.Time + GetVotingPeriod();
            proposal["yesVotes"] = 0;
            proposal["noVotes"] = 0;
            proposal["executed"] = false;

            // Store proposal
            var proposalKey = Helper.Concat(new byte[] { PREFIX_PROPOSAL }, proposalId.ToByteArray());
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            // Update next proposal ID
            Storage.Put(Storage.CurrentContext, nextIdKey, proposalId + 1);

            OnProposalCreated(proposalId, proposer, proposalType, data);
            return proposalId;
        }

        /// <summary>
        /// Vote on a proposal
        /// </summary>
        [DisplayName("vote")]
        public static bool Vote(BigInteger proposalId, bool support)
        {
            var voter = Runtime.CallingScriptHash;
            if (voter == null || !Runtime.CheckWitness(voter))
            {
                throw new Exception("Unauthorized");
            }

            // Get proposal
            var proposalKey = Helper.Concat(new byte[] { PREFIX_PROPOSAL }, proposalId.ToByteArray());
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            if (proposalData == null)
            {
                throw new Exception("Proposal does not exist");
            }

            var proposal = (Map<string, object>)StdLib.Deserialize(proposalData);
            
            // Check if voting period has ended
            if (Runtime.Time > (ulong)proposal["endTime"])
            {
                throw new Exception("Voting period has ended");
            }

            // Check if already executed
            if ((bool)proposal["executed"])
            {
                throw new Exception("Proposal already executed");
            }

            // Check if already voted
            var voteKey = Helper.Concat(Helper.Concat(new byte[] { PREFIX_VOTE }, proposalId.ToByteArray()), voter);
            var existingVote = Storage.Get(Storage.CurrentContext, voteKey);
            if (existingVote != null)
            {
                throw new Exception("Already voted");
            }

            // Get voting power
            var votingPower = GetVotingPower(voter);
            if (votingPower == 0)
            {
                throw new Exception("No voting power");
            }

            // Record vote
            Storage.Put(Storage.CurrentContext, voteKey, support ? 1 : 0);

            // Update vote counts
            if (support)
            {
                proposal["yesVotes"] = (BigInteger)proposal["yesVotes"] + votingPower;
            }
            else
            {
                proposal["noVotes"] = (BigInteger)proposal["noVotes"] + votingPower;
            }

            // Update proposal
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            OnVoted(voter, proposalId, support);
            return true;
        }

        /// <summary>
        /// Execute a proposal if it has passed
        /// </summary>
        [DisplayName("executeProposal")]
        public static bool ExecuteProposal(BigInteger proposalId)
        {
            // Get proposal
            var proposalKey = Helper.Concat(new byte[] { PREFIX_PROPOSAL }, proposalId.ToByteArray());
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            if (proposalData == null)
            {
                throw new Exception("Proposal does not exist");
            }

            var proposal = (Map<string, object>)StdLib.Deserialize(proposalData);
            
            // Check if already executed
            if ((bool)proposal["executed"])
            {
                throw new Exception("Proposal already executed");
            }

            // Check if voting period has ended
            if (Runtime.Time <= (ulong)proposal["endTime"])
            {
                throw new Exception("Voting period not ended");
            }

            // Calculate if proposal passed
            var yesVotes = (BigInteger)proposal["yesVotes"];
            var noVotes = (BigInteger)proposal["noVotes"];
            var totalVotes = yesVotes + noVotes;
            
            if (totalVotes == 0)
            {
                throw new Exception("No votes cast");
            }

            var threshold = GetVotingThreshold();
            var yesPercentage = yesVotes * 100 / totalVotes;
            
            if (yesPercentage < threshold)
            {
                proposal["executed"] = true;
                Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));
                OnProposalExecuted(proposalId, false);
                return false;
            }

            // Execute proposal based on type
            var proposalType = (byte)proposal["type"];
            var data = StdLib.Deserialize((ByteString)proposal["data"]);
            var success = false;

            switch (proposalType)
            {
                case PROPOSAL_TYPE_ADD_CONTRACT:
                    success = AddManagedContract((UInt160)data);
                    break;
                case PROPOSAL_TYPE_REMOVE_CONTRACT:
                    success = RemoveManagedContract((UInt160)data);
                    break;
                case PROPOSAL_TYPE_UPDATE_THRESHOLD:
                    success = SetVotingThreshold((BigInteger)data);
                    break;
                case PROPOSAL_TYPE_UPDATE_PERIOD:
                    success = SetVotingPeriod((ulong)data);
                    break;
                case PROPOSAL_TYPE_EXECUTE_ACTION:
                    var actionData = (object[])data;
                    var targetContract = (UInt160)actionData[0];
                    var method = (string)actionData[1];
                    var args = actionData.Length > 2 ? (object[])actionData[2] : new object[0];
                    success = ExecuteAction(targetContract, method, args);
                    break;
            }

            // Mark as executed
            proposal["executed"] = true;
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            OnProposalExecuted(proposalId, success);
            return success;
        }

        /// <summary>
        /// Add a managed contract
        /// </summary>
        private static bool AddManagedContract(UInt160 contractHash)
        {
            if (!contractHash.IsValid || contractHash.IsZero)
            {
                return false;
            }

            var key = Helper.Concat(new byte[] { PREFIX_MANAGED_CONTRACTS }, contractHash);
            Storage.Put(Storage.CurrentContext, key, 1);
            OnContractManaged(contractHash, true);
            return true;
        }

        /// <summary>
        /// Remove a managed contract
        /// </summary>
        private static bool RemoveManagedContract(UInt160 contractHash)
        {
            var key = Helper.Concat(new byte[] { PREFIX_MANAGED_CONTRACTS }, contractHash);
            Storage.Delete(Storage.CurrentContext, key);
            OnContractManaged(contractHash, false);
            return true;
        }

        /// <summary>
        /// Execute an action on a managed contract
        /// </summary>
        private static bool ExecuteAction(UInt160 targetContract, string method, object[] args)
        {
            // Check if contract is managed
            if (!IsManagedContract(targetContract))
            {
                return false;
            }

            try
            {
                Contract.Call(targetContract, method, CallFlags.All, args);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if a contract is managed
        /// </summary>
        [DisplayName("isManagedContract")]
        [Safe]
        public static bool IsManagedContract(UInt160 contractHash)
        {
            var key = Helper.Concat(new byte[] { PREFIX_MANAGED_CONTRACTS }, contractHash);
            var managed = Storage.Get(Storage.CurrentContext, key);
            return managed?.Length > 0;
        }

        /// <summary>
        /// Get voting power of an address
        /// </summary>
        [DisplayName("getVotingPower")]
        [Safe]
        public static BigInteger GetVotingPower(UInt160 account)
        {
            var tokenContract = GetTokenContract();
            if (tokenContract == UInt160.Zero)
            {
                // If no token contract, only owner has voting power
                return account == GetOwner() ? 1 : 0;
            }

            // Voting power is based on token balance
            return (BigInteger)Contract.Call(tokenContract, "balanceOf", CallFlags.ReadOnly, account);
        }

        /// <summary>
        /// Get owner
        /// </summary>
        [DisplayName("getOwner")]
        [Safe]
        public static UInt160 GetOwner()
        {
            var owner = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_OWNER });
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }

        /// <summary>
        /// Get token contract
        /// </summary>
        [DisplayName("getTokenContract")]
        [Safe]
        public static UInt160 GetTokenContract()
        {
            var contract = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_TOKEN_CONTRACT });
            return contract?.Length == 20 ? (UInt160)contract : UInt160.Zero;
        }

        /// <summary>
        /// Get voting threshold
        /// </summary>
        [DisplayName("getVotingThreshold")]
        [Safe]
        public static BigInteger GetVotingThreshold()
        {
            var threshold = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_VOTING_THRESHOLD });
            return threshold?.Length > 0 ? (BigInteger)threshold : DEFAULT_VOTING_THRESHOLD;
        }

        /// <summary>
        /// Set voting threshold (internal)
        /// </summary>
        private static bool SetVotingThreshold(BigInteger newThreshold)
        {
            if (newThreshold < 1 || newThreshold > 100)
            {
                return false;
            }

            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_VOTING_THRESHOLD }, newThreshold);
            return true;
        }

        /// <summary>
        /// Get voting period
        /// </summary>
        [DisplayName("getVotingPeriod")]
        [Safe]
        public static ulong GetVotingPeriod()
        {
            var period = Storage.Get(Storage.CurrentContext, new byte[] { PREFIX_VOTING_PERIOD });
            return period?.Length > 0 ? (ulong)(BigInteger)period : DEFAULT_VOTING_PERIOD;
        }

        /// <summary>
        /// Set voting period (internal)
        /// </summary>
        private static bool SetVotingPeriod(ulong newPeriod)
        {
            if (newPeriod < 3600) // Minimum 1 hour
            {
                return false;
            }

            Storage.Put(Storage.CurrentContext, new byte[] { PREFIX_VOTING_PERIOD }, newPeriod);
            return true;
        }

        /// <summary>
        /// Get proposal details
        /// </summary>
        [DisplayName("getProposal")]
        [Safe]
        public static Map<string, object> GetProposal(BigInteger proposalId)
        {
            var proposalKey = Helper.Concat(new byte[] { PREFIX_PROPOSAL }, proposalId.ToByteArray());
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            if (proposalData == null)
            {
                throw new Exception("Proposal does not exist");
            }

            return (Map<string, object>)StdLib.Deserialize(proposalData);
        }
    }
}