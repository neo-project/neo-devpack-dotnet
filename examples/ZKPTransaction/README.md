# Privacy-Preserving Transactions on Neo

## Overview

This example implements a sophisticated privacy-preserving transaction system on Neo N3 using zero-knowledge proofs and the BLS12-381 elliptic curve. Similar to Zcash's shielded transactions, this system allows users to transfer assets with complete privacy - hiding both the transaction amounts and the identities of the sender and receiver.

## 🔐 Key Privacy Features

### 1. **Shielded Pool**
- Assets are deposited into a common pool where individual balances are hidden
- Only the total value locked is publicly visible
- Users can make private transfers within the pool

### 2. **Note-Based System**
- Each deposit creates an encrypted "note" containing the value and owner information
- Notes are identified by cryptographic commitments (hiding the actual data)
- Only the note owner can decrypt and spend their notes

### 3. **Nullifier Mechanism**
- Prevents double-spending without revealing which note is being spent
- Each note has a unique nullifier that is revealed only when spent
- The link between nullifier and note commitment remains hidden

### 4. **Zero-Knowledge Proofs**
- Prove transaction validity without revealing transaction details
- Prove ownership of notes without revealing identity
- Prove amounts are valid (non-negative) without revealing the actual values

## Architecture

### Transaction Flow

```
┌──────────────┐      ┌──────────────┐      ┌──────────────┐
│   Deposit    │ ---> │   Private    │ ---> │  Withdrawal  │
│ (Transparent │      │  Transfer    │      │(Shielded to  │
│ to Shielded) │      │  (Shielded)  │      │ Transparent) │
└──────────────┘      └──────────────┘      └──────────────┘
```

### Core Components

#### 1. **Note Structure**
```
Note = {
    owner: Address (hidden),
    value: Amount (hidden),
    randomness: Random value for uniqueness,
    nullifier_seed: Secret for spending
}
```

#### 2. **Commitment Scheme**
- `Commitment = H(owner || value || randomness)`
- Uses BLS12-381 for homomorphic properties
- Commitments are stored in a Merkle tree

#### 3. **Nullifier Generation**
- `Nullifier = H(nullifier_seed || note_position)`
- Unique per note, unlinkable to commitment
- Published when note is spent

## Mathematical Foundation

### Pedersen Commitments
The system uses Pedersen commitments on BLS12-381:
- `C = g^v * h^r`
- Where `v` is the value, `r` is randomness
- Homomorphic: `C(v1) * C(v2) = C(v1 + v2)`

### Range Proofs
Bulletproofs ensure amounts are in valid range [0, 2^64):
- Proves `0 ≤ v < 2^64` without revealing `v`
- Compact size: O(log n) for range [0, n)
- Non-interactive via Fiat-Shamir transform

### Merkle Tree Membership
Notes are stored in a Merkle tree of depth 32:
- Supports up to 2^32 notes
- Membership proofs are ~1KB
- Root is updated with each deposit

### Zero-Knowledge Proof Statement
For a private transfer, the prover shows:
1. **Input notes exist**: Merkle tree membership
2. **Ownership**: Knowledge of spending keys
3. **No double-spend**: Nullifiers are fresh
4. **Balance preservation**: Sum(inputs) = Sum(outputs) + fee
5. **Range validity**: All amounts in [0, 2^64)

## Smart Contract Methods

### Core Functions

#### `deposit(amount, noteCommitment, encryptedNote)`
- Deposits funds from transparent account to shielded pool
- Creates a new private note
- Adds commitment to Merkle tree

#### `privateTransfer(inputNullifiers[], outputCommitments[], encryptedOutputs[], zkProof)`
- Transfers value privately within shielded pool
- Spends input notes (via nullifiers)
- Creates output notes
- Verifies zero-knowledge proof

#### `withdraw(recipient, amount, nullifier, zkProof)`
- Withdraws funds from shielded pool to transparent address
- Reveals the withdrawal amount (but not the source)
- Verifies ownership proof

### Query Functions

#### `getMerkleRoot()` - Returns current Merkle tree root
#### `isSpent(nullifier)` - Checks if a note has been spent
#### `getTotalShielded()` - Returns total value in shielded pool
#### `getEncryptedNote(index)` - Retrieves encrypted note data

## Usage Example

### 1. Depositing Funds
```typescript
// Alice deposits 100 tokens privately
const amount = 100;
const note = generateNote(alice.address, amount);
const commitment = computeCommitment(note);
const encryptedNote = encryptNote(note, alice.publicKey);

await contract.deposit(amount, commitment, encryptedNote);
// Alice's 100 tokens are now in the shielded pool
```

### 2. Private Transfer
```typescript
// Alice sends 40 tokens to Bob privately
const inputNote = alice.notes[0]; // 100 token note
const outputNote1 = generateNote(bob.address, 40);
const outputNote2 = generateNote(alice.address, 60); // change

const proof = generateTransferProof(
    [inputNote],
    [outputNote1, outputNote2]
);

await contract.privateTransfer(
    [computeNullifier(inputNote)],
    [computeCommitment(outputNote1), computeCommitment(outputNote2)],
    [encryptNote(outputNote1, bob.publicKey), encryptNote(outputNote2, alice.publicKey)],
    proof
);
// Transfer complete - amounts and parties remain private
```

### 3. Withdrawing Funds
```typescript
// Bob withdraws his 40 tokens
const withdrawalProof = generateWithdrawalProof(
    outputNote1,
    bob.address,
    40
);

await contract.withdraw(
    bob.address,
    40,
    computeNullifier(outputNote1),
    withdrawalProof
);
// Bob receives 40 tokens to his transparent address
```

## Cryptographic Security

### Privacy Guarantees
1. **Transaction Privacy**: Amounts and participants hidden
2. **Unlinkability**: Cannot link deposits to withdrawals
3. **Forward Secrecy**: Past transactions remain private even if keys leak

### Security Assumptions
1. **Discrete Log Problem**: BLS12-381 curve security
2. **Collision Resistance**: SHA-256 hash function
3. **Zero-Knowledge Soundness**: Cannot forge proofs
4. **Merkle Tree Security**: Cannot forge membership proofs

### Attack Resistance
- **Double Spending**: Prevented by nullifier mechanism
- **Front-running**: Nullifiers committed before revelation
- **Value Overflow**: Range proofs prevent negative/overflow amounts
- **Replay Attacks**: Each proof is unique to specific notes

## Performance Characteristics

### Gas Costs (Estimated)
- Deposit: ~3 GAS
- Private Transfer (2→2): ~8 GAS
- Withdrawal: ~4 GAS
- Proof Verification: ~2 GAS per proof

### Proof Generation Time
- Transfer Proof: ~2 seconds
- Withdrawal Proof: ~1 second
- Range Proof: ~500ms per amount

### Storage Requirements
- Note Commitment: 48 bytes (BLS12-381 G1 point)
- Nullifier: 32 bytes
- Encrypted Note: 64 bytes
- Merkle Node: 32 bytes

## Implementation Details

### BLS12-381 Operations Used
1. **Point Addition**: For homomorphic commitments
2. **Scalar Multiplication**: For commitment generation
3. **Pairing**: For proof verification
4. **Serialization**: For efficient storage

### Merkle Tree Implementation
- Binary tree with 32 levels
- Sparse tree optimization (empty nodes not stored)
- Incremental updates (O(log n) per insertion)
- Path caching for common queries

### Note Encryption
- Uses recipient's public key
- ChaCha20-Poly1305 authenticated encryption
- Forward secrecy through ephemeral keys
- 64-byte ciphertext includes MAC

## Advanced Features

### 1. **Multi-Asset Support**
The system can be extended to support multiple tokens:
```solidity
struct Note {
    address owner;
    uint256 amount;
    bytes32 assetId; // Token identifier
    bytes32 randomness;
}
```

### 2. **Delegated Transactions**
Allow third parties to submit proofs on behalf of users:
- Meta-transactions for gas abstraction
- Privacy-preserving relayers
- Batch transaction processing

### 3. **Compliance Features**
Optional compliance for regulated environments:
- Viewing keys for auditors
- Selective disclosure proofs
- Transaction limits with range proofs

## Comparison with Other Systems

| Feature | This Implementation | Zcash | Tornado Cash | Aztec |
|---------|-------------------|--------|--------------|--------|
| Privacy Level | Full | Full | Mixer | Full |
| Asset Types | NEP-17 | Native | ERC-20 | Multiple |
| Proof System | Groth16-like | Groth16 | Groth16 | PLONK |
| Note Model | UTXO | UTXO | Deposit | Account |
| Merkle Depth | 32 | 32 | 20 | 32 |

## Testing

### Unit Tests
```bash
# Test note generation
npm test test/note.test.js

# Test proof generation
npm test test/proof.test.js

# Test contract logic
npm test test/contract.test.js
```

### Integration Tests
```bash
# Deploy to Neo Express
./deploy-contract.sh

# Run full transaction cycle
./test-transaction-cycle.sh
```

## Security Considerations

### Auditing Requirements
1. **Cryptographic Review**: Proof system correctness
2. **Smart Contract Audit**: No vulnerabilities
3. **Economic Analysis**: No value extraction attacks
4. **Privacy Analysis**: No information leakage

### Best Practices
1. Use hardware wallet for note generation
2. Store note data encrypted off-chain
3. Verify Merkle roots before transactions
4. Monitor nullifier publications

## Future Enhancements

### Planned Features
1. **Recursive Proofs**: Aggregate multiple transactions
2. **Fast Sync**: Compressed chain state
3. **Mobile Support**: Lightweight proof generation
4. **Cross-chain**: Bridge to other networks

### Research Directions
1. **Post-Quantum**: Lattice-based commitments
2. **Scalability**: Layer 2 integration
3. **Privacy Pools**: Selective anonymity sets
4. **Programmable Privacy**: Smart contract interactions

## Example Output

```
=== PRIVATE TRANSACTION DEMONSTRATION ===

1. Alice deposits 100 tokens
   Transaction: 0xabc...123
   Note Commitment: 0x5f3...8a2
   Status: Shielded

2. Alice → Bob: 40 tokens (PRIVATE)
   Input Nullifiers: [0x9d2...4f1]
   Output Commitments: [0x7a8...3b2, 0x2c4...9e5]
   Proof Verified: ✓
   
3. Bob withdraws 40 tokens
   Recipient: NX8ej...2fK
   Amount: 40 (PUBLIC)
   Nullifier: 0x7a8...3b2
   
Total in Shielded Pool: 60 tokens
Active Notes: 1
Spent Nullifiers: 2

Privacy Preserved: ✓
- Alice's balance: HIDDEN
- Transfer details: HIDDEN
- Bob's other transactions: HIDDEN
```

## Contributing

We welcome contributions to improve the privacy system:
- Optimize proof generation
- Reduce gas costs
- Enhance user experience
- Add new privacy features

## References

1. [Zcash Protocol Specification](https://zips.z.cash/protocol/protocol.pdf)
2. [BLS12-381 For The Rest Of Us](https://hackmd.io/@benjaminion/bls12-381)
3. [Bulletproofs: Short Proofs for Confidential Transactions](https://eprint.iacr.org/2017/1066.pdf)
4. [Neo Smart Contract Documentation](https://docs.neo.org/docs/n3/develop/write/basics)

## License

MIT License - See LICENSE file for details

## Disclaimer

This is an example implementation for educational purposes. For production use:
- Conduct thorough security audits
- Implement complete proof systems
- Add proper key management
- Ensure regulatory compliance