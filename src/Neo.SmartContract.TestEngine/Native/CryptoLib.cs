using System.Collections.Generic;
using System.Numerics;

namespace Neo.TestEngine.Contracts;

public abstract class CryptoLib : Neo.SmartContract.TestEngine.Mocks.SmartContract
{
    #region Safe methods
    public abstract object bls12381Add(object x, object y);
    public abstract object bls12381Deserialize(byte[] data);
    public abstract bool bls12381Equal(object x, object y);
    public abstract object bls12381Mul(object x, byte[] mul, bool neg);
    public abstract object bls12381Pairing(object g1, object g2);
    public abstract byte[] bls12381Serialize(object g);
    public abstract byte[] murmur32(byte[] data, BigInteger seed);
    public abstract byte[] ripemd160(byte[] data);
    public abstract byte[] sha256(byte[] data);
    public abstract bool verifyWithECDsa(byte[] message, byte[] pubkey, byte[] signature, BigInteger curve);
    #endregion
    #region Constructor for internal use only
    protected CryptoLib(Neo.SmartContract.TestEngine.Engine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
