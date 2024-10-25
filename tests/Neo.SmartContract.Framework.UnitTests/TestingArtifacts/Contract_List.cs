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
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 0A
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.DROP
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.LT
    /// 14 : OpCode.JMPIF F5
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.CALLT 0000
    /// 1A : OpCode.RET
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract string? TestAdd(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSIKaGnPaUqccUVpeLUk9WhA
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 0A
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.DROP
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.LT
    /// 14 : OpCode.JMPIF F5
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("testArrayConvert")]
    public abstract IList<object>? TestArrayConvert(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSIKaGnPaUqccUVpeLUk9WjTaDcAAEA=
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 0A
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.DROP
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.LT
    /// 14 : OpCode.JMPIF F5
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.CLEARITEMS
    /// 18 : OpCode.LDLOC0
    /// 19 : OpCode.CALLT 0000
    /// 1C : OpCode.RET
    /// </remarks>
    [DisplayName("testClear")]
    public abstract string? TestClear(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBwnAQcSIKaGnPaUqccUVpeLUk9WjKQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.STLOC1
    /// 07 : OpCode.JMP 0A
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.LDLOC1
    /// 0B : OpCode.APPEND
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.INC
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.DROP
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.LT
    /// 14 : OpCode.JMPIF F5
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.SIZE
    /// 18 : OpCode.RET
    /// </remarks>
    [DisplayName("testCount")]
    public abstract BigInteger? TestCount(BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXi4JhwMSW52YWxpZCB0ZXN0IHBhcmFtZXRlcnM6wnAQcSIKaGnPaUqccUVpeLUk9Wh50mg3AABA
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.GE
    /// 06 : OpCode.JMPIFNOT 1C
    /// 08 : OpCode.PUSHDATA1 496E76616C6964207465737420706172616D6574657273
    /// 21 : OpCode.THROW
    /// 22 : OpCode.NEWARRAY0
    /// 23 : OpCode.STLOC0
    /// 24 : OpCode.PUSH0
    /// 25 : OpCode.STLOC1
    /// 26 : OpCode.JMP 0A
    /// 28 : OpCode.LDLOC0
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.APPEND
    /// 2B : OpCode.LDLOC1
    /// 2C : OpCode.DUP
    /// 2D : OpCode.INC
    /// 2E : OpCode.STLOC1
    /// 2F : OpCode.DROP
    /// 30 : OpCode.LDLOC1
    /// 31 : OpCode.LDARG0
    /// 32 : OpCode.LT
    /// 33 : OpCode.JMPIF F5
    /// 35 : OpCode.LDLOC0
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.REMOVE
    /// 38 : OpCode.LDLOC0
    /// 39 : OpCode.CALLT 0000
    /// 3C : OpCode.RET
    /// </remarks>
    [DisplayName("testRemoveAt")]
    public abstract string? TestRemoveAt(BigInteger? count, BigInteger? removeAt);

    #endregion
}
