using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
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
    #region Properties
    public abstract BigInteger Price { [DisplayName("getPrice")] get; [DisplayName("setPrice")] set; }
    public abstract bool verify { [DisplayName("verify")] get; }
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract void finish();
    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract void request(string url, string filter, string callback, object userData, BigInteger gasForResponse);
    #endregion
    #region Constructor for internal use only
    protected OracleContract(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}