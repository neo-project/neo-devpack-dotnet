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
    /// Script: VwABeFjBRVOLUEGSXegxStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDSFLD0
    /// 05 : OpCode.UNPACK
    /// 06 : OpCode.DROP
    /// 07 : OpCode.REVERSE3
    /// 08 : OpCode.CAT
    /// 09 : OpCode.SWAP
    /// 0A : OpCode.SYSCALL 925DE831
    /// 0F : OpCode.DUP
    /// 10 : OpCode.ISNULL
    /// 11 : OpCode.JMPIFNOT 04
    /// 13 : OpCode.DROP
    /// 14 : OpCode.PUSH0
    /// 15 : OpCode.CONVERT 21
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("get")]
    public abstract BigInteger? Get(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDGRhdGFBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJgRFENshQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSHDATA1 64617461
    /// 09 : OpCode.SYSCALL 9BF667CE
    /// 0E : OpCode.PUSH2
    /// 0F : OpCode.PACK
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.UNPACK
    /// 14 : OpCode.DROP
    /// 15 : OpCode.REVERSE3
    /// 16 : OpCode.CAT
    /// 17 : OpCode.SWAP
    /// 18 : OpCode.SYSCALL 925DE831
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIFNOT 04
    /// 21 : OpCode.DROP
    /// 22 : OpCode.PUSH0
    /// 23 : OpCode.CONVERT 21
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("get2")]
    public abstract BigInteger? Get2(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFnBRVOLUEGSXegxStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDSFLD1
    /// 05 : OpCode.UNPACK
    /// 06 : OpCode.DROP
    /// 07 : OpCode.REVERSE3
    /// 08 : OpCode.CAT
    /// 09 : OpCode.SWAP
    /// 0A : OpCode.SYSCALL 925DE831
    /// 0F : OpCode.DUP
    /// 10 : OpCode.ISNULL
    /// 11 : OpCode.JMPIFNOT 04
    /// 13 : OpCode.DROP
    /// 14 : OpCode.PUSH0
    /// 15 : OpCode.CONVERT 21
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("getReadonly")]
    public abstract BigInteger? GetReadonly(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEXhYwUVTi1BB5j8YhEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.LDSFLD0
    /// 06 : OpCode.UNPACK
    /// 07 : OpCode.DROP
    /// 08 : OpCode.REVERSE3
    /// 09 : OpCode.CAT
    /// 0A : OpCode.SWAP
    /// 0B : OpCode.SYSCALL E63F1884
    /// 10 : OpCode.RET
    /// </remarks>
    [DisplayName("put")]
    public abstract void Put(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDGRhdGFBm/ZnzhLAcBN4aMFFU4tQQeY/GIRA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.PUSHDATA1 64617461
    /// 09 : OpCode.SYSCALL 9BF667CE
    /// 0E : OpCode.PUSH2
    /// 0F : OpCode.PACK
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.PUSH3
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.UNPACK
    /// 15 : OpCode.DROP
    /// 16 : OpCode.REVERSE3
    /// 17 : OpCode.CAT
    /// 18 : OpCode.SWAP
    /// 19 : OpCode.SYSCALL E63F1884
    /// 1E : OpCode.RET
    /// </remarks>
    [DisplayName("put2")]
    public abstract void Put2(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEnhZwUVTi1BB5j8YhEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSH2
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.LDSFLD1
    /// 06 : OpCode.UNPACK
    /// 07 : OpCode.DROP
    /// 08 : OpCode.REVERSE3
    /// 09 : OpCode.CAT
    /// 0A : OpCode.SWAP
    /// 0B : OpCode.SYSCALL E63F1884
    /// 10 : OpCode.RET
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
