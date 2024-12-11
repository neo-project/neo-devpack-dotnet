using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":52,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":170,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":196,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAN5XAAF4ELgkHwwaTWludXMgbnVtYmVyIG5vdCBzdXBwb3J0ZWTgeBK4Jgp4eBGfNNKgQBFAVwUEeBC3JBgME0NvdW50IG9mIGRpc2tzIDw9IDDgeBGXJhnCcGgQEBATv0oQEdBKEXnQShJ70M9oQHp7eXgRnzTAcGgQEBATv0oQeNBKEXnQShJ70M97eXp4EZ80pEpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQFcAAXgQlyYECEB4ELUmB3gRniIFeBGfNANAVwABeBCXJgQJQHgQtSYHeBGeIgV4EZ80z0DHiVku"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCXJgQIQHgQtSYHeBGeIgV4EZ80A0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ADD [8 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("even")]
    public abstract bool? Even(BigInteger? n);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4JB8MGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4HgSuCYKeHgRnzTSoEARQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIF 1F [2 datoshi]
    /// PUSHDATA1 4D696E7573206E756D626572206E6F7420737570706F72746564 [8 datoshi]
    /// ABORTMSG [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL D2 [512 datoshi]
    /// MUL [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUEeBC3JBgME0NvdW50IG9mIGRpc2tzIDw9IDDgeBGXJhnCcGgQEBATv0oQEdBKEXnQShJ70M9oQHp7eXgRnzTAcGgQEBATv0oQeNBKEXnQShJ70M97eXp4EZ80pEpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQA==
    /// INITSLOT 0504 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIF 18 [2 datoshi]
    /// PUSHDATA1 436F756E74206F66206469736B73203C3D2030 [8 datoshi]
    /// ABORTMSG [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 19 [2 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDARG3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG3 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL C0 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDARG3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// APPEND [8192 datoshi]
    /// LDARG3 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL A4 [512 datoshi]
    /// DUP [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// JMP 0C [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// APPEND [8192 datoshi]
    /// LDLOC3 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// JMPLT F4 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("hanoiTower")]
    public abstract IList<object>? HanoiTower(BigInteger? n, BigInteger? src, BigInteger? aux, BigInteger? dst);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCXJgQJQHgQtSYHeBGeIgV4EZ80z0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// ADD [8 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SUB [8 datoshi]
    /// CALL CF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("odd")]
    public abstract bool? Odd(BigInteger? n);

    #endregion
}
