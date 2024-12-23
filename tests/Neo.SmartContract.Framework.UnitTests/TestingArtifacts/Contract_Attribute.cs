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
    /// LDSFLD2 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 1E [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL_L 76FFFFFF [512 datoshi]
    /// STSFLD2 [2 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// CALL 8F [512 datoshi]
    /// CALL_L 49FFFFFF [512 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// CALL C3 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantA")]
    public abstract void ReentrantA();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WdgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00CmFZNCZZNF9A
    /// LDSFLD1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 1B [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 0A [512 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL 26 [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// CALL 5F [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantB")]
    public abstract void ReentrantB();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABW9gmHQsLEsAMDXJlZW50cmFudFRlc3QB/wASTTQaY1s0NngQlyYEIgt4AHuXJgUQNM1bNF9A
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 1D [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 7265656E7472616E7454657374 'reentrantTest' [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 1A [512 datoshi]
    /// STSFLD3 [2 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// CALL 36 [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// JMP 0B [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// CALL CD [512 datoshi]
    /// LDSFLD3 [2 datoshi]
    /// CALL 5F [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WNgmKAsRwAwcQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBPRFNNAhgWDQhCEA=
    /// LDSFLD0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 28 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 4141414141414141414141414141414141414141414141414141413D [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// CALL 08 [512 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// CALL 21 [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract bool? Test();

    #endregion
}
