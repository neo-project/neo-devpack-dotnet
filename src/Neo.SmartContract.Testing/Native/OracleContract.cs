using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OracleContract : Neo.SmartContract.Testing.SmartContract
{
    #region Events
    public delegate void delOracleRequest(BigInteger Id, UInt160 RequestContract, string Url, string Filter);
    [DisplayName("OracleRequest")]
    public event delOracleRequest? OnOracleRequest;
    public delegate void delOracleResponse(BigInteger Id, UInt256 OriginalTx);
    [DisplayName("OracleResponse")]
    public event delOracleResponse? OnOracleResponse;
    #endregion
    #region Properties
    public abstract BigInteger Price { [DisplayName("getPrice")] get; [DisplayName("setPrice")] set; }
    public abstract bool Verify { [DisplayName("verify")] get; }
    #endregion
    #region Unsafe methods
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("finish")]
    public abstract void Finish();
    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("request")]
    public abstract void Request(string url, string filter, string callback, object userData, BigInteger gasForResponse);
    #endregion
    #region Constructor for internal use only
    protected OracleContract(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) {}
    #endregion
}
