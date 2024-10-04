using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticStorageMap(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticStorageMap"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""put"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""get"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":17,""safe"":false},{""name"":""putReadonly"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":41,""safe"":false},{""name"":""getReadonly"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":58,""safe"":false},{""name"":""put2"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":82,""safe"":false},{""name"":""get2"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":113,""safe"":false},{""name"":""teststoragemap_Putbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Void"",""offset"":151,""safe"":false},{""name"":""teststoragemap_Getbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":191,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":237,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0UAVcAARF4WMFFU4tQQeY/GIRAVwABeFjBRVOLUEGSXegxStgmBEUQ2yFAVwABEnhZwUVTi1BB5j8YhEBXAAF4WcFFU4tQQZJd6DFK2CYERRDbIUBXAQEMBGRhdGFBm/ZnzhLAcBN4aMFFU4tQQeY/GIRAVwEBDARkYXRhQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CYERRDbIUBXAQFBm/ZnzngRiE4QUdBQEsBwAHsMBXRlc3QxaMFFU4tQQeY/GIRAVwEBQZv2Z854EYhOEFHQUBLAcAwFdGVzdDFowUVTi1BBkl3oMUrYJgRFENshQFYCDARkYXRhQZv2Z84SwGAMDHJlYWRvbmx5ZGF0YUGb9mfOEsBhQJh7fb8="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDSFLD0
    /// 0005 : OpCode.UNPACK
    /// 0006 : OpCode.DROP
    /// 0007 : OpCode.REVERSE3
    /// 0008 : OpCode.CAT
    /// 0009 : OpCode.SWAP
    /// 000A : OpCode.SYSCALL 925DE831
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.ISNULL
    /// 0011 : OpCode.JMPIFNOT 04
    /// 0013 : OpCode.DROP
    /// 0014 : OpCode.PUSH0
    /// 0015 : OpCode.CONVERT 21
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("get")]
    public abstract BigInteger? Get(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSHDATA1 64617461
    /// 0009 : OpCode.SYSCALL 9BF667CE
    /// 000E : OpCode.PUSH2
    /// 000F : OpCode.PACK
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.LDARG0
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.UNPACK
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.REVERSE3
    /// 0016 : OpCode.CAT
    /// 0017 : OpCode.SWAP
    /// 0018 : OpCode.SYSCALL 925DE831
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIFNOT 04
    /// 0021 : OpCode.DROP
    /// 0022 : OpCode.PUSH0
    /// 0023 : OpCode.CONVERT 21
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("get2")]
    public abstract BigInteger? Get2(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDSFLD1
    /// 0005 : OpCode.UNPACK
    /// 0006 : OpCode.DROP
    /// 0007 : OpCode.REVERSE3
    /// 0008 : OpCode.CAT
    /// 0009 : OpCode.SWAP
    /// 000A : OpCode.SYSCALL 925DE831
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.ISNULL
    /// 0011 : OpCode.JMPIFNOT 04
    /// 0013 : OpCode.DROP
    /// 0014 : OpCode.PUSH0
    /// 0015 : OpCode.CONVERT 21
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("getReadonly")]
    public abstract BigInteger? GetReadonly(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSH1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.LDSFLD0
    /// 0006 : OpCode.UNPACK
    /// 0007 : OpCode.DROP
    /// 0008 : OpCode.REVERSE3
    /// 0009 : OpCode.CAT
    /// 000A : OpCode.SWAP
    /// 000B : OpCode.SYSCALL E63F1884
    /// 0010 : OpCode.RET
    /// </remarks>
    [DisplayName("put")]
    public abstract void Put(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.PUSHDATA1 64617461
    /// 0009 : OpCode.SYSCALL 9BF667CE
    /// 000E : OpCode.PUSH2
    /// 000F : OpCode.PACK
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.PUSH3
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.UNPACK
    /// 0015 : OpCode.DROP
    /// 0016 : OpCode.REVERSE3
    /// 0017 : OpCode.CAT
    /// 0018 : OpCode.SWAP
    /// 0019 : OpCode.SYSCALL E63F1884
    /// 001E : OpCode.RET
    /// </remarks>
    [DisplayName("put2")]
    public abstract void Put2(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSH2
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.LDSFLD1
    /// 0006 : OpCode.UNPACK
    /// 0007 : OpCode.DROP
    /// 0008 : OpCode.REVERSE3
    /// 0009 : OpCode.CAT
    /// 000A : OpCode.SWAP
    /// 000B : OpCode.SYSCALL E63F1884
    /// 0010 : OpCode.RET
    /// </remarks>
    [DisplayName("putReadonly")]
    public abstract void PutReadonly(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("teststoragemap_Getbyteprefix")]
    public abstract BigInteger? Teststoragemap_Getbyteprefix(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("teststoragemap_Putbyteprefix")]
    public abstract void Teststoragemap_Putbyteprefix(BigInteger? x);

    #endregion

}
