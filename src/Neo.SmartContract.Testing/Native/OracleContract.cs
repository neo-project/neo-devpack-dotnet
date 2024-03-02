using System.ComponentModel;

namespace Neo.SmartContract.Testing.Native;

public abstract class OracleContract : SmartContract, TestingStandards.IVerificable
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""OracleContract"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""finish"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""getPrice"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""request"",""parameters"":[{""name"":""url"",""type"":""String""},{""name"":""filter"",""type"":""String""},{""name"":""callback"",""type"":""String""},{""name"":""userData"",""type"":""Any""},{""name"":""gasForResponse"",""type"":""Integer""}],""returntype"":""Void"",""offset"":14,""safe"":false},{""name"":""setPrice"",""parameters"":[{""name"":""price"",""type"":""Integer""}],""returntype"":""Void"",""offset"":21,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":28,""safe"":true}],""events"":[{""name"":""OracleRequest"",""parameters"":[{""name"":""Id"",""type"":""Integer""},{""name"":""RequestContract"",""type"":""Hash160""},{""name"":""Url"",""type"":""String""},{""name"":""Filter"",""type"":""String""}]},{""name"":""OracleResponse"",""parameters"":[{""name"":""Id"",""type"":""Integer""},{""name"":""OriginalTx"",""type"":""Hash256""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

    #region Events

    public delegate void delOracleRequest(ulong Id, UInt160 RequestContract, string Url, string? Filter);

    [DisplayName("OracleRequest")]
    public event delOracleRequest? OnOracleRequest;

    public delegate void delOracleResponse(ulong Id, UInt256 OriginalTx);

    [DisplayName("OracleResponse")]
    public event delOracleResponse? OnOracleResponse;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract long Price { [DisplayName("getPrice")] get; [DisplayName("setPrice")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? Verify { [DisplayName("verify")] get; }

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
    public abstract void Request(string url, string? filter, string callback, object? userData, ulong gasForResponse);

    #endregion

    #region Constructor for internal use only

    protected OracleContract(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
