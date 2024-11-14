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
    /// Script: VwUACwsLEBAJFsAQEBK/DAFzEBAJFsBwaBXOaBLASnHKchBzIhtpa850bBDOCZc5bBHOEJc5bBLOEJc5a5xza2ow5WgTzgwBc5c5aBXOE87YOWgUzhDOEJc5aBTOEc4QlzloQA==
    /// 00 : INITSLOT 0500 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : PUSH6 [1 datoshi]
    /// 0A : PACK [2048 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : PUSH2 [1 datoshi]
    /// 0E : PACKSTRUCT [2048 datoshi]
    /// 0F : PUSHDATA1 73 's' [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : PUSHF [1 datoshi]
    /// 15 : PUSH6 [1 datoshi]
    /// 16 : PACK [2048 datoshi]
    /// 17 : STLOC0 [2 datoshi]
    /// 18 : LDLOC0 [2 datoshi]
    /// 19 : PUSH5 [1 datoshi]
    /// 1A : PICKITEM [64 datoshi]
    /// 1B : LDLOC0 [2 datoshi]
    /// 1C : PUSH2 [1 datoshi]
    /// 1D : PACK [2048 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : STLOC1 [2 datoshi]
    /// 20 : SIZE [4 datoshi]
    /// 21 : STLOC2 [2 datoshi]
    /// 22 : PUSH0 [1 datoshi]
    /// 23 : STLOC3 [2 datoshi]
    /// 24 : JMP 1B [2 datoshi]
    /// 26 : LDLOC1 [2 datoshi]
    /// 27 : LDLOC3 [2 datoshi]
    /// 28 : PICKITEM [64 datoshi]
    /// 29 : STLOC4 [2 datoshi]
    /// 2A : LDLOC4 [2 datoshi]
    /// 2B : PUSH0 [1 datoshi]
    /// 2C : PICKITEM [64 datoshi]
    /// 2D : PUSHF [1 datoshi]
    /// 2E : EQUAL [32 datoshi]
    /// 2F : ASSERT [1 datoshi]
    /// 30 : LDLOC4 [2 datoshi]
    /// 31 : PUSH1 [1 datoshi]
    /// 32 : PICKITEM [64 datoshi]
    /// 33 : PUSH0 [1 datoshi]
    /// 34 : EQUAL [32 datoshi]
    /// 35 : ASSERT [1 datoshi]
    /// 36 : LDLOC4 [2 datoshi]
    /// 37 : PUSH2 [1 datoshi]
    /// 38 : PICKITEM [64 datoshi]
    /// 39 : PUSH0 [1 datoshi]
    /// 3A : EQUAL [32 datoshi]
    /// 3B : ASSERT [1 datoshi]
    /// 3C : LDLOC3 [2 datoshi]
    /// 3D : INC [4 datoshi]
    /// 3E : STLOC3 [2 datoshi]
    /// 3F : LDLOC3 [2 datoshi]
    /// 40 : LDLOC2 [2 datoshi]
    /// 41 : JMPLT E5 [2 datoshi]
    /// 43 : LDLOC0 [2 datoshi]
    /// 44 : PUSH3 [1 datoshi]
    /// 45 : PICKITEM [64 datoshi]
    /// 46 : PUSHDATA1 73 's' [8 datoshi]
    /// 49 : EQUAL [32 datoshi]
    /// 4A : ASSERT [1 datoshi]
    /// 4B : LDLOC0 [2 datoshi]
    /// 4C : PUSH5 [1 datoshi]
    /// 4D : PICKITEM [64 datoshi]
    /// 4E : PUSH3 [1 datoshi]
    /// 4F : PICKITEM [64 datoshi]
    /// 50 : ISNULL [2 datoshi]
    /// 51 : ASSERT [1 datoshi]
    /// 52 : LDLOC0 [2 datoshi]
    /// 53 : PUSH4 [1 datoshi]
    /// 54 : PICKITEM [64 datoshi]
    /// 55 : PUSH0 [1 datoshi]
    /// 56 : PICKITEM [64 datoshi]
    /// 57 : PUSH0 [1 datoshi]
    /// 58 : EQUAL [32 datoshi]
    /// 59 : ASSERT [1 datoshi]
    /// 5A : LDLOC0 [2 datoshi]
    /// 5B : PUSH4 [1 datoshi]
    /// 5C : PICKITEM [64 datoshi]
    /// 5D : PUSH1 [1 datoshi]
    /// 5E : PICKITEM [64 datoshi]
    /// 5F : PUSH0 [1 datoshi]
    /// 60 : EQUAL [32 datoshi]
    /// 61 : ASSERT [1 datoshi]
    /// 62 : LDLOC0 [2 datoshi]
    /// 63 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInitializationExpression")]
    public abstract object? TestInitializationExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBASv0A=
    /// 00 : PUSH0 [1 datoshi]
    /// 01 : PUSH0 [1 datoshi]
    /// 02 : PUSH2 [1 datoshi]
    /// 03 : PACKSTRUCT [2048 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInitInt")]
    public abstract IList<object>? TestInitInt();

    #endregion
}
