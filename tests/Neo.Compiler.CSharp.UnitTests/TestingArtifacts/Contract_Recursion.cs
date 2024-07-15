using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":53,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":196,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":223,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPpXAAF4ELgMGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYLeHgRnzTUoCIFESICQFcFBHgQtwwTQ291bnQgb2YgZGlza3MgPD0gMOF4EZcmJMJKcEVoxUoQz0oQz0oQz0o0WkoQEdBKEXnQShJ70M9oIkl6e3l4EZ80t0pwRWjFShDPShDPShDPSjQxShB40EoRedBKEnvQz3t5engRnzSRSnHKchBzIgxpa850aGzPa5xza2ow9GgiAkBXAAFAVwABeBCXJgUIIhF4ELUmB3gRniIFeBGfNANAVwABeBCXJgUJIhF4ELUmB3gRniIFeBGfNM1AA4UAVA=="));

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
