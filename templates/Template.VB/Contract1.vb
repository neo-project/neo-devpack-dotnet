Imports Neo.SmartContract.Framework
Imports Neo.SmartContract.Framework.Services.Neo
Imports System
Imports System.Numerics

Public Class $itemname$ : Inherits SmartContract
    Public Shared Function Main(ByVal operation As String, ByVal args() As Object) As Boolean
        Storage.Put("Hello", "World")
        Return True
    End Function
End Class
