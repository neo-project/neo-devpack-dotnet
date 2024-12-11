using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP11(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP11"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":29,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":207,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":366,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":413,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":441,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":523,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8LZ2V0Q29udHJhY3QBAAEPAAD9MAMMBFRFU1RAEEAMAQBB9rRr4kGSXegxStgmBEUQQFcBAXhK2SgkBkUJIgbKABSzJCUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYFRRBA2yFAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgZFECIE2yFxaXmecWkQtSYECUBpsSQQeGjBRVOLUEEvWMXtIg9peGjBRVOLUEHmPxiECEBXAwF4ygBAtyY8DDdUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA2NCBvciBsZXNzIGJ5dGVzIGxvbmcuOhMRiE4QUdBBm/ZnzhLAcHhowUVTi1BBkl3oMUrYJjRFDC5UaGUgdG9rZW4gd2l0aCBnaXZlbiAidG9rZW5JZCIgZG9lcyBub3QgZXhpc3QuOnFpNwAAcmoQzkBXAgETEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DE3AABxyEoMBG5hbWVpEc7QQFcBABMRiE4QUdBBm/ZnzhLAcBNowUVB3zC4mkBXAQF4StkoJAZFCSIGygAUsyQkDB9UaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkOhQRiE4QUdBBm/ZnzhLAcBN4aMFFU4tQQd8wuJpAVwMDeHBo2CYFCCIReErZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOhMRiE4QUdBBm/ZnzhLAcHlowUVTi1BBkl3oMTcAAHFpEM5yakH4J+yMJAQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQFcCA3p4Nc/9//9FQZv2Z84UEYhOEFHQUBLAcHh5i9socXoQtyYQEGlowUVTi1BB5j8YhEBpaMFFU4tQQS9Yxe1AVwEEehF5eBTADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcCAHBo2KomIHt6EXgUwB8MDm9uTkVQMTFQYXltZW50eUFifVtSRUD2L0Iv"));

    #endregion

    #region Events

    public delegate void delTransfer(UInt160? from, UInt160? to, BigInteger? amount, byte[]? tokenId);

    [DisplayName("Transfer")]
    public event delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract object? Tokens { [DisplayName("tokens")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIF 25 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// THROW [512 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH1 [1 datoshi]
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
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
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
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBeMoAQLcmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNjQgb3IgbGVzcyBieXRlcyBsb25nLjoTEYhOEFHQQZv2Z84SwHB4aMFFU4tQQZJd6DFK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5A
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203634206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 34 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 54686520746F6B656E207769746820676976656E2022746F6B656E49642220646F6573206E6F742065786973742E [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("ownerOf")]
    public abstract UInt160? OwnerOf(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBExGIThBR0EGb9mfOEsBweGjBRVOLUEGSXegxNwAAcchKDARuYW1laRHO0EA=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC1 [2 datoshi]
    /// NEWMAP [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 6E616D65 'name' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// SETITEM [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("properties")]
    public abstract IDictionary<object, object>? Properties(byte[]? tokenId);

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDoUEYhOEFHQQZv2Z84SwHATeGjBRVOLUEHfMLiaQA==
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIF 24 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C6964 [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL DF30B89A 'System.Storage.Find' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tokensOf")]
    public abstract object? TokensOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDeHBo2CYFCCIReErZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOhMRiE4QUdBBm/ZnzhLAcHlowUVTi1BBkl3oMTcAAHFpEM5yakH4J+yMJAQJQGp4mCYleEppEFHQRWk3AQBKeWjBRVOLUEHmPxiERQ95ajQPEXl4NAp6eXhqNEUIQA==
    /// INITSLOT 0303 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 11 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 22 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// NOTEQUAL [32 datoshi]
    /// JMPIFNOT 25 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// DUP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// DROP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALL 0F [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 0A [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALL 45 [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
