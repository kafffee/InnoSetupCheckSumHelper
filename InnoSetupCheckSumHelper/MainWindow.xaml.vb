Option Strict On
Imports System.IO
Imports System.Security.Cryptography
Imports Microsoft.Win32

Class MainWindow

    Private NL As String = Environment.NewLine
    Private AnfZ As String = """"
    Public Function DragDrop(sender As Object, e As DragEventArgs) As List(Of String) ' Implements IDragDrop.DragDrop
        'MessageBox.Show("")
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim docPath As String() = TryCast(e.Data.GetData(DataFormats.FileDrop), String())
            Dim DateienListe As New List(Of String)
            'Dim DateiTypen() As String = {".mp3", ".flac", ".ogg", ".opus", ".wav", ".wv", ".alac", ".aac", ".aac+", ".wma", ".aiff"}
            'Dim AnzahlBereitsVorhandenerDateien As Integer = 0

            For Each Eintrag In docPath

                Dim DateiInfo As System.IO.FileInfo = New System.IO.FileInfo(Eintrag)
                'If DateiInfo.Attributes = IO.FileAttributes.Directory Then

                If Directory.Exists(Eintrag) Then
                    DateienListe.AddRange(From Datei In New System.IO.DirectoryInfo(Eintrag).GetFiles("*.*", IO.SearchOption.AllDirectories) Where Not String.IsNullOrEmpty(Datei.Extension) Select Datei.FullName)
                Else
                    If Not String.IsNullOrEmpty(DateiInfo.Extension) Then 'AndAlso DateiTypen.Contains(DateiInfo.Extension.ToLower) Then
                        DateienListe.Add(Eintrag)
                    End If
                End If
            Next

            Return DateienListe
            'Dim GepruefteDateienListe As New List(Of String)

            'For Each Eintrag In DateienListe
            '    If MainModule.InhaltGesamt.ToList.FindIndex(Function(x) x.Dateiname = Eintrag) = -1 Then
            '        GepruefteDateienListe.Add(Eintrag)
            '    End If
            'Next

            'AnzahlBereitsVorhandenerDateien = DateienListe.Count - GepruefteDateienListe.Count

            'If AnzahlBereitsVorhandenerDateien > 0 Then
            '    Dim OKVM = New OKDialogViewModel
            '    OKVM.Meldung = "Von den Dateien, die du zufügen willst, sind " & CStr(AnzahlBereitsVorhandenerDateien) & " Audiodateien bereits vorhanden in deiner Musikdatenbank. Diese werden übersprungen."
            '    dialogService.ShowModalDialog("", OKVM, Me, True, False, Services.WindowStyle.None, Services.ResizeMode.NoResize, 500, Services.SizeToContent.Height, Services.WindowStartupLocation.CenterOwner, "")
            'End If

            'If GepruefteDateienListe.Count = 0 Then
            '    Dim OKVM = New OKDialogViewModel
            '    OKVM.Meldung = "In den Dateien und/oder Ordnern, die du per Drag & Drop zugefügt hast, waren keine unterstützten Audiodateien dabei."
            '    dialogService.ShowModalDialog("", OKVM, Me, True, False, Services.WindowStyle.None, Services.ResizeMode.NoResize, 500, Services.SizeToContent.Height, Services.WindowStartupLocation.CenterOwner, "")
            'Else
            '    Dim EinlesenVM = New ViewModel.StatusFensterViewModel(StatusFensterViewModel.ZufuegeModus.DragDropZufuegen, GepruefteDateienListe)
            '    dialogService.ShowModalDialog("", EinlesenVM, Me, True, False, Services.WindowStyle.None, Services.ResizeMode.NoResize, 500, Services.SizeToContent.Height, Services.WindowStartupLocation.CenterOwner, "")

            '    LayerViewModel.SucheViewModel.SucheZuruecksetzen_Execute(Nothing)
            '    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow
            'End If
        End If
    End Function

    Private Sub brdDragDrop_Drop(sender As Object, e As DragEventArgs)

        Dim AlleDateiInfos As New List(Of DateiInfo)

        For Each Eintrag In DragDrop(sender, e)
            AlleDateiInfos.Add(New DateiInfo(Eintrag, GetHashFromFile(Eintrag)))
        Next
        'txtISS.Text = DragDrop(sender, e)(0)

        txtISS.Text = "[Files]" & NL

        For Each Info In AlleDateiInfos
            txtISS.Text += "Source: " & AnfZ & Info.DateiPfad & AnfZ & "; DestDir:" & AnfZ & "{app}" & AnfZ & "; Flags: ignoreversion; Check: IsFileCorrupt('" & Info.DateiPfad & "', '" & Info.CheckSumme & "')" & NL
        Next

        txtISS.Text += AddeCode()

        brdDragDrop.Background = New SolidColorBrush(System.Windows.Media.Colors.Green)
    End Sub

    Private Function GetHashFromFile(argDateiPfad As String) As String

        Dim CheckSumme As String = ""

        Using cryptoProvider As New SHA1CryptoServiceProvider()
            CheckSumme = BitConverter.ToString(cryptoProvider.ComputeHash(File.ReadAllBytes(argDateiPfad)))
        End Using

        CheckSumme = CheckSumme.Replace("-", "").ToLower
        Return CheckSumme

    End Function

    Private Sub btnBerechnen_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        '    If Not File.Exists(txtPfad.Text) Then
        '        MessageBox.Show("Bitte suche eine gültige Datei aus.")
        '        Return
        '    End If
        '    Try
        '        Using fileStream As FileStream = File.OpenRead(txtPfad.Text)
        '            Dim SHA As New SHA256Managed
        '            Dim chksum As Byte() = SHA.ComputeHash(fileStream)
        '            CheckSumme = BitConverter.ToString(chksum).Replace("-", "")
        '            MessageBox.Show("Checksumme wurde erfolgreich berechnet.")
        '        End Using
        '    Catch ex As Exception
        '        MessageBox.Show("Fehler beim Berechnen der Checksumme." & Environment.NewLine & "Fehlermeldung: " & ex.Message)
        '    End Try
    End Sub

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
        Code += NL
        Code += "function InitializeSetup() : boolean;" & NL
        Code += "  begin" & NL
        Code += "    CancelWithoutPrompt := false;" & NL
        Code += "    result := true;" & NL
        Code += "  end;" & NL
        Code += NL
        Code += "function IsFileCorrupt(argPfad: string; argCheckSum: string): boolean;" & NL
        Code += "  begin" & NL
        Code += "    if GetSHA1OfFile(argPfad) = argChecksum then" & NL
        Code += "      result := true" & NL
        Code += "    else" & NL
        Code += "      begin" & NL
        Code += "        MsgBox('Eine Installationsdatei ist beschädigt. Bitte versuche, den Installer erneut herunterzuladen. Falls das Problem besteht, kontaktiere bitte den Entwickler.', mbError, MB_OK);" & NL
        Code += "        CancelWithoutPrompt:=True;" & NL
        Code += "        WizardForm.Close;" & NL
        Code += "        result := false" & NL
        Code += "      end;                " & NL
        Code += "  end;"

        Return Code
    End Function
End Class
