using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class GasToken : Neo.SmartContract.Testing.SmartContract
{
#region Events
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    public event delTransfer? Transfer;
#endregion
#region Safe methods
    public abstract BigInteger balanceOf(UInt160 account);
    public abstract BigInteger decimals();
    public abstract string symbol();
    public abstract BigInteger totalSupply();
#endregion
#region Unsafe methods
    public abstract bool transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
#endregion
#region Constructor for internal use only
    protected GasToken(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
#endregion
}