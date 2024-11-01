using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ClassInit(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ClassInit"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInitInt"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testInitializationExpression"",""parameters"":[],""returntype"":""Any"",""offset"":5,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGkQEBK/QFcFAAsLCxAQCRbAEBASvwwBcxAQCRbAcGgVzmgSwEpxynIQcyIbaWvOdGwQzgmXOWwRzhCXOWwSzhCXOWucc2tqMOVoE84MAXOXOWgVzhPO2DloFM4QzhCXOWgUzhHOEJc5aECYKH3y"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUACwsLEBAJFsAQEBK/DHMQEAkWwHBoFc5oEsBKccpyEHMiG2lrznRsEM4JlzlsEc4QlzlsEs4QlzlrnHNrajDlaBPODHOXOWgVzhPO2DloFM4QzhCXOWgUzhHOEJc5aEA=
    /// 00 : OpCode.INITSLOT 0500 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH0 [1 datoshi]
    /// 07 : OpCode.PUSH0 [1 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.PUSH6 [1 datoshi]
    /// 0A : OpCode.PACK [2048 datoshi]
    /// 0B : OpCode.PUSH0 [1 datoshi]
    /// 0C : OpCode.PUSH0 [1 datoshi]
    /// 0D : OpCode.PUSH2 [1 datoshi]
    /// 0E : OpCode.PACKSTRUCT [2048 datoshi]
    /// 0F : OpCode.PUSHDATA1 73 [8 datoshi]
    /// 12 : OpCode.PUSH0 [1 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHF [1 datoshi]
    /// 15 : OpCode.PUSH6 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.STLOC0 [2 datoshi]
    /// 18 : OpCode.LDLOC0 [2 datoshi]
    /// 19 : OpCode.PUSH5 [1 datoshi]
    /// 1A : OpCode.PICKITEM [64 datoshi]
    /// 1B : OpCode.LDLOC0 [2 datoshi]
    /// 1C : OpCode.PUSH2 [1 datoshi]
    /// 1D : OpCode.PACK [2048 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.STLOC1 [2 datoshi]
    /// 20 : OpCode.SIZE [4 datoshi]
    /// 21 : OpCode.STLOC2 [2 datoshi]
    /// 22 : OpCode.PUSH0 [1 datoshi]
    /// 23 : OpCode.STLOC3 [2 datoshi]
    /// 24 : OpCode.JMP 1B [2 datoshi]
    /// 26 : OpCode.LDLOC1 [2 datoshi]
    /// 27 : OpCode.LDLOC3 [2 datoshi]
    /// 28 : OpCode.PICKITEM [64 datoshi]
    /// 29 : OpCode.STLOC4 [2 datoshi]
    /// 2A : OpCode.LDLOC4 [2 datoshi]
    /// 2B : OpCode.PUSH0 [1 datoshi]
    /// 2C : OpCode.PICKITEM [64 datoshi]
    /// 2D : OpCode.PUSHF [1 datoshi]
    /// 2E : OpCode.EQUAL [32 datoshi]
    /// 2F : OpCode.ASSERT [1 datoshi]
    /// 30 : OpCode.LDLOC4 [2 datoshi]
    /// 31 : OpCode.PUSH1 [1 datoshi]
    /// 32 : OpCode.PICKITEM [64 datoshi]
    /// 33 : OpCode.PUSH0 [1 datoshi]
    /// 34 : OpCode.EQUAL [32 datoshi]
    /// 35 : OpCode.ASSERT [1 datoshi]
    /// 36 : OpCode.LDLOC4 [2 datoshi]
    /// 37 : OpCode.PUSH2 [1 datoshi]
    /// 38 : OpCode.PICKITEM [64 datoshi]
    /// 39 : OpCode.PUSH0 [1 datoshi]
    /// 3A : OpCode.EQUAL [32 datoshi]
    /// 3B : OpCode.ASSERT [1 datoshi]
    /// 3C : OpCode.LDLOC3 [2 datoshi]
    /// 3D : OpCode.INC [4 datoshi]
    /// 3E : OpCode.STLOC3 [2 datoshi]
    /// 3F : OpCode.LDLOC3 [2 datoshi]
    /// 40 : OpCode.LDLOC2 [2 datoshi]
    /// 41 : OpCode.JMPLT E5 [2 datoshi]
    /// 43 : OpCode.LDLOC0 [2 datoshi]
    /// 44 : OpCode.PUSH3 [1 datoshi]
    /// 45 : OpCode.PICKITEM [64 datoshi]
    /// 46 : OpCode.PUSHDATA1 73 [8 datoshi]
    /// 49 : OpCode.EQUAL [32 datoshi]
    /// 4A : OpCode.ASSERT [1 datoshi]
    /// 4B : OpCode.LDLOC0 [2 datoshi]
    /// 4C : OpCode.PUSH5 [1 datoshi]
    /// 4D : OpCode.PICKITEM [64 datoshi]
    /// 4E : OpCode.PUSH3 [1 datoshi]
    /// 4F : OpCode.PICKITEM [64 datoshi]
    /// 50 : OpCode.ISNULL [2 datoshi]
    /// 51 : OpCode.ASSERT [1 datoshi]
    /// 52 : OpCode.LDLOC0 [2 datoshi]
    /// 53 : OpCode.PUSH4 [1 datoshi]
    /// 54 : OpCode.PICKITEM [64 datoshi]
    /// 55 : OpCode.PUSH0 [1 datoshi]
    /// 56 : OpCode.PICKITEM [64 datoshi]
    /// 57 : OpCode.PUSH0 [1 datoshi]
    /// 58 : OpCode.EQUAL [32 datoshi]
    /// 59 : OpCode.ASSERT [1 datoshi]
    /// 5A : OpCode.LDLOC0 [2 datoshi]
    /// 5B : OpCode.PUSH4 [1 datoshi]
    /// 5C : OpCode.PICKITEM [64 datoshi]
    /// 5D : OpCode.PUSH1 [1 datoshi]
    /// 5E : OpCode.PICKITEM [64 datoshi]
    /// 5F : OpCode.PUSH0 [1 datoshi]
    /// 60 : OpCode.EQUAL [32 datoshi]
    /// 61 : OpCode.ASSERT [1 datoshi]
    /// 62 : OpCode.LDLOC0 [2 datoshi]
    /// 63 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInitializationExpression")]
    public abstract object? TestInitializationExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testInitInt")]
    public abstract IList<object>? TestInitInt();

    #endregion
}
