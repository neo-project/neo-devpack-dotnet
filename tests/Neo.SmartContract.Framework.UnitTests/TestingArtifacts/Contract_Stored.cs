using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stored(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stored"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""withoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":644,""safe"":true},{""name"":""putWithoutConstructor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":654,""safe"":false},{""name"":""getWithoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":664,""safe"":true},{""name"":""withKey"",""parameters"":[],""returntype"":""Integer"",""offset"":95,""safe"":true},{""name"":""putWithKey"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":136,""safe"":false},{""name"":""getWithKey"",""parameters"":[],""returntype"":""Integer"",""offset"":143,""safe"":true},{""name"":""withString"",""parameters"":[],""returntype"":""Integer"",""offset"":146,""safe"":true},{""name"":""putWithString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":197,""safe"":false},{""name"":""getWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":204,""safe"":true},{""name"":""setPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":254,""safe"":false},{""name"":""getPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":296,""safe"":true},{""name"":""setNonStaticPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":674,""safe"":false},{""name"":""getNonStaticPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":684,""safe"":true},{""name"":""multiWithStringStstic"",""parameters"":[],""returntype"":""Integer"",""offset"":418,""safe"":true},{""name"":""multiWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":694,""safe"":true},{""name"":""testStaticMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":549,""safe"":false},{""name"":""testStaticMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":585,""safe"":false},{""name"":""testMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":704,""safe"":false},{""name"":""testMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":711,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.11.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3PAlcAAQwSV2l0aG91dENvbnN0cnVjdG9yQfa0a+JBkl3oMUrYJgdFeBDOQNshQFcAAnkMEldpdGhvdXRDb25zdHJ1Y3RvckGb9mfOQeY/GIRAVwACeXg02EBXAAF4NKRADAEBQfa0a+JBkl3oMUrYJgVFEEDbIUBXAAF4DAEBQZv2Z85B5j8YhEBXAAF4NOpANNBADAZ0ZXN0TWVB9rRr4kGSXegxStgmBUUQQNshQFcAAXgMBnRlc3RNZUGb9mfOQeY/GIRAVwABeDTlQDTGQAwZUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckH2tGviQZJd6DFK2CYFRRBA2yFAVwABeAwZUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckGb9mfOQeY/GIRANKdAVwABDCJOb25TdGF0aWNQcml2YXRlR2V0dGVyUHVibGljU2V0dGVyQfa0a+JBkl3oMUrYJgdFeBHOQNshQFcAAnkMIk5vblN0YXRpY1ByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJBm/ZnzkHmPxiEQFcAAXg0jEAMD3Rlc3RTdGF0aWNNdWx0aUH2tGviQZJd6DFK2CYFRRBA2yFAVwABeAwPdGVzdFN0YXRpY011bHRpQZv2Z85B5j8YhEBXAAEMCXRlc3RNdWx0aUH2tGviQZJd6DFK2CYHRXgSzkDbIUBXAAJ5DAl0ZXN0TXVsdGlBm/ZnzkHmPxiEQDV9////Spw0m0U1c////0qcNJFFNWn///9KnDSHRTVf////QDVZ////QFcAAXhKNJNOnFA0skV4SjSJTpxQNKhFeEo1f////06cUDSbRXg1c////0BXAAF4NWn///9AEBAQE8Ajd/3//xAQEBPAI739//8QEBATwCO7/f//EBAQE8Ajwf7//xAQEBPAI+r+//8QEBATwCMs////EBAQE8AiihAQEBPAIq5A2sGumg==").AsSerializable<Neo.SmartContract.NefFile>();

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
    public abstract BigInteger? WithString { [DisplayName("withString")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? WithoutConstructor { [DisplayName("withoutConstructor")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NNBA
    /// CALL D0 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithKey")]
    public abstract BigInteger? GetWithKey();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: NMZA
    /// CALL C6 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithString")]
    public abstract BigInteger? GetWithString();

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDSkQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL A4 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getWithoutConstructor")]
    public abstract BigInteger? GetWithoutConstructor();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDTqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL EA [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithKey")]
    public abstract void PutWithKey(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDTlQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL E5 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithString")]
    public abstract void PutWithString(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg02EA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL D8 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putWithoutConstructor")]
    public abstract void PutWithoutConstructor(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDVp////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 69FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiGet")]
    public abstract BigInteger? TestMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEo0k06cUDSyRXhKNIlOnFA0qEV4SjV/////TpxQNJtFeDVz////QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL 93 [512 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL B2 [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL 89 [512 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL A8 [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// CALL_L 7FFFFFFF [512 datoshi]
    /// TUCK [2 datoshi]
    /// INC [4 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 9B [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 73FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiSet")]
    public abstract BigInteger? TestMultiSet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NVn///9A
    /// CALL_L 59FFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiGet")]
    public abstract BigInteger? TestStaticMultiGet();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NX3///9KnDSbRTVz////Spw0kUU1af///0qcNIdFNV////9A
    /// CALL_L 7DFFFFFF [512 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// CALL 9B [512 datoshi]
    /// DROP [2 datoshi]
    /// CALL_L 73FFFFFF [512 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// CALL 91 [512 datoshi]
    /// DROP [2 datoshi]
    /// CALL_L 69FFFFFF [512 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// CALL 87 [512 datoshi]
    /// DROP [2 datoshi]
    /// CALL_L 5FFFFFFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticMultiSet")]
    public abstract BigInteger? TestStaticMultiSet();

    #endregion
}
