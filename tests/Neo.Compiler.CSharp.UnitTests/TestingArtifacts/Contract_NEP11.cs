using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP11(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP11"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-11""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":29,""safe"":true},{""name"":""ownerOf"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Hash160"",""offset"":169,""safe"":true},{""name"":""properties"",""parameters"":[{""name"":""tokenId"",""type"":""ByteArray""}],""returntype"":""Map"",""offset"":317,""safe"":true},{""name"":""tokens"",""parameters"":[],""returntype"":""InteropInterface"",""offset"":474,""safe"":true},{""name"":""tokensOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""InteropInterface"",""offset"":491,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""tokenId"",""type"":""ByteArray""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":562,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""tokenId"",""type"":""ByteArray""}]}]},""permissions"":[{""contract"":""0x726cb6e0cd8628a1350a611384688911ab75f51b"",""methods"":[""sha256""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""isContract""]},{""contract"":""*"",""methods"":[""onNEP11Payment""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPA7znO4OTpJcbCoGp54UQN2G/OrAtkZXNlcmlhbGl6ZQEAAQXA7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEF/aP6Q0bqUyolj8SX3a3bZDfJ/f8KaXNDb250cmFjdAEAAQUAAP2aAwwEVEVTVEAQQAwBAEH2tGviQZJd6DFK2CYERRBAVwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46eHAMAQHbMGiLQdWNXuhK2CYFRRBA2yFAVwICeHEMAQHbMGmLcGhB1Y1e6ErYJgZFECIE2yFxaXmecWkQtSYECUBpsSQKaEF1VPWUIglpaEE5DOMKCEBXAwF4ygArtyY8DDdUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA0MyBvciBsZXNzIGJ5dGVzIGxvbmcuOnhxDAED2zBpi3BoQdWNXuhK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5AVwMBeMoAK7cmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNDMgb3IgbGVzcyBieXRlcyBsb25nLjp4cQwBA9swaYtwaEHVjV7oStgmNEUMLlRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByyEoMBG5hbWVqEc7QQFcBAAwBA9swcBNoQQd2UvNAVwIBeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDp4cQwBBNswaYtwE2hBB3ZS80BXBAN4StkoJAZFCSIGygAUsyQiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp5ygArtyY8DDdUaGUgYXJndW1lbnQgInRva2VuSWQiIHNob3VsZCBiZSA0MyBvciBsZXNzIGJ5dGVzIGxvbmcuOnlxDAED2zBpi3BoQdWNXuhK2CY0RQwuVGhlIHRva2VuIHdpdGggZ2l2ZW4gInRva2VuSWQiIGRvZXMgbm90IGV4aXN0LjpxaTcAAHJqEM5za0H4J+yMJAQJQGt4mCYbahB40GhqNwEAUEE5DOMKD3lrNA8ReXg0Cnp5eGs0MQhAVwMDeng1QP3//0V5eHFyDAEE2zBpi2qLcHoQtyYKEGhBOQzjCkBoQXVU9ZRAVwEEeHkRelQUwAwIVHJhbnNmZXJBlQFvYXlwaNgmBQkiBnk3AgAmIHt6EXgUwB8MDm9uTkVQMTFQYXltZW50eUFifVtSRUB8g4Z+").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46eHAMAQHbMGiLQdWNXuhK2CYFRRBA2yFA
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
    /// LDARG0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
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
    /// Script: VwMBeMoAK7cmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNDMgb3IgbGVzcyBieXRlcyBsb25nLjp4cQwBA9swaYtwaEHVjV7oStgmNEUMLlRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByahDOQA==
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 2B [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203433206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
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
    /// Script: VwMBeMoAK7cmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNDMgb3IgbGVzcyBieXRlcyBsb25nLjp4cQwBA9swaYtwaEHVjV7oStgmNEUMLlRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByyEoMBG5hbWVqEc7QQA==
    /// INITSLOT 0301 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 2B [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203433206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
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
    /// NEWMAP [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 6E616D65 'name' [8 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// Script: VwIBeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZDp4cQwBBNswaYtwE2hBB3ZS80A=
    /// INITSLOT 0201 [64 datoshi]
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
    /// LDARG0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 04 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 077652F3 'System.Storage.Local.Find' [32768 datoshi]
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
    /// Script: VwQDeErZKCQGRQkiBsoAFLMkIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ecoAK7cmPAw3VGhlIGFyZ3VtZW50ICJ0b2tlbklkIiBzaG91bGQgYmUgNDMgb3IgbGVzcyBieXRlcyBsb25nLjp5cQwBA9swaYtwaEHVjV7oStgmNEUMLlRoZSB0b2tlbiB3aXRoIGdpdmVuICJ0b2tlbklkIiBkb2VzIG5vdCBleGlzdC46cWk3AAByahDOc2tB+CfsjCQECUBreJgmG2oQeNBoajcBAFBBOQzjCg95azQPEXl4NAp6eXhrNDEIQA==
    /// INITSLOT 0403 [64 datoshi]
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
    /// JMPIF 22 [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 2B [1 datoshi]
    /// GT [8 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// PUSHDATA1 54686520617267756D656E742022746F6B656E4964222073686F756C64206265203433206F72206C657373206279746573206C6F6E672E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 03 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
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
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// NOTEQUAL [32 datoshi]
    /// JMPIFNOT 1B [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSHM1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// CALL 0F [512 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 0A [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// CALL 31 [512 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? to, byte[]? tokenId, object? data = null);

    #endregion
}
