using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stored(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stored"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""withoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":639,""safe"":true},{""name"":""putWithoutConstructor"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":655,""safe"":false},{""name"":""getWithoutConstructor"",""parameters"":[],""returntype"":""Integer"",""offset"":671,""safe"":true},{""name"":""withKey"",""parameters"":[],""returntype"":""Integer"",""offset"":110,""safe"":true},{""name"":""putWithKey"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":148,""safe"":false},{""name"":""getWithKey"",""parameters"":[],""returntype"":""Integer"",""offset"":157,""safe"":true},{""name"":""withString"",""parameters"":[],""returntype"":""Integer"",""offset"":160,""safe"":true},{""name"":""putWithString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":208,""safe"":false},{""name"":""getWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":217,""safe"":true},{""name"":""setPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":264,""safe"":false},{""name"":""getPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":306,""safe"":true},{""name"":""setNonStaticPrivateGetterPublicSetter"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":687,""safe"":false},{""name"":""getNonStaticPrivateGetterPublicSetter"",""parameters"":[],""returntype"":""Integer"",""offset"":703,""safe"":true},{""name"":""multiWithStringStstic"",""parameters"":[],""returntype"":""Integer"",""offset"":425,""safe"":true},{""name"":""multiWithString"",""parameters"":[],""returntype"":""Integer"",""offset"":719,""safe"":true},{""name"":""testStaticMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":550,""safe"":false},{""name"":""testStaticMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":583,""safe"":false},{""name"":""testMultiSet"",""parameters"":[],""returntype"":""Integer"",""offset"":735,""safe"":false},{""name"":""testMultiGet"",""parameters"":[],""returntype"":""Integer"",""offset"":751,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0AA1cAAQwSV2l0aG91dENvbnN0cnVjdG9yQfa0a+JBkl3oMUrYJgZFeBDOQFcAAXgQENB4ERDQeBIQ0EBXAAJ5DBJXaXRob3V0Q29uc3RydWN0b3JBm/ZnzkHmPxiEQFcAAnlKeDTXRUBXAAF4NJVADAEBQfa0a+JBkl3oMUrYJgRFEEBXAAF4DAEBQZv2Z85B5j8YhEBXAAF4SjTpRUA00UAMBnRlc3RNZUH2tGviQZJd6DFK2CYERRBAVwABeAwGdGVzdE1lQZv2Z85B5j8YhEBXAAF4SjTkRUA0x0AMGVByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJB9rRr4kGSXegxStgmBEUQQFcAAXgMGVByaXZhdGVHZXR0ZXJQdWJsaWNTZXR0ZXJBm/ZnzkHmPxiEQDSqQFcAAQwiTm9uU3RhdGljUHJpdmF0ZUdldHRlclB1YmxpY1NldHRlckH2tGviQZJd6DFK2CYGRXgRzkBXAAJ5DCJOb25TdGF0aWNQcml2YXRlR2V0dGVyUHVibGljU2V0dGVyQZv2Z85B5j8YhEBXAAF4NI9ADA90ZXN0U3RhdGljTXVsdGlB9rRr4kGSXegxStgmBEUQQFcAAXgMD3Rlc3RTdGF0aWNNdWx0aUGb9mfOQeY/GIRAVwABDAl0ZXN0TXVsdGlB9rRr4kGSXegxStgmBkV4Es5AVwACeQwJdGVzdE11bHRpQZv2Z85B5j8YhEA0g0qcNKFFNXz///9KnDSXRTVy////Spw0jUU1aP///0A1Yv///0BXAAF4SjSZTpxQNLVFeEo0j06cUDSrRXhKNIVOnFA0oUV4NXz///9AVwABeDVy////QBAQEBPASjWl/f//I3b9//8QEBATwEo1lf3//yPD/f//EBAQE8BKNYX9//8jvf3//xAQEBPASjV1/f//I7X+//8QEBATwEo1Zf3//yPY/v//EBAQE8BKNVX9//8jEf///xAQEBPASjVF/f//I2P///8QEBATwEo1Nf3//yN7////QCPxtsA="));

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
    /// Script: VwABeDSVQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : CALL 95 [512 datoshi]
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
