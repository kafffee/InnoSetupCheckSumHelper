Option Strict On
Imports System.IO
Imports System.Security.Cryptography
Imports Microsoft.Win32

Class MainWindow

    Private NL As String = Environment.NewLine
    Private AnfZ As String = """"
    Private AlleDateiInfos As New List(Of DateiInfo)
    Private FilesCode As String = ""
    Private AppPath As String = ""
    Public Function DragDrop(sender As Object, e As DragEventArgs) As List(Of String)

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim docPath As String() = TryCast(e.Data.GetData(DataFormats.FileDrop), String())
            Dim DateienListe As New List(Of String)

            For Each Eintrag In docPath

                Dim DateiInfo As System.IO.FileInfo = New System.IO.FileInfo(Eintrag)

                If Directory.Exists(Eintrag) Then
                    DateienListe.AddRange(From Datei In New System.IO.DirectoryInfo(Eintrag).GetFiles("*.*", IO.SearchOption.AllDirectories) Where Not String.IsNullOrEmpty(Datei.Extension) Select Datei.FullName)
                Else
                    If Not String.IsNullOrEmpty(DateiInfo.Extension) Then
                        DateienListe.Add(Eintrag)
                    End If
                End If
            Next

            Return DateienListe

        End If
    End Function

    Private Sub brdDragDrop_Drop(sender As Object, e As DragEventArgs)


        Dim Skript As String = "[Files]" & NL
        Dim ZaehlerBereitsvorhanden As Integer = 0
        Dim DateiListe As List(Of String)

        DateiListe = DragDrop(sender, e)

        If (AlleDateiInfos.Count = 0) Then
            If DateiListe(0).EndsWith(".exe") Then
                AppPath = Path.GetDirectoryName(DateiListe(0))
            Else
                MessageBox.Show("Die erste Datei, die du zufügst, muss die Hauptanwendung sein und die Dateiendung .exe haben.")
                Return
            End If
        End If

        For Each Eintrag In DateiListe
            If Not (AlleDateiInfos.Any(Function(x) x.SourceDateiPfad = Eintrag)) Then
                AlleDateiInfos.Add(New DateiInfo(Eintrag, GetHashFromFile(Eintrag)))
            Else
                ZaehlerBereitsvorhanden += 1
            End If
        Next

        If ZaehlerBereitsvorhanden > 0 Then
            MessageBox.Show("Es waren " & CStr(ZaehlerBereitsvorhanden) & " Dateien bereits zugefügt. Diese wurden übersprungen.")
        End If

        FilesCode = ""


        For i = 0 To AlleDateiInfos.Count - 1

            If AlleDateiInfos(i).SourceDateiPfad.Contains(AppPath) Then
                AlleDateiInfos(i).RelativesDateiVerzeichnis = "{app}" & AlleDateiInfos(i).SourceDateiPfad.Substring(AppPath.Length)
            End If
        Next

        For Each Info In AlleDateiInfos
            FilesCode += "Source: " & AnfZ & Info.SourceDateiPfad & AnfZ & "; DestDir:" & AnfZ & Path.GetDirectoryName(Info.RelativesDateiVerzeichnis) & AnfZ & "; Flags: ignoreversion; AfterInstall: IsFileCorrupt('" & Info.SourceDateiPfad & "', '" & Info.CheckSumme & "')" & NL & NL
        Next

        txtISS.Text = Skript & FilesCode & AddeCode()
    End Sub

    Private Function GetHashFromFile(argDateiPfad As String) As String

        Dim CheckSumme As String = ""

        Using cryptoProvider As New SHA1CryptoServiceProvider()
            CheckSumme = BitConverter.ToString(cryptoProvider.ComputeHash(File.ReadAllBytes(argDateiPfad)))
        End Using

        CheckSumme = CheckSumme.Replace("-", "").ToLower
        Return CheckSumme

    End Function

    Private Sub btnCopyToClipBoard_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        Clipboard.SetText(txtISS.Text)
        MessageBox.Show("Das Skript wurde erfolgreich in die Zwischenablage kopiert.")
    End Sub

    Private Sub btnBeenden_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        Me.Close()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Me.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight

    End Sub

    Private Function AddeCode() As String

        Dim Code As String = NL

        Code += "[Code]" & NL
        Code += "var CancelWithoutPrompt: boolean;" & NL
        Code += "var ZielDateiPfad: string;" & NL
        Code += NL
        Code += "function InitializeSetup() : boolean;" & NL
        Code += "  begin" & NL
        Code += "    CancelWithoutPrompt := false;" & NL
        Code += "    result := true;" & NL
        Code += "  end;" & NL
        Code += NL
        Code += "procedure IsFileCorrupt(argPfad: string; argCheckSum: string);" & NL
        Code += "  begin" & NL
        Code += "    ZielDateiPfad := ExpandConstant('{app}') + copy(CurrentFilename, 6, Length(CurrentFilename)-5);" & NL
        Code += "    if GetSHA1OfFile(ZielDateiPfad) = argChecksum then" & NL
        Code += "    else" & NL
        Code += "      begin" & NL
        Code += "        MsgBox('Eine Installationsdatei (' + ZielDateiPfad + ') ist beschädigt. Bitte starte die Anwendung nicht und versuche stattdessen, den Installer erneut herunterzuladen und zu starten. Falls das Problem besteht, kontaktiere bitte den Entwickler.', mbError, MB_OK);" & NL
        Code += "        CancelWithoutPrompt:=True;" & NL
        Code += "        WizardForm.Close;" & NL
        Code += "      end;" & NL
        Code += "  end;"

        Return Code
    End Function
End Class
