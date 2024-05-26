Option Strict On

Public Class DateiInfo

    Public SourceDateiPfad As String
    Public CheckSumme As String
    Public RelativesDateiVerzeichnis As String

    Public Sub New(argSourceDateiPfad As String, argCheckSumme As String)
        SourceDateiPfad = argSourceDateiPfad
        CheckSumme = argCheckSumme
    End Sub

End Class
