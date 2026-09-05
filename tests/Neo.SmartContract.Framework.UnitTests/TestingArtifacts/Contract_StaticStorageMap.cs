using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticStorageMap(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticStorageMap"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""put"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""get"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":19,""safe"":false},{""name"":""putReadonly"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":45,""safe"":false},{""name"":""getReadonly"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""put2"",""parameters"":[{""name"":""message"",""type"":""String""}],""returntype"":""Void"",""offset"":90,""safe"":false},{""name"":""get2"",""parameters"":[{""name"":""msg"",""type"":""String""}],""returntype"":""Integer"",""offset"":124,""safe"":false},{""name"":""teststoragemap_Putbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Void"",""offset"":165,""safe"":false},{""name"":""teststoragemap_Getbyteprefix"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":207,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":255,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0oAVcAAVgReBJSwUVTi1BB5j8YhEBXAAFYeFDBRVOLUEGSXegxStgmBUUQQNshQFcAAVkSeBJSwUVTi1BB5j8YhEBXAAFZeFDBRVOLUEGSXegxStgmBUUQQNshQFcBAUGb9mfODARkYXRhUBLAcGgTeBJSwUVTi1BB5j8YhEBXAQFBm/ZnzgwEZGF0YVASwHBoeFDBRVOLUEGSXegxStgmBUUQQNshQFcBAUGb9mfOeBGIThBR0FASwHBoAHsMBXRlc3QxElLBRVOLUEHmPxiEQFcBAUGb9mfOeBGIThBR0FASwHBoDAV0ZXN0MVDBRVOLUEGSXegxStgmBUUQQNshQFYCQZv2Z84MBGRhdGFQEsBgQZv2Z84MDHJlYWRvbmx5ZGF0YVASwGFAQTzL2Q==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABWHhQwUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("get")]
    public abstract BigInteger? Get(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z84MBGRhdGFQEsBwaHhQwUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// INITSLOT 0101 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 64617461 'data' [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("get2")]
    public abstract BigInteger? Get2(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABWXhQwUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getReadonly")]
    public abstract BigInteger? GetReadonly(string? msg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABWBF4ElLBRVOLUEHmPxiEQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("put")]
    public abstract void Put(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z84MBGRhdGFQEsBwaBN4ElLBRVOLUEHmPxiEQA==
    /// INITSLOT 0101 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 64617461 'data' [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("put2")]
    public abstract void Put2(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABWRJ4ElLBRVOLUEHmPxiEQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putReadonly")]
    public abstract void PutReadonly(string? message);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z854EYhOEFHQUBLAcGgMBXRlc3QxUMFFU4tQQZJd6DFK2CYFRRBA2yFA
    /// INITSLOT 0101 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("teststoragemap_Getbyteprefix")]
    public abstract BigInteger? Teststoragemap_Getbyteprefix(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z854EYhOEFHQUBLAcGgAewwFdGVzdDESUsFFU4tQQeY/GIRA
    /// INITSLOT 0101 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("teststoragemap_Putbyteprefix")]
    public abstract void Teststoragemap_Putbyteprefix(BigInteger? x);

    #endregion
}
