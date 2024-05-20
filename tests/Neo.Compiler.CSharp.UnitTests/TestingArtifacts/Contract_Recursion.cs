using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":53,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":196,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":225,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9/gBXAAF4ELgMGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYLeHgRnzTUoCIFESICQFcFBHgQtwwTQ291bnQgb2YgZGlza3MgPD0gMOF4EZcmJMJKcEVoxUoQz0oQz0oQz0o0WkoQEdBKEXnQShJ70M9oIkl6e3l4EZ80t0pwRWjFShDPShDPShDPSjQxShB40EoRedBKEnvQz3t5engRnzSRSnHKchBzIgxpa850aGzPa5xza2ow9GgiAkBXAAFAVwABeBCXJgcR2yAiEXgQtSYHeBGeIgV4EZ80A0BXAAF4EJcmBxDbICIReBC1Jgd4EZ4iBXgRnzTJQC+ppWM="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("even")]
    public abstract bool? Even(BigInteger? n);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("hanoiTower")]
    public abstract IList<object>? HanoiTower(BigInteger? n, BigInteger? src, BigInteger? aux, BigInteger? dst);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("odd")]
    public abstract bool? Odd(BigInteger? n);

    #endregion

    #region Constructor for internal use only

    protected Contract_Recursion(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
