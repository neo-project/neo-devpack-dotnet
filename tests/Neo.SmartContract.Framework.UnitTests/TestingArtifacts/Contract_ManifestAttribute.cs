using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ManifestAttribute(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ManifestAttribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":167,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Author"":""core-dev"",""E-mail"":""dev@neo.org"",""Version"":""v3.6.3"",""Description"":""This is a test contract."",""ExtraKey"":""ExtraValue"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAKpXAAFY2CYbCwsSwAwLbm9SZWVudHJhbnQB/wASTTQaYFg0NngQlyYEIgt4AHuXJgUQNM9YNF9AVwADekp4EVHQRUGb9mfOeRGIThBR0FASwEp4EFHQRUBXAQF4Ec54EM7BRVOLUEGSXegxcGjYJBQMD0FscmVhZHkgZW50ZXJlZOAReBHOeBDOwUVTi1BB5j8YhEBXAAF4Ec54EM7BRVOLUEEvWMXtQFYBQL34PGQ="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABWNgmGwsLEsAMC25vUmVlbnRyYW50Af8AEk00GmBYNDZ4EJcmBCILeAB7lyYFEDTPWDRfQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDSFLD0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 1B [2 datoshi]
    /// 07 : PUSHNULL [1 datoshi]
    /// 08 : PUSHNULL [1 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PACK [2048 datoshi]
    /// 0B : PUSHDATA1 6E6F5265656E7472616E74 'noReentrant' [8 datoshi]
    /// 18 : PUSHINT16 FF00 [1 datoshi]
    /// 1B : PUSH2 [1 datoshi]
    /// 1C : PICK [2 datoshi]
    /// 1D : CALL 1A [512 datoshi]
    /// 1F : STSFLD0 [2 datoshi]
    /// 20 : LDSFLD0 [2 datoshi]
    /// 21 : CALL 36 [512 datoshi]
    /// 23 : LDARG0 [2 datoshi]
    /// 24 : PUSH0 [1 datoshi]
    /// 25 : EQUAL [32 datoshi]
    /// 26 : JMPIFNOT 04 [2 datoshi]
    /// 28 : JMP 0B [2 datoshi]
    /// 2A : LDARG0 [2 datoshi]
    /// 2B : PUSHINT8 7B [1 datoshi]
    /// 2D : EQUAL [32 datoshi]
    /// 2E : JMPIFNOT 05 [2 datoshi]
    /// 30 : PUSH0 [1 datoshi]
    /// 31 : CALL CF [512 datoshi]
    /// 33 : LDSFLD0 [2 datoshi]
    /// 34 : CALL 5F [512 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    #endregion
}
