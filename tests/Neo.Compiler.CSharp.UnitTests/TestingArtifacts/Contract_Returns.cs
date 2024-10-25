using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Returns(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Returns"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""subtract"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":false},{""name"":""div"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Array"",""offset"":14,""safe"":false},{""name"":""mix"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":26,""safe"":false},{""name"":""byteStringAdd"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":44,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADVXAAJ4eZ5AVwACeHmfQFcAAnh5onh5oRK/QFcCAnl4NO9KwUVwcUVpaDTeQFcAAnh5i9soQGLQVyI="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.CAT
    /// 06 : OpCode.CONVERT 28
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringAdd")]
    public abstract byte[]? ByteStringAdd(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmieHmhEr9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.MOD
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.DIV
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PACKSTRUCT
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("div")]
    public abstract IList<object>? Div(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXg070rBRXBxRWloNN5A
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.CALL EF
    /// 07 : OpCode.DUP
    /// 08 : OpCode.UNPACK
    /// 09 : OpCode.DROP
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.DROP
    /// 0D : OpCode.LDLOC1
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.CALL DE
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("mix")]
    public abstract BigInteger? Mix(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.SUB
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("subtract")]
    public abstract BigInteger? Subtract(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    #endregion
}
