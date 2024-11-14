using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PropertyMethod(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PropertyMethod"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testProperty"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testProperty2"",""parameters"":[],""returntype"":""Void"",""offset"":51,""safe"":false},{""name"":""testProperty3"",""parameters"":[],""returntype"":""Any"",""offset"":72,""safe"":false},{""name"":""testProperty4"",""parameters"":[],""returntype"":""Map"",""offset"":99,""safe"":false},{""name"":""testProperty5"",""parameters"":[],""returntype"":""Array"",""offset"":115,""safe"":false},{""name"":""testPropertyInit"",""parameters"":[],""returntype"":""Array"",""offset"":125,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALRXAQALEAsTwBoMBE5FTzMSTTQMcGgRzmgQzhK/QFcAA3gRENB5SngQUdBFekp4EVHQRUBXAQALEAsTwBoMBE5FTzMSTTTZcEALEAsTwEo0DQwETkVPM0sQUdBAVwABeBEQ0EDISgwETmFtZQwETkVPM9BAwkUVFBMSERXAQFcBAAsQCxPAGgwETkVPMxJNNI8METEyMyBCbG9ja2NoYWluIFN0SxJR0HBoEs5oEc5oEM4Tv0BYGk0b"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk00DHBoEc5oEM4Sv0A=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : PUSH10 [1 datoshi]
    /// 09 : PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0F : PUSH2 [1 datoshi]
    /// 10 : PICK [2 datoshi]
    /// 11 : CALL 0C [512 datoshi]
    /// 13 : STLOC0 [2 datoshi]
    /// 14 : LDLOC0 [2 datoshi]
    /// 15 : PUSH1 [1 datoshi]
    /// 16 : PICKITEM [64 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : PUSH0 [1 datoshi]
    /// 19 : PICKITEM [64 datoshi]
    /// 1A : PUSH2 [1 datoshi]
    /// 1B : PACKSTRUCT [2048 datoshi]
    /// 1C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty")]
    public abstract IList<object>? TestProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk002XBA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : PUSH10 [1 datoshi]
    /// 09 : PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0F : PUSH2 [1 datoshi]
    /// 10 : PICK [2 datoshi]
    /// 11 : CALL D9 [512 datoshi]
    /// 13 : STLOC0 [2 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty2")]
    public abstract void TestProperty2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxALE8BKNA0MBE5FTzNLEFHQQA==
    /// 00 : PUSHNULL [1 datoshi]
    /// 01 : PUSH0 [1 datoshi]
    /// 02 : PUSHNULL [1 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : PACK [2048 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : CALL 0D [512 datoshi]
    /// 08 : PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0E : OVER [2 datoshi]
    /// 0F : PUSH0 [1 datoshi]
    /// 10 : ROT [2 datoshi]
    /// 11 : SETITEM [8192 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty3")]
    public abstract object? TestProperty3();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: yEoMBE5hbWUMBE5FTzPQQA==
    /// 00 : NEWMAP [8 datoshi]
    /// 01 : DUP [2 datoshi]
    /// 02 : PUSHDATA1 4E616D65 'Name' [8 datoshi]
    /// 08 : PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty4")]
    public abstract IDictionary<object, object>? TestProperty4();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: wkUVFBMSERXAQA==
    /// 00 : NEWARRAY0 [16 datoshi]
    /// 01 : DROP [2 datoshi]
    /// 02 : PUSH5 [1 datoshi]
    /// 03 : PUSH4 [1 datoshi]
    /// 04 : PUSH3 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : PUSH5 [1 datoshi]
    /// 08 : PACK [2048 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty5")]
    public abstract IList<object>? TestProperty5();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk00jwwRMTIzIEJsb2NrY2hhaW4gU3RLElHQcGgSzmgRzmgQzhO/QA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : PUSH10 [1 datoshi]
    /// 09 : PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0F : PUSH2 [1 datoshi]
    /// 10 : PICK [2 datoshi]
    /// 11 : CALL 8F [512 datoshi]
    /// 13 : PUSHDATA1 31323320426C6F636B636861696E205374 [8 datoshi]
    /// 26 : OVER [2 datoshi]
    /// 27 : PUSH2 [1 datoshi]
    /// 28 : ROT [2 datoshi]
    /// 29 : SETITEM [8192 datoshi]
    /// 2A : STLOC0 [2 datoshi]
    /// 2B : LDLOC0 [2 datoshi]
    /// 2C : PUSH2 [1 datoshi]
    /// 2D : PICKITEM [64 datoshi]
    /// 2E : LDLOC0 [2 datoshi]
    /// 2F : PUSH1 [1 datoshi]
    /// 30 : PICKITEM [64 datoshi]
    /// 31 : LDLOC0 [2 datoshi]
    /// 32 : PUSH0 [1 datoshi]
    /// 33 : PICKITEM [64 datoshi]
    /// 34 : PUSH3 [1 datoshi]
    /// 35 : PACKSTRUCT [2048 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyInit")]
    public abstract IList<object>? TestPropertyInit();

    #endregion
}
