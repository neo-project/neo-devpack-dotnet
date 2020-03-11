Imports Neo


Public Class SmartContract1 : Inherits SmartContract.Framework.SmartContract

    Public Shared Function Main()
        Main = UnitTest_001()
    End Function

    Public Shared Function UnitTest_001() As Byte()
        Dim buffer = New Byte() {1, 2, 3, 4}
        UnitTest_001 = buffer
    End Function

End Class
