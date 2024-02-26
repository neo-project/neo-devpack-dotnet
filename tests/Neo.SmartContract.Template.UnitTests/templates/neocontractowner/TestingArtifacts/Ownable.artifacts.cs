using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Ownable : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.IOwnable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Ownable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":62,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":216,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":242,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":377,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":422,""safe"":false}],""events"":[{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy42LjIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACCaHR0cHM6Ly9naXRodWIuY29tL25lby1wcm9qZWN0L25lby1kZXZwYWNrLWRvdG5ldC90cmVlL21hc3Rlci9zcmMvTmVvLlNtYXJ0Q29udHJhY3QuVGVtcGxhdGUvdGVtcGxhdGVzL25lb2NvbnRyYWN0b3duZXIvT3duYWJsZS5jcwAC/aP6Q0bqUyolj8SX3a3bZDfJ/f8GdXBkYXRlAwAAD/2j+kNG6lMqJY/El92t22Q3yf3/B2Rlc3Ryb3kAAAAPAAD9ygEMAf/bMDQQStgkCUrKABQoAzoiAkBXAAF4Qfa0a+JBkl3oMUBBkl3oMUBB9rRr4kA00EH4J+yMQEH4J+yMQFcBATTvENsglyYWDBFObyBBdXRob3JpemF0aW9uITp4StkoUMoAFLOrJAcQ2yAiBngQs6oME293bmVyIG11c3QgYmUgdmFsaWThNXj///9weAwB/9swNCjCSmjPSnjPDAhTZXRPd25lckGVAW9hQOFAStkoUMoAFLOrQBCzQFcAAnl4QZv2Z85B5j8YhEBB5j8YhEBBm/ZnzkAMBUhlbGxvQZv2Z85Bkl3oMSICQEGSXegxQFcBAnkmBCJ0eHBoC5cmDEEtUQgwE85KgEV4cGhK2ShQygAUs6skBxDbICIGaBCzqgwRb3duZXIgbXVzdCBleGlzdHPhaAwB/9swNIDCSgvPSmjPDAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQEEtUQgwQEHmPxiEQFcAAzW0/v//ENsglyYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AABANwAAQDWK/v//qiYWDBFObyBhdXRob3JpemF0aW9uLjo3AQBANwEAQOAcT+k="));

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected Ownable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
