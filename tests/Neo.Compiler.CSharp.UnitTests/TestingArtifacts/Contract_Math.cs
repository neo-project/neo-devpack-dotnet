using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Math(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Math"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""max"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""min"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":false},{""name"":""sign"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""abs"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":20,""safe"":false},{""name"":""bigMul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":26,""safe"":false},{""name"":""divRemByte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":64,""safe"":false},{""name"":""divRemShort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":78,""safe"":false},{""name"":""divRemInt"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":92,""safe"":false},{""name"":""divRemLong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":106,""safe"":false},{""name"":""divRemSbyte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":120,""safe"":false},{""name"":""divRemUshort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":134,""safe"":false},{""name"":""divRemUint"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":148,""safe"":false},{""name"":""divRemUlong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":162,""safe"":false},{""name"":""clampByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":176,""safe"":false},{""name"":""clampSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":191,""safe"":false},{""name"":""clampShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":206,""safe"":false},{""name"":""clampUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":221,""safe"":false},{""name"":""clampInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":236,""safe"":false},{""name"":""clampUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":251,""safe"":false},{""name"":""clampLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":266,""safe"":false},{""name"":""clampULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":281,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0oAVcAAnl4ukBXAAJ5eLlAVwABeJlAVwABeJpAVwACeXigSgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQDOkBXAAJ5eEoSTaFTohLAQFcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAJ5eEoSTaFTohLAQFcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAJ5eEoSTaFTohLAQFcAAnl4ShJNoVOiEsBAVwADeHl6S0syAzpTurlAVwADeHl6S0syAzpTurlAVwADeHl6S0syAzpTurlAVwADeHl6S0syAzpTurlAVwADeHl6S0syAzpTurlAVwADeHl6S0syAzpTurlAVwADeHl6S0syAzpTurlAVwADeHl6S0syAzpTurlA6UPMmg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ABS [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("abs")]
    public abstract BigInteger? Abs(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXigSgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQDOkA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.MUL [8 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.PUSHINT64 0000000000000080 [1 datoshi]
    /// 10 : OpCode.PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 21 : OpCode.WITHIN [8 datoshi]
    /// 22 : OpCode.JMPIF 03 [2 datoshi]
    /// 24 : OpCode.THROW [512 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigMul")]
    public abstract BigInteger? BigMul(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampByte")]
    public abstract BigInteger? ClampByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampInt")]
    public abstract BigInteger? ClampInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampLong")]
    public abstract BigInteger? ClampLong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampSByte")]
    public abstract BigInteger? ClampSByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampShort")]
    public abstract BigInteger? ClampShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUInt")]
    public abstract BigInteger? ClampUInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampULong")]
    public abstract BigInteger? ClampULong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG2 [2 datoshi]
    /// 06 : OpCode.OVER [2 datoshi]
    /// 07 : OpCode.OVER [2 datoshi]
    /// 08 : OpCode.JMPLE 03 [2 datoshi]
    /// 0A : OpCode.THROW [512 datoshi]
    /// 0B : OpCode.REVERSE3 [2 datoshi]
    /// 0C : OpCode.MAX [8 datoshi]
    /// 0D : OpCode.MIN [8 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUShort")]
    public abstract BigInteger? ClampUShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemByte")]
    public abstract IList<object>? DivRemByte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemInt")]
    public abstract IList<object>? DivRemInt(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemLong")]
    public abstract IList<object>? DivRemLong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemSbyte")]
    public abstract IList<object>? DivRemSbyte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemShort")]
    public abstract IList<object>? DivRemShort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUint")]
    public abstract IList<object>? DivRemUint(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUlong")]
    public abstract IList<object>? DivRemUlong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PICK [2 datoshi]
    /// 08 : OpCode.DIV [8 datoshi]
    /// 09 : OpCode.REVERSE3 [2 datoshi]
    /// 0A : OpCode.MOD [8 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUshort")]
    public abstract IList<object>? DivRemUshort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXi6QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.MAX [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("max")]
    public abstract BigInteger? Max(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXi5QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.MIN [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("min")]
    public abstract BigInteger? Min(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJlA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SIGN [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sign")]
    public abstract BigInteger? Sign(BigInteger? a);

    #endregion
}
