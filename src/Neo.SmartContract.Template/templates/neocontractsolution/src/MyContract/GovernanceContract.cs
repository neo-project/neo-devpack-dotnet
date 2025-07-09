using System;
using System.ComponentModel;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace MyContract
{
    [DisplayName("MyContract")]
    [ManifestExtra("Author", "MyContract Author")]
    [ManifestExtra("Email", "developer@mycontract.com")]
    [ManifestExtra("Description", "Governance Contract with Voting")]
    [ContractSourceCode("https://github.com/mycompany/mycontract")]
    public class GovernanceContract : SmartContract
    {
        // Storage prefixes
        private const byte Prefix_Proposal = 0x01;
        private const byte Prefix_Vote = 0x02;
        private const byte Prefix_NextProposalId = 0x03;
        private const byte Prefix_VotingPower = 0x04;
        private const byte Prefix_Config = 0x05;

        // Configuration keys
        private const string Config_VotingPeriod = "VotingPeriod";
        private const string Config_QuorumPercentage = "QuorumPercentage";
        private const string Config_PassPercentage = "PassPercentage";

        // Default values
        private const ulong DefaultVotingPeriod = 7 * 24 * 3600 * 1000; // 7 days in milliseconds
        private const byte DefaultQuorumPercentage = 30; // 30%
        private const byte DefaultPassPercentage = 51; // 51%

        #if (enableSecurityFeatures)
        // Security: Contract owner for administrative functions
        private static readonly UInt160 Owner = "NYourOwnerAddressHere".ToScriptHash();
        
        private static void RequireOwner()
        {
            if (!Runtime.CheckWitness(Owner))
                throw new Exception("Only owner can perform this action");
        }
        #endif

        // Proposal structure
        public struct Proposal
        {
            public BigInteger Id;
            public UInt160 Proposer;
            public string Title;
            public string Description;
            public string IpfsHash; // For detailed documentation
            public ulong StartTime;
            public ulong EndTime;
            public BigInteger ForVotes;
            public BigInteger AgainstVotes;
            public BigInteger TotalVotes;
            public bool Executed;
            public byte[] Actions; // Serialized contract calls
        }

        // Deploy
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            // Initialize configuration
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextProposalId }, 1);
            SetConfig(Config_VotingPeriod, DefaultVotingPeriod);
            SetConfig(Config_QuorumPercentage, DefaultQuorumPercentage);
            SetConfig(Config_PassPercentage, DefaultPassPercentage);

            #if (enableSecurityFeatures)
            // Give owner initial voting power
            SetVotingPower(Owner, 1000000); // 1 million voting power
            #endif
        }

        // Create a new proposal
        [DisplayName("createProposal")]
        public static BigInteger CreateProposal(
            string title, 
            string description, 
            string ipfsHash,
            byte[] actions)
        {
            var proposer = (UInt160)Runtime.CallingScriptHash;
            
            if (!Runtime.CheckWitness(proposer))
                throw new Exception("Invalid witness");

            if (string.IsNullOrEmpty(title))
                throw new Exception("Title cannot be empty");

            if (GetVotingPower(proposer) <= 0)
                throw new Exception("No voting power");

            // Get proposal ID
            var proposalIdBytes = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_NextProposalId });
            var proposalId = (BigInteger)proposalIdBytes;

            // Create proposal
            var proposal = new Proposal
            {
                Id = proposalId,
                Proposer = proposer,
                Title = title,
                Description = description,
                IpfsHash = ipfsHash,
                StartTime = Runtime.Time,
                EndTime = Runtime.Time + GetVotingPeriod(),
                ForVotes = 0,
                AgainstVotes = 0,
                TotalVotes = 0,
                Executed = false,
                Actions = actions
            };

            // Store proposal
            var proposalKey = CreateProposalKey(proposalId);
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            // Increment proposal counter
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextProposalId }, proposalId + 1);

            OnProposalCreated(proposalId, proposer, title);

            return proposalId;
        }

        // Vote on a proposal
        [DisplayName("vote")]
        public static void Vote(BigInteger proposalId, bool support)
        {
            var voter = (UInt160)Runtime.CallingScriptHash;
            
            if (!Runtime.CheckWitness(voter))
                throw new Exception("Invalid witness");

            var proposal = GetProposal(proposalId);
            if (proposal == null)
                throw new Exception("Proposal not found");

            if (Runtime.Time < proposal.StartTime || Runtime.Time > proposal.EndTime)
                throw new Exception("Voting period has ended");

            if (proposal.Executed)
                throw new Exception("Proposal already executed");

            var votingPower = GetVotingPower(voter);
            if (votingPower <= 0)
                throw new Exception("No voting power");

            // Check if already voted
            var voteKey = CreateVoteKey(proposalId, voter);
            var existingVote = Storage.Get(Storage.CurrentContext, voteKey);
            if (existingVote.Length > 0)
                throw new Exception("Already voted");

            // Record vote
            Storage.Put(Storage.CurrentContext, voteKey, support ? (byte)1 : (byte)0);

            // Update vote counts
            if (support)
                proposal.ForVotes += votingPower;
            else
                proposal.AgainstVotes += votingPower;
            
            proposal.TotalVotes += votingPower;

            // Update proposal
            var proposalKey = CreateProposalKey(proposalId);
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            OnVoted(proposalId, voter, support, votingPower);
        }

        // Execute a passed proposal
        [DisplayName("execute")]
        public static void Execute(BigInteger proposalId)
        {
            var proposal = GetProposal(proposalId);
            if (proposal == null)
                throw new Exception("Proposal not found");

            if (proposal.Executed)
                throw new Exception("Proposal already executed");

            if (Runtime.Time <= proposal.EndTime)
                throw new Exception("Voting period not ended");

            // Check quorum
            var totalPower = GetTotalVotingPower();
            var quorumRequired = totalPower * GetQuorumPercentage() / 100;
            
            if (proposal.TotalVotes < quorumRequired)
                throw new Exception("Quorum not reached");

            // Check if proposal passed
            var passPercentage = GetPassPercentage();
            var requiredForVotes = proposal.TotalVotes * passPercentage / 100;
            
            if (proposal.ForVotes < requiredForVotes)
                throw new Exception("Proposal did not pass");

            // Mark as executed
            proposal.Executed = true;
            var proposalKey = CreateProposalKey(proposalId);
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            // Execute actions if any
            if (proposal.Actions.Length > 0)
            {
                // Deserialize and execute contract calls
                // This would need proper implementation based on action format
                ExecuteActions(proposal.Actions);
            }

            OnProposalExecuted(proposalId, (UInt160)Runtime.CallingScriptHash);
        }

        // Get proposal details
        [DisplayName("getProposal")]
        [Safe]
        public static Proposal? GetProposal(BigInteger proposalId)
        {
            var proposalKey = CreateProposalKey(proposalId);
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            
            if (proposalData.Length == 0)
                return null;

            return (Proposal)StdLib.Deserialize(proposalData);
        }

        // Get voting power
        [DisplayName("getVotingPower")]
        [Safe]
        public static BigInteger GetVotingPower(UInt160 account)
        {
            if (!account.IsValid || account.IsZero)
                return 0;

            var key = CreateVotingPowerKey(account);
            return (BigInteger)Storage.Get(Storage.CurrentContext, key);
        }

        // Set voting power (admin function)
        [DisplayName("setVotingPower")]
        public static void SetVotingPower(UInt160 account, BigInteger power)
        {
            #if (enableSecurityFeatures)
            RequireOwner();
            #endif

            if (!account.IsValid || account.IsZero)
                throw new Exception("Invalid account");

            if (power < 0)
                throw new Exception("Power must be non-negative");

            var key = CreateVotingPowerKey(account);
            
            if (power == 0)
                Storage.Delete(Storage.CurrentContext, key);
            else
                Storage.Put(Storage.CurrentContext, key, power);

            OnVotingPowerUpdated(account, power);
        }

        // Configuration getters
        [DisplayName("getVotingPeriod")]
        [Safe]
        public static ulong GetVotingPeriod()
        {
            return (ulong)GetConfig(Config_VotingPeriod);
        }

        [DisplayName("getQuorumPercentage")]
        [Safe]
        public static BigInteger GetQuorumPercentage()
        {
            return GetConfig(Config_QuorumPercentage);
        }

        [DisplayName("getPassPercentage")]
        [Safe]
        public static BigInteger GetPassPercentage()
        {
            return GetConfig(Config_PassPercentage);
        }

        // Configuration setters (admin)
        #if (enableSecurityFeatures)
        [DisplayName("setVotingPeriod")]
        public static void SetVotingPeriod(ulong period)
        {
            RequireOwner();
            
            if (period < 3600000) // Minimum 1 hour
                throw new Exception("Voting period too short");

            SetConfig(Config_VotingPeriod, period);
            OnConfigUpdated(Config_VotingPeriod, period);
        }

        [DisplayName("setQuorumPercentage")]
        public static void SetQuorumPercentage(BigInteger percentage)
        {
            RequireOwner();
            
            if (percentage < 1 || percentage > 100)
                throw new Exception("Invalid percentage");

            SetConfig(Config_QuorumPercentage, percentage);
            OnConfigUpdated(Config_QuorumPercentage, percentage);
        }

        [DisplayName("setPassPercentage")]
        public static void SetPassPercentage(BigInteger percentage)
        {
            RequireOwner();
            
            if (percentage < 1 || percentage > 100)
                throw new Exception("Invalid percentage");

            SetConfig(Config_PassPercentage, percentage);
            OnConfigUpdated(Config_PassPercentage, percentage);
        }
        #endif

        // Helper methods
        private static byte[] CreateProposalKey(BigInteger proposalId)
        {
            return new byte[] { Prefix_Proposal }.Concat(proposalId.ToByteArray());
        }

        private static byte[] CreateVoteKey(BigInteger proposalId, UInt160 voter)
        {
            return new byte[] { Prefix_Vote }.Concat(proposalId.ToByteArray()).Concat(voter);
        }

        private static byte[] CreateVotingPowerKey(UInt160 account)
        {
            return new byte[] { Prefix_VotingPower }.Concat(account);
        }

        private static byte[] CreateConfigKey(string key)
        {
            return new byte[] { Prefix_Config }.Concat(key.ToByteArray());
        }

        private static BigInteger GetConfig(string key)
        {
            var configKey = CreateConfigKey(key);
            return (BigInteger)Storage.Get(Storage.CurrentContext, configKey);
        }

        private static void SetConfig(string key, BigInteger value)
        {
            var configKey = CreateConfigKey(key);
            Storage.Put(Storage.CurrentContext, configKey, value);
        }

        private static BigInteger GetTotalVotingPower()
        {
            // In a real implementation, this would track total voting power
            // For now, return a placeholder
            return 10000000; // 10 million
        }

        private static void ExecuteActions(byte[] actions)
        {
            // Implementation would deserialize and execute contract calls
            // This is a placeholder for the actual implementation
        }

        // Events
        [DisplayName("ProposalCreated")]
        public static event Action<BigInteger, UInt160, string> OnProposalCreated;

        [DisplayName("Voted")]
        public static event Action<BigInteger, UInt160, bool, BigInteger> OnVoted;

        [DisplayName("ProposalExecuted")]
        public static event Action<BigInteger, UInt160> OnProposalExecuted;

        [DisplayName("VotingPowerUpdated")]
        public static event Action<UInt160, BigInteger> OnVotingPowerUpdated;

        #if (enableSecurityFeatures)
        [DisplayName("ConfigUpdated")]
        public static event Action<string, BigInteger> OnConfigUpdated;
        #endif

        // Update contract
        [DisplayName("update")]
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            #if (enableSecurityFeatures)
            RequireOwner();
            #endif
            
            ContractManagement.Update(nefFile, manifest, data);
        }

        // Destroy contract
        [DisplayName("destroy")]
        public static void Destroy()
        {
            #if (enableSecurityFeatures)
            RequireOwner();
            #endif
            
            ContractManagement.Destroy();
        }
    }
}