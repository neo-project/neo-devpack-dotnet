using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleZKP(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleZKP"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""veify"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""},{""name"":""c"",""type"":""ByteArray""},{""name"":""public_input"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":278,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Version"":""0.0.1"",""Description"":""A sample contract to demonstrate how to use Example.SmartContract.ZKPil"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErMTQ2YzczYzZjYmQ3YTMyMTRlZGVmZWRhZmMxM2FmYjFiM2QuLi4AAAMb9XWrEYlohBNhCjWhKIbN4LZscg9ibHMxMjM4MVBhaXJpbmcCAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFNdWwDAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFBZGQCAAEPAAD9mARXCQR5eDcAAHBYWTcAAHF7ynJaynNrahGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn5gmHQwYZXJyb3I6IGlucHV0bGVuIG9yIGljbGVuOloQznQQdSJ4CXttzlptEZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfzjcBAHZubDcCAEp0RW1KnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ91RW1qtSSHW2w3AAB1XHo3AAB2bWk3AgB3B25vBzcCAHcIaG8IlyICQDcAAEA3AQBANwIAQFYFDGAAaqebpKpDnu1OWwfzv7rdG2EGvsHMVc5TOAPRhPndXnwU9XGPRvWfaNUll9F9oI8WTvxh4deF8FSV545TdJyIeC3ym8ep9OTd9aAc9ydTBqwZft/nKF6aRWd7afKjnNzbMGEMwANbHhQ9yo4hpCHXatsniGBw/nU3nCw3ffA/ps6dEckLIaziOv7KLoACs+Ml5n95dgY7VJFoxEQl0TZWlJv7JG5/vs00ZIjixPmsetfmKlyvvngTUDiU7J1sSi0dnfNgXhEQ8qU4hN+AACPuCIqwZuzysfyXmF7mgm+5+sMPfR+AA2a1OBPDeeBS5AMpH3rchRLUX8LJuUtv6WJQLwm/sncx7h7r2SgIx/17CFVOZCBvuTnF8EwG/BByUlqj8JIEAtswYAzAF0UvbHOt/stZQ7fgsBp/hFmiY/FC5LEROVUDDZRYojbcvSGsJsB07A1z28kzpv3wDCBNUqG98MaUuBFcopGmN/z1wl9H0NcXE1+Kk5UaI2yNGYtnOzC9WMxk/3TC5Z0FExAfnt4tl1banRH8HYN5a6gukbB6kl20tWLlBB0iid5dfFrTYzNaYL/LDiIOBU3RAD6Klmde/Mi6QMUbreW9wcRLKF9rJDJakjvXyrhNVxQ18dBIni0WUTXcKN4aReb92zBjDMABTlOvn2d/2VDVAMJsHtLxitEApHUgRGZ5JChBWc3GAQ6QxOyw1neL4XbXuSTYtxsWfsEVrdT6aBlFayjHoOTvcGaQVTptekndqpG8PAnksiTjr4wotZ6vW71cqVpaHpkE4bs1znI8bTO4AmQnXyshUo2hyIiSIRLKjSveQFGXOo2SCNafbqet/Tm+Pl5Y9TsUeenReioNuHIAEyB4j2x2a/HatkWHdSrnv8dYWJGGGIXTNUgX1mlhhv50WaZ33d/bMGQMYBGexxOJ06H4dpX6kS7doFYopW7GoMu8VNJTn7BxbwrrwPPybrzSYsdKQnb7A7w6VBc3WKgl8Hn4Foully+jSHJQmBygTDr8om5ryq052093xPlU1+kLOzLMF5VAWIejvtswDGAOmP2fZY7jBaZHmM8gmDisvyu4HJQo4CqHibXXYCLIf02XpQuCOVtTRyb9n2e/i3gUCVt4ahDRWFfO0emBVw+LXKRUljNc3Lxz2YPB1RfhgPSHX4C1f5/D27CYELpQBY/bMBLAYkBeDnKe").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("veify")]
    public abstract bool? Veify(byte[]? a, byte[]? b, byte[]? c, IList<object>? public_input);

    #endregion
}
