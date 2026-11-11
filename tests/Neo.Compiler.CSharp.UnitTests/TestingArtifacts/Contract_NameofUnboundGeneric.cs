using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_NameofUnboundGeneric(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NameofUnboundGeneric"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""Dictionary"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""describeNested"",""parameters"":[],""returntype"":""String"",""offset"":33,""safe"":false},{""name"":""List"",""parameters"":[],""returntype"":""String"",""offset"":74,""safe"":false},{""name"":""describeNestedMembers"",""parameters"":[],""returntype"":""String"",""offset"":107,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy4xMC4yK2UyY2FjZTgzZTQ1YTNkZDJlY2U5YjRmNjNkMTU1YjMyNjkuLi4AAAAAAJVXAgAMCkRpY3Rpb25hcnlwDARGdW5jcWgMATqLaYvbKEBXAgAMCkVudW1lcmF0b3JwDAxLZXlWYWx1ZVBhaXJxaAwBfItpi9soQFcCAAwETGlzdHAMClZhbHVlVHVwbGVxaAwBOotpi9soQFcCAAwNS2V5Q29sbGVjdGlvbnAMCkVudW1lcmF0b3JxaAwBfItpi9soQIxHkWM=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADApEaWN0aW9uYXJ5cAwERnVuY3FoDAE6i2mL2yhA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 44696374696F6E617279 'Dictionary' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 46756E63 'Func' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 3A ':' [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract string? Dictionary();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADARMaXN0cAwKVmFsdWVUdXBsZXFoDAE6i2mL2yhA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 4C697374 'List' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 56616C75655475706C65 'ValueTuple' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 3A ':' [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    public abstract string? List();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADApFbnVtZXJhdG9ycAwMS2V5VmFsdWVQYWlycWgMAXyLaYvbKEA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 456E756D657261746F72 'Enumerator' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 4B657956616C756550616972 'KeyValuePair' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 7C [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("describeNested")]
    public abstract string? DescribeNested();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADA1LZXlDb2xsZWN0aW9ucAwKRW51bWVyYXRvcnFoDAF8i2mL2yhA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 4B6579436F6C6C656374696F6E 'KeyCollection' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 456E756D657261746F72 'Enumerator' [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 7C [8 datoshi]
    /// CAT [2048 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("describeNestedMembers")]
    public abstract string? DescribeNestedMembers();

    #endregion
}
