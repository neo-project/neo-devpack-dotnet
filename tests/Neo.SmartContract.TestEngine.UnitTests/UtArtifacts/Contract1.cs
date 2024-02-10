using System.Numerics;

namespace Neo.TestEngine.Contracts;

public abstract class Contract1 : Neo.SmartContract.TestEngine.Mocks.SmartContract
{
    #region Events
    public delegate void delSetOwner(UInt160 newOwner);
    public event delSetOwner? SetOwner;
    public delegate void delTransfer(UInt160 from, UInt160 to, BigInteger amount);
    public event delTransfer? Transfer;
    #endregion
    #region Safe methods
    public abstract BigInteger balanceOf(UInt160 owner);
    public abstract BigInteger decimals();
    public abstract UInt160 getOwner();
    public abstract string symbol();
    public abstract BigInteger totalSupply();
    public abstract bool verify();
    #endregion
    #region Unsafe methods
    public abstract void burn(UInt160 account, BigInteger amount);
    public abstract void mint(UInt160 to, BigInteger amount);
    public abstract string myMethod();
    public abstract void onNEP17Payment(UInt160 from, BigInteger amount, object data);
    public abstract void setOwner(UInt160 newOwner);
    public abstract bool transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    public abstract void update(byte[] nefFile, string manifest);
    public abstract bool withdraw(UInt160 token, UInt160 to, BigInteger amount);
    #endregion
    #region Constructor for internal use only
    protected Contract1(Neo.SmartContract.TestEngine.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
