using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticStorageMap(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticStorageMap"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""put"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""get"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":17,""safe"":false},{""name"":""putReadonly"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":42,""safe"":false},{""name"":""getReadonly"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":59,""safe"":false},{""name"":""put2"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":84,""safe"":false},{""name"":""get2"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":115,""safe"":false},{""name"":""teststoragemap_Putbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Void"",""offset"":154,""safe"":false},{""name"":""teststoragemap_Getbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":194,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":241,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0YAVcAARF4WMFFU4tQQeY/GIRAVwABeFjBRVOLUEGSXegxStgmBUUQQNshQFcAARJ4WcFFU4tQQeY/GIRAVwABeFnBRVOLUEGSXegxStgmBUUQQNshQFcBAQwEZGF0YUGb9mfOEsBwE3howUVTi1BB5j8YhEBXAQEMBGRhdGFBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJgVFEEDbIUBXAQFBm/ZnzngRiE4QUdBQEsBwAHsMBXRlc3QxaMFFU4tQQeY/GIRAVwEBQZv2Z854EYhOEFHQUBLAcAwFdGVzdDFowUVTi1BBkl3oMUrYJgVFEEDbIUBWAgwEZGF0YUGb9mfOEsBgDAxyZWFkb25seWRhdGFBm/ZnzhLAYUACGm2M").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFjBRVOLUEGSXegxStgmBUUQQNshQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDSFLD0 [2 datoshi]
    /// 05 : UNPACK [2048 datoshi]
    /// 06 : DROP [2 datoshi]
    /// 07 : REVERSE3 [2 datoshi]
    /// 08 : CAT [2048 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : ISNULL [2 datoshi]
    /// 11 : JMPIFNOT 05 [2 datoshi]
    /// 13 : DROP [2 datoshi]
    /// 14 : PUSH0 [1 datoshi]
    /// 15 : RET [0 datoshi]
    /// 16 : CONVERT 21 'Integer' [8192 datoshi]
    /// 18 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("get")]
    public abstract BigInteger? Get(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDARkYXRhQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CYFRRBA2yFA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHDATA1 64617461 'data' [8 datoshi]
    /// 09 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : PACK [2048 datoshi]
    /// 10 : STLOC0 [2 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : UNPACK [2048 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : REVERSE3 [2 datoshi]
    /// 16 : CAT [2048 datoshi]
    /// 17 : SWAP [2 datoshi]
    /// 18 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIFNOT 05 [2 datoshi]
    /// 21 : DROP [2 datoshi]
    /// 22 : PUSH0 [1 datoshi]
    /// 23 : RET [0 datoshi]
    /// 24 : CONVERT 21 'Integer' [8192 datoshi]
    /// 26 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("get2")]
    public abstract BigInteger? Get2(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeFnBRVOLUEGSXegxStgmBUUQQNshQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDSFLD1 [2 datoshi]
    /// 05 : UNPACK [2048 datoshi]
    /// 06 : DROP [2 datoshi]
    /// 07 : REVERSE3 [2 datoshi]
    /// 08 : CAT [2048 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : ISNULL [2 datoshi]
    /// 11 : JMPIFNOT 05 [2 datoshi]
    /// 13 : DROP [2 datoshi]
    /// 14 : PUSH0 [1 datoshi]
    /// 15 : RET [0 datoshi]
    /// 16 : CONVERT 21 'Integer' [8192 datoshi]
    /// 18 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getReadonly")]
    public abstract BigInteger? GetReadonly(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEXhYwUVTi1BB5j8YhEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : LDSFLD0 [2 datoshi]
    /// 06 : UNPACK [2048 datoshi]
    /// 07 : DROP [2 datoshi]
    /// 08 : REVERSE3 [2 datoshi]
    /// 09 : CAT [2048 datoshi]
    /// 0A : SWAP [2 datoshi]
    /// 0B : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("put")]
    public abstract void Put(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDARkYXRhQZv2Z84SwHATeGjBRVOLUEHmPxiEQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHDATA1 64617461 'data' [8 datoshi]
    /// 09 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : PACK [2048 datoshi]
    /// 10 : STLOC0 [2 datoshi]
    /// 11 : PUSH3 [1 datoshi]
    /// 12 : LDARG0 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : UNPACK [2048 datoshi]
    /// 15 : DROP [2 datoshi]
    /// 16 : REVERSE3 [2 datoshi]
    /// 17 : CAT [2048 datoshi]
    /// 18 : SWAP [2 datoshi]
    /// 19 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("put2")]
    public abstract void Put2(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABEnhZwUVTi1BB5j8YhEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSH2 [1 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : LDSFLD1 [2 datoshi]
    /// 06 : UNPACK [2048 datoshi]
    /// 07 : DROP [2 datoshi]
    /// 08 : REVERSE3 [2 datoshi]
    /// 09 : CAT [2048 datoshi]
    /// 0A : SWAP [2 datoshi]
    /// 0B : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 10 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("putReadonly")]
    public abstract void PutReadonly(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z854EYhOEFHQUBLAcAwFdGVzdDFowUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH1 [1 datoshi]
    /// 0A : NEWBUFFER [256 datoshi]
    /// 0B : TUCK [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : ROT [2 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : SWAP [2 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : PACK [2048 datoshi]
    /// 12 : STLOC0 [2 datoshi]
    /// 13 : PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 1A : LDLOC0 [2 datoshi]
    /// 1B : UNPACK [2048 datoshi]
    /// 1C : DROP [2 datoshi]
    /// 1D : REVERSE3 [2 datoshi]
    /// 1E : CAT [2048 datoshi]
    /// 1F : SWAP [2 datoshi]
    /// 20 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : ISNULL [2 datoshi]
    /// 27 : JMPIFNOT 05 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : PUSH0 [1 datoshi]
    /// 2B : RET [0 datoshi]
    /// 2C : CONVERT 21 'Integer' [8192 datoshi]
    /// 2E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("teststoragemap_Getbyteprefix")]
    public abstract BigInteger? Teststoragemap_Getbyteprefix(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z854EYhOEFHQUBLAcAB7DAV0ZXN0MWjBRVOLUEHmPxiEQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH1 [1 datoshi]
    /// 0A : NEWBUFFER [256 datoshi]
    /// 0B : TUCK [2 datoshi]
    /// 0C : PUSH0 [1 datoshi]
    /// 0D : ROT [2 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : SWAP [2 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : PACK [2048 datoshi]
    /// 12 : STLOC0 [2 datoshi]
    /// 13 : PUSHINT8 7B [1 datoshi]
    /// 15 : PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 1C : LDLOC0 [2 datoshi]
    /// 1D : UNPACK [2048 datoshi]
    /// 1E : DROP [2 datoshi]
    /// 1F : REVERSE3 [2 datoshi]
    /// 20 : CAT [2048 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("teststoragemap_Putbyteprefix")]
    public abstract void Teststoragemap_Putbyteprefix(BigInteger? x);

    #endregion
}
