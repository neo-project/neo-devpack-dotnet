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
    /// CALL D1 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithKey")]
    public abstract BigInteger? GetWithKey();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDSlQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL A5 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithoutConstructor")]
    public abstract BigInteger? GetWithoutConstructor();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NMdA
    /// CALL C7 [512 datoshi]
    /// RET [0 datoshi]
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
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL E9 [512 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithKey")]
    public abstract void PutWithKey(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeUp4NNdFQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL D7 [512 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithoutConstructor")]
    public abstract void PutWithoutConstructor(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo05EVA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL E4 [512 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithString")]
    public abstract void PutWithString(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDVy////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 72FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiGet")]
    public abstract BigInteger? TestMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo0mU6cUDS1RXhKNI9OnFA0q0V4SjSFTpxQNKFFeDV8////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL 99 [512 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL B5 [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL 8F [512 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL AB [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL 85 [512 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL A1 [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 7CFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiSet")]
    public abstract BigInteger? TestMultiSet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NWL///9A
    /// CALL_L 62FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiGet")]
    public abstract BigInteger? TestStaticMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NINKnDShRTV8////Spw0l0U1cv///0qcNI1FNWj///9A
    /// CALL 83 [512 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// CALL A1 [512 datoshi]
    /// DROP [2 datoshi]
    /// CALL_L 7CFFFFFF [512 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// CALL 97 [512 datoshi]
    /// DROP [2 datoshi]
    /// CALL_L 72FFFFFF [512 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// CALL 8D [512 datoshi]
    /// DROP [2 datoshi]
    /// CALL_L 68FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiSet")]
    public abstract BigInteger? TestStaticMultiSet();

    #endregion
}
