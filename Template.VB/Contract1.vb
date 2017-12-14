Public Class $itemname$ : Inherits SmartContract
    Public Shared Sub Main()
        Storage.Put(Storage.CurrentContext, "Hello", "World")
    End Sub
End Class
