using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleZKP(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleZKP"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""veify"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""},{""name"":""c"",""type"":""ByteArray""},{""name"":""public_input"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":262,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Version"":""0.0.1"",""Description"":""A sample contract to demonstrate how to use Example.SmartContract.ZKPil"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMb9XWrEYlohBNhCjWhKIbN4LZscg9ibHMxMjM4MVBhaXJpbmcCAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFNdWwDAAEPG/V1qxGJaIQTYQo1oSiGzeC2bHILYmxzMTIzODFBZGQCAAEPAAD9iARXCQR5eDcAAHBYWTcAAHF7ynJaynNrahGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn5gmHQwYZXJyb3I6IGlucHV0bGVuIG9yIGljbGVuOloQznQQdSJ2CXttzlptEZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfzjcBAHZubDcCAHRtSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfdUVtarUkiVtsNwAAdVx6NwAAdm1pNwIAdwdubwc3AgB3CGhvCJdAVgUMYABqp5ukqkOe7U5bB/O/ut0bYQa+wcxVzlM4A9GE+d1efBT1cY9G9Z9o1SWX0X2gjxZO/GHh14XwVJXnjlN0nIh4LfKbx6n05N31oBz3J1MGrBl+3+coXppFZ3tp8qOc3NswYQzAA1seFD3KjiGkIddq2yeIYHD+dTecLDd98D+mzp0RyQshrOI6/sougAKz4yXmf3l2BjtUkWjERCXRNlaUm/skbn++zTRkiOLE+ax61+YqXK++eBNQOJTsnWxKLR2d82BeERDypTiE34AAI+4IirBm7PKx/JeYXuaCb7n6ww99H4ADZrU4E8N54FLkAykfetyFEtRfwsm5S2/pYlAvCb+ydzHuHuvZKAjH/XsIVU5kIG+5OcXwTAb8EHJSWqPwkgQC2zBgDMAXRS9sc63+y1lDt+CwGn+EWaJj8ULksRE5VQMNlFiiNty9IawmwHTsDXPbyTOm/fAMIE1Sob3wxpS4EVyikaY3/PXCX0fQ1xcTX4qTlRojbI0Zi2c7ML1YzGT/dMLlnQUTEB+e3i2XVtqdEfwdg3lrqC6RsHqSXbS1YuUEHSKJ3l18WtNjM1pgv8sOIg4FTdEAPoqWZ178yLpAxRut5b3BxEsoX2skMlqSO9fKuE1XFDXx0EieLRZRNdwo3hpF5v3bMGMMwAFOU6+fZ3/ZUNUAwmwe0vGK0QCkdSBEZnkkKEFZzcYBDpDE7LDWd4vhdte5JNi3GxZ+wRWt1PpoGUVrKMeg5O9wZpBVOm16Sd2qkbw8CeSyJOOvjCi1nq9bvVypWloemQThuzXOcjxtM7gCZCdfKyFSjaHIiJIhEsqNK95AUZc6jZII1p9up639Ob4+Xlj1OxR56dF6Kg24cgATIHiPbHZr8dq2RYd1Kue/x1hYkYYYhdM1SBfWaWGG/nRZpnfd39swZAxgEZ7HE4nTofh2lfqRLt2gViilbsagy7xU0lOfsHFvCuvA8/JuvNJix0pCdvsDvDpUFzdYqCXwefgWi6WXL6NIclCYHKBMOvyibmvKrTnbT3fE+VTX6Qs7MswXlUBYh6O+2zAMYA6Y/Z9ljuMFpkeYzyCYOKy/K7gclCjgKoeJtddgIsh/TZelC4I5W1NHJv2fZ7+LeBQJW3hqENFYV87R6YFXD4tcpFSWM1zcvHPZg8HVF+GA9IdfgLV/n8PbsJgQulAFj9swEsBiQO6h9P0="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("veify")]
    public abstract bool? Veify(byte[]? a, byte[]? b, byte[]? c, IList<object>? public_input);

    #endregion
}
