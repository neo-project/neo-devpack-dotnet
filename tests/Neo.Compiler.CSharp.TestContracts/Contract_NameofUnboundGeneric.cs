using Neo.SmartContract.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_NameofUnboundGeneric : SmartContract.Framework.SmartContract
{
    [DisplayName(nameof(Dictionary<,>))]
    public static string DescribeDictionary()
    {
        string dictName = nameof(Dictionary<,>);
        string funcName = nameof(Func<,,>);
        return $"{dictName}:{funcName}";
    }

    public static string DescribeNested()
    {
        string enumeratorName = nameof(Dictionary<,>.Enumerator);
        string keyValueName = nameof(KeyValuePair<,>);
        return $"{enumeratorName}|{keyValueName}";
    }

    [DisplayName(nameof(System.Collections.Generic.List<>))]
    public static string DescribeLists()
    {
        string listName = nameof(System.Collections.Generic.List<>);
        string tupleName = nameof(System.ValueTuple<,,,>);
        return $"{listName}:{tupleName}";
    }

    public static string DescribeNestedMembers()
    {
        string keyCollection = nameof(Dictionary<,>.KeyCollection);
        string valueCollection = nameof(Dictionary<,>.ValueCollection.Enumerator);
        return $"{keyCollection}|{valueCollection}";
    }
}
