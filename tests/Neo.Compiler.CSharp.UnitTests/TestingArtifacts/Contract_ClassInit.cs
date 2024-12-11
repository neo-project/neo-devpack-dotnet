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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHUQEBK/QFcFAAsLCxAQCRbAShMMAXPQShQQEBK/0EoVCwsLEBAJFsDQcGgVzmgSwEpxynIQcyIbaWvOdGwQzgmXOWwRzhCXOWwSzhCXOWucc2tqMOVoE84MAXOXOWgVzhPO2DloFM4QzhCXOWgUzhHOEJc5aED14mC7"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUACwsLEBAJFsBKEwwBc9BKFBAQEr/QShULCwsQEAkWwNBwaBXOaBLASnHKchBzIhtpa850bBDOCZc5bBHOEJc5bBLOEJc5a5xza2ow5WgTzgwBc5c5aBXOE87YOWgUzhDOEJc5aBTOEc4QlzloQA==
    /// 00 : INITSLOT 0500 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : PUSH6 [1 datoshi]
    /// 0A : PACK [2048 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : PUSH3 [1 datoshi]
    /// 0D : PUSHDATA1 73 's' [8 datoshi]
    /// 10 : SETITEM [8192 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : PUSH4 [1 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : PUSH0 [1 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : PACKSTRUCT [2048 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSH5 [1 datoshi]
    /// 1A : PUSHNULL [1 datoshi]
    /// 1B : PUSHNULL [1 datoshi]
    /// 1C : PUSHNULL [1 datoshi]
    /// 1D : PUSH0 [1 datoshi]
    /// 1E : PUSH0 [1 datoshi]
    /// 1F : PUSHF [1 datoshi]
    /// 20 : PUSH6 [1 datoshi]
    /// 21 : PACK [2048 datoshi]
    /// 22 : SETITEM [8192 datoshi]
    /// 23 : STLOC0 [2 datoshi]
    /// 24 : LDLOC0 [2 datoshi]
    /// 25 : PUSH5 [1 datoshi]
    /// 26 : PICKITEM [64 datoshi]
    /// 27 : LDLOC0 [2 datoshi]
    /// 28 : PUSH2 [1 datoshi]
    /// 29 : PACK [2048 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : STLOC1 [2 datoshi]
    /// 2C : SIZE [4 datoshi]
    /// 2D : STLOC2 [2 datoshi]
    /// 2E : PUSH0 [1 datoshi]
    /// 2F : STLOC3 [2 datoshi]
    /// 30 : JMP 1B [2 datoshi]
    /// 32 : LDLOC1 [2 datoshi]
    /// 33 : LDLOC3 [2 datoshi]
    /// 34 : PICKITEM [64 datoshi]
    /// 35 : STLOC4 [2 datoshi]
    /// 36 : LDLOC4 [2 datoshi]
    /// 37 : PUSH0 [1 datoshi]
    /// 38 : PICKITEM [64 datoshi]
    /// 39 : PUSHF [1 datoshi]
    /// 3A : EQUAL [32 datoshi]
    /// 3B : ASSERT [1 datoshi]
    /// 3C : LDLOC4 [2 datoshi]
    /// 3D : PUSH1 [1 datoshi]
    /// 3E : PICKITEM [64 datoshi]
    /// 3F : PUSH0 [1 datoshi]
    /// 40 : EQUAL [32 datoshi]
    /// 41 : ASSERT [1 datoshi]
    /// 42 : LDLOC4 [2 datoshi]
    /// 43 : PUSH2 [1 datoshi]
    /// 44 : PICKITEM [64 datoshi]
    /// 45 : PUSH0 [1 datoshi]
    /// 46 : EQUAL [32 datoshi]
    /// 47 : ASSERT [1 datoshi]
    /// 48 : LDLOC3 [2 datoshi]
    /// 49 : INC [4 datoshi]
    /// 4A : STLOC3 [2 datoshi]
    /// 4B : LDLOC3 [2 datoshi]
    /// 4C : LDLOC2 [2 datoshi]
    /// 4D : JMPLT E5 [2 datoshi]
    /// 4F : LDLOC0 [2 datoshi]
    /// 50 : PUSH3 [1 datoshi]
    /// 51 : PICKITEM [64 datoshi]
    /// 52 : PUSHDATA1 73 's' [8 datoshi]
    /// 55 : EQUAL [32 datoshi]
    /// 56 : ASSERT [1 datoshi]
    /// 57 : LDLOC0 [2 datoshi]
    /// 58 : PUSH5 [1 datoshi]
    /// 59 : PICKITEM [64 datoshi]
    /// 5A : PUSH3 [1 datoshi]
    /// 5B : PICKITEM [64 datoshi]
    /// 5C : ISNULL [2 datoshi]
    /// 5D : ASSERT [1 datoshi]
    /// 5E : LDLOC0 [2 datoshi]
    /// 5F : PUSH4 [1 datoshi]
    /// 60 : PICKITEM [64 datoshi]
    /// 61 : PUSH0 [1 datoshi]
    /// 62 : PICKITEM [64 datoshi]
    /// 63 : PUSH0 [1 datoshi]
    /// 64 : EQUAL [32 datoshi]
    /// 65 : ASSERT [1 datoshi]
    /// 66 : LDLOC0 [2 datoshi]
    /// 67 : PUSH4 [1 datoshi]
    /// 68 : PICKITEM [64 datoshi]
    /// 69 : PUSH1 [1 datoshi]
    /// 6A : PICKITEM [64 datoshi]
    /// 6B : PUSH0 [1 datoshi]
    /// 6C : EQUAL [32 datoshi]
    /// 6D : ASSERT [1 datoshi]
    /// 6E : LDLOC0 [2 datoshi]
    /// 6F : RET [0 datoshi]
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
