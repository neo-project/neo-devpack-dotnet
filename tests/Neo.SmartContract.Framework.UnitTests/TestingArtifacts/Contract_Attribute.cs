using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Attribute(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Attribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""reentrantB"",""parameters"":[],""returntype"":""Void"",""offset"":102,""safe"":false},{""name"":""reentrantA"",""parameters"":[],""returntype"":""Void"",""offset"":250,""safe"":false},{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":294,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":463,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base64Decode""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPAAD90gFY2CYoCxHADBxBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUE9EU00CGBYNCEIQFcAAnk3AADbMNsoStgkCUrKABQoAzpKeBBR0EVAVwABeBDOQfgn7IwkDgwJZXhjZXB0aW9uOkBZ2CYbCwsSwAwLbm9SZWVudHJhbnQB/wASTTQKYVk0Jlk0X0BXAAN6SngRUdBFQZv2Z855EYhOEFHQUBLASngQUdBFQFcBAXgRzngQzsFFU4tQQZJd6DFwaNgkFAwPQWxyZWFkeSBlbnRlcmVk4BF4Ec54EM7BRVOLUEHmPxiEQFcAAXgRzngQzsFFU4tQQS9Yxe1AWtgmHgsLEsAMC25vUmVlbnRyYW50Af8AEk01dv///2JaNI81Sf///1o0w0BXAAFb2CYdCwsSwAwNcmVlbnRyYW50VGVzdAH/ABJNNBpjWzQ2eBCXJgQiC3gAe5cmBRA0zVs0X0BXAAN6SngRUdBFQZv2Z855EYhOEFHQUBLASngQUdBFQFcBAXgRzngQzsFFU4tQQZJd6DFwaNgkFAwPQWxyZWFkeSBlbnRlcmVk4BF4Ec54EM7BRVOLUEHmPxiEQFcAAXgRzngQzsFFU4tQQS9Yxe1AVgRAolSrOg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WtgmHgsLEsAMC25vUmVlbnRyYW50Af8AEk01dv///2JaNI81Sf///1o0w0A=
    /// 00 : LDSFLD2 [2 datoshi]
    /// 01 : ISNULL [2 datoshi]
    /// 02 : JMPIFNOT 1E [2 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// 15 : PUSHINT16 FF00 [1 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : PICK [2 datoshi]
    /// 1A : CALL_L 76FFFFFF [512 datoshi]
    /// 1F : STSFLD2 [2 datoshi]
    /// 20 : LDSFLD2 [2 datoshi]
    /// 21 : CALL 8F [512 datoshi]
    /// 23 : CALL_L 49FFFFFF [512 datoshi]
    /// 28 : LDSFLD2 [2 datoshi]
    /// 29 : CALL C3 [512 datoshi]
    /// 2B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantA")]
    public abstract void ReentrantA();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WdgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00CmFZNCZZNF9A
    /// 00 : LDSFLD1 [2 datoshi]
    /// 01 : ISNULL [2 datoshi]
    /// 02 : JMPIFNOT 1B [2 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// 15 : PUSHINT16 FF00 [1 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : PICK [2 datoshi]
    /// 1A : CALL 0A [512 datoshi]
    /// 1C : STSFLD1 [2 datoshi]
    /// 1D : LDSFLD1 [2 datoshi]
    /// 1E : CALL 26 [512 datoshi]
    /// 20 : LDSFLD1 [2 datoshi]
    /// 21 : CALL 5F [512 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantB")]
    public abstract void ReentrantB();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABW9gmHQsLEsAMDXJlZW50cmFudFRlc3QB/wASTTQaY1s0NngQlyYEIgt4AHuXJgUQNM1bNF9A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDSFLD3 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 1D [2 datoshi]
    /// 07 : PUSHNULL [1 datoshi]
    /// 08 : PUSHNULL [1 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PACK [2048 datoshi]
    /// 0B : PUSHDATA1 7265656E7472616E7454657374 'reentrantTest' [8 datoshi]
    /// 1A : PUSHINT16 FF00 [1 datoshi]
    /// 1D : PUSH2 [1 datoshi]
    /// 1E : PICK [2 datoshi]
    /// 1F : CALL 1A [512 datoshi]
    /// 21 : STSFLD3 [2 datoshi]
    /// 22 : LDSFLD3 [2 datoshi]
    /// 23 : CALL 36 [512 datoshi]
    /// 25 : LDARG0 [2 datoshi]
    /// 26 : PUSH0 [1 datoshi]
    /// 27 : EQUAL [32 datoshi]
    /// 28 : JMPIFNOT 04 [2 datoshi]
    /// 2A : JMP 0B [2 datoshi]
    /// 2C : LDARG0 [2 datoshi]
    /// 2D : PUSHINT8 7B [1 datoshi]
    /// 2F : EQUAL [32 datoshi]
    /// 30 : JMPIFNOT 05 [2 datoshi]
    /// 32 : PUSH0 [1 datoshi]
    /// 33 : CALL CD [512 datoshi]
    /// 35 : LDSFLD3 [2 datoshi]
    /// 36 : CALL 5F [512 datoshi]
    /// 38 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WNgmKAsRwAwcQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBPRFNNAhgWDQhCEA=
    /// 00 : LDSFLD0 [2 datoshi]
    /// 01 : ISNULL [2 datoshi]
    /// 02 : JMPIFNOT 28 [2 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSH1 [1 datoshi]
    /// 06 : PACK [2048 datoshi]
    /// 07 : PUSHDATA1 4141414141414141414141414141414141414141414141414141413D [8 datoshi]
    /// 25 : PUSH1 [1 datoshi]
    /// 26 : PICK [2 datoshi]
    /// 27 : CALL 08 [512 datoshi]
    /// 29 : STSFLD0 [2 datoshi]
    /// 2A : LDSFLD0 [2 datoshi]
    /// 2B : CALL 21 [512 datoshi]
    /// 2D : PUSHT [1 datoshi]
    /// 2E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract bool? Test();

    #endregion
}
