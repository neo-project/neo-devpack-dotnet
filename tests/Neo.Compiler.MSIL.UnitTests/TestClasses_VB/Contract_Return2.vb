Imports Neo


Public Class SmartContract2 : Inherits SmartContract.Framework.SmartContract

    Public Shared Function Main()
        Main = UnitTest_002()
    End Function

    Public Shared Function UnitTest_002() 'no return type here.
        Dim buffer = New Byte() {1, 2, 3, 4}
        UnitTest_002 = buffer(2)
    End Function

End Class
