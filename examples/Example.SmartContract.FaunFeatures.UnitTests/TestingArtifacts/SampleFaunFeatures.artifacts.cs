using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleFaunFeatures(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleFaunFeatures"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""hexEncode"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""hexDecode"",""parameters"":[{""name"":""hex"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":19,""safe"":true},{""name"":""execFeeFactor"",""parameters"":[],""returntype"":""Integer"",""offset"":38,""safe"":true},{""name"":""execPicoFeeFactor"",""parameters"":[],""returntype"":""Integer"",""offset"":48,""safe"":true},{""name"":""isCommitteeSigned"",""parameters"":[],""returntype"":""Boolean"",""offset"":58,""safe"":true}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Description"":""Demonstrates Neo v3.9 HF_Faun native additions"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy45LjArNDIzNzFmNWY0ZTBiZTI4N2ExZmYyOGYzNThhYjI0NmY1YjQuLi4AAAXA7znO4OTpJcbCoGp54UQN2G/OrAloZXhFbmNvZGUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwJaGV4RGVjb2RlAQABD3vGgcCh9x1UNFe2i7qNX5/dTl7MEGdldEV4ZWNGZWVGYWN0b3IAAAEPe8aBwKH3HVQ0V7aLuo1fn91OXswUZ2V0RXhlY1BpY29GZWVGYWN0b3IAAAEPwTpWyYNTp+pqMk2ag10bW/ImYxUGdmVyaWZ5AAABDwAARFcAAXjbKDcAACICQDcAAEDbKEBXAAF4NwEA2zAiAkDbMEA3AQBANwIAIgJANwIAQDcDACICQDcDAEA3BAAiAkA3BABAfBXzsg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? ExecFeeFactor { [DisplayName("execFeeFactor")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? ExecPicoFeeFactor { [DisplayName("execPicoFeeFactor")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? IsCommitteeSigned { [DisplayName("isCommitteeSigned")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("hexDecode")]
    public abstract byte[]? HexDecode(string? hex);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("hexEncode")]
    public abstract string? HexEncode(byte[]? data);

    #endregion

}
