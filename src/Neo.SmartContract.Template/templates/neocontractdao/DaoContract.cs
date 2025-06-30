using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Company.SmartContract
{
    [DisplayName("DaoContract")]
    [ManifestExtra("Author", "Your Name")]
    [ManifestExtra("Email", "your.email@example.com")]
    [ManifestExtra("Description", "A Decentralized Autonomous Organization")]
    [ContractPermission("*", "*")]
    public class DaoContract : SmartContract
    {
        #region Storage Prefixes

        private const byte Prefix_Admin = 0x01;
        private const byte Prefix_Member = 0x02;
        private const byte Prefix_Proposal = 0x03;
        private const byte Prefix_Vote = 0x04;
        private const byte Prefix_ProposalCount = 0x05;
        private const byte Prefix_VotingPower = 0x06;
        private const byte Prefix_Settings = 0x07;

        #endregion

        #region Events

        [DisplayName("ProposalCreated")]
        public static event Action<BigInteger, UInt160, string, BigInteger> OnProposalCreated;

        [DisplayName("VoteCast")]
        public static event Action<BigInteger, UInt160, bool, BigInteger> OnVoteCast;

        [DisplayName("ProposalExecuted")]
        public static event Action<BigInteger> OnProposalExecuted;

        [DisplayName("MemberAdded")]
        public static event Action<UInt160, BigInteger> OnMemberAdded;

        [DisplayName("MemberRemoved")]
        public static event Action<UInt160> OnMemberRemoved;

        #endregion

        #region Deployment

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            var tx = (Transaction)Runtime.ScriptContainer;
            var admin = tx.Sender;
            
            // Set initial admin
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Admin }, admin);
            
            // Initialize settings
            InitializeSettings();
            
            // Initialize counters
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_ProposalCount }, 0);
            
            // Add admin as first member with full voting power
            AddMember(admin, 1000);
        }

        private static void InitializeSettings()
        {
            var settings = new Map<string, object>
            {
                ["votingPeriod"] = 7 * 24 * 60 * 60 * 1000, // 7 days in milliseconds
                ["quorum"] = 51, // 51% quorum
                ["proposalThreshold"] = 100, // Minimum voting power to create proposal
                ["executionDelay"] = 2 * 24 * 60 * 60 * 1000 // 2 days delay before execution
            };
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Settings }, StdLib.Serialize(settings));
        }

        #endregion

        #region Admin Functions

        [Safe]
        public static UInt160 GetAdmin()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Admin });
        }

        private static bool IsAdmin()
        {
            return Runtime.CheckWitness(GetAdmin());
        }

        public static void SetAdmin(UInt160 newAdmin)
        {
            if (!IsAdmin())
                throw new Exception("Only admin can set new admin");
            
            if (!newAdmin.IsValid)
                throw new Exception("Invalid admin address");
            
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Admin }, newAdmin);
        }

        #endregion

        #region Member Management

        [Safe]
        public static bool IsMember(UInt160 address)
        {
            var memberKey = Prefix_Member.ToByteArray().Concat(address);
            return Storage.Get(Storage.CurrentContext, memberKey) != null;
        }

        [Safe]
        public static BigInteger GetVotingPower(UInt160 member)
        {
            var powerKey = Prefix_VotingPower.ToByteArray().Concat(member);
            var power = Storage.Get(Storage.CurrentContext, powerKey);
            return power != null ? (BigInteger)power : 0;
        }

        public static void AddMember(UInt160 member, BigInteger votingPower)
        {
            if (!IsAdmin())
                throw new Exception("Only admin can add members");
            
            if (!member.IsValid)
                throw new Exception("Invalid member address");
            
            if (votingPower <= 0)
                throw new Exception("Voting power must be positive");

            var memberKey = Prefix_Member.ToByteArray().Concat(member);
            var powerKey = Prefix_VotingPower.ToByteArray().Concat(member);
            
            Storage.Put(Storage.CurrentContext, memberKey, true);
            Storage.Put(Storage.CurrentContext, powerKey, votingPower);
            
            OnMemberAdded(member, votingPower);
        }

        public static void RemoveMember(UInt160 member)
        {
            if (!IsAdmin())
                throw new Exception("Only admin can remove members");
            
            if (!IsMember(member))
                throw new Exception("Address is not a member");

            var memberKey = Prefix_Member.ToByteArray().Concat(member);
            var powerKey = Prefix_VotingPower.ToByteArray().Concat(member);
            
            Storage.Delete(Storage.CurrentContext, memberKey);
            Storage.Delete(Storage.CurrentContext, powerKey);
            
            OnMemberRemoved(member);
        }

        public static void UpdateVotingPower(UInt160 member, BigInteger newPower)
        {
            if (!IsAdmin())
                throw new Exception("Only admin can update voting power");
            
            if (!IsMember(member))
                throw new Exception("Address is not a member");
            
            if (newPower <= 0)
                throw new Exception("Voting power must be positive");

            var powerKey = Prefix_VotingPower.ToByteArray().Concat(member);
            Storage.Put(Storage.CurrentContext, powerKey, newPower);
        }

        #endregion

        #region Proposals

        public static BigInteger CreateProposal(string title, string description, ByteString callData, UInt160 target)
        {
            var sender = (Transaction)Runtime.ScriptContainer;
            var creator = sender.Sender;
            
            if (!IsMember(creator))
                throw new Exception("Only members can create proposals");

            var settings = GetSettings();
            var threshold = (BigInteger)settings["proposalThreshold"];
            
            if (GetVotingPower(creator) < threshold)
                throw new Exception("Insufficient voting power to create proposal");

            // Get next proposal ID
            var proposalCount = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_ProposalCount });
            var proposalId = (BigInteger)proposalCount + 1;
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_ProposalCount }, proposalId);

            // Create proposal
            var proposal = new Map<string, object>
            {
                ["title"] = title,
                ["description"] = description,
                ["creator"] = creator,
                ["callData"] = callData,
                ["target"] = target,
                ["createdAt"] = Runtime.Time,
                ["executed"] = false,
                ["yesVotes"] = 0,
                ["noVotes"] = 0,
                ["totalVotes"] = 0
            };

            var proposalKey = Prefix_Proposal.ToByteArray().Concat(proposalId.ToByteArray());
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            OnProposalCreated(proposalId, creator, title, Runtime.Time + (BigInteger)settings["votingPeriod"]);
            return proposalId;
        }

        public static void Vote(BigInteger proposalId, bool support)
        {
            var sender = (Transaction)Runtime.ScriptContainer;
            var voter = sender.Sender;
            
            if (!IsMember(voter))
                throw new Exception("Only members can vote");

            var proposalKey = Prefix_Proposal.ToByteArray().Concat(proposalId.ToByteArray());
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            if (proposalData == null)
                throw new Exception("Proposal does not exist");

            var proposal = (Map<string, object>)StdLib.Deserialize(proposalData);
            if ((bool)proposal["executed"])
                throw new Exception("Proposal already executed");

            // Check voting period
            var settings = GetSettings();
            var votingPeriod = (BigInteger)settings["votingPeriod"];
            var createdAt = (BigInteger)proposal["createdAt"];
            
            if (Runtime.Time > createdAt + votingPeriod)
                throw new Exception("Voting period has ended");

            // Check if already voted
            var voteKey = Prefix_Vote.ToByteArray().Concat(proposalId.ToByteArray()).Concat(voter);
            if (Storage.Get(Storage.CurrentContext, voteKey) != null)
                throw new Exception("Already voted on this proposal");

            var votingPower = GetVotingPower(voter);
            if (votingPower <= 0)
                throw new Exception("No voting power");

            // Record vote
            Storage.Put(Storage.CurrentContext, voteKey, support);

            // Update proposal vote counts
            if (support)
            {
                proposal["yesVotes"] = (BigInteger)proposal["yesVotes"] + votingPower;
            }
            else
            {
                proposal["noVotes"] = (BigInteger)proposal["noVotes"] + votingPower;
            }
            proposal["totalVotes"] = (BigInteger)proposal["totalVotes"] + votingPower;

            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            OnVoteCast(proposalId, voter, support, votingPower);
        }

        public static void ExecuteProposal(BigInteger proposalId)
        {
            var proposalKey = Prefix_Proposal.ToByteArray().Concat(proposalId.ToByteArray());
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            if (proposalData == null)
                throw new Exception("Proposal does not exist");

            var proposal = (Map<string, object>)StdLib.Deserialize(proposalData);
            if ((bool)proposal["executed"])
                throw new Exception("Proposal already executed");

            var settings = GetSettings();
            var votingPeriod = (BigInteger)settings["votingPeriod"];
            var executionDelay = (BigInteger)settings["executionDelay"];
            var createdAt = (BigInteger)proposal["createdAt"];
            
            // Check if voting period has ended
            if (Runtime.Time <= createdAt + votingPeriod)
                throw new Exception("Voting period not yet ended");
            
            // Check execution delay
            if (Runtime.Time <= createdAt + votingPeriod + executionDelay)
                throw new Exception("Execution delay not yet passed");

            var yesVotes = (BigInteger)proposal["yesVotes"];
            var totalVotes = (BigInteger)proposal["totalVotes"];
            var quorum = (BigInteger)settings["quorum"];

            // Check if proposal passed
            if (totalVotes == 0 || (yesVotes * 100 / totalVotes) < quorum)
                throw new Exception("Proposal did not pass");

            // Mark as executed
            proposal["executed"] = true;
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposal));

            // Execute the proposal
            var target = (UInt160)proposal["target"];
            var callData = (ByteString)proposal["callData"];
            
            if (target.IsValid && callData.Length > 0)
            {
                Contract.Call(target, "execute", CallFlags.All, callData);
            }

            OnProposalExecuted(proposalId);
        }

        #endregion

        #region View Methods

        [Safe]
        public static Map<string, object> GetProposal(BigInteger proposalId)
        {
            var proposalKey = Prefix_Proposal.ToByteArray().Concat(proposalId.ToByteArray());
            var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
            if (proposalData == null)
                return null;

            return (Map<string, object>)StdLib.Deserialize(proposalData);
        }

        [Safe]
        public static bool HasVoted(BigInteger proposalId, UInt160 voter)
        {
            var voteKey = Prefix_Vote.ToByteArray().Concat(proposalId.ToByteArray()).Concat(voter);
            return Storage.Get(Storage.CurrentContext, voteKey) != null;
        }

        [Safe]
        public static BigInteger GetProposalCount()
        {
            return Storage.Get(Storage.CurrentContext, new byte[] { Prefix_ProposalCount });
        }

        [Safe]
        public static Map<string, object> GetSettings()
        {
            var settingsData = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Settings });
            return (Map<string, object>)StdLib.Deserialize(settingsData);
        }

        #endregion

        #region Settings Management

        public static void UpdateSettings(string key, object value)
        {
            if (!IsAdmin())
                throw new Exception("Only admin can update settings");

            var settings = GetSettings();
            settings[key] = value;
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Settings }, StdLib.Serialize(settings));
        }

        #endregion

        #region Token Handling

        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            // Accept payments to DAO treasury
        }

        public static void OnNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
        {
            // Accept NFT donations to DAO
        }

        #endregion

        #region Admin

        public static void Update(ByteString nefFile, string manifest)
        {
            if (!IsAdmin())
                throw new Exception("Only admin can update contract");
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void Destroy()
        {
            if (!IsAdmin())
                throw new Exception("Only admin can destroy contract");
            ContractManagement.Destroy();
        }

        #endregion
    }
}