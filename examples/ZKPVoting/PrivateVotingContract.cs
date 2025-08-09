// Privacy-Preserving Voting Contract using BLS12-381 and Zero-Knowledge Proofs
// This contract implements anonymous voting where votes are tallied without revealing individual choices

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Examples.ZKPVoting
{
    [DisplayName("PrivateVotingContract")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "Privacy-preserving voting using BLS12-381 ZKP")]
    [ContractPermission("*", "*")]
    public class PrivateVotingContract : SmartContract
    {
        #region Constants
        
        // Storage prefixes - using ByteString for Neo compatibility
        private static readonly ByteString PREFIX_PROPOSAL = "\x01";
        private static readonly ByteString PREFIX_VOTER_COMMITMENT = "\x02";
        private static readonly ByteString PREFIX_NULLIFIER = "\x03";
        private static readonly ByteString PREFIX_ENCRYPTED_VOTE = "\x04";
        private static readonly ByteString PREFIX_TALLY_COMMITMENT = "\x05";
        private static readonly ByteString PREFIX_PROPOSAL_STATUS = "\x06";
        private static readonly ByteString PREFIX_VOTER_REGISTERED = "\x07";
        private static readonly ByteString PREFIX_VOTING_POWER = "\x08";
        private static readonly ByteString PREFIX_MERKLE_ROOT = "\x09";
        private static readonly ByteString PREFIX_ADMIN = "\x0A";
        
        // Proposal statuses
        private const byte STATUS_SETUP = 0x00;
        private const byte STATUS_REGISTRATION = 0x01;
        private const byte STATUS_VOTING = 0x02;
        private const byte STATUS_TALLYING = 0x03;
        private const byte STATUS_COMPLETED = 0x04;
        
        #endregion
        
        #region Events
        
        [DisplayName("ProposalCreated")]
        public static event Action<UInt256, string, BigInteger, BigInteger> OnProposalCreated;
        
        [DisplayName("VoterRegistered")]
        public static event Action<UInt256, ByteString> OnVoterRegistered;
        
        [DisplayName("VoteCommitted")]
        public static event Action<UInt256, ByteString, ByteString> OnVoteCommitted;
        
        [DisplayName("TallyRevealed")]
        public static event Action<UInt256, BigInteger, BigInteger> OnTallyRevealed;
        
        [DisplayName("ProposalStatusChanged")]
        public static event Action<UInt256, byte> OnProposalStatusChanged;
        
        #endregion
        
        #region Contract Methods
        
        /// <summary>
        /// Initializes the contract with an admin
        /// </summary>
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (!update)
            {
                var tx = Runtime.Transaction;
                Storage.Put(Storage.CurrentContext, PREFIX_ADMIN, (ByteString)tx.Sender);
            }
        }
        
        /// <summary>
        /// Creates a new voting proposal
        /// </summary>
        [DisplayName("createProposal")]
        public static bool CreateProposal(UInt256 proposalId, string description, 
            BigInteger registrationDeadline, BigInteger votingDeadline)
        {
            // Check admin permission
            if (!CheckAdmin())
                throw new Exception("Only admin can create proposals");
                
            if (registrationDeadline <= Ledger.CurrentIndex)
                throw new Exception("Registration deadline must be in the future");
                
            if (votingDeadline <= registrationDeadline)
                throw new Exception("Voting deadline must be after registration deadline");
            
            var proposalKey = GetProposalKey(proposalId);
            if (Storage.Get(Storage.CurrentContext, proposalKey) != null)
                throw new Exception("Proposal already exists");
            
            // Store proposal data
            var proposalData = StdLib.Serialize(new object[] { 
                description, 
                registrationDeadline, 
                votingDeadline,
                0, // yes votes (encrypted)
                0  // no votes (encrypted)
            });
            Storage.Put(Storage.CurrentContext, proposalKey, proposalData);
            
            // Set initial status
            SetProposalStatus(proposalId, STATUS_REGISTRATION);
            
            OnProposalCreated(proposalId, description, registrationDeadline, votingDeadline);
            return true;
        }
        
        /// <summary>
        /// Registers a voter for a proposal with a commitment to their identity
        /// </summary>
        [DisplayName("registerVoter")]
        public static bool RegisterVoter(UInt256 proposalId, ByteString voterCommitment, ByteString votingPowerProof)
        {
            var status = GetProposalStatus(proposalId);
            if (status != STATUS_REGISTRATION)
                throw new Exception("Not in registration phase");
            
            // Verify the commitment is a valid BLS12-381 point
            var commitment = CryptoLib.Bls12381Deserialize((byte[])voterCommitment);
            if (commitment == null)
                throw new Exception("Invalid voter commitment");
            
            // Verify voting power proof (simplified - in practice would verify token holdings)
            var votingPower = VerifyVotingPower(votingPowerProof);
            if (votingPower <= 0)
                throw new Exception("Invalid voting power");
            
            // Store voter commitment
            var voterKey = GetVoterCommitmentKey(proposalId, voterCommitment);
            if (Storage.Get(Storage.CurrentContext, voterKey) != null)
                throw new Exception("Voter already registered");
                
            Storage.Put(Storage.CurrentContext, voterKey, votingPower);
            
            // Update Merkle tree root for registered voters
            UpdateVoterMerkleRoot(proposalId, voterCommitment);
            
            OnVoterRegistered(proposalId, voterCommitment);
            return true;
        }
        
        /// <summary>
        /// Casts an encrypted vote with zero-knowledge proof
        /// </summary>
        [DisplayName("castVote")]
        public static bool CastVote(UInt256 proposalId, ByteString encryptedVote, 
            ByteString nullifier, ByteString zkProof)
        {
            var status = GetProposalStatus(proposalId);
            if (status != STATUS_VOTING)
                throw new Exception("Not in voting phase");
            
            // Check nullifier hasn't been used (prevents double voting)
            var nullifierKey = GetNullifierKey(proposalId, nullifier);
            if (Storage.Get(Storage.CurrentContext, nullifierKey) != null)
                throw new Exception("Vote already cast (nullifier exists)");
            
            // Deserialize and validate encrypted vote
            var voteCommitment = CryptoLib.Bls12381Deserialize((byte[])encryptedVote);
            if (voteCommitment == null)
                throw new Exception("Invalid vote commitment");
            
            // Verify the zero-knowledge proof
            if (!VerifyVoteProof(proposalId, encryptedVote, nullifier, zkProof))
                throw new Exception("Invalid vote proof");
            
            // Store the encrypted vote
            var voteKey = GetEncryptedVoteKey(proposalId, nullifier);
            Storage.Put(Storage.CurrentContext, voteKey, encryptedVote);
            
            // Mark nullifier as used
            Storage.Put(Storage.CurrentContext, nullifierKey, 1);
            
            // Add to homomorphic tally (votes remain encrypted)
            UpdateEncryptedTally(proposalId, voteCommitment);
            
            OnVoteCommitted(proposalId, encryptedVote, nullifier);
            return true;
        }
        
        /// <summary>
        /// Reveals the final tally using the decryption key
        /// </summary>
        [DisplayName("revealTally")]
        public static bool RevealTally(UInt256 proposalId, ByteString decryptionProof)
        {
            if (!CheckAdmin())
                throw new Exception("Only admin can reveal tally");
                
            var status = GetProposalStatus(proposalId);
            if (status != STATUS_TALLYING)
                throw new Exception("Not in tallying phase");
            
            // In a real implementation, this would decrypt the homomorphic sum
            // For demonstration, we'll simulate the decryption process
            var (yesVotes, noVotes) = DecryptTally(proposalId, decryptionProof);
            
            // Store final results
            var proposalKey = GetProposalKey(proposalId);
            var proposalData = (object[])StdLib.Deserialize(
                Storage.Get(Storage.CurrentContext, proposalKey));
            proposalData[3] = yesVotes;
            proposalData[4] = noVotes;
            Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(proposalData));
            
            SetProposalStatus(proposalId, STATUS_COMPLETED);
            
            OnTallyRevealed(proposalId, yesVotes, noVotes);
            return true;
        }
        
        /// <summary>
        /// Advances the proposal to the next phase
        /// </summary>
        [DisplayName("advancePhase")]
        public static bool AdvancePhase(UInt256 proposalId)
        {
            if (!CheckAdmin())
                throw new Exception("Only admin can advance phase");
                
            var currentStatus = GetProposalStatus(proposalId);
            var proposalData = GetProposalData(proposalId);
            var registrationDeadline = (BigInteger)proposalData[1];
            var votingDeadline = (BigInteger)proposalData[2];
            var currentHeight = Ledger.CurrentIndex;
            
            byte newStatus;
            
            if (currentStatus == STATUS_REGISTRATION && currentHeight >= registrationDeadline)
            {
                newStatus = STATUS_VOTING;
            }
            else if (currentStatus == STATUS_VOTING && currentHeight >= votingDeadline)
            {
                newStatus = STATUS_TALLYING;
            }
            else
            {
                throw new Exception("Cannot advance phase yet");
            }
            
            SetProposalStatus(proposalId, newStatus);
            OnProposalStatusChanged(proposalId, newStatus);
            return true;
        }
        
        #endregion
        
        #region Zero-Knowledge Proof Verification
        
        /// <summary>
        /// Verifies a zero-knowledge proof that a vote is valid
        /// </summary>
        private static bool VerifyVoteProof(UInt256 proposalId, ByteString encryptedVote, 
            ByteString nullifier, ByteString zkProof)
        {
            var proofBytes = (byte[])zkProof;
            
            // Deserialize proof components
            if (proofBytes.Length < 192) // Minimum size for proof components
                return false;
                
            // Extract proof elements (simplified)
            var proofG1 = proofBytes.Range(0, 96);
            var proofG2 = proofBytes.Range(96, 96);
            
            // Deserialize BLS12-381 points
            var g1Point = CryptoLib.Bls12381Deserialize(proofG1);
            var g2Point = CryptoLib.Bls12381Deserialize(proofG2);
            
            if (g1Point == null || g2Point == null)
                return false;
            
            // Perform pairing check to verify proof
            var pairingResult = CryptoLib.Bls12381Pairing(g1Point, g2Point);
            
            // In practice, we'd check this against expected values
            return pairingResult != null;
        }
        
        /// <summary>
        /// Verifies voting power based on proof (simplified)
        /// </summary>
        private static BigInteger VerifyVotingPower(ByteString proof)
        {
            // In a real implementation, this would verify token holdings
            // For demonstration, we'll accept any non-empty proof as 1 vote
            if (proof.Length > 0)
                return 1;
            return 0;
        }
        
        #endregion
        
        #region Homomorphic Operations
        
        /// <summary>
        /// Updates the encrypted tally using homomorphic addition
        /// </summary>
        private static void UpdateEncryptedTally(UInt256 proposalId, object voteCommitment)
        {
            var tallyKey = GetTallyCommitmentKey(proposalId);
            var currentTally = Storage.Get(Storage.CurrentContext, tallyKey);
            
            object newTally;
            if (currentTally == null || currentTally.Length == 0)
            {
                newTally = voteCommitment;
            }
            else
            {
                // Homomorphic addition of encrypted votes
                var currentCommitment = CryptoLib.Bls12381Deserialize((byte[])currentTally);
                newTally = CryptoLib.Bls12381Add(currentCommitment, voteCommitment);
            }
            
            Storage.Put(Storage.CurrentContext, tallyKey, 
                (ByteString)CryptoLib.Bls12381Serialize(newTally));
        }
        
        /// <summary>
        /// Decrypts the final tally (simulated for demonstration)
        /// </summary>
        private static (BigInteger, BigInteger) DecryptTally(UInt256 proposalId, ByteString decryptionProof)
        {
            // In a real implementation, this would use the decryption key
            // to decrypt the homomorphic sum
            // For demonstration, we'll return simulated values
            
            var tallyKey = GetTallyCommitmentKey(proposalId);
            var encryptedTally = Storage.Get(Storage.CurrentContext, tallyKey);
            
            if (encryptedTally == null)
                return (0, 0);
            
            // Simulate decryption based on proof length (for demo)
            var proofBytes = (byte[])decryptionProof;
            BigInteger yesVotes = proofBytes.Length % 10;
            BigInteger noVotes = proofBytes.Length % 7;
            
            return (yesVotes, noVotes);
        }
        
        #endregion
        
        #region Merkle Tree Operations
        
        /// <summary>
        /// Updates the Merkle root with a new voter commitment
        /// </summary>
        private static void UpdateVoterMerkleRoot(UInt256 proposalId, ByteString voterCommitment)
        {
            var merkleKey = GetMerkleRootKey(proposalId);
            var currentRoot = Storage.Get(Storage.CurrentContext, merkleKey);
            
            ByteString newRoot;
            if (currentRoot == null || currentRoot.Length == 0)
            {
                // First voter, they become the root
                newRoot = CryptoLib.Sha256(voterCommitment);
            }
            else
            {
                // Combine with existing root
                newRoot = CryptoLib.Sha256(currentRoot + voterCommitment);
            }
            
            Storage.Put(Storage.CurrentContext, merkleKey, newRoot);
        }
        
        #endregion
        
        #region Helper Methods
        
        private static bool CheckAdmin()
        {
            var admin = Storage.Get(Storage.CurrentContext, PREFIX_ADMIN);
            if (admin == null) return false;
            return Runtime.CheckWitness((UInt160)admin);
        }
        
        private static ByteString GetProposalKey(UInt256 proposalId)
        {
            return PREFIX_PROPOSAL + (ByteString)proposalId;
        }
        
        private static ByteString GetVoterCommitmentKey(UInt256 proposalId, ByteString commitment)
        {
            return PREFIX_VOTER_COMMITMENT + (ByteString)proposalId + 
                CryptoLib.Sha256(commitment);
        }
        
        private static ByteString GetNullifierKey(UInt256 proposalId, ByteString nullifier)
        {
            return PREFIX_NULLIFIER + (ByteString)proposalId + nullifier;
        }
        
        private static ByteString GetEncryptedVoteKey(UInt256 proposalId, ByteString nullifier)
        {
            return PREFIX_ENCRYPTED_VOTE + (ByteString)proposalId + nullifier;
        }
        
        private static ByteString GetTallyCommitmentKey(UInt256 proposalId)
        {
            return PREFIX_TALLY_COMMITMENT + (ByteString)proposalId;
        }
        
        private static ByteString GetProposalStatusKey(UInt256 proposalId)
        {
            return PREFIX_PROPOSAL_STATUS + (ByteString)proposalId;
        }
        
        private static ByteString GetMerkleRootKey(UInt256 proposalId)
        {
            return PREFIX_MERKLE_ROOT + (ByteString)proposalId;
        }
        
        private static byte GetProposalStatus(UInt256 proposalId)
        {
            var key = GetProposalStatusKey(proposalId);
            var status = Storage.Get(Storage.CurrentContext, key);
            if (status == null || status.Length == 0)
                throw new Exception("Proposal not found");
            return ((byte[])status)[0];
        }
        
        private static void SetProposalStatus(UInt256 proposalId, byte status)
        {
            var key = GetProposalStatusKey(proposalId);
            Storage.Put(Storage.CurrentContext, key, (ByteString)new byte[] { status });
        }
        
        private static object[] GetProposalData(UInt256 proposalId)
        {
            var key = GetProposalKey(proposalId);
            var data = Storage.Get(Storage.CurrentContext, key);
            if (data == null)
                throw new Exception("Proposal not found");
            return (object[])StdLib.Deserialize(data);
        }
        
        #endregion
        
        #region Query Methods
        
        /// <summary>
        /// Gets the current status of a proposal
        /// </summary>
        [DisplayName("getProposalStatus")]
        [Safe]
        public static string GetProposalStatusString(UInt256 proposalId)
        {
            var status = GetProposalStatus(proposalId);
            if (status == STATUS_SETUP) return "setup";
            if (status == STATUS_REGISTRATION) return "registration";
            if (status == STATUS_VOTING) return "voting";
            if (status == STATUS_TALLYING) return "tallying";
            if (status == STATUS_COMPLETED) return "completed";
            return "unknown";
        }
        
        /// <summary>
        /// Gets proposal details
        /// </summary>
        [DisplayName("getProposal")]
        [Safe]
        public static object[] GetProposal(UInt256 proposalId)
        {
            return GetProposalData(proposalId);
        }
        
        /// <summary>
        /// Checks if a nullifier has been used
        /// </summary>
        [DisplayName("isNullifierUsed")]
        [Safe]
        public static bool IsNullifierUsed(UInt256 proposalId, ByteString nullifier)
        {
            var key = GetNullifierKey(proposalId, nullifier);
            return Storage.Get(Storage.CurrentReadOnlyContext, key) != null;
        }
        
        #endregion
    }
}