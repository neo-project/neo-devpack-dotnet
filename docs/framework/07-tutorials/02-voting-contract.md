# Tutorial: Creating a Voting Contract

This tutorial demonstrates how to build a simple on-chain voting contract where users can vote on predefined proposals.

## 1. Project Setup

1.  Create a new C# class library project (`dotnet new classlib -n SimpleVoting`).
2.  Add NuGet packages: `Neo.SmartContract.Framework`, `Neo.Compiler.CSharp`.
3.  Configure the `.csproj` file for Neo compilation.

## 2. Contract Code (`SimpleVoting.cs`)

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Examples.Voting
{
    [DisplayName("SimpleVotingContract")]
    [ManifestExtra("Author", "Your Name")]
    [ContractPermission("*")] // Example: Allow necessary calls
    public class SimpleVoting : SmartContract
    {
        // Storage Prefixes
        private static readonly StorageMap Proposals = new StorageMap(Storage.CurrentContext, "PROP"); // Proposal ID -> Proposal Info (Serialized)
        private static readonly StorageMap Votes = new StorageMap(Storage.CurrentContext, "VOTE");     // Voter + Proposal ID -> Voted Option
        private static readonly byte[] ProposalCounterKey = { 0x11 };
        private static readonly byte[] OwnerKey = { 0xFF };

        // Event for new proposals
        public delegate void ProposalAddedDelegate(BigInteger proposalId, string description, UInt160 proposer);
        [DisplayName("ProposalAdded")]
        public static event ProposalAddedDelegate OnProposalAdded;

        // Event for votes cast
        public delegate void VotedDelegate(UInt160 voter, BigInteger proposalId, int optionIndex);
        [DisplayName("Voted")]
        public static event VotedDelegate OnVoted;

        // Structure to hold proposal information (simple example)
        public class ProposalInfo
        {
            public string Description;
            public List<string> Options; // Voting options
            public Map<int, BigInteger> VoteCounts; // Option Index -> Count
            public uint EndBlock; // Block height when voting ends
            public UInt160 Proposer;
        }

        // Deploy: Set Owner
        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, OwnerKey, Runtime.Transaction.Sender);
                Storage.Put(Storage.CurrentContext, ProposalCounterKey, 0);
            }
        }

        // --- Proposal Management ---

        // Only Owner can add proposals
        public static BigInteger AddProposal(string description, List<string> options, uint votingDurationBlocks)
        {
            ByteString owner = Storage.Get(Storage.CurrentContext, OwnerKey);
            Helper.Assert(owner != null && Runtime.CheckWitness((UInt160)owner), "Unauthorized: Only owner can add proposals");
            Helper.Assert(options.Count > 1, "Must have at least two options");

            BigInteger currentCounter = (BigInteger)Storage.Get(Storage.CurrentContext, ProposalCounterKey);
            BigInteger newProposalId = currentCounter + 1;
            Storage.Put(Storage.CurrentContext, ProposalCounterKey, newProposalId);

            ProposalInfo info = new ProposalInfo();
            info.Description = description;
            info.Options = options;
            info.VoteCounts = new Map<int, BigInteger>();
            // Initialize vote counts to zero for each option
            for (int i = 0; i < options.Count; i++)
            {
                info.VoteCounts[i] = 0;
            }
            info.EndBlock = LedgerContract.CurrentIndex + votingDurationBlocks;
            info.Proposer = (UInt160)owner;

            // Store the serialized proposal info
            Proposals.Put(newProposalId, StdLib.Serialize(info));

            OnProposalAdded(newProposalId, description, (UInt160)owner);
            return newProposalId;
        }

        [Safe]
        public static ProposalInfo GetProposal(BigInteger proposalId)
        {
            ByteString data = Proposals.Get(proposalId);
            if (data == null) return null;
            return (ProposalInfo)StdLib.Deserialize(data);
        }

        [Safe]
        public static BigInteger GetProposalCount()
        {
            return (BigInteger)Storage.Get(Storage.CurrentContext, ProposalCounterKey);
        }

        // --- Voting --- 

        public static bool Vote(BigInteger proposalId, int optionIndex)
        {
            UInt160 voter = Runtime.Transaction.Sender; // Use Sender as voter ID
            Helper.Assert(Runtime.CheckWitness(voter), "Unauthorized: CheckWitness failed for voter");

            ProposalInfo info = GetProposal(proposalId);
            Helper.Assert(info != null, "Proposal not found");
            Helper.Assert(LedgerContract.CurrentIndex < info.EndBlock, "Voting period has ended");
            Helper.Assert(optionIndex >= 0 && optionIndex < info.Options.Count, "Invalid option index");

            // Check if already voted
            ByteString voteKey = voter + proposalId; // Combine voter and proposal ID for unique key
            if (Votes.Get(voteKey) != null)
            {
                Runtime.Log("Voter has already voted on this proposal.");
                return false; // Already voted
            }

            // Record vote and increment count
            Votes.Put(voteKey, optionIndex);
            info.VoteCounts[optionIndex]++;

            // Save updated proposal info
            Proposals.Put(proposalId, StdLib.Serialize(info));

            OnVoted(voter, proposalId, optionIndex);
            return true;
        }

        [Safe]
        public static int GetVote(BigInteger proposalId, UInt160 voter)
        {
            ByteString voteKey = voter + proposalId;
            ByteString voteData = Votes.Get(voteKey);
            if (voteData == null) return -1; // Not voted
            return (int)(BigInteger)voteData; // Cast stored option index
        }
    }
}

```

## 3. Key Elements Explained

*   **Owner:** Set during deployment, only the owner can add new proposals.
*   **Proposal ID:** A simple counter ensures unique IDs for each proposal.
*   **`ProposalInfo` Struct:** Holds all relevant data for a proposal (description, options, vote counts, end block). C# structs are implicitly serialized/deserialized by `StdLib.Serialize`/`Deserialize`.
*   **Storage:**
    *   `Proposals`: Maps Proposal ID to serialized `ProposalInfo`.
    *   `Votes`: Maps a combined key (`voter + proposalId`) to the option index voted for. This prevents a user from voting multiple times on the same proposal.
    *   `ProposalCounterKey`, `OwnerKey`: Simple keys for global values.
*   **`AddProposal`:**
    *   Checks owner authorization.
    *   Increments proposal counter.
    *   Initializes `ProposalInfo` (including vote counts to 0).
    *   Serializes and stores the info.
    *   Emits `OnProposalAdded` event.
*   **`GetProposal`:** Retrieves and deserializes proposal info.
*   **`Vote`:**
    *   Checks voter authorization (`CheckWitness`).
    *   Validates proposal existence, voting period, and option index.
    *   **Prevents double voting** by checking if an entry exists in the `Votes` map for the `voter + proposalId` key.
    *   Records the vote in the `Votes` map.
    *   Increments the vote count in the `ProposalInfo` struct.
    *   **Re-serializes and saves** the updated `ProposalInfo`.
    *   Emits `Voted` event.
*   **`GetVote`:** Retrieves the option a specific voter chose for a proposal.
*   **Serialization:** `StdLib.Serialize` and `StdLib.Deserialize` are used to store and retrieve the `ProposalInfo` struct in storage.

## 4. Compile and Deploy

1.  Compile: `dotnet build`
2.  Deploy the `.nef` and `.manifest.json` files.

## Potential Enhancements

*   **Weighted Voting:** Use token balances (e.g., NEO or a custom NEP-17) instead of 1-person-1-vote.
*   **Gas Sponsorship:** Allow the owner or proposal creators to sponsor GAS fees for voters.
*   **More Complex Proposal Structures:** Allow different voting types (ranked choice, etc.).
*   **UI Integration:** Build a frontend application to interact with the contract.

[Previous: Building a NEP-17 Token](./01-nep17-token.md) | [Next: Using Oracles](./03-oracle-usage.md)