# Native Contract: CryptoLib

Namespace: `Neo.SmartContract.Framework.Native`

Provides common cryptographic functions useful within smart contracts.

## Key Methods

*   **`Sha256(ByteString value)` (`ByteString`)**: Computes the SHA-256 hash of the input data.
*   **`Sha256(byte[] value)` (`byte[]`)**: Computes the SHA-256 hash of the input data.

*   **`Ripemd160(ByteString value)` (`ByteString`)**: Computes the RIPEMD-160 hash of the input data.
*   **`Ripemd160(byte[] value)` (`byte[]`)**: Computes the RIPEMD-160 hash of the input data.

*   **`Murmur32(ByteString value, uint seed)` (`ByteString`)**: Computes the Murmur32 hash.
*   **`Murmur32(byte[] value, uint seed)` (`byte[]`)**: Computes the Murmur32 hash.

*   **`VerifyWithECDsaSecp256r1(ByteString message, ECPoint pubkey, ByteString signature)` (`bool`)**: Verifies a signature against a message and public key using the ECDSA algorithm with the secp256r1 curve (the standard curve used by Neo).
*   **`VerifyWithECDsaSecp256r1(byte[] message, ECPoint pubkey, byte[] signature)` (`bool`)**: Verifies using `byte[]` inputs.

*   **`VerifyWithECDsaSecp256k1(ByteString message, ECPoint pubkey, ByteString signature)` (`bool`)**: Verifies a signature using the ECDSA algorithm with the secp256k1 curve (used by Bitcoin, Ethereum).
*   **`VerifyWithECDsaSecp256k1(byte[] message, ECPoint pubkey, byte[] signature)` (`bool`)**: Verifies using `byte[]` inputs.

*   **`CheckMultisigWithECDsaSecp256r1(ByteString message, ECPoint[] pubkeys, ByteString[] signatures)` (`bool`)**: Verifies multiple signatures against a message for a multi-sig scenario using secp256r1. Requires a quorum (typically M-of-N) of valid signatures corresponding to the provided public keys.
*   **`CheckMultisigWithECDsaSecp256r1(byte[] message, ECPoint[] pubkeys, byte[][] signatures)` (`bool`)**: Verifies using `byte[]` inputs.

*   **`CheckMultisigWithECDsaSecp256k1(ByteString message, ECPoint[] pubkeys, ByteString[] signatures)` (`bool`)**: Verifies multiple signatures using secp256k1.
*   **`CheckMultisigWithECDsaSecp256k1(byte[] message, ECPoint[] pubkeys, byte[][] signatures)` (`bool`)**: Verifies using `byte[]` inputs.

## Example Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

[ContractPermission(nameof(CryptoLib), "*")] // Allow calling all CryptoLib methods
public class CryptoDemo : SmartContract
{
    public static ByteString HashDataSha256(string data)
    {
        ByteString byteData = (ByteString)data;
        return CryptoLib.Sha256(byteData);
    }

    public static ByteString HashDataRipemd160(string data)
    {
        ByteString byteData = (ByteString)data;
        return CryptoLib.Ripemd160(byteData);
    }

    // Example: Verify a signature provided externally
    // Note: Typically Runtime.CheckWitness is used for transaction authorization.
    // This method is for verifying signatures provided as arguments, e.g., for off-chain message signing.
    public static bool VerifyExternalSignature(
        ByteString message,
        ECPoint publicKey, // Public key of the claimed signer
        ByteString signature) // The signature provided
    {
        // Assumes secp256r1 curve (standard Neo)
        return CryptoLib.VerifyWithECDsaSecp256r1(message, publicKey, signature);
    }
}
```

**Note on Signature Verification:**

*   For standard transaction authorization (checking if a user signed the *current* transaction), always use `Runtime.CheckWitness(hashOrPubkey)`. It's more efficient and integrates directly with the transaction context.
*   Use `CryptoLib.VerifyWithECDsa*` primarily for verifying signatures that were created *outside* the current transaction context, for example, verifying signed messages provided as arguments to your contract method.

[Previous: ContractManagement](./ContractManagement.md) | [Next: GasToken (GAS)](./GasToken.md)