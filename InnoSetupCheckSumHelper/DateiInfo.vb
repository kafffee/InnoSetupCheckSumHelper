Option Strict On

Public Class DateiInfo

    Public DateiPfad As String
    Public CheckSumme As String

    Public Sub New(argDateiPfad As String, argCheckSumme As String)
        DateiPfad = argDateiPfad
        CheckSumme = argCheckSumme
    End Sub

End Class
