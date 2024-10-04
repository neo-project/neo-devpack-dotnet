using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticVarInit(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticVarInit"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""staticInit"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":false},{""name"":""directGet"",""parameters"":[],""returntype"":""Hash160"",""offset"":5,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":11,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQ0A0BYQEHb/qh0QFYBQdv+qHRgQCYfjt4="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.SYSCALL DBFEA874
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("directGet")]
    public abstract UInt160? DirectGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.CALL 03
    /// 0002 : OpCode.RET
    /// </remarks>
    [DisplayName("staticInit")]
    public abstract UInt160? StaticInit();

    #endregion

}
