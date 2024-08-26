using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types_ECPoint(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types_ECPoint"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isValid"",""parameters"":[{""name"":""point"",""type"":""PublicKey""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""ecpoint2String"",""parameters"":[],""returntype"":""String"",""offset"":14,""safe"":false},{""name"":""ecpointReturn"",""parameters"":[],""returntype"":""PublicKey"",""offset"":16,""safe"":false},{""name"":""ecpoint2ByteArray"",""parameters"":[],""returntype"":""Any"",""offset"":18,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":22,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD1XAAF4StkoUMoAIbOrQFhAWEBY2zBAVgEMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqWBAJIWWNA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ecpoint2ByteArray")]
    public abstract object? Ecpoint2ByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ecpoint2String")]
    public abstract string? Ecpoint2String();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ecpointReturn")]
    public abstract ECPoint? EcpointReturn();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isValid")]
    public abstract bool? IsValid(ECPoint? point);

    #endregion

}
