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
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHINT64 000064A7B3B6E00D
    /// 0C : OpCode.PUSHINT32 00CA9A3B
    /// 11 : OpCode.PUSHINT32 40420F00
    /// 16 : OpCode.PUSHINT16 1027
    /// 19 : OpCode.PUSH4
    /// 1A : OpCode.PACK
    /// 1B : OpCode.STLOC0
    /// 1C : OpCode.NEWARRAY0
    /// 1D : OpCode.STLOC1
    /// 1E : OpCode.LDLOC0
    /// 1F : OpCode.DUP
    /// 20 : OpCode.STLOC2
    /// 21 : OpCode.SIZE
    /// 22 : OpCode.STLOC3
    /// 23 : OpCode.PUSH0
    /// 24 : OpCode.STLOC4
    /// 25 : OpCode.JMP 0C
    /// 27 : OpCode.LDLOC2
    /// 28 : OpCode.LDLOC4
    /// 29 : OpCode.PICKITEM
    /// 2A : OpCode.STLOC5
    /// 2B : OpCode.LDLOC1
    /// 2C : OpCode.LDLOC5
    /// 2D : OpCode.APPEND
    /// 2E : OpCode.LDLOC4
    /// 2F : OpCode.INC
    /// 30 : OpCode.STLOC4
    /// 31 : OpCode.LDLOC4
    /// 32 : OpCode.LDLOC3
    /// 33 : OpCode.JMPLT F4
    /// 35 : OpCode.LDLOC1
    /// 36 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAEKEdswcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 010A11
    /// 08 : OpCode.CONVERT 30
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.NEWARRAY0
    /// 0C : OpCode.STLOC1
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.STLOC2
    /// 10 : OpCode.SIZE
    /// 11 : OpCode.STLOC3
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.STLOC4
    /// 14 : OpCode.JMP 0C
    /// 16 : OpCode.LDLOC2
    /// 17 : OpCode.LDLOC4
    /// 18 : OpCode.PICKITEM
    /// 19 : OpCode.STLOC5
    /// 1A : OpCode.LDLOC1
    /// 1B : OpCode.LDLOC5
    /// 1C : OpCode.APPEND
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.INC
    /// 1F : OpCode.STLOC4
    /// 20 : OpCode.LDLOC4
    /// 21 : OpCode.LDLOC3
    /// 22 : OpCode.JMPLT F4
    /// 24 : OpCode.LDLOC1
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADHBoykA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHDATA1
    /// 05 : OpCode.STLOC0
    /// 06 : OpCode.LDLOC0
    /// 07 : OpCode.SIZE
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAwMaGlqDGRlZgxhYmMVwHAMcWhKcspzEHQiD2psznVpbYvbKHFsnHRsazDxaUA=
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1
    /// 05 : OpCode.PUSHDATA1
    /// 07 : OpCode.PUSHDATA1 68696A
    /// 0C : OpCode.PUSHDATA1 646566
    /// 11 : OpCode.PUSHDATA1 616263
    /// 16 : OpCode.PUSH5
    /// 17 : OpCode.PACK
    /// 18 : OpCode.STLOC0
    /// 19 : OpCode.PUSHDATA1
    /// 1B : OpCode.STLOC1
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.STLOC2
    /// 1F : OpCode.SIZE
    /// 20 : OpCode.STLOC3
    /// 21 : OpCode.PUSH0
    /// 22 : OpCode.STLOC4
    /// 23 : OpCode.JMP 0F
    /// 25 : OpCode.LDLOC2
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.PICKITEM
    /// 28 : OpCode.STLOC5
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.LDLOC5
    /// 2B : OpCode.CAT
    /// 2C : OpCode.CONVERT 28
    /// 2E : OpCode.STLOC1
    /// 2F : OpCode.LDLOC4
    /// 30 : OpCode.INC
    /// 31 : OpCode.STLOC4
    /// 32 : OpCode.LDLOC4
    /// 33 : OpCode.LDLOC3
    /// 34 : OpCode.JMPLT F1
    /// 36 : OpCode.LDLOC1
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqdsoStgkCUrKACEoAzoMAkcA2y6Q2fAsT5/IYqusqScl+VtP3cyNf/pThpPs9GOp2yhK2CQJSsoAISgDOhLAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9
    /// 26 : OpCode.CONVERT 28
    /// 28 : OpCode.DUP
    /// 29 : OpCode.ISNULL
    /// 2A : OpCode.JMPIF 09
    /// 2C : OpCode.DUP
    /// 2D : OpCode.SIZE
    /// 2E : OpCode.PUSHINT8 21
    /// 30 : OpCode.JMPEQ 03
    /// 32 : OpCode.THROW
    /// 33 : OpCode.PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9
    /// 56 : OpCode.CONVERT 28
    /// 58 : OpCode.DUP
    /// 59 : OpCode.ISNULL
    /// 5A : OpCode.JMPIF 09
    /// 5C : OpCode.DUP
    /// 5D : OpCode.SIZE
    /// 5E : OpCode.PUSHINT8 21
    /// 60 : OpCode.JMPEQ 03
    /// 62 : OpCode.THROW
    /// 63 : OpCode.PUSH2
    /// 64 : OpCode.PACK
    /// 65 : OpCode.STLOC0
    /// 66 : OpCode.NEWARRAY0
    /// 67 : OpCode.STLOC1
    /// 68 : OpCode.LDLOC0
    /// 69 : OpCode.DUP
    /// 6A : OpCode.STLOC2
    /// 6B : OpCode.SIZE
    /// 6C : OpCode.STLOC3
    /// 6D : OpCode.PUSH0
    /// 6E : OpCode.STLOC4
    /// 6F : OpCode.JMP 0C
    /// 71 : OpCode.LDLOC2
    /// 72 : OpCode.LDLOC4
    /// 73 : OpCode.PICKITEM
    /// 74 : OpCode.STLOC5
    /// 75 : OpCode.LDLOC1
    /// 76 : OpCode.LDLOC5
    /// 77 : OpCode.APPEND
    /// 78 : OpCode.LDLOC4
    /// 79 : OpCode.INC
    /// 7A : OpCode.STLOC4
    /// 7B : OpCode.LDLOC4
    /// 7C : OpCode.LDLOC3
    /// 7D : OpCode.JMPLT F4
    /// 7F : OpCode.LDLOC1
    /// 80 : OpCode.RET
    /// </remarks>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFBMSERTAcBBxaEpyynMQdCINamzOdWltnnFsnHRsazDzaUA=
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSH4
    /// 04 : OpCode.PUSH3
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PACK
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.STLOC2
    /// 0F : OpCode.SIZE
    /// 10 : OpCode.STLOC3
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.STLOC4
    /// 13 : OpCode.JMP 0D
    /// 15 : OpCode.LDLOC2
    /// 16 : OpCode.LDLOC4
    /// 17 : OpCode.PICKITEM
    /// 18 : OpCode.STLOC5
    /// 19 : OpCode.LDLOC1
    /// 1A : OpCode.LDLOC5
    /// 1B : OpCode.ADD
    /// 1C : OpCode.STLOC1
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.INC
    /// 1F : OpCode.STLOC4
    /// 20 : OpCode.LDLOC4
    /// 21 : OpCode.LDLOC3
    /// 22 : OpCode.JMPLT F3
    /// 24 : OpCode.LDLOC1
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBFBMSERTAcBBxOycAaEpyynMQdCIXamzOdXhKnYAQtiYEIg1pbZ5xbJx0bGsw6T0Fcj0CaUA=
    /// 00 : OpCode.INITSLOT 0601
    /// 03 : OpCode.PUSH4
    /// 04 : OpCode.PUSH3
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PACK
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.TRY 2700
    /// 0F : OpCode.LDLOC0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.STLOC2
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.STLOC3
    /// 14 : OpCode.PUSH0
    /// 15 : OpCode.STLOC4
    /// 16 : OpCode.JMP 17
    /// 18 : OpCode.LDLOC2
    /// 19 : OpCode.LDLOC4
    /// 1A : OpCode.PICKITEM
    /// 1B : OpCode.STLOC5
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.DEC
    /// 1F : OpCode.STARG0
    /// 20 : OpCode.PUSH0
    /// 21 : OpCode.LE
    /// 22 : OpCode.JMPIFNOT 04
    /// 24 : OpCode.JMP 0D
    /// 26 : OpCode.LDLOC1
    /// 27 : OpCode.LDLOC5
    /// 28 : OpCode.ADD
    /// 29 : OpCode.STLOC1
    /// 2A : OpCode.LDLOC4
    /// 2B : OpCode.INC
    /// 2C : OpCode.STLOC4
    /// 2D : OpCode.LDLOC4
    /// 2E : OpCode.LDLOC3
    /// 2F : OpCode.JMPLT E9
    /// 31 : OpCode.ENDTRY 05
    /// 33 : OpCode.STLOC2
    /// 34 : OpCode.ENDTRY 02
    /// 36 : OpCode.LDLOC1
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAFBMSERTAcBBxEHIiDWloas6ecWpKnHJFamjKtSTxaUA=
    /// 00 : OpCode.INITSLOT 0300
    /// 03 : OpCode.PUSH4
    /// 04 : OpCode.PUSH3
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PACK
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.PUSH0
    /// 0D : OpCode.STLOC2
    /// 0E : OpCode.JMP 0D
    /// 10 : OpCode.LDLOC1
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.LDLOC2
    /// 13 : OpCode.PICKITEM
    /// 14 : OpCode.ADD
    /// 15 : OpCode.STLOC1
    /// 16 : OpCode.LDLOC2
    /// 17 : OpCode.DUP
    /// 18 : OpCode.INC
    /// 19 : OpCode.STLOC2
    /// 1A : OpCode.DROP
    /// 1B : OpCode.LDLOC2
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.SIZE
    /// 1E : OpCode.LT
    /// 1F : OpCode.JMPIF F1
    /// 21 : OpCode.LDLOC1
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAHsMdGVzdAwBAtswE8BwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHINT8 7B
    /// 05 : OpCode.PUSHDATA1 74657374
    /// 0B : OpCode.PUSHDATA1 0102
    /// 0F : OpCode.CONVERT 30
    /// 11 : OpCode.PUSH3
    /// 12 : OpCode.PACK
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.NEWARRAY0
    /// 15 : OpCode.STLOC1
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.DUP
    /// 18 : OpCode.STLOC2
    /// 19 : OpCode.SIZE
    /// 1A : OpCode.STLOC3
    /// 1B : OpCode.PUSH0
    /// 1C : OpCode.STLOC4
    /// 1D : OpCode.JMP 0C
    /// 1F : OpCode.LDLOC2
    /// 20 : OpCode.LDLOC4
    /// 21 : OpCode.PICKITEM
    /// 22 : OpCode.STLOC5
    /// 23 : OpCode.LDLOC1
    /// 24 : OpCode.LDLOC5
    /// 25 : OpCode.APPEND
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.INC
    /// 28 : OpCode.STLOC4
    /// 29 : OpCode.LDLOC4
    /// 2A : OpCode.LDLOC3
    /// 2B : OpCode.JMPLT F4
    /// 2D : OpCode.LDLOC1
    /// 2E : OpCode.RET
    /// </remarks>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADGhpagxkZWYMYWJjE8BwDHFoSnLKcxB0Ig9qbM51aW2L2yhxbJx0bGsw8WlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 68696A
    /// 08 : OpCode.PUSHDATA1 646566
    /// 0D : OpCode.PUSHDATA1 616263
    /// 12 : OpCode.PUSH3
    /// 13 : OpCode.PACK
    /// 14 : OpCode.STLOC0
    /// 15 : OpCode.PUSHDATA1
    /// 17 : OpCode.STLOC1
    /// 18 : OpCode.LDLOC0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.STLOC2
    /// 1B : OpCode.SIZE
    /// 1C : OpCode.STLOC3
    /// 1D : OpCode.PUSH0
    /// 1E : OpCode.STLOC4
    /// 1F : OpCode.JMP 0F
    /// 21 : OpCode.LDLOC2
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.PICKITEM
    /// 24 : OpCode.STLOC5
    /// 25 : OpCode.LDLOC1
    /// 26 : OpCode.LDLOC5
    /// 27 : OpCode.CAT
    /// 28 : OpCode.CONVERT 28
    /// 2A : OpCode.STLOC1
    /// 2B : OpCode.LDLOC4
    /// 2C : OpCode.INC
    /// 2D : OpCode.STLOC4
    /// 2E : OpCode.LDLOC4
    /// 2F : OpCode.LDLOC3
    /// 30 : OpCode.JMPLT F1
    /// 32 : OpCode.LDLOC1
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwgAEAsSv3AMdGVzdDFKaBBR0EURSmgRUdBFEAsSv3EMdGVzdDJKaRBR0EUSSmkRUdBFaWgSwHLIc2pKdMp1EHYiF2xuzncHbwcRzkpvBxDOa1PQRW6cdm5tMOlrQA==
    /// 00 : OpCode.INITSLOT 0800
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PACKSTRUCT
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.PUSHDATA1 7465737431
    /// 0F : OpCode.DUP
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.ROT
    /// 13 : OpCode.SETITEM
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSH1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.LDLOC0
    /// 18 : OpCode.PUSH1
    /// 19 : OpCode.ROT
    /// 1A : OpCode.SETITEM
    /// 1B : OpCode.DROP
    /// 1C : OpCode.PUSH0
    /// 1D : OpCode.PUSHNULL
    /// 1E : OpCode.PUSH2
    /// 1F : OpCode.PACKSTRUCT
    /// 20 : OpCode.STLOC1
    /// 21 : OpCode.PUSHDATA1 7465737432
    /// 28 : OpCode.DUP
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.PUSH0
    /// 2B : OpCode.ROT
    /// 2C : OpCode.SETITEM
    /// 2D : OpCode.DROP
    /// 2E : OpCode.PUSH2
    /// 2F : OpCode.DUP
    /// 30 : OpCode.LDLOC1
    /// 31 : OpCode.PUSH1
    /// 32 : OpCode.ROT
    /// 33 : OpCode.SETITEM
    /// 34 : OpCode.DROP
    /// 35 : OpCode.LDLOC1
    /// 36 : OpCode.LDLOC0
    /// 37 : OpCode.PUSH2
    /// 38 : OpCode.PACK
    /// 39 : OpCode.STLOC2
    /// 3A : OpCode.NEWMAP
    /// 3B : OpCode.STLOC3
    /// 3C : OpCode.LDLOC2
    /// 3D : OpCode.DUP
    /// 3E : OpCode.STLOC4
    /// 3F : OpCode.SIZE
    /// 40 : OpCode.STLOC5
    /// 41 : OpCode.PUSH0
    /// 42 : OpCode.STLOC6
    /// 43 : OpCode.JMP 17
    /// 45 : OpCode.LDLOC4
    /// 46 : OpCode.LDLOC6
    /// 47 : OpCode.PICKITEM
    /// 48 : OpCode.STLOC 07
    /// 4A : OpCode.LDLOC 07
    /// 4C : OpCode.PUSH1
    /// 4D : OpCode.PICKITEM
    /// 4E : OpCode.DUP
    /// 4F : OpCode.LDLOC 07
    /// 51 : OpCode.PUSH0
    /// 52 : OpCode.PICKITEM
    /// 53 : OpCode.LDLOC3
    /// 54 : OpCode.REVERSE3
    /// 55 : OpCode.SETITEM
    /// 56 : OpCode.DROP
    /// 57 : OpCode.LDLOC6
    /// 58 : OpCode.INC
    /// 59 : OpCode.STLOC6
    /// 5A : OpCode.LDLOC6
    /// 5B : OpCode.LDLOC5
    /// 5C : OpCode.JMPLT E9
    /// 5E : OpCode.LDLOC3
    /// 5F : OpCode.RET
    /// </remarks>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFRQTEhEVwHAQcTsmAGhKcspzEHQiFmpsznVtEqIQlyYEIgZpbZ5xbJx0bGsw6j0Fcj0CaUA=
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSH5
    /// 04 : OpCode.PUSH4
    /// 05 : OpCode.PUSH3
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PUSH1
    /// 08 : OpCode.PUSH5
    /// 09 : OpCode.PACK
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.STLOC1
    /// 0D : OpCode.TRY 2600
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.STLOC2
    /// 13 : OpCode.SIZE
    /// 14 : OpCode.STLOC3
    /// 15 : OpCode.PUSH0
    /// 16 : OpCode.STLOC4
    /// 17 : OpCode.JMP 16
    /// 19 : OpCode.LDLOC2
    /// 1A : OpCode.LDLOC4
    /// 1B : OpCode.PICKITEM
    /// 1C : OpCode.STLOC5
    /// 1D : OpCode.LDLOC5
    /// 1E : OpCode.PUSH2
    /// 1F : OpCode.MOD
    /// 20 : OpCode.PUSH0
    /// 21 : OpCode.EQUAL
    /// 22 : OpCode.JMPIFNOT 04
    /// 24 : OpCode.JMP 06
    /// 26 : OpCode.LDLOC1
    /// 27 : OpCode.LDLOC5
    /// 28 : OpCode.ADD
    /// 29 : OpCode.STLOC1
    /// 2A : OpCode.LDLOC4
    /// 2B : OpCode.INC
    /// 2C : OpCode.STLOC4
    /// 2D : OpCode.LDLOC4
    /// 2E : OpCode.LDLOC3
    /// 2F : OpCode.JMPLT EA
    /// 31 : OpCode.ENDTRY 05
    /// 33 : OpCode.STLOC2
    /// 34 : OpCode.ENDTRY 02
    /// 36 : OpCode.LDLOC1
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoNwAAQc/nR5ZoSpxwRWgVtSTvQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.CALLT 0000
    /// 09 : OpCode.SYSCALL CFE74796
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.DUP
    /// 10 : OpCode.INC
    /// 11 : OpCode.STLOC0
    /// 12 : OpCode.DROP
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH5
    /// 15 : OpCode.LT
    /// 16 : OpCode.JMPIF EF
    /// 18 : OpCode.RET
    /// </remarks>
    [DisplayName("testDo")]
    public abstract void TestDo();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUANPJKcMpxEHIiHmhqzsFFc3RrNwAADDogi2yL2yhBz+dHlmqccmppMOJA
    /// 00 : OpCode.INITSLOT 0500
    /// 03 : OpCode.CALL F2
    /// 05 : OpCode.DUP
    /// 06 : OpCode.STLOC0
    /// 07 : OpCode.SIZE
    /// 08 : OpCode.STLOC1
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.STLOC2
    /// 0B : OpCode.JMP 1E
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.LDLOC2
    /// 0F : OpCode.PICKITEM
    /// 10 : OpCode.UNPACK
    /// 11 : OpCode.DROP
    /// 12 : OpCode.STLOC3
    /// 13 : OpCode.STLOC4
    /// 14 : OpCode.LDLOC3
    /// 15 : OpCode.CALLT 0000
    /// 18 : OpCode.PUSHDATA1 3A20
    /// 1C : OpCode.CAT
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.CAT
    /// 1F : OpCode.CONVERT 28
    /// 21 : OpCode.SYSCALL CFE74796
    /// 26 : OpCode.LDLOC2
    /// 27 : OpCode.INC
    /// 28 : OpCode.STLOC2
    /// 29 : OpCode.LDLOC2
    /// 2A : OpCode.LDLOC1
    /// 2B : OpCode.JMPLT E2
    /// 2D : OpCode.RET
    /// </remarks>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAE0Gb9mfOExGIThBR0FASwMFFQd8wuJpwaHEiEWlB81S/HXJq2yhBz+dHlmlBnAjtnCTrQA==
    /// 00 : OpCode.INITSLOT 0300
    /// 03 : OpCode.PUSH3
    /// 04 : OpCode.SYSCALL 9BF667CE
    /// 09 : OpCode.PUSH3
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.NEWBUFFER
    /// 0C : OpCode.TUCK
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.ROT
    /// 0F : OpCode.SETITEM
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.PUSH2
    /// 12 : OpCode.PACK
    /// 13 : OpCode.UNPACK
    /// 14 : OpCode.DROP
    /// 15 : OpCode.SYSCALL DF30B89A
    /// 1A : OpCode.STLOC0
    /// 1B : OpCode.LDLOC0
    /// 1C : OpCode.STLOC1
    /// 1D : OpCode.JMP 11
    /// 1F : OpCode.LDLOC1
    /// 20 : OpCode.SYSCALL F354BF1D
    /// 25 : OpCode.STLOC2
    /// 26 : OpCode.LDLOC2
    /// 27 : OpCode.CONVERT 28
    /// 29 : OpCode.SYSCALL CFE74796
    /// 2E : OpCode.LDLOC1
    /// 2F : OpCode.SYSCALL 9C08ED9C
    /// 34 : OpCode.JMPIF EB
    /// 36 : OpCode.RET
    /// </remarks>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoFbUmEmg3AABBz+dHlmhKnHBFIu1A
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH5
    /// 07 : OpCode.LT
    /// 08 : OpCode.JMPIFNOT 12
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.CALLT 0000
    /// 0E : OpCode.SYSCALL CFE74796
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.DUP
    /// 15 : OpCode.INC
    /// 16 : OpCode.STLOC0
    /// 17 : OpCode.DROP
    /// 18 : OpCode.JMP ED
    /// 1A : OpCode.RET
    /// </remarks>
    [DisplayName("testWhile")]
    public abstract void TestWhile();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 19 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 2F : OpCode.PUSH2
    /// 30 : OpCode.PACK
    /// 31 : OpCode.STLOC0
    /// 32 : OpCode.NEWARRAY0
    /// 33 : OpCode.STLOC1
    /// 34 : OpCode.LDLOC0
    /// 35 : OpCode.DUP
    /// 36 : OpCode.STLOC2
    /// 37 : OpCode.SIZE
    /// 38 : OpCode.STLOC3
    /// 39 : OpCode.PUSH0
    /// 3A : OpCode.STLOC4
    /// 3B : OpCode.JMP 0C
    /// 3D : OpCode.LDLOC2
    /// 3E : OpCode.LDLOC4
    /// 3F : OpCode.PICKITEM
    /// 40 : OpCode.STLOC5
    /// 41 : OpCode.LDLOC1
    /// 42 : OpCode.LDLOC5
    /// 43 : OpCode.APPEND
    /// 44 : OpCode.LDLOC4
    /// 45 : OpCode.INC
    /// 46 : OpCode.STLOC4
    /// 47 : OpCode.LDLOC4
    /// 48 : OpCode.LDLOC3
    /// 49 : OpCode.JMPLT F4
    /// 4B : OpCode.LDLOC1
    /// 4C : OpCode.RET
    /// </remarks>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 25 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 47 : OpCode.PUSH2
    /// 48 : OpCode.PACK
    /// 49 : OpCode.STLOC0
    /// 4A : OpCode.NEWARRAY0
    /// 4B : OpCode.STLOC1
    /// 4C : OpCode.LDLOC0
    /// 4D : OpCode.DUP
    /// 4E : OpCode.STLOC2
    /// 4F : OpCode.SIZE
    /// 50 : OpCode.STLOC3
    /// 51 : OpCode.PUSH0
    /// 52 : OpCode.STLOC4
    /// 53 : OpCode.JMP 0C
    /// 55 : OpCode.LDLOC2
    /// 56 : OpCode.LDLOC4
    /// 57 : OpCode.PICKITEM
    /// 58 : OpCode.STLOC5
    /// 59 : OpCode.LDLOC1
    /// 5A : OpCode.LDLOC5
    /// 5B : OpCode.APPEND
    /// 5C : OpCode.LDLOC4
    /// 5D : OpCode.INC
    /// 5E : OpCode.STLOC4
    /// 5F : OpCode.LDLOC4
    /// 60 : OpCode.LDLOC3
    /// 61 : OpCode.JMPLT F4
    /// 63 : OpCode.LDLOC1
    /// 64 : OpCode.RET
    /// </remarks>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();

    #endregion
}
