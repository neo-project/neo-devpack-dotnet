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
    /// 00 : OpCode.LDSFLD2 [2 datoshi]
    /// 01 : OpCode.ISNULL [2 datoshi]
    /// 02 : OpCode.JMPIFNOT 1E [2 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.PUSHDATA1 6E6F5265656E7472616E74 [8 datoshi]
    /// 15 : OpCode.PUSHINT16 FF00 [1 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.PICK [2 datoshi]
    /// 1A : OpCode.CALL_L 76FFFFFF [512 datoshi]
    /// 1F : OpCode.STSFLD2 [2 datoshi]
    /// 20 : OpCode.LDSFLD2 [2 datoshi]
    /// 21 : OpCode.CALL 8F [512 datoshi]
    /// 23 : OpCode.CALL_L 49FFFFFF [512 datoshi]
    /// 28 : OpCode.LDSFLD2 [2 datoshi]
    /// 29 : OpCode.CALL C3 [512 datoshi]
    /// 2B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantA")]
    public abstract void ReentrantA();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WdgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00CmFZNCZZNF9A
    /// 00 : OpCode.LDSFLD1 [2 datoshi]
    /// 01 : OpCode.ISNULL [2 datoshi]
    /// 02 : OpCode.JMPIFNOT 1B [2 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.PUSHDATA1 6E6F5265656E7472616E74 [8 datoshi]
    /// 15 : OpCode.PUSHINT16 FF00 [1 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.PICK [2 datoshi]
    /// 1A : OpCode.CALL 0A [512 datoshi]
    /// 1C : OpCode.STSFLD1 [2 datoshi]
    /// 1D : OpCode.LDSFLD1 [2 datoshi]
    /// 1E : OpCode.CALL 26 [512 datoshi]
    /// 20 : OpCode.LDSFLD1 [2 datoshi]
    /// 21 : OpCode.CALL 5F [512 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantB")]
    public abstract void ReentrantB();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABW9gmHQsLEsAMDXJlZW50cmFudFRlc3QB/wASTTQaY1s0NngQlyYEIgt4AHuXJgUQNM1bNF9A
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDSFLD3 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 1D [2 datoshi]
    /// 07 : OpCode.PUSHNULL [1 datoshi]
    /// 08 : OpCode.PUSHNULL [1 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.PACK [2048 datoshi]
    /// 0B : OpCode.PUSHDATA1 7265656E7472616E7454657374 [8 datoshi]
    /// 1A : OpCode.PUSHINT16 FF00 [1 datoshi]
    /// 1D : OpCode.PUSH2 [1 datoshi]
    /// 1E : OpCode.PICK [2 datoshi]
    /// 1F : OpCode.CALL 1A [512 datoshi]
    /// 21 : OpCode.STSFLD3 [2 datoshi]
    /// 22 : OpCode.LDSFLD3 [2 datoshi]
    /// 23 : OpCode.CALL 36 [512 datoshi]
    /// 25 : OpCode.LDARG0 [2 datoshi]
    /// 26 : OpCode.PUSH0 [1 datoshi]
    /// 27 : OpCode.EQUAL [32 datoshi]
    /// 28 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 2A : OpCode.JMP 0B [2 datoshi]
    /// 2C : OpCode.LDARG0 [2 datoshi]
    /// 2D : OpCode.PUSHINT8 7B [1 datoshi]
    /// 2F : OpCode.EQUAL [32 datoshi]
    /// 30 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 32 : OpCode.PUSH0 [1 datoshi]
    /// 33 : OpCode.CALL CD [512 datoshi]
    /// 35 : OpCode.LDSFLD3 [2 datoshi]
    /// 36 : OpCode.CALL 5F [512 datoshi]
    /// 38 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WNgmKAsRwAwcQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBPRFNNAhgWDQhCEA=
    /// 00 : OpCode.LDSFLD0 [2 datoshi]
    /// 01 : OpCode.ISNULL [2 datoshi]
    /// 02 : OpCode.JMPIFNOT 28 [2 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSH1 [1 datoshi]
    /// 06 : OpCode.PACK [2048 datoshi]
    /// 07 : OpCode.PUSHDATA1 4141414141414141414141414141414141414141414141414141413D [8 datoshi]
    /// 25 : OpCode.PUSH1 [1 datoshi]
    /// 26 : OpCode.PICK [2 datoshi]
    /// 27 : OpCode.CALL 08 [512 datoshi]
    /// 29 : OpCode.STSFLD0 [2 datoshi]
    /// 2A : OpCode.LDSFLD0 [2 datoshi]
    /// 2B : OpCode.CALL 21 [512 datoshi]
    /// 2D : OpCode.PUSHT [1 datoshi]
    /// 2E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract bool? Test();

    #endregion
}
