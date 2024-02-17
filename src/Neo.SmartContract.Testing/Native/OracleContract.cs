using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OracleContract : SmartContract
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

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger Price { [DisplayName("getPrice")] get; [DisplayName("setPrice")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
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
    public abstract void Request(string? url, string? filter, string? callback, object? userData, BigInteger? gasForResponse);

    #endregion

    #region Constructor for internal use only

    protected OracleContract(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
