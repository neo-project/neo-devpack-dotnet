using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class LedgerContract : Neo.SmartContract.Testing.SmartContract
{
#region Safe methods
    public abstract UInt256 currentHash();
    public abstract BigInteger currentIndex();
    public abstract List<object> getBlock(byte[] indexOrHash);
    public abstract List<object> getTransaction(UInt256 hash);
    public abstract List<object> getTransactionFromBlock(byte[] blockIndexOrHash, BigInteger txIndex);
    public abstract BigInteger getTransactionHeight(UInt256 hash);
    public abstract List<object> getTransactionSigners(UInt256 hash);
    public abstract BigInteger getTransactionVMState(UInt256 hash);
#endregion
#region Constructor for internal use only
    protected LedgerContract(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
#endregion
}