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
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.PUSH10 [1 datoshi]
    /// 09 : OpCode.PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0F : OpCode.PUSH2 [1 datoshi]
    /// 10 : OpCode.PICK [2 datoshi]
    /// 11 : OpCode.CALL 0C [512 datoshi]
    /// 13 : OpCode.STLOC0 [2 datoshi]
    /// 14 : OpCode.LDLOC0 [2 datoshi]
    /// 15 : OpCode.PUSH1 [1 datoshi]
    /// 16 : OpCode.PICKITEM [64 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.PUSH0 [1 datoshi]
    /// 19 : OpCode.PICKITEM [64 datoshi]
    /// 1A : OpCode.PUSH2 [1 datoshi]
    /// 1B : OpCode.PACKSTRUCT [2048 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty")]
    public abstract IList<object>? TestProperty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk002XBA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.PUSH10 [1 datoshi]
    /// 09 : OpCode.PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0F : OpCode.PUSH2 [1 datoshi]
    /// 10 : OpCode.PICK [2 datoshi]
    /// 11 : OpCode.CALL D9 [512 datoshi]
    /// 13 : OpCode.STLOC0 [2 datoshi]
    /// 14 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty2")]
    public abstract void TestProperty2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CxALE8BKNA0MBE5FTzNLEFHQQA==
    /// 00 : OpCode.PUSHNULL [1 datoshi]
    /// 01 : OpCode.PUSH0 [1 datoshi]
    /// 02 : OpCode.PUSHNULL [1 datoshi]
    /// 03 : OpCode.PUSH3 [1 datoshi]
    /// 04 : OpCode.PACK [2048 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.CALL 0D [512 datoshi]
    /// 08 : OpCode.PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0E : OpCode.OVER [2 datoshi]
    /// 0F : OpCode.PUSH0 [1 datoshi]
    /// 10 : OpCode.ROT [2 datoshi]
    /// 11 : OpCode.SETITEM [8192 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty3")]
    public abstract object? TestProperty3();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: yEoMBE5hbWUMBE5FTzPQQA==
    /// 00 : OpCode.NEWMAP [8 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.PUSHDATA1 4E616D65 'Name' [8 datoshi]
    /// 08 : OpCode.PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0E : OpCode.SETITEM [8192 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty4")]
    public abstract IDictionary<object, object>? TestProperty4();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: wkUVFBMSERXAQA==
    /// 00 : OpCode.NEWARRAY0 [16 datoshi]
    /// 01 : OpCode.DROP [2 datoshi]
    /// 02 : OpCode.PUSH5 [1 datoshi]
    /// 03 : OpCode.PUSH4 [1 datoshi]
    /// 04 : OpCode.PUSH3 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PUSH1 [1 datoshi]
    /// 07 : OpCode.PUSH5 [1 datoshi]
    /// 08 : OpCode.PACK [2048 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testProperty5")]
    public abstract IList<object>? TestProperty5();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AaDARORU8zEk00jwwRMTIzIEJsb2NrY2hhaW4gU3RLElHQcGgSzmgRzmgQzhO/QA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.PUSH10 [1 datoshi]
    /// 09 : OpCode.PUSHDATA1 4E454F33 'NEO3' [8 datoshi]
    /// 0F : OpCode.PUSH2 [1 datoshi]
    /// 10 : OpCode.PICK [2 datoshi]
    /// 11 : OpCode.CALL 8F [512 datoshi]
    /// 13 : OpCode.PUSHDATA1 31323320426C6F636B636861696E205374 [8 datoshi]
    /// 26 : OpCode.OVER [2 datoshi]
    /// 27 : OpCode.PUSH2 [1 datoshi]
    /// 28 : OpCode.ROT [2 datoshi]
    /// 29 : OpCode.SETITEM [8192 datoshi]
    /// 2A : OpCode.STLOC0 [2 datoshi]
    /// 2B : OpCode.LDLOC0 [2 datoshi]
    /// 2C : OpCode.PUSH2 [1 datoshi]
    /// 2D : OpCode.PICKITEM [64 datoshi]
    /// 2E : OpCode.LDLOC0 [2 datoshi]
    /// 2F : OpCode.PUSH1 [1 datoshi]
    /// 30 : OpCode.PICKITEM [64 datoshi]
    /// 31 : OpCode.LDLOC0 [2 datoshi]
    /// 32 : OpCode.PUSH0 [1 datoshi]
    /// 33 : OpCode.PICKITEM [64 datoshi]
    /// 34 : OpCode.PUSH3 [1 datoshi]
    /// 35 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 36 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyInit")]
    public abstract IList<object>? TestPropertyInit();

    #endregion
}
