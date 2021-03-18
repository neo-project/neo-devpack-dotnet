Imports Neo.SmartContract.Framework
Imports Neo.SmartContract.Framework.Services
Imports System
Imports System.Numerics

Public Class $itemname$ : Inherits SmartContract
    Public Shared Function Main() As Boolean
        Storage.Put(Storage.CurrentContext, "Hello", "World")
        Return True
    End Function
End Class
