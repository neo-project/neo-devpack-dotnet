using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Foreach : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Foreach"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""intForeach"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""stringForeach"",""parameters"":[],""returntype"":""String"",""offset"":86,""safe"":false},{""name"":""byteStringEmpty"",""parameters"":[],""returntype"":""Integer"",""offset"":140,""safe"":false},{""name"":""byteStringForeach"",""parameters"":[],""returntype"":""ByteArray"",""offset"":149,""safe"":false},{""name"":""structForeach"",""parameters"":[],""returntype"":""Map"",""offset"":207,""safe"":false},{""name"":""byteArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":319,""safe"":false},{""name"":""uInt160Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":357,""safe"":false},{""name"":""uInt256Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":434,""safe"":false},{""name"":""eCPointForeach"",""parameters"":[],""returntype"":""Array"",""offset"":535,""safe"":false},{""name"":""bigIntegerForeach"",""parameters"":[],""returntype"":""Array"",""offset"":596,""safe"":false},{""name"":""objectArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":651,""safe"":false},{""name"":""intForeachBreak"",""parameters"":[{""name"":""breakIndex"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":698,""safe"":false},{""name"":""testContinue"",""parameters"":[],""returntype"":""Integer"",""offset"":854,""safe"":false},{""name"":""intForloop"",""parameters"":[],""returntype"":""Integer"",""offset"":958,""safe"":false},{""name"":""testIteratorForEach"",""parameters"":[],""returntype"":""Void"",""offset"":1089,""safe"":false},{""name"":""testForEachVariable"",""parameters"":[],""returntype"":""Void"",""offset"":1195,""safe"":false},{""name"":""testDo"",""parameters"":[],""returntype"":""Void"",""offset"":1241,""safe"":false},{""name"":""testWhile"",""parameters"":[],""returntype"":""Void"",""offset"":1312,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1385,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/aIFVwYAFBMSERTAcBBxaEpyynMQdCI9amzOdWltnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcUVsnHRsazDDaUBXBgAMA2hpagwDZGVmDANhYmMTwHAMAHFoSnLKcxB0IhFqbM51aW2L2yhKcUVsnHRsazDvaUBXAQAMAHBoykBXBgAMAAwADANoaWoMA2RlZgwDYWJjFcBwDABxaEpyynMQdCIRamzOdWlti9soSnFFbJx0bGsw72lAVwgAxUoLz0oQz0o0YXAMBXRlc3QxSmgQUdBFEUpoEVHQRcVKC89KEM9KNEJxDAV0ZXN0MkppEFHQRRJKaRFR0EVpaBLAcshzakp0ynUQdiIXbG7OdwdvBxHOSm8HEM5rU9BFbpx2bm0w6WtAVwABQFcGAAwDAQoR2zBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlAVwYADBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAASwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgAMIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABLAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQFcGAFjbKErYJAlKygAhKAM6WNsoStgkCUrKACEoAzoSwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgADAABkp7O24A0CAMqaOwJAQg8AARAnFMBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlAVwYAAHsMBHRlc3QMAgEC2zATwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgEUExIRFMBwEHE8iwAAAAAAAABoSnLKcxB0InVqbM51eEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4AQtiYEIj1pbZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnFFbJx0bGswiz0Fcj0CaUBXBgAVFBMSERXAcBBxO1YAaEpyynMQdCJGamzOdW0SohCXJgQiNmltnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcUVsnHRsazC6PQVyPQJpQFcDABQTEhEUwHAQcRBKckUia2loas6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pxRWpKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9yRWpoyrUkk2lAVwMAE0Gb9mfOExGIThBR0FASwMFFQd8wuJpwaHEiEWlB81S/HXJq2yhBz+dHlmlBnAjtnCTrQMVKEM9KC89KWc8MBXdvcmxkEhJNNBvFShDPSgvPSlnPDAVoZWxsbxESTTQFEsBAVwABQFcFADTKSnDKcRByIh5oas7BRXN0azcAAAwCOiCLbIvbKEHP50eWapxyamkw4kBXAQAQcGg3AABBz+dHlmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWgVtSTBQFcBABBwaBW1JkBoNwAAQc/nR5ZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEUiv0BWAgwhAkcA2y6Q2fAsT5/IYqusqScl+VtP3cyNf/pThpPs9GOpYAoAAAAACgAAAAAKAAAAABPAYUBo2EKv"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDo")]
    public abstract void TestDo();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testWhile")]
    public abstract void TestWhile();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();

    #endregion

    #region Constructor for internal use only

    protected Contract_Foreach(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
