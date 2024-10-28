using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_List(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_List"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCount"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testAdd"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""String"",""offset"":25,""safe"":false},{""name"":""testRemoveAt"",""parameters"":[{""name"":""count"",""type"":""Integer""},{""name"":""removeAt"",""type"":""Integer""}],""returntype"":""String"",""offset"":52,""safe"":false},{""name"":""testClear"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""String"",""offset"":113,""safe"":false},{""name"":""testArrayConvert"",""parameters"":[{""name"":""count"",""type"":""Integer""}],""returntype"":""Array"",""offset"":142,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""jsonSerialize""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrA1qc29uU2VyaWFsaXplAQABDwAAplcCAcJwEHEiCmhpz2lKnHFFaXi1JPVoykBXAgHCcBBxIgpoac9pSpxxRWl4tST1aDcAAEBXAgJ5eLgmHAwXSW52YWxpZCB0ZXN0IHBhcmFtZXRlcnM6wnAQcSIKaGnPaUqccUVpeLUk9Wh50mg3AABAVwIBwnAQcSIKaGnPaUqccUVpeLUk9WjTaDcAAEBXAgHCcBBxIgpoac9pSpxxRWl4tST1aEBgd6JG"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSIKaGnPaUqccUVpeLUk9Wg3AABA
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 0A [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.DROP [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDARG0 [2 datoshi]
    /// 13 : OpCode.LT [8 datoshi]
    /// 14 : OpCode.JMPIF F5 [2 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.CALLT 0000 [32768 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract string? TestAdd(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSIKaGnPaUqccUVpeLUk9WhA
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 0A [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.DROP [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDARG0 [2 datoshi]
    /// 13 : OpCode.LT [8 datoshi]
    /// 14 : OpCode.JMPIF F5 [2 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArrayConvert")]
    public abstract IList<object>? TestArrayConvert(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSIKaGnPaUqccUVpeLUk9WjTaDcAAEA=
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 0A [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.DROP [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDARG0 [2 datoshi]
    /// 13 : OpCode.LT [8 datoshi]
    /// 14 : OpCode.JMPIF F5 [2 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.CLEARITEMS [16 datoshi]
    /// 18 : OpCode.LDLOC0 [2 datoshi]
    /// 19 : OpCode.CALLT 0000 [32768 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testClear")]
    public abstract string? TestClear(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSIKaGnPaUqccUVpeLUk9WjKQA==
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.JMP 0A [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC1 [2 datoshi]
    /// 0B : OpCode.APPEND [8192 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.INC [4 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.DROP [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDARG0 [2 datoshi]
    /// 13 : OpCode.LT [8 datoshi]
    /// 14 : OpCode.JMPIF F5 [2 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.SIZE [4 datoshi]
    /// 18 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXi4JhwMSW52YWxpZCB0ZXN0IHBhcmFtZXRlcnM6wnAQcSIKaGnPaUqccUVpeLUk9Wh50mg3AABA
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.GE [8 datoshi]
    /// 06 : OpCode.JMPIFNOT 1C [2 datoshi]
    /// 08 : OpCode.PUSHDATA1 496E76616C6964207465737420706172616D6574657273 [8 datoshi]
    /// 21 : OpCode.THROW [512 datoshi]
    /// 22 : OpCode.NEWARRAY0 [16 datoshi]
    /// 23 : OpCode.STLOC0 [2 datoshi]
    /// 24 : OpCode.PUSH0 [1 datoshi]
    /// 25 : OpCode.STLOC1 [2 datoshi]
    /// 26 : OpCode.JMP 0A [2 datoshi]
    /// 28 : OpCode.LDLOC0 [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.APPEND [8192 datoshi]
    /// 2B : OpCode.LDLOC1 [2 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.INC [4 datoshi]
    /// 2E : OpCode.STLOC1 [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.LDLOC1 [2 datoshi]
    /// 31 : OpCode.LDARG0 [2 datoshi]
    /// 32 : OpCode.LT [8 datoshi]
    /// 33 : OpCode.JMPIF F5 [2 datoshi]
    /// 35 : OpCode.LDLOC0 [2 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.REMOVE [16 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.CALLT 0000 [32768 datoshi]
    /// 3C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRemoveAt")]
    public abstract string? TestRemoveAt(BigInteger? count, BigInteger? removeAt);

    #endregion
}
