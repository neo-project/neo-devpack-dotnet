using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PropertyMethod(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PropertyMethod"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testProperty"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testProperty2"",""parameters"":[],""returntype"":""Void"",""offset"":47,""safe"":false},{""name"":""testProperty3"",""parameters"":[],""returntype"":""Any"",""offset"":68,""safe"":false},{""name"":""testProperty4"",""parameters"":[],""returntype"":""Map"",""offset"":84,""safe"":false},{""name"":""testProperty5"",""parameters"":[],""returntype"":""Array"",""offset"":100,""safe"":false},{""name"":""testPropertyInit"",""parameters"":[],""returntype"":""Array"",""offset"":110,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKVXAQALEAsTwBoMBE5FTzMSTTQMcGgRzmgQzhK/QFcAA3lKeBBR0EV6SngRUdBFQFcBAAsQCxPAGgwETkVPMxJNNN1wQAsQCxPADARORU8zSxBR0EDISgwETmFtZQwETkVPM9BAwkUVFBMSERXAQFcBAAsQCxPAGgwETkVPMxJNNJ4METEyMyBCbG9ja2NoYWluIFN0SxJR0HBoEs5oEc5oEM4Tv0DdDgQJ"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk00DHBoEc5oEM4Sv0A=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH10 [1 datoshi]
    /// PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 0C [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty")]
    public abstract IList<object>? TestProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk003XBA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH10 [1 datoshi]
    /// PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL DD [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty2")]
    public abstract void TestProperty2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxALE8AMBE5FTzNLEFHQQA==
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// OVER [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty3")]
    public abstract object? TestProperty3();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: yEoMBE5hbWUMBE5FTzPQQA==
    /// NEWMAP [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 4E616D65 'Name' [8 datoshi]
    /// PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// SETITEM [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty4")]
    public abstract IDictionary<object, object>? TestProperty4();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: wkUVFBMSERXAQA==
    /// NEWARRAY0 [16 datoshi]
    /// DROP [2 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty5")]
    public abstract IList<object>? TestProperty5();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk00ngwRMTIzIEJsb2NrY2hhaW4gU3RLElHQcGgSzmgRzmgQzhO/QA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH10 [1 datoshi]
    /// PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 9E [512 datoshi]
    /// PUSHDATA1 31323320426C6F636B636861696E205374 [8 datoshi]
    /// OVER [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyInit")]
    public abstract IList<object>? TestPropertyInit();

    #endregion
}
