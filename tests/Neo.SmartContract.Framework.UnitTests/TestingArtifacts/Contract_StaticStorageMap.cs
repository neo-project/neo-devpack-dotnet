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
    [DisplayName("get")]
    public abstract BigInteger? Get(string? msg);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDSFLD0
    // 0005 : UNPACK
    // 0006 : DROP
    // 0007 : REVERSE3
    // 0008 : CAT
    // 0009 : SWAP
    // 000A : SYSCALL
    // 000F : DUP
    // 0010 : ISNULL
    // 0011 : JMPIFNOT
    // 0013 : DROP
    // 0014 : PUSH0
    // 0015 : CONVERT
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("get2")]
    public abstract BigInteger? Get2(string? msg);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0009 : SYSCALL
    // 000E : PUSH2
    // 000F : PACK
    // 0010 : STLOC0
    // 0011 : LDARG0
    // 0012 : LDLOC0
    // 0013 : UNPACK
    // 0014 : DROP
    // 0015 : REVERSE3
    // 0016 : CAT
    // 0017 : SWAP
    // 0018 : SYSCALL
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIFNOT
    // 0021 : DROP
    // 0022 : PUSH0
    // 0023 : CONVERT
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getReadonly")]
    public abstract BigInteger? GetReadonly(string? msg);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDSFLD1
    // 0005 : UNPACK
    // 0006 : DROP
    // 0007 : REVERSE3
    // 0008 : CAT
    // 0009 : SWAP
    // 000A : SYSCALL
    // 000F : DUP
    // 0010 : ISNULL
    // 0011 : JMPIFNOT
    // 0013 : DROP
    // 0014 : PUSH0
    // 0015 : CONVERT
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("put")]
    public abstract void Put(string? message);
    // 0000 : INITSLOT
    // 0003 : PUSH1
    // 0004 : LDARG0
    // 0005 : LDSFLD0
    // 0006 : UNPACK
    // 0007 : DROP
    // 0008 : REVERSE3
    // 0009 : CAT
    // 000A : SWAP
    // 000B : SYSCALL
    // 0010 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("put2")]
    public abstract void Put2(string? message);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0009 : SYSCALL
    // 000E : PUSH2
    // 000F : PACK
    // 0010 : STLOC0
    // 0011 : PUSH3
    // 0012 : LDARG0
    // 0013 : LDLOC0
    // 0014 : UNPACK
    // 0015 : DROP
    // 0016 : REVERSE3
    // 0017 : CAT
    // 0018 : SWAP
    // 0019 : SYSCALL
    // 001E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("putReadonly")]
    public abstract void PutReadonly(string? message);
    // 0000 : INITSLOT
    // 0003 : PUSH2
    // 0004 : LDARG0
    // 0005 : LDSFLD1
    // 0006 : UNPACK
    // 0007 : DROP
    // 0008 : REVERSE3
    // 0009 : CAT
    // 000A : SWAP
    // 000B : SYSCALL
    // 0010 : RET

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
