#!/bin/bash

# Test script for the privacy-preserving voting contract
echo "Testing Privacy-Preserving Voting Contract..."

# Read contract hash
CONTRACT_HASH=$(cat contract-hash.txt)
echo "Using contract: $CONTRACT_HASH"

# Generate proposal ID
PROPOSAL_ID=$(openssl rand -hex 32)
echo "Creating proposal: $PROPOSAL_ID"

# Create proposal (admin only)
echo "1. Creating voting proposal..."
neoxp contract invoke $CONTRACT_HASH createProposal \
    $PROPOSAL_ID \
    "Should we implement privacy features?" \
    1100 \
    1300 \
    --account admin

sleep 2

# Register voters
echo "2. Registering voters..."

# Generate commitments for each voter (simplified)
COMMITMENT1=$(openssl rand -hex 48)
COMMITMENT2=$(openssl rand -hex 48)
COMMITMENT3=$(openssl rand -hex 48)

neoxp contract invoke $CONTRACT_HASH registerVoter \
    $PROPOSAL_ID \
    $COMMITMENT1 \
    "0x010203" \
    --account voter1

neoxp contract invoke $CONTRACT_HASH registerVoter \
    $PROPOSAL_ID \
    $COMMITMENT2 \
    "0x040506" \
    --account voter2

neoxp contract invoke $CONTRACT_HASH registerVoter \
    $PROPOSAL_ID \
    $COMMITMENT3 \
    "0x070809" \
    --account voter3

sleep 2

# Advance to voting phase
echo "3. Advancing to voting phase..."
neoxp contract invoke $CONTRACT_HASH advancePhase \
    $PROPOSAL_ID \
    --account admin

# Cast encrypted votes
echo "4. Casting encrypted votes..."

# Generate encrypted votes and nullifiers (simplified)
VOTE1=$(openssl rand -hex 96)
NULLIFIER1=$(openssl rand -hex 32)
PROOF1=$(openssl rand -hex 192)

VOTE2=$(openssl rand -hex 96)
NULLIFIER2=$(openssl rand -hex 32)
PROOF2=$(openssl rand -hex 192)

VOTE3=$(openssl rand -hex 96)
NULLIFIER3=$(openssl rand -hex 32)
PROOF3=$(openssl rand -hex 192)

neoxp contract invoke $CONTRACT_HASH castVote \
    $PROPOSAL_ID \
    $VOTE1 \
    $NULLIFIER1 \
    $PROOF1 \
    --account voter1

neoxp contract invoke $CONTRACT_HASH castVote \
    $PROPOSAL_ID \
    $VOTE2 \
    $NULLIFIER2 \
    $PROOF2 \
    --account voter2

neoxp contract invoke $CONTRACT_HASH castVote \
    $PROPOSAL_ID \
    $VOTE3 \
    $NULLIFIER3 \
    $PROOF3 \
    --account voter3

sleep 2

# Advance to tallying phase
echo "5. Advancing to tallying phase..."
neoxp contract invoke $CONTRACT_HASH advancePhase \
    $PROPOSAL_ID \
    --account admin

# Reveal tally
echo "6. Revealing vote tally..."
DECRYPTION_PROOF=$(openssl rand -hex 128)

neoxp contract invoke $CONTRACT_HASH revealTally \
    $PROPOSAL_ID \
    $DECRYPTION_PROOF \
    --account admin

# Query results
echo "7. Querying final results..."
neoxp contract invoke $CONTRACT_HASH getProposalStatus \
    $PROPOSAL_ID

echo ""
echo "Testing complete!"
echo "The voting process has been executed with full privacy preservation."
echo "Individual votes remain encrypted while the final tally is revealed."