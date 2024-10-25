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
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDSFLD0 [2 datoshi]
    /// 05 : OpCode.UNPACK [2048 datoshi]
    /// 06 : OpCode.DROP [2 datoshi]
    /// 07 : OpCode.REVERSE3 [2 datoshi]
    /// 08 : OpCode.CAT [2048 datoshi]
    /// 09 : OpCode.SWAP [2 datoshi]
    /// 0A : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.ISNULL [2 datoshi]
    /// 11 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 13 : OpCode.DROP [2 datoshi]
    /// 14 : OpCode.PUSH0 [1 datoshi]
    /// 15 : OpCode.CONVERT (Integer) [8192 datoshi]
    /// 17 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("get")]
    public abstract BigInteger? Get(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDGRhdGFBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJgRFENshQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 64617461 [8 datoshi]
    /// 09 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : OpCode.PUSH2 [1 datoshi]
    /// 0F : OpCode.PACK [2048 datoshi]
    /// 10 : OpCode.STLOC0 [2 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.UNPACK [2048 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.REVERSE3 [2 datoshi]
    /// 16 : OpCode.CAT [2048 datoshi]
    /// 17 : OpCode.SWAP [2 datoshi]
    /// 18 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 21 : OpCode.DROP [2 datoshi]
    /// 22 : OpCode.PUSH0 [1 datoshi]
    /// 23 : OpCode.CONVERT (Integer) [8192 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("get2")]
    public abstract BigInteger? Get2(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFnBRVOLUEGSXegxStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDSFLD1 [2 datoshi]
    /// 05 : OpCode.UNPACK [2048 datoshi]
    /// 06 : OpCode.DROP [2 datoshi]
    /// 07 : OpCode.REVERSE3 [2 datoshi]
    /// 08 : OpCode.CAT [2048 datoshi]
    /// 09 : OpCode.SWAP [2 datoshi]
    /// 0A : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.ISNULL [2 datoshi]
    /// 11 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 13 : OpCode.DROP [2 datoshi]
    /// 14 : OpCode.PUSH0 [1 datoshi]
    /// 15 : OpCode.CONVERT (Integer) [8192 datoshi]
    /// 17 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getReadonly")]
    public abstract BigInteger? GetReadonly(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEXhYwUVTi1BB5j8YhEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.LDSFLD0 [2 datoshi]
    /// 06 : OpCode.UNPACK [2048 datoshi]
    /// 07 : OpCode.DROP [2 datoshi]
    /// 08 : OpCode.REVERSE3 [2 datoshi]
    /// 09 : OpCode.CAT [2048 datoshi]
    /// 0A : OpCode.SWAP [2 datoshi]
    /// 0B : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("put")]
    public abstract void Put(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDGRhdGFBm/ZnzhLAcBN4aMFFU4tQQeY/GIRA
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 64617461 [8 datoshi]
    /// 09 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : OpCode.PUSH2 [1 datoshi]
    /// 0F : OpCode.PACK [2048 datoshi]
    /// 10 : OpCode.STLOC0 [2 datoshi]
    /// 11 : OpCode.PUSH3 [1 datoshi]
    /// 12 : OpCode.LDARG0 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.UNPACK [2048 datoshi]
    /// 15 : OpCode.DROP [2 datoshi]
    /// 16 : OpCode.REVERSE3 [2 datoshi]
    /// 17 : OpCode.CAT [2048 datoshi]
    /// 18 : OpCode.SWAP [2 datoshi]
    /// 19 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("put2")]
    public abstract void Put2(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEnhZwUVTi1BB5j8YhEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSH2 [1 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.LDSFLD1 [2 datoshi]
    /// 06 : OpCode.UNPACK [2048 datoshi]
    /// 07 : OpCode.DROP [2 datoshi]
    /// 08 : OpCode.REVERSE3 [2 datoshi]
    /// 09 : OpCode.CAT [2048 datoshi]
    /// 0A : OpCode.SWAP [2 datoshi]
    /// 0B : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
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
