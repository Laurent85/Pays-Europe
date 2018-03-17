Imports System.IO

Class MainWindow

    Public temp As Integer
    Public score As Integer
    Public coups1 As Integer
    Public p As Integer = 0
    Public triche As Integer = 0
    Private Sub Début(sender As Object, e As RoutedEventArgs) Handles Principal.Loaded

        For i As Int32 = 0 To Numéros.Children.Count - 1
            Dim Numéro As Label = CType(Numéros.Children.Item(i), Label)
            AddHandler Numéro.DragEnter, AddressOf lbl_Dragenter
            AddHandler Numéro.Drop, AddressOf lbl_Drop
            AddHandler Numéro.MouseDown, AddressOf lbl_MouseDown
        Next

        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            AddHandler cible.DragEnter, AddressOf lbl_Dragenter
            AddHandler cible.Drop, AddressOf lbl_Drop
            AddHandler cible.MouseDown, AddressOf lbl_MouseDown
            AddHandler cible.MouseRightButtonDown, AddressOf lbl_clear
            cible.AllowDrop = True
        Next

        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim numéro As Label = CType(Numéros.Children.Item(i), Label)
            Dim source As Label = CType(Sources.Children.Item(i), Label)
            source.Foreground = Brushes.Gray
            AddHandler numéro.MouseEnter, AddressOf Souris
            AddHandler numéro.MouseLeave, AddressOf plus_souris
        Next

        Bilan_des_placements()
        Points.Foreground = Brushes.SaddleBrown
        Points.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
        Points.VerticalContentAlignment = Windows.VerticalAlignment.Top
        Points.Content = score
        Coups.Content = "Coups : " & coups1

    End Sub

    Private Sub lbl_MouseDown(sender As Object, e As MouseButtonEventArgs)
        'Activation du "glisser"
        Dim lbl As Label = CType(sender, Label)
        DragDrop.DoDragDrop(lbl, lbl.Content, DragDropEffects.Copy)
    End Sub

    Private Sub lbl_Dragenter(sender As Object, e As DragEventArgs)
        'Activation de la copie de l'élément glissé
        If e.Data.GetDataPresent(DataFormats.Text) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub lbl_Drop(sender As Object, e As DragEventArgs)

        For j = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(j), Label)
            Dim numero As Label = CType(Numéros.Children.Item(j), Label)
            'Cherche si existe déjà
            'Si cible = source et source différent de "?" alors..
            If cible.Content = e.Data.GetData(DataFormats.Text) And e.Data.GetData(DataFormats.Text) <> "?" Then
                cible.Content = "?"
                cible.ToolTip = vbNullString
                cible.Foreground = Brushes.Red
                CType(sender, Label).Content = e.Data.GetData(DataFormats.Text)
                CType(sender, Label).Foreground = Brushes.Red
            Else
                CType(sender, Label).Content = e.Data.GetData(DataFormats.Text)
                CType(sender, Label).Foreground = Brushes.Red
            End If
        Next j

        'Numéros gris et infobulle pour les départements placés
        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim numero As Label = CType(Numéros.Children.Item(i), Label)
            For j As Int32 = 0 To Cibles.Children.Count - 1
                Dim cible As Label = CType(Cibles.Children.Item(j), Label)
                If cible.Content = numero.Content Then
                    numero.Foreground = Brushes.LightGray
                    cible.ToolTip = CType(Sources.Children.Item(i), Label).Content
                    Exit For
                Else
                    numero.Foreground = Brushes.Red
                End If
                If cible.Content = "?" Then
                    cible.ToolTip = vbNullString
                    cible.Foreground = Brushes.Red
                End If
            Next j
        Next i

        p = 0
        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            If cible.Content <> "?" And (Equals(cible.Foreground, Brushes.Red)) Then
                p = p + 1
                En_cours.Foreground = Brushes.Gray
                En_cours.Content = "Pays non vérifiés : " & p.ToString
            End If
        Next

        Test_réponse()

    End Sub

    Private Sub lbl_clear(sender As Object, e As MouseButtonEventArgs) Handles Cible_finlande.MouseRightButtonDown

        'effacement par clic-droit
        CType(sender, Label).AllowDrop = True

        For i As Int32 = 0 To Numéros.Children.Count - 1
            Dim source As Label = CType(Numéros.Children.Item(i), Label)
            If CType(sender, Label).Content = source.Content Then
                source.Foreground = Brushes.Red
                AddHandler source.MouseDown, AddressOf lbl_MouseDown 'activation du glissé
            End If
        Next i

        CType(sender, Label).Content = "?"
        CType(sender, Label).Foreground = Brushes.Red
        CType(sender, Label).ToolTip = vbNullString

    End Sub

    Private Sub Vérifier_Click(sender As Object, e As RoutedEventArgs) Handles Vérifier.Click

        Dim b As Integer = 0
        Dim m As Integer = 0
        Dim n As Integer = 0
        Dim j As String
        Dim total As Integer

        Bilan_des_placements()

        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            If i < 9 Then
                j = "0" & i + 1.ToString
            Else
                j = i + 1.ToString
            End If

            If cible.Content = j.ToString Then
                cible.Foreground = Brushes.Green
                b = b + 1
                Bien.Foreground = Brushes.Green
                Bien.Content = "Pays bien placés : " & b.ToString
            End If
            If cible.Content = "?" Then
                cible.Foreground = Brushes.Blue
                n = n + 1
                Absent.Foreground = Brushes.Blue
                Absent.Content = "Pays non placés : " & n.ToString
            End If
            If cible.Content <> "?" And cible.Content <> j.ToString Then
                cible.Foreground = Brushes.Red
                m = m + 1
                Mal.Foreground = Brushes.Red
                Mal.Content = "Pays mal placés : " & m.ToString
            End If
        Next i

        Test_réponse()

        If temp <> b Then
            total = (b - temp) * 2
        Else
            total = 0
        End If
        score = score + Val(total) - m
        temp = b
        coups1 = coups1 + 1
        Coups.Content = "Coups : " & coups1

        Points.Foreground = Brushes.SaddleBrown
        Points.HorizontalContentAlignment = Windows.HorizontalAlignment.Center
        Points.VerticalContentAlignment = Windows.VerticalAlignment.Top
        Points.Content = score
        'My.Computer.Audio.Play(My.Resources.Bruit, AudioPlayMode.Background)

        'Test_réponse()

        p = 0
        En_cours.Content = "Pays non vérifiés : " & p.ToString
        'Cacher_Checked(sender, e)
        'Voir_Checked(sender, e)

        If b = 48 And triche = 0 Then
            Dim result As MessageBoxResult = MessageBox.Show("Bravo " & copie.Content & " !" & Chr(10) & "Tu as placé les 48 pays en " & coups1 & " coups." & Chr(10) & "Tu as totalisé " & score & " points." & Chr(10) & "Veux-tu refaire une partie ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question)
            If result = MessageBoxResult.Yes Then
                fichier_Scores()
                'Effacer_Click(sender, e)
            Else
                fichier_Scores()
                Me.Close()
            End If
        End If
    End Sub

    Sub Bilan_des_placements()
        Bien.Foreground = Brushes.Green
        Mal.Foreground = Brushes.Red
        Absent.Foreground = Brushes.Blue
        Bien.Content = "Pays bien placés : 0"
        Mal.Content = "Pays mal placés : 0"
        Absent.Content = "Pays non placés : 0"
        En_cours.Content = "Pays non vérifiés : 0"
    End Sub

    Sub fichier_Scores()
        If triche = 0 Then
            Try
                Dim FichierScores As StreamWriter = New StreamWriter("\\laurent\d\Pays_Europe.txt", True)
                FichierScores.WriteLine(Now & Chr(9) & copie.Content & Chr(9) & "Points : " & Points.Content & Chr(9) & Coups.Content & Chr(9) & Bien.Content & Chr(9) & Mal.Content & Chr(9) & Absent.Content)
                FichierScores.Close()

            Catch ex As Exception

            End Try

            Try
                Dim FichierScores1 As StreamWriter = New StreamWriter("c:\Pays_Europe.txt", True)
                FichierScores1.WriteLine(Now & Chr(9) & copie.Content & Chr(9) & "Points : " & Points.Content & Chr(9) & Coups.Content & Chr(9) & Bien.Content & Chr(9) & Mal.Content & Chr(9) & Absent.Content)
                FichierScores1.Close()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub Window1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim result As MessageBoxResult = MessageBox.Show("Voulez-vous quitter le jeu ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question)

        If result = MessageBoxResult.Yes Then
            fichier_Scores()
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub Solutions_Click(sender As Object, e As RoutedEventArgs) Handles Solutions.Click

        Dim b As Integer = 0
        Dim m As Integer = 0
        Dim n As Integer = 0
        Dim j As String
        triche = 1

        Bilan_des_placements()

        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            If i < 9 Then
                j = "0" & i + 1.ToString
                cible.Content = j.ToString
                cible.ToolTip = CType(Sources.Children.Item(i), Label).Content
            Else
                j = i + 1.ToString
                cible.Content = j.ToString
                cible.ToolTip = CType(Sources.Children.Item(i), Label).Content
            End If

            If cible.Content = j.ToString Then
                cible.Foreground = Brushes.Green
                b = b + 1
                Bien.Foreground = Brushes.Green
                Bien.Content = "Pays bien placés : " & b.ToString
            End If
            If cible.Content = "?" Then
                cible.Foreground = Brushes.Blue
                n = n + 1
                Absent.Foreground = Brushes.Blue
                Absent.Content = "Pays non placés : " & n.ToString
            End If
            If cible.Content <> "?" And cible.Content <> j.ToString Then
                cible.Foreground = Brushes.Red
                m = m + 1
                Mal.Foreground = Brushes.Red
                Mal.Content = "Pays mal placés : " & m.ToString
            End If
        Next i

        For i As Integer = 0 To Cibles.Children.Count - 1
            Dim cible As Label = Me.Cibles.Children.Item(i)
            If cible IsNot Nothing Then
                cible.AllowDrop = False
            End If
        Next

        fichier_Scores()
        Test_réponse()

    End Sub

    Private Sub Effacer_Click(sender As Object, e As RoutedEventArgs) Handles Effacer.Click

        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            Dim source As Label = CType(Sources.Children.Item(i), Label)
            Dim numéro As Label = CType(Numéros.Children.Item(i), Label)
            cible.Content = "?"
            cible.AllowDrop = True
            cible.Foreground = Brushes.Blue
            cible.ToolTip = vbNullString
            source.Foreground = Brushes.Black
            numéro.Foreground = Brushes.Red
            AddHandler source.MouseDown, AddressOf lbl_MouseDown
            AddHandler cible.MouseDown, AddressOf lbl_MouseDown
            AddHandler cible.MouseRightButtonDown, AddressOf lbl_clear
            Bilan_des_placements()
        Next i

        fichier_Scores()

        temp = 0
        score = 0
        coups1 = 0
        Coups.Content = "Coups : " & coups1
        Points.Content = score

    End Sub

    Sub Test_réponse()
        'Si bonne réponse, on met la source en vert et on bloque le déplacement du département
        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim numéro As Label = CType(Numéros.Children.Item(i), Label)
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            If (Equals(cible.Foreground, Brushes.Green)) Then
                numéro.Foreground = Brushes.Green
                RemoveHandler numéro.MouseDown, AddressOf lbl_MouseDown
                RemoveHandler cible.DragEnter, AddressOf lbl_Dragenter
                RemoveHandler cible.Drop, AddressOf lbl_Drop
                RemoveHandler cible.MouseDown, AddressOf lbl_MouseDown
                RemoveHandler cible.MouseRightButtonDown, AddressOf lbl_clear
                cible.AllowDrop = False
                cible.ToolTip = CType(Sources.Children.Item(i), Label).Content
            Else
                'cible.ToolTip = vbNullString
            End If
        Next i
    End Sub

    Private Sub Souris(sender As Object, e As MouseEventArgs)

        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            Dim source As Label = CType(Sources.Children.Item(i), Label)
            Dim numéro As Label = CType(Numéros.Children.Item(i), Label)
            If numéro.IsMouseOver = True And numéro.Foreground.ToString = "#FFFF0000" Then
                source.Foreground = Brushes.Red
                source.FontSize = 14
            End If
            If numéro.IsMouseOver = True And (Equals(numéro.Foreground, Brushes.Red)) Then
                source.Foreground = Brushes.Red
                source.FontSize = 14
            End If
        Next

    End Sub

    Private Sub plus_souris(sender As Object, e As MouseEventArgs)

        For i As Int32 = 0 To Cibles.Children.Count - 1
            Dim cible As Label = CType(Cibles.Children.Item(i), Label)
            Dim source As Label = CType(Sources.Children.Item(i), Label)
            Dim numéro As Label = CType(Numéros.Children.Item(i), Label)
            If numéro.IsMouseOver = False Then
                source.Foreground = Brushes.Gray
                source.FontSize = 12
            End If
        Next

    End Sub

End Class
