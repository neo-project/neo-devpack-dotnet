using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class PolicyContract : Neo.SmartContract.Testing.SmartContract
{
#region Safe methods
    public abstract BigInteger getAttributeFee(BigInteger attributeType);
    public abstract BigInteger getExecFeeFactor();
    public abstract BigInteger getFeePerByte();
    public abstract BigInteger getStoragePrice();
    public abstract bool isBlocked(UInt160 account);
#endregion
#region Unsafe methods
    public abstract bool blockAccount(UInt160 account);
    public abstract void setAttributeFee(BigInteger attributeType, BigInteger value);
    public abstract void setExecFeeFactor(BigInteger value);
    public abstract void setFeePerByte(BigInteger value);
    public abstract void setStoragePrice(BigInteger value);
    public abstract bool unblockAccount(UInt160 account);
#endregion
#region Constructor for internal use only
    protected PolicyContract(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
#endregion
}