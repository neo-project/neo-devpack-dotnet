# Privacy-Preserving Voting System on Neo

## Overview

This example demonstrates a sophisticated privacy-preserving voting system built on Neo N3 using BLS12-381 elliptic curve cryptography and zero-knowledge proofs (ZKP). The system enables completely anonymous voting where individual votes remain private while the final tally is publicly verifiable.

## Key Features

### 🔐 Complete Vote Privacy
- Individual votes are never revealed, even after tallying
- Voters remain anonymous throughout the process
- Uses BLS12-381 cryptographic commitments

### ✅ Verifiable Integrity
- Zero-knowledge proofs ensure vote validity
- Prevents double voting using nullifiers
- Publicly verifiable tally process

### 🔗 Neo N3 Integration
- Leverages Neo's native BLS12-381 support
- Efficient on-chain proof verification
- Gas-optimized smart contract design

## Architecture

### 1. Cryptographic Components

#### BLS12-381 Curve
The system uses Neo's native BLS12-381 implementation for:
- **Commitments**: Hiding voter identities and vote choices
- **Homomorphic Encryption**: Allows tallying encrypted votes
- **Pairing-based Proofs**: Enables efficient ZKP verification

#### Zero-Knowledge Proofs
The voting system implements several ZK protocols:
- **Vote Validity Proof**: Proves vote is binary (YES/NO) without revealing which
- **Voter Eligibility Proof**: Proves registration without revealing identity
- **Nullifier Proof**: Prevents double voting while maintaining anonymity

### 2. Voting Phases

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ Registration │ --> │    Voting    │ --> │   Tallying   │ --> │  Completed   │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
```

1. **Registration Phase**
   - Voters register with cryptographic commitments
   - Voting power is verified and recorded
   - Merkle tree of eligible voters is constructed

2. **Voting Phase**
   - Voters cast encrypted votes with ZK proofs
   - Nullifiers prevent double voting
   - Votes are homomorphically added (still encrypted)

3. **Tallying Phase**
   - Admin provides decryption proof
   - Final tally is revealed
   - Individual votes remain private

4. **Completed**
   - Results are final and publicly visible
   - Audit trail available for verification

### 3. Smart Contract Methods

#### Administrative Functions
- `createProposal`: Initialize a new voting proposal
- `advancePhase`: Move proposal to next phase
- `revealTally`: Decrypt and reveal final results

#### Voter Functions
- `registerVoter`: Register with commitment and voting power proof
- `castVote`: Submit encrypted vote with ZK proof

#### Query Functions
- `getProposalStatus`: Check current phase
- `getProposal`: Get proposal details
- `isNullifierUsed`: Check if vote was already cast

## Mathematical Foundation

### Commitment Scheme
Voter commitment: `C = g^s * h^r`
- `s`: Voter's secret
- `r`: Random blinding factor
- `g, h`: Generator points on BLS12-381 G1

### Homomorphic Encryption
Vote encryption: `E(v) = (g^r, pk^r * g^v)`
- `v ∈ {0, 1}`: Vote value
- `r`: Random encryption factor
- `pk`: Public key for tallying

### Zero-Knowledge Proof Structure
The system uses Σ-protocols for proving:
1. Knowledge of committed secret
2. Vote is binary (OR proof)
3. Correct nullifier formation

## Usage

### Prerequisites

```bash
# Install Neo Express
dotnet tool install -g Neo.Express

# Install Neo compiler
dotnet tool install -g Neo.Compiler.CSharp
```

### Setup

1. **Initialize Neo Express environment:**
```bash
cd examples/ZKPVoting
chmod +x *.sh
./setup-neo-express.sh
```

2. **Build the contract:**
```bash
dotnet build
```

3. **Deploy to Neo Express:**
```bash
./deploy-contract.sh
```

### Running a Vote

1. **Create a proposal (admin only):**
```csharp
var proposalId = new UInt256(randomBytes);
var description = "Should we implement feature X?";
var registrationDeadline = currentBlock + 100;
var votingDeadline = registrationDeadline + 200;

contract.CreateProposal(proposalId, description, 
    registrationDeadline, votingDeadline);
```

2. **Register voters:**
```csharp
var (commitment, randomness) = proofGenerator.GenerateVoterCommitment(voterSecret);
var votingPowerProof = proofGenerator.GenerateVotingPowerProof(tokenBalance);

contract.RegisterVoter(proposalId, commitment, votingPowerProof);
```

3. **Cast votes:**
```csharp
var (encryptedVote, nullifier, proof) = proofGenerator.GenerateVoteProof(
    voteChoice, voterSecret, randomness);

contract.CastVote(proposalId, encryptedVote, nullifier, proof);
```

4. **Reveal results:**
```csharp
var decryptionProof = proofGenerator.GenerateDecryptionProof(
    encryptedTally, decryptionKey);

contract.RevealTally(proposalId, decryptionProof);
```

### Testing

Run the complete test suite:
```bash
./test-voting.sh
```

Or use the C# test client:
```bash
dotnet run --project VotingClient
```

## Security Considerations

### Privacy Guarantees
- **Vote Privacy**: Individual votes are never decrypted
- **Voter Anonymity**: Commitments hide voter identities
- **Receipt-Freeness**: Voters cannot prove how they voted

### Attack Resistance
- **Double Voting**: Prevented by nullifier mechanism
- **Vote Buying**: Mitigated by receipt-freeness
- **Sybil Attacks**: Controlled by registration phase

### Trust Assumptions
- Admin cannot decrypt individual votes
- BLS12-381 discrete log problem remains hard
- Neo blockchain remains secure and available

## Performance

### Gas Costs (Estimated)
- Proposal Creation: ~5 GAS
- Voter Registration: ~2 GAS per voter
- Vote Casting: ~3 GAS per vote
- Tally Revelation: ~5 GAS

### Scalability
- Supports thousands of voters per proposal
- Constant-time vote verification
- Efficient homomorphic tallying

## Advanced Features

### Weighted Voting
The system supports weighted voting based on token holdings:
```csharp
var votingPower = CalculateVotingPower(tokenBalance);
var proof = GenerateVotingPowerProof(votingPower);
```

### Multi-Choice Voting
Extension for multiple options (not just YES/NO):
```csharp
// Use range proofs for vote ∈ {0, 1, 2, ..., n-1}
var proof = GenerateRangeProof(voteChoice, numOptions);
```

### Delegated Voting
Allow vote delegation while preserving privacy:
```csharp
var delegationProof = GenerateDelegationProof(
    delegatorSecret, delegateCommitment);
```

## Technical Details

### BLS12-381 Operations Used
1. **Point Addition**: For homomorphic vote aggregation
2. **Scalar Multiplication**: For commitment generation
3. **Pairing**: For proof verification
4. **Serialization**: For efficient storage

### Storage Layout
```
PREFIX_PROPOSAL (0x01)       -> Proposal data
PREFIX_VOTER_COMMITMENT (0x02) -> Registered voters
PREFIX_NULLIFIER (0x03)      -> Used nullifiers
PREFIX_ENCRYPTED_VOTE (0x04) -> Encrypted votes
PREFIX_TALLY_COMMITMENT (0x05) -> Homomorphic sum
```

## Example Output

```
=== PRIVACY-PRESERVING VOTING DEMONSTRATION ===

Deploying Privacy-Preserving Voting Contract...
Contract deployed at: 0x123...abc

Creating test proposal...
Proposal created: 0x456...def
Description: Should we implement feature X?
Registration deadline: block 1100
Voting deadline: block 1300

Registering 3 test voters...
Voter 1 registered with commitment: 0xa1b2c3...
Voter 2 registered with commitment: 0xd4e5f6...
Voter 3 registered with commitment: 0x789abc...

Casting encrypted votes...
Voter 1 cast YES vote (encrypted)
  Nullifier: 0x111...
Voter 2 cast NO vote (encrypted)
  Nullifier: 0x222...
Voter 3 cast YES vote (encrypted)
  Nullifier: 0x333...

Revealing vote tally...
Tally revealed!

=== VOTING RESULTS ===
Proposal ID: 0x456...def
Status: COMPLETED
Results (decrypted):
  YES votes: 2
  NO votes: 1
  Result: PASSED

All votes were tallied without revealing individual voter choices!
```

## Troubleshooting

### Common Issues

1. **"Invalid voter commitment"**
   - Ensure commitment is properly serialized BLS12-381 point
   - Check byte length (should be 48 bytes for G1)

2. **"Invalid vote proof"**
   - Verify proof generation uses correct parameters
   - Ensure pairing check passes

3. **"Vote already cast (nullifier exists)"**
   - Each voter can only vote once per proposal
   - Check nullifier generation is deterministic

## Contributing

Contributions are welcome! Areas for improvement:
- Implement full SNARK/STARK proofs
- Add support for ranked-choice voting
- Optimize gas consumption
- Enhance voter registration process

## References

- [BLS12-381 Specification](https://electriccoin.co/blog/new-snark-curve/)
- [Zero-Knowledge Proofs in Voting](https://eprint.iacr.org/2016/765.pdf)
- [Neo Smart Contract Documentation](https://docs.neo.org/docs/n3/develop/write/basics)
- [Homomorphic Encryption](https://en.wikipedia.org/wiki/Homomorphic_encryption)

## License

MIT License - See LICENSE file for details

## Disclaimer

This is an example implementation for educational purposes. For production use, conduct thorough security audits and implement additional safeguards.