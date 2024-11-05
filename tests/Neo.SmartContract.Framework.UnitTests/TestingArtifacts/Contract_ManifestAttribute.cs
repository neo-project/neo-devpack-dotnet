using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ManifestAttribute : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ManifestAttribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":207,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":191,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Author"":""core-dev"",""E-mail"":""dev@neo.org"",""Version"":""v3.6.3"",""Description"":""This is a test contract."",""ExtraKey"":""ExtraValue""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANhXAAJY2CYeCwsSwEpZzwwLbm9SZWVudHJhbnQB/wASTTQeYFg0QXkQlyYEIgx5AHuXJgYQeDTLWDVoAAAAQFcAA3g0H3pKeBFR0EVBm/ZnznkRiE4QUdBQEsBKeBBR0EVAVwABQFcBAXgRzngQzsFFU4tQQZJd6DFwaAuXDA9BbHJlYWR5IGVudGVyZWThEXgRzngQzsFFU4tQQeY/GIRAVwABeBHOeBDOwUVTi1BBL1jF7UBXAAF4NANAVwABQFYCCt////8Kn////xLAYUDCSjTjIy3///8XnFx4"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    #endregion

    #region Constructor for internal use only

    protected Contract_ManifestAttribute(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
