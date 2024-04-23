using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IndexOrRange : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IndexOrRange"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":63,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwEaXRvYQEAAQ8AAP3KAjcAAEA3AQBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcUAAwKAQIDBAUGBwgJCtswcGhKylAQUUufjHFoE1AQUUufjHJoSspQElFLn4xzaBVQE1FLn4x0aErKUErKEp9RS5+MdWhKyhOfUBBRS5+MdmhKyhSfUBNRS5+MdwdoSsoSn1BKyhSfUUufjHcIaBDOdwlpyjcCAEHP50eWaso3AgBBz+dHlmvKNwIAQc/nR5ZsyjcCAEHP50eWbco3AgBBz+dHlm7KNwIAQc/nR5ZvB8o3AgBBz+dHlm8IyjcCAEHP50eWbwk3AgBBz+dHlgwJMTIzNDU2Nzg5dwpvCkrKUBBRS5+M2yh3C28KE1AQUUufjNsodwxvCkrKUBJRS5+M2yh3DW8KFVATUUufjNsodw5vCkrKUErKEp9RS5+M2yh3D28KSsoTn1AQUUufjNsodxBvCkrKFJ9QE1FLn4zbKHcRbwpKyhKfUErKFJ9RS5+M2yh3Em8KEM53E28LShDOEs42Qc/nR5ZvDEoQzhLONkHP50eWbw1KEM4SzjZBz+dHlm8OShDOEs42Qc/nR5ZvD0oQzhLONkHP50eWbxBKEM4SzjZBz+dHlm8RShDOEs42Qc/nR5ZvEkoQzhLONkHP50eWbxNKEM4SzjZBz+dHlkBBz+dHlkBXAAN5ep5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAVwABDARiYXNlIgJAVwABDARiYXNlIgJAVwADeXqgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcAAXgQzkBXAAJ5StgmGkUMFHZhbHVlIGNhbm5vdCBiZSBudWxsOkp4EFHQQM8awso="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion

    #region Constructor for internal use only

    protected Contract_IndexOrRange(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
