using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stored(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stored"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""withoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":623,""safe"":true},{""name"":""putWithoutConstructor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":633,""safe"":false},{""name"":""getWithoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":643,""safe"":true},{""name"":""withKey"",""parameters"":[],""returntype"":""Integer"",""offset"":94,""safe"":true},{""name"":""putWithKey"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":132,""safe"":false},{""name"":""getWithKey"",""parameters"":[],""returntype"":""Integer"",""offset"":141,""safe"":true},{""name"":""withString"",""parameters"":[],""returntype"":""Integer"",""offset"":144,""safe"":true},{""name"":""putWithString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":192,""safe"":false},{""name"":""getWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":201,""safe"":true},{""name"":""setPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":248,""safe"":false},{""name"":""getPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":290,""safe"":true},{""name"":""setNonStaticPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":653,""safe"":false},{""name"":""getNonStaticPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":663,""safe"":true},{""name"":""multiWithStringStstic"",""parameters"":[],""returntype"":""Integer"",""offset"":409,""safe"":true},{""name"":""multiWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":673,""safe"":true},{""name"":""testStaticMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":534,""safe"":false},{""name"":""testStaticMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":567,""safe"":false},{""name"":""testMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":683,""safe"":false},{""name"":""testMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":690,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP26AlcAAQwSV2l0aG91dENvbnN0cnVjdG9yQfa0a+JBkl3oMUrYJgZFeBDOQFcAAnkMEldpdGhvdXRDb25zdHJ1Y3RvckGb9mfOQeY/GIRAVwACeUp4NNdFQFcAAXg0pUAMAQFB9rRr4kGSXegxStgmBEUQQFcAAXgMAQFBm/ZnzkHmPxiEQFcAAXhKNOlFQDTRQAwGdGVzdE1lQfa0a+JBkl3oMUrYJgRFEEBXAAF4DAZ0ZXN0TWVBm/ZnzkHmPxiEQFcAAXhKNORFQDTHQAwZUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckH2tGviQZJd6DFK2CYERRBAVwABeAwZUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckGb9mfOQeY/GIRANKpAVwABDCJOb25TdGF0aWNQcml2YXRlR2V0dGVyUHVibGljU2V0dGVyQfa0a+JBkl3oMUrYJgZFeBHOQFcAAnkMIk5vblN0YXRpY1ByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJBm/ZnzkHmPxiEQFcAAXg0j0AMD3Rlc3RTdGF0aWNNdWx0aUH2tGviQZJd6DFK2CYERRBAVwABeAwPdGVzdFN0YXRpY011bHRpQZv2Z85B5j8YhEBXAAEMCXRlc3RNdWx0aUH2tGviQZJd6DFK2CYGRXgSzkBXAAJ5DAl0ZXN0TXVsdGlBm/ZnzkHmPxiEQDSDSpw0oUU1fP///0qcNJdFNXL///9KnDSNRTVo////QDVi////QFcAAXhKNJlOnFA0tUV4SjSPTpxQNKtFeEo0hU6cUDShRXg1fP///0BXAAF4NXL///9AEBAQE8AjjP3//xAQEBPAI8/9//8QEBATwCPP/f//EBAQE8Ajzf7//xAQEBPAI/b+//8QEBATwCM1////EBAQE8AijRAQEBPAIq5ANlAYuw=="));

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? NonStaticPrivateGetterPublicSetter { [DisplayName("getNonStaticPrivateGetterPublicSetter")] get; [DisplayName("setNonStaticPrivateGetterPublicSetter")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? PrivateGetterPublicSetter { [DisplayName("getPrivateGetterPublicSetter")] get; [DisplayName("setPrivateGetterPublicSetter")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? MultiWithString { [DisplayName("multiWithString")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? MultiWithStringStstic { [DisplayName("multiWithStringStstic")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? WithKey { [DisplayName("withKey")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? WithoutConstructor { [DisplayName("withoutConstructor")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? WithString { [DisplayName("withString")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NNFA
    /// 00 : CALL D1 [512 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithKey")]
    public abstract BigInteger? GetWithKey();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDSlQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL A5 [512 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithoutConstructor")]
    public abstract BigInteger? GetWithoutConstructor();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NMdA
    /// 00 : CALL C7 [512 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithString")]
    public abstract BigInteger? GetWithString();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo06UVA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : CALL E9 [512 datoshi]
    /// 07 : DROP [2 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithKey")]
    public abstract void PutWithKey(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeUp4NNdFQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : CALL D7 [512 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithoutConstructor")]
    public abstract void PutWithoutConstructor(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo05EVA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : CALL E4 [512 datoshi]
    /// 07 : DROP [2 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithString")]
    public abstract void PutWithString(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDVy////QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL_L 72FFFFFF [512 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiGet")]
    public abstract BigInteger? TestMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo0mU6cUDS1RXhKNI9OnFA0q0V4SjSFTpxQNKFFeDV8////QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : CALL 99 [512 datoshi]
    /// 07 : TUCK [2 datoshi]
    /// 08 : INC [4 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : CALL B5 [512 datoshi]
    /// 0C : DROP [2 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : CALL 8F [512 datoshi]
    /// 11 : TUCK [2 datoshi]
    /// 12 : INC [4 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : CALL AB [512 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : LDARG0 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : CALL 85 [512 datoshi]
    /// 1B : TUCK [2 datoshi]
    /// 1C : INC [4 datoshi]
    /// 1D : SWAP [2 datoshi]
    /// 1E : CALL A1 [512 datoshi]
    /// 20 : DROP [2 datoshi]
    /// 21 : LDARG0 [2 datoshi]
    /// 22 : CALL_L 7CFFFFFF [512 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiSet")]
    public abstract BigInteger? TestMultiSet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NWL///9A
    /// 00 : CALL_L 62FFFFFF [512 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiGet")]
    public abstract BigInteger? TestStaticMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NINKnDShRTV8////Spw0l0U1cv///0qcNI1FNWj///9A
    /// 00 : CALL 83 [512 datoshi]
    /// 02 : DUP [2 datoshi]
    /// 03 : INC [4 datoshi]
    /// 04 : CALL A1 [512 datoshi]
    /// 06 : DROP [2 datoshi]
    /// 07 : CALL_L 7CFFFFFF [512 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : INC [4 datoshi]
    /// 0E : CALL 97 [512 datoshi]
    /// 10 : DROP [2 datoshi]
    /// 11 : CALL_L 72FFFFFF [512 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : INC [4 datoshi]
    /// 18 : CALL 8D [512 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : CALL_L 68FFFFFF [512 datoshi]
    /// 20 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiSet")]
    public abstract BigInteger? TestStaticMultiSet();

    #endregion
}
