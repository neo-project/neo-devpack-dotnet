using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OracleContract : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delOracleRequest(BigInteger Id, UInt160 RequestContract, string Url, string Filter);
    public event delOracleRequest? OracleRequest;
    public delegate void delOracleResponse(BigInteger Id, UInt256 OriginalTx);
    public event delOracleResponse? OracleResponse;
    #endregion
    #region Safe methods
    public abstract BigInteger getPrice();
    public abstract bool verify();
    #endregion
    #region Unsafe methods
    public abstract void finish();
    public abstract void request(string url, string filter, string callback, object userData, BigInteger gasForResponse);
    public abstract void setPrice(BigInteger price);
    #endregion
    #region Constructor for internal use only
    protected OracleContract(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
