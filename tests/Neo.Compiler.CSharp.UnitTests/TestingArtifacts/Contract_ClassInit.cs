using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ClassInit(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ClassInit"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testInitInt"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testInitializationExpression"",""parameters"":[],""returntype"":""Any"",""offset"":20,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ0QEBK/SjQDQFcAAXgQENB4ERDQQFcFAAsLCxAQCRbASjRtShMMAXPQShQQEBK/SjTZ0EoVCwsLEBAJFsBKNFDQcGgVzmgSwEpxynIQcyIbaWvOdGwQzgmXOWwRzhCXOWwSzhCXOWucc2tqMOVoE84MAXOXOWgVzhPO2DloFM4QzhCXOWgUzhHOEJc5aEBXAAF4EAnQeBEQ0HgSENBAGg4wkQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUACwsLEBAJFsBKNG1KEwwBc9BKFBAQEr9KNNnQShULCwsQEAkWwEo0UNBwaBXOaBLASnHKchBzIhtpa850bBDOCZc5bBHOEJc5bBLOEJc5a5xza2ow5WgTzgwBc5c5aBXOE87YOWgUzhDOEJc5aBTOEc4QlzloQA==
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
    /// 0C : CALL 6D [512 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH3 [1 datoshi]
    /// 10 : PUSHDATA1 73 's' [8 datoshi]
    /// 13 : SETITEM [8192 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSH4 [1 datoshi]
    /// 16 : PUSH0 [1 datoshi]
    /// 17 : PUSH0 [1 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : PACKSTRUCT [2048 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : CALL D9 [512 datoshi]
    /// 1D : SETITEM [8192 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSH5 [1 datoshi]
    /// 20 : PUSHNULL [1 datoshi]
    /// 21 : PUSHNULL [1 datoshi]
    /// 22 : PUSHNULL [1 datoshi]
    /// 23 : PUSH0 [1 datoshi]
    /// 24 : PUSH0 [1 datoshi]
    /// 25 : PUSHF [1 datoshi]
    /// 26 : PUSH6 [1 datoshi]
    /// 27 : PACK [2048 datoshi]
    /// 28 : DUP [2 datoshi]
    /// 29 : CALL 50 [512 datoshi]
    /// 2B : SETITEM [8192 datoshi]
    /// 2C : STLOC0 [2 datoshi]
    /// 2D : LDLOC0 [2 datoshi]
    /// 2E : PUSH5 [1 datoshi]
    /// 2F : PICKITEM [64 datoshi]
    /// 30 : LDLOC0 [2 datoshi]
    /// 31 : PUSH2 [1 datoshi]
    /// 32 : PACK [2048 datoshi]
    /// 33 : DUP [2 datoshi]
    /// 34 : STLOC1 [2 datoshi]
    /// 35 : SIZE [4 datoshi]
    /// 36 : STLOC2 [2 datoshi]
    /// 37 : PUSH0 [1 datoshi]
    /// 38 : STLOC3 [2 datoshi]
    /// 39 : JMP 1B [2 datoshi]
    /// 3B : LDLOC1 [2 datoshi]
    /// 3C : LDLOC3 [2 datoshi]
    /// 3D : PICKITEM [64 datoshi]
    /// 3E : STLOC4 [2 datoshi]
    /// 3F : LDLOC4 [2 datoshi]
    /// 40 : PUSH0 [1 datoshi]
    /// 41 : PICKITEM [64 datoshi]
    /// 42 : PUSHF [1 datoshi]
    /// 43 : EQUAL [32 datoshi]
    /// 44 : ASSERT [1 datoshi]
    /// 45 : LDLOC4 [2 datoshi]
    /// 46 : PUSH1 [1 datoshi]
    /// 47 : PICKITEM [64 datoshi]
    /// 48 : PUSH0 [1 datoshi]
    /// 49 : EQUAL [32 datoshi]
    /// 4A : ASSERT [1 datoshi]
    /// 4B : LDLOC4 [2 datoshi]
    /// 4C : PUSH2 [1 datoshi]
    /// 4D : PICKITEM [64 datoshi]
    /// 4E : PUSH0 [1 datoshi]
    /// 4F : EQUAL [32 datoshi]
    /// 50 : ASSERT [1 datoshi]
    /// 51 : LDLOC3 [2 datoshi]
    /// 52 : INC [4 datoshi]
    /// 53 : STLOC3 [2 datoshi]
    /// 54 : LDLOC3 [2 datoshi]
    /// 55 : LDLOC2 [2 datoshi]
    /// 56 : JMPLT E5 [2 datoshi]
    /// 58 : LDLOC0 [2 datoshi]
    /// 59 : PUSH3 [1 datoshi]
    /// 5A : PICKITEM [64 datoshi]
    /// 5B : PUSHDATA1 73 's' [8 datoshi]
    /// 5E : EQUAL [32 datoshi]
    /// 5F : ASSERT [1 datoshi]
    /// 60 : LDLOC0 [2 datoshi]
    /// 61 : PUSH5 [1 datoshi]
    /// 62 : PICKITEM [64 datoshi]
    /// 63 : PUSH3 [1 datoshi]
    /// 64 : PICKITEM [64 datoshi]
    /// 65 : ISNULL [2 datoshi]
    /// 66 : ASSERT [1 datoshi]
    /// 67 : LDLOC0 [2 datoshi]
    /// 68 : PUSH4 [1 datoshi]
    /// 69 : PICKITEM [64 datoshi]
    /// 6A : PUSH0 [1 datoshi]
    /// 6B : PICKITEM [64 datoshi]
    /// 6C : PUSH0 [1 datoshi]
    /// 6D : EQUAL [32 datoshi]
    /// 6E : ASSERT [1 datoshi]
    /// 6F : LDLOC0 [2 datoshi]
    /// 70 : PUSH4 [1 datoshi]
    /// 71 : PICKITEM [64 datoshi]
    /// 72 : PUSH1 [1 datoshi]
    /// 73 : PICKITEM [64 datoshi]
    /// 74 : PUSH0 [1 datoshi]
    /// 75 : EQUAL [32 datoshi]
    /// 76 : ASSERT [1 datoshi]
    /// 77 : LDLOC0 [2 datoshi]
    /// 78 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInitializationExpression")]
    public abstract object? TestInitializationExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBASv0o0A0A=
    /// 00 : PUSH0 [1 datoshi]
    /// 01 : PUSH0 [1 datoshi]
    /// 02 : PUSH2 [1 datoshi]
    /// 03 : PACKSTRUCT [2048 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : CALL 03 [512 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInitInt")]
    public abstract IList<object>? TestInitInt();

    #endregion
}
