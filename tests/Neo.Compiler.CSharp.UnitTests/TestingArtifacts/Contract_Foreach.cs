using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Foreach(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Foreach"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""intForeach"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""stringForeach"",""parameters"":[],""returntype"":""String"",""offset"":38,""safe"":false},{""name"":""byteStringEmpty"",""parameters"":[],""returntype"":""Integer"",""offset"":90,""safe"":false},{""name"":""byteStringForeach"",""parameters"":[],""returntype"":""ByteArray"",""offset"":99,""safe"":false},{""name"":""structForeach"",""parameters"":[],""returntype"":""Map"",""offset"":155,""safe"":false},{""name"":""byteArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":251,""safe"":false},{""name"":""uInt160Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":289,""safe"":false},{""name"":""uInt256Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":366,""safe"":false},{""name"":""eCPointForeach"",""parameters"":[],""returntype"":""Array"",""offset"":467,""safe"":false},{""name"":""bigIntegerForeach"",""parameters"":[],""returntype"":""Array"",""offset"":596,""safe"":false},{""name"":""objectArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":651,""safe"":false},{""name"":""intForeachBreak"",""parameters"":[{""name"":""breakIndex"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":698,""safe"":false},{""name"":""testContinue"",""parameters"":[],""returntype"":""Integer"",""offset"":754,""safe"":false},{""name"":""intForloop"",""parameters"":[],""returntype"":""Integer"",""offset"":810,""safe"":false},{""name"":""testIteratorForEach"",""parameters"":[],""returntype"":""Void"",""offset"":845,""safe"":false},{""name"":""testForEachVariable"",""parameters"":[],""returntype"":""Void"",""offset"":911,""safe"":false},{""name"":""testDo"",""parameters"":[],""returntype"":""Void"",""offset"":957,""safe"":false},{""name"":""testWhile"",""parameters"":[],""returntype"":""Void"",""offset"":982,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/fEDVwYAFBMSERTAcBBxaEpyynMQdCINamzOdWltnnFsnHRsazDzaUBXBgAMA2hpagwDZGVmDANhYmMTwHAMAHFoSnLKcxB0Ig9qbM51aW2L2yhxbJx0bGsw8WlAVwEADABwaMpAVwYADAAMAAwDaGlqDANkZWYMA2FiYxXAcAwAcWhKcspzEHQiD2psznVpbYvbKHFsnHRsazDxaUBXCAAQCxK/cAwFdGVzdDFKaBBR0EURSmgRUdBFEAsSv3EMBXRlc3QySmkQUdBFEkppEVHQRWloEsByyHNqSnTKdRB2Ihdsbs53B28HEc5KbwcQzmtT0EVunHZubTDpa0BXBgAMAwEKEdswcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQFcGAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAAMFAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlAVwYADCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAASwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgAMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqdsoStgkCUrKACEoAzoMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqdsoStgkCUrKACEoAzoSwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgADAABkp7O24A0CAMqaOwJAQg8AARAnFMBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlAVwYAAHsMBHRlc3QMAgEC2zATwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgEUExIRFMBwEHE7JwBoSnLKcxB0IhdqbM51eEqdgBC2JgQiDWltnnFsnHRsazDpPQVyPQJpQFcGABUUExIRFcBwEHE7JgBoSnLKcxB0IhZqbM51bRKiEJcmBCIGaW2ecWycdGxrMOo9BXI9AmlAVwMAFBMSERTAcBBxEHIiDWloas6ecWpKnHJFamjKtSTxaUBXAwATQZv2Z84TEYhOEFHQUBLAwUVB3zC4mnBocSIRaUHzVL8dcmrbKEHP50eWaUGcCO2cJOtACxASvwsQEr8SwEBXBQA08kpwynEQciIeaGrOwUVzdGs3AAAMAjogi2yL2yhBz+dHlmqccmppMOJAVwEAEHBoNwAAQc/nR5ZoSpxwRWgVtSTvQFcBABBwaBW1JhJoNwAAQc/nR5ZoSpxwRSLtQH8KJq4="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAwAAZKeztuANAgDKmjsCQEIPAAEQJxTAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHINT64 000064A7B3B6E00D [1 datoshi]
    /// 0C : OpCode.PUSHINT32 00CA9A3B [1 datoshi]
    /// 11 : OpCode.PUSHINT32 40420F00 [1 datoshi]
    /// 16 : OpCode.PUSHINT16 1027 [1 datoshi]
    /// 19 : OpCode.PUSH4 [1 datoshi]
    /// 1A : OpCode.PACK [2048 datoshi]
    /// 1B : OpCode.STLOC0 [2 datoshi]
    /// 1C : OpCode.NEWARRAY0 [16 datoshi]
    /// 1D : OpCode.STLOC1 [2 datoshi]
    /// 1E : OpCode.LDLOC0 [2 datoshi]
    /// 1F : OpCode.DUP [2 datoshi]
    /// 20 : OpCode.STLOC2 [2 datoshi]
    /// 21 : OpCode.SIZE [4 datoshi]
    /// 22 : OpCode.STLOC3 [2 datoshi]
    /// 23 : OpCode.PUSH0 [1 datoshi]
    /// 24 : OpCode.STLOC4 [2 datoshi]
    /// 25 : OpCode.JMP 0C [2 datoshi]
    /// 27 : OpCode.LDLOC2 [2 datoshi]
    /// 28 : OpCode.LDLOC4 [2 datoshi]
    /// 29 : OpCode.PICKITEM [64 datoshi]
    /// 2A : OpCode.STLOC5 [2 datoshi]
    /// 2B : OpCode.LDLOC1 [2 datoshi]
    /// 2C : OpCode.LDLOC5 [2 datoshi]
    /// 2D : OpCode.APPEND [8192 datoshi]
    /// 2E : OpCode.LDLOC4 [2 datoshi]
    /// 2F : OpCode.INC [4 datoshi]
    /// 30 : OpCode.STLOC4 [2 datoshi]
    /// 31 : OpCode.LDLOC4 [2 datoshi]
    /// 32 : OpCode.LDLOC3 [2 datoshi]
    /// 33 : OpCode.JMPLT F4 [2 datoshi]
    /// 35 : OpCode.LDLOC1 [2 datoshi]
    /// 36 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAMBChHbMHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUA=
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 010A11 [8 datoshi]
    /// 08 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.NEWARRAY0 [16 datoshi]
    /// 0C : OpCode.STLOC1 [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.STLOC2 [2 datoshi]
    /// 10 : OpCode.SIZE [4 datoshi]
    /// 11 : OpCode.STLOC3 [2 datoshi]
    /// 12 : OpCode.PUSH0 [1 datoshi]
    /// 13 : OpCode.STLOC4 [2 datoshi]
    /// 14 : OpCode.JMP 0C [2 datoshi]
    /// 16 : OpCode.LDLOC2 [2 datoshi]
    /// 17 : OpCode.LDLOC4 [2 datoshi]
    /// 18 : OpCode.PICKITEM [64 datoshi]
    /// 19 : OpCode.STLOC5 [2 datoshi]
    /// 1A : OpCode.LDLOC1 [2 datoshi]
    /// 1B : OpCode.LDLOC5 [2 datoshi]
    /// 1C : OpCode.APPEND [8192 datoshi]
    /// 1D : OpCode.LDLOC4 [2 datoshi]
    /// 1E : OpCode.INC [4 datoshi]
    /// 1F : OpCode.STLOC4 [2 datoshi]
    /// 20 : OpCode.LDLOC4 [2 datoshi]
    /// 21 : OpCode.LDLOC3 [2 datoshi]
    /// 22 : OpCode.JMPLT F4 [2 datoshi]
    /// 24 : OpCode.LDLOC1 [2 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADABwaMpA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 [8 datoshi]
    /// 05 : OpCode.STLOC0 [2 datoshi]
    /// 06 : OpCode.LDLOC0 [2 datoshi]
    /// 07 : OpCode.SIZE [4 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAMAAwDaGlqDANkZWYMA2FiYxXAcAwAcWhKcspzEHQiD2psznVpbYvbKHFsnHRsazDxaUA=
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 [8 datoshi]
    /// 05 : OpCode.PUSHDATA1 [8 datoshi]
    /// 07 : OpCode.PUSHDATA1 68696A 'hij' [8 datoshi]
    /// 0C : OpCode.PUSHDATA1 646566 'def' [8 datoshi]
    /// 11 : OpCode.PUSHDATA1 616263 'abc' [8 datoshi]
    /// 16 : OpCode.PUSH5 [1 datoshi]
    /// 17 : OpCode.PACK [2048 datoshi]
    /// 18 : OpCode.STLOC0 [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 [8 datoshi]
    /// 1B : OpCode.STLOC1 [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.STLOC2 [2 datoshi]
    /// 1F : OpCode.SIZE [4 datoshi]
    /// 20 : OpCode.STLOC3 [2 datoshi]
    /// 21 : OpCode.PUSH0 [1 datoshi]
    /// 22 : OpCode.STLOC4 [2 datoshi]
    /// 23 : OpCode.JMP 0F [2 datoshi]
    /// 25 : OpCode.LDLOC2 [2 datoshi]
    /// 26 : OpCode.LDLOC4 [2 datoshi]
    /// 27 : OpCode.PICKITEM [64 datoshi]
    /// 28 : OpCode.STLOC5 [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.LDLOC5 [2 datoshi]
    /// 2B : OpCode.CAT [2048 datoshi]
    /// 2C : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 2E : OpCode.STLOC1 [2 datoshi]
    /// 2F : OpCode.LDLOC4 [2 datoshi]
    /// 30 : OpCode.INC [4 datoshi]
    /// 31 : OpCode.STLOC4 [2 datoshi]
    /// 32 : OpCode.LDLOC4 [2 datoshi]
    /// 33 : OpCode.LDLOC3 [2 datoshi]
    /// 34 : OpCode.JMPLT F1 [2 datoshi]
    /// 36 : OpCode.LDLOC1 [2 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6nbKErYJAlKygAhKAM6DCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6nbKErYJAlKygAhKAM6EsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9 [8 datoshi]
    /// 26 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 28 : OpCode.DUP [2 datoshi]
    /// 29 : OpCode.ISNULL [2 datoshi]
    /// 2A : OpCode.JMPIF 09 [2 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.SIZE [4 datoshi]
    /// 2E : OpCode.PUSHINT8 21 [1 datoshi]
    /// 30 : OpCode.JMPEQ 03 [2 datoshi]
    /// 32 : OpCode.THROW [512 datoshi]
    /// 33 : OpCode.PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9 [8 datoshi]
    /// 56 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 58 : OpCode.DUP [2 datoshi]
    /// 59 : OpCode.ISNULL [2 datoshi]
    /// 5A : OpCode.JMPIF 09 [2 datoshi]
    /// 5C : OpCode.DUP [2 datoshi]
    /// 5D : OpCode.SIZE [4 datoshi]
    /// 5E : OpCode.PUSHINT8 21 [1 datoshi]
    /// 60 : OpCode.JMPEQ 03 [2 datoshi]
    /// 62 : OpCode.THROW [512 datoshi]
    /// 63 : OpCode.PUSH2 [1 datoshi]
    /// 64 : OpCode.PACK [2048 datoshi]
    /// 65 : OpCode.STLOC0 [2 datoshi]
    /// 66 : OpCode.NEWARRAY0 [16 datoshi]
    /// 67 : OpCode.STLOC1 [2 datoshi]
    /// 68 : OpCode.LDLOC0 [2 datoshi]
    /// 69 : OpCode.DUP [2 datoshi]
    /// 6A : OpCode.STLOC2 [2 datoshi]
    /// 6B : OpCode.SIZE [4 datoshi]
    /// 6C : OpCode.STLOC3 [2 datoshi]
    /// 6D : OpCode.PUSH0 [1 datoshi]
    /// 6E : OpCode.STLOC4 [2 datoshi]
    /// 6F : OpCode.JMP 0C [2 datoshi]
    /// 71 : OpCode.LDLOC2 [2 datoshi]
    /// 72 : OpCode.LDLOC4 [2 datoshi]
    /// 73 : OpCode.PICKITEM [64 datoshi]
    /// 74 : OpCode.STLOC5 [2 datoshi]
    /// 75 : OpCode.LDLOC1 [2 datoshi]
    /// 76 : OpCode.LDLOC5 [2 datoshi]
    /// 77 : OpCode.APPEND [8192 datoshi]
    /// 78 : OpCode.LDLOC4 [2 datoshi]
    /// 79 : OpCode.INC [4 datoshi]
    /// 7A : OpCode.STLOC4 [2 datoshi]
    /// 7B : OpCode.LDLOC4 [2 datoshi]
    /// 7C : OpCode.LDLOC3 [2 datoshi]
    /// 7D : OpCode.JMPLT F4 [2 datoshi]
    /// 7F : OpCode.LDLOC1 [2 datoshi]
    /// 80 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFBMSERTAcBBxaEpyynMQdCINamzOdWltnnFsnHRsazDzaUA=
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSH4 [1 datoshi]
    /// 04 : OpCode.PUSH3 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PUSH1 [1 datoshi]
    /// 07 : OpCode.PUSH4 [1 datoshi]
    /// 08 : OpCode.PACK [2048 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC1 [2 datoshi]
    /// 0C : OpCode.LDLOC0 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.STLOC2 [2 datoshi]
    /// 0F : OpCode.SIZE [4 datoshi]
    /// 10 : OpCode.STLOC3 [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.STLOC4 [2 datoshi]
    /// 13 : OpCode.JMP 0D [2 datoshi]
    /// 15 : OpCode.LDLOC2 [2 datoshi]
    /// 16 : OpCode.LDLOC4 [2 datoshi]
    /// 17 : OpCode.PICKITEM [64 datoshi]
    /// 18 : OpCode.STLOC5 [2 datoshi]
    /// 19 : OpCode.LDLOC1 [2 datoshi]
    /// 1A : OpCode.LDLOC5 [2 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.STLOC1 [2 datoshi]
    /// 1D : OpCode.LDLOC4 [2 datoshi]
    /// 1E : OpCode.INC [4 datoshi]
    /// 1F : OpCode.STLOC4 [2 datoshi]
    /// 20 : OpCode.LDLOC4 [2 datoshi]
    /// 21 : OpCode.LDLOC3 [2 datoshi]
    /// 22 : OpCode.JMPLT F3 [2 datoshi]
    /// 24 : OpCode.LDLOC1 [2 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBFBMSERTAcBBxOycAaEpyynMQdCIXamzOdXhKnYAQtiYEIg1pbZ5xbJx0bGsw6T0Fcj0CaUA=
    /// 00 : OpCode.INITSLOT 0601 [64 datoshi]
    /// 03 : OpCode.PUSH4 [1 datoshi]
    /// 04 : OpCode.PUSH3 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PUSH1 [1 datoshi]
    /// 07 : OpCode.PUSH4 [1 datoshi]
    /// 08 : OpCode.PACK [2048 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC1 [2 datoshi]
    /// 0C : OpCode.TRY 2700 [4 datoshi]
    /// 0F : OpCode.LDLOC0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.STLOC2 [2 datoshi]
    /// 12 : OpCode.SIZE [4 datoshi]
    /// 13 : OpCode.STLOC3 [2 datoshi]
    /// 14 : OpCode.PUSH0 [1 datoshi]
    /// 15 : OpCode.STLOC4 [2 datoshi]
    /// 16 : OpCode.JMP 17 [2 datoshi]
    /// 18 : OpCode.LDLOC2 [2 datoshi]
    /// 19 : OpCode.LDLOC4 [2 datoshi]
    /// 1A : OpCode.PICKITEM [64 datoshi]
    /// 1B : OpCode.STLOC5 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.DEC [4 datoshi]
    /// 1F : OpCode.STARG0 [2 datoshi]
    /// 20 : OpCode.PUSH0 [1 datoshi]
    /// 21 : OpCode.LE [8 datoshi]
    /// 22 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 24 : OpCode.JMP 0D [2 datoshi]
    /// 26 : OpCode.LDLOC1 [2 datoshi]
    /// 27 : OpCode.LDLOC5 [2 datoshi]
    /// 28 : OpCode.ADD [8 datoshi]
    /// 29 : OpCode.STLOC1 [2 datoshi]
    /// 2A : OpCode.LDLOC4 [2 datoshi]
    /// 2B : OpCode.INC [4 datoshi]
    /// 2C : OpCode.STLOC4 [2 datoshi]
    /// 2D : OpCode.LDLOC4 [2 datoshi]
    /// 2E : OpCode.LDLOC3 [2 datoshi]
    /// 2F : OpCode.JMPLT E9 [2 datoshi]
    /// 31 : OpCode.ENDTRY 05 [4 datoshi]
    /// 33 : OpCode.STLOC2 [2 datoshi]
    /// 34 : OpCode.ENDTRY 02 [4 datoshi]
    /// 36 : OpCode.LDLOC1 [2 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAFBMSERTAcBBxEHIiDWloas6ecWpKnHJFamjKtSTxaUA=
    /// 00 : OpCode.INITSLOT 0300 [64 datoshi]
    /// 03 : OpCode.PUSH4 [1 datoshi]
    /// 04 : OpCode.PUSH3 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PUSH1 [1 datoshi]
    /// 07 : OpCode.PUSH4 [1 datoshi]
    /// 08 : OpCode.PACK [2048 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC1 [2 datoshi]
    /// 0C : OpCode.PUSH0 [1 datoshi]
    /// 0D : OpCode.STLOC2 [2 datoshi]
    /// 0E : OpCode.JMP 0D [2 datoshi]
    /// 10 : OpCode.LDLOC1 [2 datoshi]
    /// 11 : OpCode.LDLOC0 [2 datoshi]
    /// 12 : OpCode.LDLOC2 [2 datoshi]
    /// 13 : OpCode.PICKITEM [64 datoshi]
    /// 14 : OpCode.ADD [8 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.LDLOC2 [2 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.INC [4 datoshi]
    /// 19 : OpCode.STLOC2 [2 datoshi]
    /// 1A : OpCode.DROP [2 datoshi]
    /// 1B : OpCode.LDLOC2 [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.SIZE [4 datoshi]
    /// 1E : OpCode.LT [8 datoshi]
    /// 1F : OpCode.JMPIF F1 [2 datoshi]
    /// 21 : OpCode.LDLOC1 [2 datoshi]
    /// 22 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAHsMBHRlc3QMAgEC2zATwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUA=
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHINT8 7B [1 datoshi]
    /// 05 : OpCode.PUSHDATA1 74657374 'test' [8 datoshi]
    /// 0B : OpCode.PUSHDATA1 0102 [8 datoshi]
    /// 0F : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 11 : OpCode.PUSH3 [1 datoshi]
    /// 12 : OpCode.PACK [2048 datoshi]
    /// 13 : OpCode.STLOC0 [2 datoshi]
    /// 14 : OpCode.NEWARRAY0 [16 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.STLOC2 [2 datoshi]
    /// 19 : OpCode.SIZE [4 datoshi]
    /// 1A : OpCode.STLOC3 [2 datoshi]
    /// 1B : OpCode.PUSH0 [1 datoshi]
    /// 1C : OpCode.STLOC4 [2 datoshi]
    /// 1D : OpCode.JMP 0C [2 datoshi]
    /// 1F : OpCode.LDLOC2 [2 datoshi]
    /// 20 : OpCode.LDLOC4 [2 datoshi]
    /// 21 : OpCode.PICKITEM [64 datoshi]
    /// 22 : OpCode.STLOC5 [2 datoshi]
    /// 23 : OpCode.LDLOC1 [2 datoshi]
    /// 24 : OpCode.LDLOC5 [2 datoshi]
    /// 25 : OpCode.APPEND [8192 datoshi]
    /// 26 : OpCode.LDLOC4 [2 datoshi]
    /// 27 : OpCode.INC [4 datoshi]
    /// 28 : OpCode.STLOC4 [2 datoshi]
    /// 29 : OpCode.LDLOC4 [2 datoshi]
    /// 2A : OpCode.LDLOC3 [2 datoshi]
    /// 2B : OpCode.JMPLT F4 [2 datoshi]
    /// 2D : OpCode.LDLOC1 [2 datoshi]
    /// 2E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADANoaWoMA2RlZgwDYWJjE8BwDABxaEpyynMQdCIPamzOdWlti9socWycdGxrMPFpQA==
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 68696A 'hij' [8 datoshi]
    /// 08 : OpCode.PUSHDATA1 646566 'def' [8 datoshi]
    /// 0D : OpCode.PUSHDATA1 616263 'abc' [8 datoshi]
    /// 12 : OpCode.PUSH3 [1 datoshi]
    /// 13 : OpCode.PACK [2048 datoshi]
    /// 14 : OpCode.STLOC0 [2 datoshi]
    /// 15 : OpCode.PUSHDATA1 [8 datoshi]
    /// 17 : OpCode.STLOC1 [2 datoshi]
    /// 18 : OpCode.LDLOC0 [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.STLOC2 [2 datoshi]
    /// 1B : OpCode.SIZE [4 datoshi]
    /// 1C : OpCode.STLOC3 [2 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.STLOC4 [2 datoshi]
    /// 1F : OpCode.JMP 0F [2 datoshi]
    /// 21 : OpCode.LDLOC2 [2 datoshi]
    /// 22 : OpCode.LDLOC4 [2 datoshi]
    /// 23 : OpCode.PICKITEM [64 datoshi]
    /// 24 : OpCode.STLOC5 [2 datoshi]
    /// 25 : OpCode.LDLOC1 [2 datoshi]
    /// 26 : OpCode.LDLOC5 [2 datoshi]
    /// 27 : OpCode.CAT [2048 datoshi]
    /// 28 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 2A : OpCode.STLOC1 [2 datoshi]
    /// 2B : OpCode.LDLOC4 [2 datoshi]
    /// 2C : OpCode.INC [4 datoshi]
    /// 2D : OpCode.STLOC4 [2 datoshi]
    /// 2E : OpCode.LDLOC4 [2 datoshi]
    /// 2F : OpCode.LDLOC3 [2 datoshi]
    /// 30 : OpCode.JMPLT F1 [2 datoshi]
    /// 32 : OpCode.LDLOC1 [2 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwgAEAsSv3AMBXRlc3QxSmgQUdBFEUpoEVHQRRALEr9xDAV0ZXN0MkppEFHQRRJKaRFR0EVpaBLAcshzakp0ynUQdiIXbG7OdwdvBxHOSm8HEM5rU9BFbpx2bm0w6WtA
    /// 00 : OpCode.INITSLOT 0800 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.ROT [2 datoshi]
    /// 13 : OpCode.SETITEM [8192 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSH1 [1 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.PUSH1 [1 datoshi]
    /// 19 : OpCode.ROT [2 datoshi]
    /// 1A : OpCode.SETITEM [8192 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.PUSH0 [1 datoshi]
    /// 1D : OpCode.PUSHNULL [1 datoshi]
    /// 1E : OpCode.PUSH2 [1 datoshi]
    /// 1F : OpCode.PACKSTRUCT [2048 datoshi]
    /// 20 : OpCode.STLOC1 [2 datoshi]
    /// 21 : OpCode.PUSHDATA1 7465737432 'test2' [8 datoshi]
    /// 28 : OpCode.DUP [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.PUSH0 [1 datoshi]
    /// 2B : OpCode.ROT [2 datoshi]
    /// 2C : OpCode.SETITEM [8192 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.PUSH2 [1 datoshi]
    /// 2F : OpCode.DUP [2 datoshi]
    /// 30 : OpCode.LDLOC1 [2 datoshi]
    /// 31 : OpCode.PUSH1 [1 datoshi]
    /// 32 : OpCode.ROT [2 datoshi]
    /// 33 : OpCode.SETITEM [8192 datoshi]
    /// 34 : OpCode.DROP [2 datoshi]
    /// 35 : OpCode.LDLOC1 [2 datoshi]
    /// 36 : OpCode.LDLOC0 [2 datoshi]
    /// 37 : OpCode.PUSH2 [1 datoshi]
    /// 38 : OpCode.PACK [2048 datoshi]
    /// 39 : OpCode.STLOC2 [2 datoshi]
    /// 3A : OpCode.NEWMAP [8 datoshi]
    /// 3B : OpCode.STLOC3 [2 datoshi]
    /// 3C : OpCode.LDLOC2 [2 datoshi]
    /// 3D : OpCode.DUP [2 datoshi]
    /// 3E : OpCode.STLOC4 [2 datoshi]
    /// 3F : OpCode.SIZE [4 datoshi]
    /// 40 : OpCode.STLOC5 [2 datoshi]
    /// 41 : OpCode.PUSH0 [1 datoshi]
    /// 42 : OpCode.STLOC6 [2 datoshi]
    /// 43 : OpCode.JMP 17 [2 datoshi]
    /// 45 : OpCode.LDLOC4 [2 datoshi]
    /// 46 : OpCode.LDLOC6 [2 datoshi]
    /// 47 : OpCode.PICKITEM [64 datoshi]
    /// 48 : OpCode.STLOC 07 [2 datoshi]
    /// 4A : OpCode.LDLOC 07 [2 datoshi]
    /// 4C : OpCode.PUSH1 [1 datoshi]
    /// 4D : OpCode.PICKITEM [64 datoshi]
    /// 4E : OpCode.DUP [2 datoshi]
    /// 4F : OpCode.LDLOC 07 [2 datoshi]
    /// 51 : OpCode.PUSH0 [1 datoshi]
    /// 52 : OpCode.PICKITEM [64 datoshi]
    /// 53 : OpCode.LDLOC3 [2 datoshi]
    /// 54 : OpCode.REVERSE3 [2 datoshi]
    /// 55 : OpCode.SETITEM [8192 datoshi]
    /// 56 : OpCode.DROP [2 datoshi]
    /// 57 : OpCode.LDLOC6 [2 datoshi]
    /// 58 : OpCode.INC [4 datoshi]
    /// 59 : OpCode.STLOC6 [2 datoshi]
    /// 5A : OpCode.LDLOC6 [2 datoshi]
    /// 5B : OpCode.LDLOC5 [2 datoshi]
    /// 5C : OpCode.JMPLT E9 [2 datoshi]
    /// 5E : OpCode.LDLOC3 [2 datoshi]
    /// 5F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFRQTEhEVwHAQcTsmAGhKcspzEHQiFmpsznVtEqIQlyYEIgZpbZ5xbJx0bGsw6j0Fcj0CaUA=
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSH5 [1 datoshi]
    /// 04 : OpCode.PUSH4 [1 datoshi]
    /// 05 : OpCode.PUSH3 [1 datoshi]
    /// 06 : OpCode.PUSH2 [1 datoshi]
    /// 07 : OpCode.PUSH1 [1 datoshi]
    /// 08 : OpCode.PUSH5 [1 datoshi]
    /// 09 : OpCode.PACK [2048 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.PUSH0 [1 datoshi]
    /// 0C : OpCode.STLOC1 [2 datoshi]
    /// 0D : OpCode.TRY 2600 [4 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.DUP [2 datoshi]
    /// 12 : OpCode.STLOC2 [2 datoshi]
    /// 13 : OpCode.SIZE [4 datoshi]
    /// 14 : OpCode.STLOC3 [2 datoshi]
    /// 15 : OpCode.PUSH0 [1 datoshi]
    /// 16 : OpCode.STLOC4 [2 datoshi]
    /// 17 : OpCode.JMP 16 [2 datoshi]
    /// 19 : OpCode.LDLOC2 [2 datoshi]
    /// 1A : OpCode.LDLOC4 [2 datoshi]
    /// 1B : OpCode.PICKITEM [64 datoshi]
    /// 1C : OpCode.STLOC5 [2 datoshi]
    /// 1D : OpCode.LDLOC5 [2 datoshi]
    /// 1E : OpCode.PUSH2 [1 datoshi]
    /// 1F : OpCode.MOD [8 datoshi]
    /// 20 : OpCode.PUSH0 [1 datoshi]
    /// 21 : OpCode.EQUAL [32 datoshi]
    /// 22 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 24 : OpCode.JMP 06 [2 datoshi]
    /// 26 : OpCode.LDLOC1 [2 datoshi]
    /// 27 : OpCode.LDLOC5 [2 datoshi]
    /// 28 : OpCode.ADD [8 datoshi]
    /// 29 : OpCode.STLOC1 [2 datoshi]
    /// 2A : OpCode.LDLOC4 [2 datoshi]
    /// 2B : OpCode.INC [4 datoshi]
    /// 2C : OpCode.STLOC4 [2 datoshi]
    /// 2D : OpCode.LDLOC4 [2 datoshi]
    /// 2E : OpCode.LDLOC3 [2 datoshi]
    /// 2F : OpCode.JMPLT EA [2 datoshi]
    /// 31 : OpCode.ENDTRY 05 [4 datoshi]
    /// 33 : OpCode.STLOC2 [2 datoshi]
    /// 34 : OpCode.ENDTRY 02 [4 datoshi]
    /// 36 : OpCode.LDLOC1 [2 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoNwAAQc/nR5ZoSpxwRWgVtSTvQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.CALLT 0000 [32768 datoshi]
    /// 09 : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.INC [4 datoshi]
    /// 11 : OpCode.STLOC0 [2 datoshi]
    /// 12 : OpCode.DROP [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.PUSH5 [1 datoshi]
    /// 15 : OpCode.LT [8 datoshi]
    /// 16 : OpCode.JMPIF EF [2 datoshi]
    /// 18 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDo")]
    public abstract void TestDo();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUANPJKcMpxEHIiHmhqzsFFc3RrNwAADAI6IItsi9soQc/nR5ZqnHJqaTDiQA==
    /// 00 : OpCode.INITSLOT 0500 [64 datoshi]
    /// 03 : OpCode.CALL F2 [512 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.STLOC0 [2 datoshi]
    /// 07 : OpCode.SIZE [4 datoshi]
    /// 08 : OpCode.STLOC1 [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.STLOC2 [2 datoshi]
    /// 0B : OpCode.JMP 1E [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC2 [2 datoshi]
    /// 0F : OpCode.PICKITEM [64 datoshi]
    /// 10 : OpCode.UNPACK [2048 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.STLOC3 [2 datoshi]
    /// 13 : OpCode.STLOC4 [2 datoshi]
    /// 14 : OpCode.LDLOC3 [2 datoshi]
    /// 15 : OpCode.CALLT 0000 [32768 datoshi]
    /// 18 : OpCode.PUSHDATA1 3A20 [8 datoshi]
    /// 1C : OpCode.CAT [2048 datoshi]
    /// 1D : OpCode.LDLOC4 [2 datoshi]
    /// 1E : OpCode.CAT [2048 datoshi]
    /// 1F : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 21 : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 26 : OpCode.LDLOC2 [2 datoshi]
    /// 27 : OpCode.INC [4 datoshi]
    /// 28 : OpCode.STLOC2 [2 datoshi]
    /// 29 : OpCode.LDLOC2 [2 datoshi]
    /// 2A : OpCode.LDLOC1 [2 datoshi]
    /// 2B : OpCode.JMPLT E2 [2 datoshi]
    /// 2D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAE0Gb9mfOExGIThBR0FASwMFFQd8wuJpwaHEiEWlB81S/HXJq2yhBz+dHlmlBnAjtnCTrQA==
    /// 00 : OpCode.INITSLOT 0300 [64 datoshi]
    /// 03 : OpCode.PUSH3 [1 datoshi]
    /// 04 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 09 : OpCode.PUSH3 [1 datoshi]
    /// 0A : OpCode.PUSH1 [1 datoshi]
    /// 0B : OpCode.NEWBUFFER [256 datoshi]
    /// 0C : OpCode.TUCK [2 datoshi]
    /// 0D : OpCode.PUSH0 [1 datoshi]
    /// 0E : OpCode.ROT [2 datoshi]
    /// 0F : OpCode.SETITEM [8192 datoshi]
    /// 10 : OpCode.SWAP [2 datoshi]
    /// 11 : OpCode.PUSH2 [1 datoshi]
    /// 12 : OpCode.PACK [2048 datoshi]
    /// 13 : OpCode.UNPACK [2048 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.SYSCALL DF30B89A 'System.Storage.Find' [32768 datoshi]
    /// 1A : OpCode.STLOC0 [2 datoshi]
    /// 1B : OpCode.LDLOC0 [2 datoshi]
    /// 1C : OpCode.STLOC1 [2 datoshi]
    /// 1D : OpCode.JMP 11 [2 datoshi]
    /// 1F : OpCode.LDLOC1 [2 datoshi]
    /// 20 : OpCode.SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// 25 : OpCode.STLOC2 [2 datoshi]
    /// 26 : OpCode.LDLOC2 [2 datoshi]
    /// 27 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 29 : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 2E : OpCode.LDLOC1 [2 datoshi]
    /// 2F : OpCode.SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// 34 : OpCode.JMPIF EB [2 datoshi]
    /// 36 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoFbUmEmg3AABBz+dHlmhKnHBFIu1A
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.PUSH5 [1 datoshi]
    /// 07 : OpCode.LT [8 datoshi]
    /// 08 : OpCode.JMPIFNOT 12 [2 datoshi]
    /// 0A : OpCode.LDLOC0 [2 datoshi]
    /// 0B : OpCode.CALLT 0000 [32768 datoshi]
    /// 0E : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.DUP [2 datoshi]
    /// 15 : OpCode.INC [4 datoshi]
    /// 16 : OpCode.STLOC0 [2 datoshi]
    /// 17 : OpCode.DROP [2 datoshi]
    /// 18 : OpCode.JMP ED [2 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testWhile")]
    public abstract void TestWhile();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAASwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUA=
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 19 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 2F : OpCode.PUSH2 [1 datoshi]
    /// 30 : OpCode.PACK [2048 datoshi]
    /// 31 : OpCode.STLOC0 [2 datoshi]
    /// 32 : OpCode.NEWARRAY0 [16 datoshi]
    /// 33 : OpCode.STLOC1 [2 datoshi]
    /// 34 : OpCode.LDLOC0 [2 datoshi]
    /// 35 : OpCode.DUP [2 datoshi]
    /// 36 : OpCode.STLOC2 [2 datoshi]
    /// 37 : OpCode.SIZE [4 datoshi]
    /// 38 : OpCode.STLOC3 [2 datoshi]
    /// 39 : OpCode.PUSH0 [1 datoshi]
    /// 3A : OpCode.STLOC4 [2 datoshi]
    /// 3B : OpCode.JMP 0C [2 datoshi]
    /// 3D : OpCode.LDLOC2 [2 datoshi]
    /// 3E : OpCode.LDLOC4 [2 datoshi]
    /// 3F : OpCode.PICKITEM [64 datoshi]
    /// 40 : OpCode.STLOC5 [2 datoshi]
    /// 41 : OpCode.LDLOC1 [2 datoshi]
    /// 42 : OpCode.LDLOC5 [2 datoshi]
    /// 43 : OpCode.APPEND [8192 datoshi]
    /// 44 : OpCode.LDLOC4 [2 datoshi]
    /// 45 : OpCode.INC [4 datoshi]
    /// 46 : OpCode.STLOC4 [2 datoshi]
    /// 47 : OpCode.LDLOC4 [2 datoshi]
    /// 48 : OpCode.LDLOC3 [2 datoshi]
    /// 49 : OpCode.JMPLT F4 [2 datoshi]
    /// 4B : OpCode.LDLOC1 [2 datoshi]
    /// 4C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAASwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUA=
    /// 00 : OpCode.INITSLOT 0600 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000 [8 datoshi]
    /// 25 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000 [8 datoshi]
    /// 47 : OpCode.PUSH2 [1 datoshi]
    /// 48 : OpCode.PACK [2048 datoshi]
    /// 49 : OpCode.STLOC0 [2 datoshi]
    /// 4A : OpCode.NEWARRAY0 [16 datoshi]
    /// 4B : OpCode.STLOC1 [2 datoshi]
    /// 4C : OpCode.LDLOC0 [2 datoshi]
    /// 4D : OpCode.DUP [2 datoshi]
    /// 4E : OpCode.STLOC2 [2 datoshi]
    /// 4F : OpCode.SIZE [4 datoshi]
    /// 50 : OpCode.STLOC3 [2 datoshi]
    /// 51 : OpCode.PUSH0 [1 datoshi]
    /// 52 : OpCode.STLOC4 [2 datoshi]
    /// 53 : OpCode.JMP 0C [2 datoshi]
    /// 55 : OpCode.LDLOC2 [2 datoshi]
    /// 56 : OpCode.LDLOC4 [2 datoshi]
    /// 57 : OpCode.PICKITEM [64 datoshi]
    /// 58 : OpCode.STLOC5 [2 datoshi]
    /// 59 : OpCode.LDLOC1 [2 datoshi]
    /// 5A : OpCode.LDLOC5 [2 datoshi]
    /// 5B : OpCode.APPEND [8192 datoshi]
    /// 5C : OpCode.LDLOC4 [2 datoshi]
    /// 5D : OpCode.INC [4 datoshi]
    /// 5E : OpCode.STLOC4 [2 datoshi]
    /// 5F : OpCode.LDLOC4 [2 datoshi]
    /// 60 : OpCode.LDLOC3 [2 datoshi]
    /// 61 : OpCode.JMPLT F4 [2 datoshi]
    /// 63 : OpCode.LDLOC1 [2 datoshi]
    /// 64 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();

    #endregion
}
