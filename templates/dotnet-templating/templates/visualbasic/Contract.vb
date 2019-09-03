Imports Neo.SmartContract.Framework
Imports Neo.SmartContract.Framework.Services.Neo

Public Class SmartContract1 : Inherits SmartContract
    Public Shared Sub Main()
        Storage.Put(Storage.CurrentContext, "Hello", "World")
    End Sub
End Class
