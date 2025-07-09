using System;
using System.ComponentModel;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace DeploymentExample.Contracts
{
    [DisplayName("ExampleGovernance")]
    [ManifestExtra("Author", "Neo Deployment Example")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Example Governance Contract with Voting")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [ContractPermission("*", "transfer")]
    [ContractPermission("*", "balanceOf")]
    public class GovernanceContract : SmartContract
    {
        // Contract configuration
        private static readonly UInt160 Owner = "NiHURp5SrPnxKHVQNvpDcPVHZnCUXn3w7G".ToScriptHash();
        private static readonly ulong DefaultVotingPeriod = 7 * 24 * 3600 * 1000; // 7 days
        private static readonly byte DefaultQuorumPercentage = 30; // 30%
        private static readonly byte DefaultPassPercentage = 51; // 51%

        // Storage prefixes
        private const byte Prefix_Proposal = 0x01;
        private const byte Prefix_Vote = 0x02;
        private const byte Prefix_NextProposalId = 0x03;
        private const byte Prefix_VotingPower = 0x04;
        private const byte Prefix_Config = 0x05;
        private const byte Prefix_Delegate = 0x06;
        private const byte Prefix_ProposalCount = 0x07;
        private const byte Prefix_VotingToken = 0x08;

        // Proposal states
        private const byte ProposalState_Active = 0;
        private const byte ProposalState_Passed = 1;
        private const byte ProposalState_Rejected = 2;
        private const byte ProposalState_Executed = 3;
        private const byte ProposalState_Cancelled = 4;

        // Helper methods
        private static void RequireOwner()
        {
            if (!Runtime.CheckWitness(Owner))
                throw new Exception("Only owner can perform this action");
        }

        // Deploy
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            // Initialize configuration
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextProposalId }, 1);
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_ProposalCount }, 0);
            
            SetConfig("VotingPeriod", DefaultVotingPeriod);
            SetConfig("QuorumPercentage", DefaultQuorumPercentage);
            SetConfig("PassPercentage", DefaultPassPercentage);
            SetConfig("ProposalThreshold", 100_00000000); // 100 tokens required to create proposal

            // Set default voting token (can be updated later)
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_VotingToken }, UInt160.Zero);

            OnDeployed();
        }

        // Set voting token
        [DisplayName("setVotingToken")]
        public static void SetVotingToken(UInt160 tokenHash)
        {
            RequireOwner();

            if (!tokenHash.IsValid)
                throw new Exception("Invalid token hash");

            // Verify it's a valid NEP-17 token
            try
            {
                var symbol = (string)Contract.Call(tokenHash, "symbol", CallFlags.ReadOnly);
                if (string.IsNullOrEmpty(symbol))
                    throw new Exception("Invalid token contract");
            }
            catch
            {
                throw new Exception("Token contract not found or invalid");
            }

            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_VotingToken }, tokenHash);
            OnVotingTokenUpdated(tokenHash);
        }

        // Create proposal
        [DisplayName("createProposal")]
        public static BigInteger CreateProposal(
            string title, 
            string description, 
            string category,
            string ipfsHash,
            ulong votingPeriodOverride,
            byte[] actions)
        {
            var proposer = Runtime.CallingScriptHash;
            
            if (!Runtime.CheckWitness(proposer))
                throw new Exception("Invalid witness");

            if (string.IsNullOrEmpty(title) || title.Length > 100)
                throw new Exception("Invalid title (max 100 chars)");

            if (string.IsNullOrEmpty(description) || description.Length > 1000)
                throw new Exception("Invalid description (max 1000 chars)");

            // Check proposer has enough voting power
            var votingPower = GetVotingPower(proposer);
            var threshold = (BigInteger)GetConfig("ProposalThreshold");
            
            if (votingPower < threshold)
                throw new Exception($"Insufficient voting power. Required: {threshold}");

            // Get proposal ID
            var proposalIdBytes = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_NextProposalId });
            var proposalId = (BigInteger)proposalIdBytes;

            // Determine voting period
            var votingPeriod = votingPeriodOverride > 0 ? votingPeriodOverride : (ulong)GetConfig("VotingPeriod");

            // Create proposal data
            var proposal = new Map<string, object>();
            proposal["id"] = proposalId;
            proposal["proposer"] = proposer;
            proposal["title"] = title;
            proposal["description"] = description;
            proposal["category"] = category;
            proposal["ipfsHash"] = ipfsHash;
            proposal["startTime"] = Runtime.Time;
            proposal["endTime"] = Runtime.Time + votingPeriod;
            proposal["forVotes"] = 0;
            proposal["againstVotes"] = 0;
            proposal["abstainVotes"] = 0;
            proposal["totalVotes"] = 0;
            proposal["state"] = ProposalState_Active;
            proposal["actions"] = actions;

            // Store proposal
            var proposalKey = CreateProposalKey(proposalId);
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            // Increment counters
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_NextProposalId }, proposalId + 1);
            var currentCount = (BigInteger)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_ProposalCount });
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_ProposalCount }, currentCount + 1);

            OnProposalCreated(proposalId, proposer, title, category);

            return proposalId;
        }

        // Vote on proposal
        [DisplayName("vote")]
        public static void Vote(BigInteger proposalId, byte support)
        {
            // support: 0 = against, 1 = for, 2 = abstain
            if (support > 2)
                throw new Exception("Invalid vote option");

            var voter = Runtime.CallingScriptHash;
            
            if (!Runtime.CheckWitness(voter))
                throw new Exception("Invalid witness");

            var proposal = GetProposalData(proposalId);
            if (proposal == null)
                throw new Exception("Proposal not found");

            var state = (byte)proposal["state"];
            if (state != ProposalState_Active)
                throw new Exception("Proposal is not active");

            var endTime = (ulong)proposal["endTime"];
            if (Runtime.Time > endTime)
                throw new Exception("Voting period has ended");

            var votingPower = GetVotingPower(voter);
            if (votingPower <= 0)
                throw new Exception("No voting power");

            // Check if already voted
            var voteKey = CreateVoteKey(proposalId, voter);
            var existingVote = Storage.Get(Storage.CurrentContext, voteKey);
            if (existingVote.Length > 0)
                throw new Exception("Already voted");

            // Check delegation
            var delegateKey = new byte[] { Prefix_Delegate }.Concat(voter);
            var delegateData = Storage.Get(Storage.CurrentContext, delegateKey);
            if (delegateData.Length > 0)
            {
                var delegateInfo = (Map<string, object>)StdLib.Deserialize(delegateData);
                var delegatedTo = (UInt160)delegateInfo["delegatedTo"];
                if (delegatedTo != UInt160.Zero)
                    throw new Exception("Voting power is delegated");
            }

            // Record vote
            var voteData = new Map<string, object>();
            voteData["voter"] = voter;
            voteData["support"] = support;
            voteData["votingPower"] = votingPower;
            voteData["timestamp"] = Runtime.Time;
            
            Storage.Put(Storage.CurrentContext, voteKey, StdLib.Serialize(voteData));

            // Update vote counts
            var forVotes = (BigInteger)proposal["forVotes"];
            var againstVotes = (BigInteger)proposal["againstVotes"];
            var abstainVotes = (BigInteger)proposal["abstainVotes"];
            var totalVotes = (BigInteger)proposal["totalVotes"];

            switch (support)
            {
                case 0: // Against
                    proposal["againstVotes"] = againstVotes + votingPower;
                    break;
                case 1: // For
                    proposal["forVotes"] = forVotes + votingPower;
                    break;
                case 2: // Abstain
                    proposal["abstainVotes"] = abstainVotes + votingPower;
                    break;
            }
            
            proposal["totalVotes"] = totalVotes + votingPower;

            // Update proposal
            var proposalKey = CreateProposalKey(proposalId);
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            OnVoted(proposalId, voter, support, votingPower);
        }

        // Execute passed proposal
        [DisplayName("execute")]
        public static void Execute(BigInteger proposalId)
        {
            var proposal = GetProposalData(proposalId);
            if (proposal == null)
                throw new Exception("Proposal not found");

            var state = (byte)proposal["state"];
            if (state != ProposalState_Active)
                throw new Exception("Proposal is not active");

            var endTime = (ulong)proposal["endTime"];
            if (Runtime.Time <= endTime)
                throw new Exception("Voting period not ended");

            // Calculate results
            var forVotes = (BigInteger)proposal["forVotes"];
            var againstVotes = (BigInteger)proposal["againstVotes"];
            var totalVotes = (BigInteger)proposal["totalVotes"];

            // Check quorum
            var totalPower = GetTotalVotingPower();
            var quorumPercentage = (BigInteger)GetConfig("QuorumPercentage");
            var quorumRequired = totalPower * quorumPercentage / 100;
            
            if (totalVotes < quorumRequired)
            {
                proposal["state"] = ProposalState_Rejected;
                proposal["rejectionReason"] = "Quorum not reached";
            }
            else
            {
                // Check if proposal passed
                var passPercentage = (BigInteger)GetConfig("PassPercentage");
                var requiredForVotes = (forVotes + againstVotes) * passPercentage / 100;
                
                if (forVotes >= requiredForVotes)
                {
                    proposal["state"] = ProposalState_Passed;
                    
                    // Execute actions if any
                    var actions = (byte[])proposal["actions"];
                    if (actions.Length > 0)
                    {
                        try
                        {
                            ExecuteActions(actions);
                            proposal["state"] = ProposalState_Executed;
                            proposal["executionTime"] = Runtime.Time;
                        }
                        catch (Exception e)
                        {
                            proposal["state"] = ProposalState_Passed;
                            proposal["executionError"] = e.Message;
                        }
                    }
                }
                else
                {
                    proposal["state"] = ProposalState_Rejected;
                    proposal["rejectionReason"] = "Insufficient support";
                }
            }

            // Update proposal
            var proposalKey = CreateProposalKey(proposalId);
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            var newState = (byte)proposal["state"];
            if (newState == ProposalState_Executed)
                OnProposalExecuted(proposalId, Runtime.CallingScriptHash);
            else if (newState == ProposalState_Rejected)
                OnProposalRejected(proposalId, proposal["rejectionReason"].ToString());
            else
                OnProposalPassed(proposalId);
        }

        // Cancel proposal (only by proposer or owner)
        [DisplayName("cancelProposal")]
        public static void CancelProposal(BigInteger proposalId)
        {
            var proposal = GetProposalData(proposalId);
            if (proposal == null)
                throw new Exception("Proposal not found");

            var state = (byte)proposal["state"];
            if (state != ProposalState_Active)
                throw new Exception("Proposal is not active");

            var proposer = (UInt160)proposal["proposer"];
            
            if (!Runtime.CheckWitness(proposer) && !Runtime.CheckWitness(Owner))
                throw new Exception("Only proposer or owner can cancel");

            proposal["state"] = ProposalState_Cancelled;
            proposal["cancelledBy"] = Runtime.CallingScriptHash;
            proposal["cancelTime"] = Runtime.Time;

            var proposalKey = CreateProposalKey(proposalId);
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            OnProposalCancelled(proposalId, Runtime.CallingScriptHash);
        }

        // Delegate voting power
        [DisplayName("delegate")]
        public static void Delegate(UInt160 delegateTo)
        {
            var delegator = Runtime.CallingScriptHash;
            
            if (!Runtime.CheckWitness(delegator))
                throw new Exception("Invalid witness");

            if (!delegateTo.IsValid)
                throw new Exception("Invalid delegate address");

            if (delegator == delegateTo)
                throw new Exception("Cannot delegate to self");

            var delegateInfo = new Map<string, object>();
            delegateInfo["delegatedTo"] = delegateTo;
            delegateInfo["delegationTime"] = Runtime.Time;

            var delegateKey = new byte[] { Prefix_Delegate }.Concat(delegator);
            Storage.Put(Storage.CurrentContext, delegateKey, StdLib.Serialize(delegateInfo));

            OnDelegated(delegator, delegateTo);
        }

        // Remove delegation
        [DisplayName("undelegate")]
        public static void Undelegate()
        {
            var delegator = Runtime.CallingScriptHash;
            
            if (!Runtime.CheckWitness(delegator))
                throw new Exception("Invalid witness");

            var delegateKey = new byte[] { Prefix_Delegate }.Concat(delegator);
            Storage.Delete(Storage.CurrentContext, delegateKey);

            OnUndelegated(delegator);
        }

        // Get methods
        [DisplayName("getProposal")]
        [Safe]
        public static Map<string, object>? GetProposal(BigInteger proposalId)
        {
            return GetProposalData(proposalId);
        }

        [DisplayName("getProposalVote")]
        [Safe]
        public static Map<string, object>? GetProposalVote(BigInteger proposalId, UInt160 voter)
        {
            if (!voter.IsValid || voter.IsZero)
                return null;

            var voteKey = CreateVoteKey(proposalId, voter);
            var voteData = Storage.Get(Storage.CurrentContext, voteKey);
            
            if (voteData.Length == 0)
                return null;

            return (Map<string, object>)StdLib.Deserialize(voteData);
        }

        [DisplayName("getActiveProposals")]
        [Safe]
        public static BigInteger[] GetActiveProposals()
        {
            var activeProposals = new System.Collections.Generic.List<BigInteger>();
            var nextId = (BigInteger)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_NextProposalId });

            for (BigInteger i = 1; i < nextId; i++)
            {
                var proposal = GetProposalData(i);
                if (proposal != null)
                {
                    var state = (byte)proposal["state"];
                    if (state == ProposalState_Active)
                    {
                        activeProposals.Add(i);
                    }
                }
            }

            return activeProposals.ToArray();
        }

        [DisplayName("getVotingPower")]
        [Safe]
        public static BigInteger GetVotingPower(UInt160 account)
        {
            if (!account.IsValid || account.IsZero)
                return 0;

            // Check for delegation
            var delegateKey = new byte[] { Prefix_Delegate }.Concat(account);
            var delegateData = Storage.Get(Storage.CurrentContext, delegateKey);
            
            if (delegateData.Length > 0)
            {
                var delegateInfo = (Map<string, object>)StdLib.Deserialize(delegateData);
                var delegatedTo = (UInt160)delegateInfo["delegatedTo"];
                if (delegatedTo != UInt160.Zero)
                    return 0; // Delegated power, so this account has 0 voting power
            }

            // Get voting token
            var tokenHash = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_VotingToken });
            if (tokenHash.Length == 0 || (UInt160)tokenHash == UInt160.Zero)
            {
                // Use manually set voting power
                var powerKey = new byte[] { Prefix_VotingPower }.Concat(account);
                return (BigInteger)Storage.Get(Storage.CurrentContext, powerKey);
            }

            // Get balance from voting token
            try
            {
                return (BigInteger)Contract.Call((UInt160)tokenHash, "balanceOf", CallFlags.ReadOnly, account);
            }
            catch
            {
                return 0;
            }
        }

        [DisplayName("getTotalVotingPower")]
        [Safe]
        public static BigInteger GetTotalVotingPower()
        {
            var tokenHash = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_VotingToken });
            if (tokenHash.Length == 0 || (UInt160)tokenHash == UInt160.Zero)
            {
                // Sum all manually set voting power
                var total = new BigInteger(0);
                var iterator = Storage.Find(Storage.CurrentContext, new byte[] { Prefix_VotingPower }, FindOptions.RemovePrefix);
                while (iterator.Next())
                {
                    total += (BigInteger)iterator.Value;
                }
                return total;
            }

            // Get total supply from voting token
            try
            {
                return (BigInteger)Contract.Call((UInt160)tokenHash, "totalSupply", CallFlags.ReadOnly);
            }
            catch
            {
                return 0;
            }
        }

        // Configuration methods
        [DisplayName("getConfig")]
        [Safe]
        public static object GetConfig(string key)
        {
            var configKey = new byte[] { Prefix_Config } + key.ToByteArray();
            var data = Storage.Get(Storage.CurrentContext, configKey);
            
            if (data.Length == 0)
                return "";

            return StdLib.Deserialize(data);
        }

        [DisplayName("setConfig")]
        public static void SetConfig(string key, object value)
        {
            RequireOwner();

            if (string.IsNullOrEmpty(key))
                throw new Exception("Config key cannot be empty");

            // Validate specific configs
            switch (key)
            {
                case "VotingPeriod":
                    var period = (ulong)value;
                    if (period < 3600000) // Min 1 hour
                        throw new Exception("Voting period too short");
                    break;

                case "QuorumPercentage":
                case "PassPercentage":
                    var percentage = (BigInteger)value;
                    if (percentage < 1 || percentage > 100)
                        throw new Exception("Percentage must be between 1 and 100");
                    break;
            }

            var configKey = new byte[] { Prefix_Config } + key.ToByteArray();
            Storage.Put(Storage.CurrentContext, configKey, StdLib.Serialize(value));

            OnConfigUpdated(key, value);
        }

        // Manual voting power management (when no token is set)
        [DisplayName("setVotingPower")]
        public static void SetVotingPower(UInt160 account, BigInteger power)
        {
            RequireOwner();

            if (!account.IsValid || account.IsZero)
                throw new Exception("Invalid account");

            if (power < 0)
                throw new Exception("Power must be non-negative");

            var key = new byte[] { Prefix_VotingPower }.Concat(account);
            
            if (power == 0)
                Storage.Delete(Storage.CurrentContext, key);
            else
                Storage.Put(Storage.CurrentContext, key, power);

            OnVotingPowerUpdated(account, power);
        }

        // Get governance info
        [DisplayName("getInfo")]
        [Safe]
        public static Map<string, object> GetInfo()
        {
            var info = new Map<string, object>();
            info["owner"] = Owner;
            info["proposalCount"] = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_ProposalCount });
            info["nextProposalId"] = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_NextProposalId });
            info["votingToken"] = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_VotingToken });
            info["votingPeriod"] = GetConfig("VotingPeriod");
            info["quorumPercentage"] = GetConfig("QuorumPercentage");
            info["passPercentage"] = GetConfig("PassPercentage");
            info["proposalThreshold"] = GetConfig("ProposalThreshold");

            return info;
        }

        // Helper methods
        private static Map<string, object>? GetProposalData(BigInteger proposalId)
        {
            var proposalKey = CreateProposalKey(proposalId);
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            
            if (proposalData.Length == 0)
                return null;

            return (Map<string, object>)StdLib.Deserialize(proposalData);
        }

        private static byte[] CreateProposalKey(BigInteger proposalId)
        {
            return new byte[] { Prefix_Proposal }.Concat(proposalId.ToByteArray());
        }

        private static byte[] CreateVoteKey(BigInteger proposalId, UInt160 voter)
        {
            return new byte[] { Prefix_Vote }.Concat(proposalId.ToByteArray()).Concat(voter);
        }

        private static void ExecuteActions(byte[] actions)
        {
            // In a real implementation, this would deserialize and execute contract calls
            // For this example, we'll just validate the actions format
            if (actions.Length == 0)
                return;

            // Actions could be encoded as:
            // [contractHash, method, args][]
            // This is a placeholder for actual implementation
        }

        // Events
        [DisplayName("Deployed")]
        public static event Action OnDeployed;

        [DisplayName("ProposalCreated")]
        public static event Action<BigInteger, UInt160, string, string> OnProposalCreated;

        [DisplayName("Voted")]
        public static event Action<BigInteger, UInt160, byte, BigInteger> OnVoted;

        [DisplayName("ProposalPassed")]
        public static event Action<BigInteger> OnProposalPassed;

        [DisplayName("ProposalRejected")]
        public static event Action<BigInteger, string> OnProposalRejected;

        [DisplayName("ProposalExecuted")]
        public static event Action<BigInteger, UInt160> OnProposalExecuted;

        [DisplayName("ProposalCancelled")]
        public static event Action<BigInteger, UInt160> OnProposalCancelled;

        [DisplayName("Delegated")]
        public static event Action<UInt160, UInt160> OnDelegated;

        [DisplayName("Undelegated")]
        public static event Action<UInt160> OnUndelegated;

        [DisplayName("VotingTokenUpdated")]
        public static event Action<UInt160> OnVotingTokenUpdated;

        [DisplayName("VotingPowerUpdated")]
        public static event Action<UInt160, BigInteger> OnVotingPowerUpdated;

        [DisplayName("ConfigUpdated")]
        public static event Action<string, object> OnConfigUpdated;

        // Contract management
        [DisplayName("update")]
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            RequireOwner();
            ContractManagement.Update(nefFile, manifest, data);
        }

        [DisplayName("destroy")]
        public static void Destroy()
        {
            RequireOwner();
            ContractManagement.Destroy();
        }
    }
}