using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleZKP : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleZKP"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""veify"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""},{""name"":""c"",""type"":""ByteArray""},{""name"":""public_input"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":264,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Version"":""0.0.1"",""Description"":""A sample contract to demonstrate how to use Example.SmartContract.ZKPil"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMb9XWrEYlohBNhCjWhKIbN4LZscg9ibHMxMjM4MVBhaXJpbmcCAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFNdWwDAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFBZGQCAAEPAAD9igRXCQR5eDcAAHBYWTcAAHF7ynJaynNrahGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn5gmHQwYZXJyb3I6IGlucHV0bGVuIG9yIGljbGVuOloQznQQdSJ4CXttzlptEZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfzjcBAHZubDcCAEp0RW1KnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ91RW1qtSSHW2w3AAB1XHo3AAB2bWk3AgB3B25vBzcCAHcIaG8Il0BWBQxgAGqnm6SqQ57tTlsH87+63RthBr7BzFXOUzgD0YT53V58FPVxj0b1n2jVJZfRfaCPFk78YeHXhfBUleeOU3SciHgt8pvHqfTk3fWgHPcnUwasGX7f5yhemkVne2nyo5zc2zBhDMADWx4UPcqOIaQh12rbJ4hgcP51N5wsN33wP6bOnRHJCyGs4jr+yi6AArPjJeZ/eXYGO1SRaMREJdE2VpSb+yRuf77NNGSI4sT5rHrX5ipcr754E1A4lOydbEotHZ3zYF4REPKlOITfgAAj7giKsGbs8rH8l5he5oJvufrDD30fgANmtTgTw3ngUuQDKR963IUS1F/CyblLb+liUC8Jv7J3Me4e69koCMf9ewhVTmQgb7k5xfBMBvwQclJao/CSBALbMGAMwBdFL2xzrf7LWUO34LAaf4RZomPxQuSxETlVAw2UWKI23L0hrCbAdOwNc9vJM6b98AwgTVKhvfDGlLgRXKKRpjf89cJfR9DXFxNfipOVGiNsjRmLZzswvVjMZP90wuWdBRMQH57eLZdW2p0R/B2DeWuoLpGwepJdtLVi5QQdIoneXXxa02MzWmC/yw4iDgVN0QA+ipZnXvzIukDFG63lvcHESyhfayQyWpI718q4TVcUNfHQSJ4tFlE13CjeGkXm/dswYwzAAU5Tr59nf9lQ1QDCbB7S8YrRAKR1IERmeSQoQVnNxgEOkMTssNZ3i+F217kk2LcbFn7BFa3U+mgZRWsox6Dk73BmkFU6bXpJ3aqRvDwJ5LIk46+MKLWer1u9XKlaWh6ZBOG7Nc5yPG0zuAJkJ18rIVKNociIkiESyo0r3kBRlzqNkgjWn26nrf05vj5eWPU7FHnp0XoqDbhyABMgeI9sdmvx2rZFh3Uq57/HWFiRhhiF0zVIF9ZpYYb+dFmmd93f2zBkDGARnscTidOh+HaV+pEu3aBWKKVuxqDLvFTSU5+wcW8K68Dz8m680mLHSkJ2+wO8OlQXN1ioJfB5+BaLpZcvo0hyUJgcoEw6/KJua8qtOdtPd8T5VNfpCzsyzBeVQFiHo77bMAxgDpj9n2WO4wWmR5jPIJg4rL8ruByUKOAqh4m112AiyH9Nl6ULgjlbU0cm/Z9nv4t4FAlbeGoQ0VhXztHpgVcPi1ykVJYzXNy8c9mDwdUX4YD0h1+AtX+fw9uwmBC6UAWP2zASwGJAaEf9ew=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("veify")]
    public abstract bool? Veify(byte[]? a, byte[]? b, byte[]? c, IList<object>? public_input);

    #endregion

    #region Constructor for internal use only

    protected SampleZKP(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
