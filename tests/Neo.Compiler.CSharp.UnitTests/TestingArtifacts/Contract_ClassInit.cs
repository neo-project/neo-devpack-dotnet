using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ0QEBK/SjQDQFcAAXgQENB4ERDQQFcFAAsLCxAQCRbASjRtShMMAXPQShQQEBK/SjTZ0EoVCwsLEBAJFsBKNFDQcGgVzmgSwEpxynIQcyIbaWvOdGwQzgmXOWwRzhCXOWwSzhCXOWucc2tqMOVoE84MAXOXOWgVzhPO2DloFM4QzhCXOWgUzhHOEJc5aEBXAAF4EAnQeBEQ0HgSENBAGg4wkQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUACwsLEBAJFsBKNG1KEwwBc9BKFBAQEr9KNNnQShULCwsQEAkWwEo0UNBwaBXOaBLASnHKchBzIhtpa850bBDOCZc5bBHOEJc5bBLOEJc5a5xza2ow5WgTzgwBc5c5aBXOE87YOWgUzhDOEJc5aBTOEc4QlzloQA==
    /// 00 : OpCode.INITSLOT 0500 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH0 [1 datoshi]
    /// 07 : OpCode.PUSH0 [1 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.PUSH6 [1 datoshi]
    /// 0A : OpCode.PACK [2048 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.CALL 6D [512 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH3 [1 datoshi]
    /// 10 : OpCode.PUSHDATA1 73 's' [8 datoshi]
    /// 13 : OpCode.SETITEM [8192 datoshi]
    /// 14 : OpCode.DUP [2 datoshi]
    /// 15 : OpCode.PUSH4 [1 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.CALL D9 [512 datoshi]
    /// 1D : OpCode.SETITEM [8192 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.PUSH5 [1 datoshi]
    /// 20 : OpCode.PUSHNULL [1 datoshi]
    /// 21 : OpCode.PUSHNULL [1 datoshi]
    /// 22 : OpCode.PUSHNULL [1 datoshi]
    /// 23 : OpCode.PUSH0 [1 datoshi]
    /// 24 : OpCode.PUSH0 [1 datoshi]
    /// 25 : OpCode.PUSHF [1 datoshi]
    /// 26 : OpCode.PUSH6 [1 datoshi]
    /// 27 : OpCode.PACK [2048 datoshi]
    /// 28 : OpCode.DUP [2 datoshi]
    /// 29 : OpCode.CALL 50 [512 datoshi]
    /// 2B : OpCode.SETITEM [8192 datoshi]
    /// 2C : OpCode.STLOC0 [2 datoshi]
    /// 2D : OpCode.LDLOC0 [2 datoshi]
    /// 2E : OpCode.PUSH5 [1 datoshi]
    /// 2F : OpCode.PICKITEM [64 datoshi]
    /// 30 : OpCode.LDLOC0 [2 datoshi]
    /// 31 : OpCode.PUSH2 [1 datoshi]
    /// 32 : OpCode.PACK [2048 datoshi]
    /// 33 : OpCode.DUP [2 datoshi]
    /// 34 : OpCode.STLOC1 [2 datoshi]
    /// 35 : OpCode.SIZE [4 datoshi]
    /// 36 : OpCode.STLOC2 [2 datoshi]
    /// 37 : OpCode.PUSH0 [1 datoshi]
    /// 38 : OpCode.STLOC3 [2 datoshi]
    /// 39 : OpCode.JMP 1B [2 datoshi]
    /// 3B : OpCode.LDLOC1 [2 datoshi]
    /// 3C : OpCode.LDLOC3 [2 datoshi]
    /// 3D : OpCode.PICKITEM [64 datoshi]
    /// 3E : OpCode.STLOC4 [2 datoshi]
    /// 3F : OpCode.LDLOC4 [2 datoshi]
    /// 40 : OpCode.PUSH0 [1 datoshi]
    /// 41 : OpCode.PICKITEM [64 datoshi]
    /// 42 : OpCode.PUSHF [1 datoshi]
    /// 43 : OpCode.EQUAL [32 datoshi]
    /// 44 : OpCode.ASSERT [1 datoshi]
    /// 45 : OpCode.LDLOC4 [2 datoshi]
    /// 46 : OpCode.PUSH1 [1 datoshi]
    /// 47 : OpCode.PICKITEM [64 datoshi]
    /// 48 : OpCode.PUSH0 [1 datoshi]
    /// 49 : OpCode.EQUAL [32 datoshi]
    /// 4A : OpCode.ASSERT [1 datoshi]
    /// 4B : OpCode.LDLOC4 [2 datoshi]
    /// 4C : OpCode.PUSH2 [1 datoshi]
    /// 4D : OpCode.PICKITEM [64 datoshi]
    /// 4E : OpCode.PUSH0 [1 datoshi]
    /// 4F : OpCode.EQUAL [32 datoshi]
    /// 50 : OpCode.ASSERT [1 datoshi]
    /// 51 : OpCode.LDLOC3 [2 datoshi]
    /// 52 : OpCode.INC [4 datoshi]
    /// 53 : OpCode.STLOC3 [2 datoshi]
    /// 54 : OpCode.LDLOC3 [2 datoshi]
    /// 55 : OpCode.LDLOC2 [2 datoshi]
    /// 56 : OpCode.JMPLT E5 [2 datoshi]
    /// 58 : OpCode.LDLOC0 [2 datoshi]
    /// 59 : OpCode.PUSH3 [1 datoshi]
    /// 5A : OpCode.PICKITEM [64 datoshi]
    /// 5B : OpCode.PUSHDATA1 73 's' [8 datoshi]
    /// 5E : OpCode.EQUAL [32 datoshi]
    /// 5F : OpCode.ASSERT [1 datoshi]
    /// 60 : OpCode.LDLOC0 [2 datoshi]
    /// 61 : OpCode.PUSH5 [1 datoshi]
    /// 62 : OpCode.PICKITEM [64 datoshi]
    /// 63 : OpCode.PUSH3 [1 datoshi]
    /// 64 : OpCode.PICKITEM [64 datoshi]
    /// 65 : OpCode.ISNULL [2 datoshi]
    /// 66 : OpCode.ASSERT [1 datoshi]
    /// 67 : OpCode.LDLOC0 [2 datoshi]
    /// 68 : OpCode.PUSH4 [1 datoshi]
    /// 69 : OpCode.PICKITEM [64 datoshi]
    /// 6A : OpCode.PUSH0 [1 datoshi]
    /// 6B : OpCode.PICKITEM [64 datoshi]
    /// 6C : OpCode.PUSH0 [1 datoshi]
    /// 6D : OpCode.EQUAL [32 datoshi]
    /// 6E : OpCode.ASSERT [1 datoshi]
    /// 6F : OpCode.LDLOC0 [2 datoshi]
    /// 70 : OpCode.PUSH4 [1 datoshi]
    /// 71 : OpCode.PICKITEM [64 datoshi]
    /// 72 : OpCode.PUSH1 [1 datoshi]
    /// 73 : OpCode.PICKITEM [64 datoshi]
    /// 74 : OpCode.PUSH0 [1 datoshi]
    /// 75 : OpCode.EQUAL [32 datoshi]
    /// 76 : OpCode.ASSERT [1 datoshi]
    /// 77 : OpCode.LDLOC0 [2 datoshi]
    /// 78 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInitializationExpression")]
    public abstract object? TestInitializationExpression();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBASv0o0A0A=
    /// 00 : OpCode.PUSH0 [1 datoshi]
    /// 01 : OpCode.PUSH0 [1 datoshi]
    /// 02 : OpCode.PUSH2 [1 datoshi]
    /// 03 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.CALL 03 [512 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testInitInt")]
    public abstract IList<object>? TestInitInt();

    #endregion
}
