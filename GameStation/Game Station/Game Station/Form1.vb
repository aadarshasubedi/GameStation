Imports System.IO

Public Class Form1
    ''define global variables

    '' This is for the timer
    Dim limit As Integer = 0
    Dim timerOn As Boolean = True
    ''This is the location of the .ini file.
    Dim inifile As String = Application.StartupPath + "\Game Station.ini"
    ''This is the location of the text document, to be determined from the .ini file
    Dim ffile As String

    Dim file As String
    Dim game As Process




    ''This reads what's in the feedback file.
    Public Function GetFileContents(ByVal FullPath As String, Optional ByRef ErrInfo As String = "Incorrect path") As String
        Dim strContents As String
        Dim objReader As StreamReader
        Try
            
            objReader = New StreamReader(FullPath)
            strContents = objReader.ReadToEnd()
            objReader.Close()
            Return strContents
        Catch ex As Exception
            ErrInfo = ex.Message
        End Try
    End Function
 
    'loads the form  reads from .ini file
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Dim ini As String
        Dim objReader As StreamReader

        TextBox1.Text = "Enter your feedback here."

        'read from the .ini file
        objReader = New StreamReader(inifile)
        ini = objReader.ReadToEnd()
        objReader.Close()


        'create the directory
        If My.Computer.FileSystem.DirectoryExists(ini) = False Then
            My.Computer.FileSystem.CreateDirectory(ini)
        End If

        'adds onto the path
        ini = (ini + "feedback.txt")

        'saves the path to ffile so it can be used elsewhere
        ffile = ini

        ' create the file
        If My.Computer.FileSystem.FileExists(ffile) = False Then
            My.Computer.FileSystem.WriteAllText(ffile, "", False)
        End If

    End Sub
    'The button to browse for a game
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        browser.InitialDirectory = "C:\Program Files\OGRE\Game Station\Games"
        browser.ShowDialog()
        TextBox1.Text = "Enter your feedback here."

    End Sub
    ' This sub will start the timer, send "ffile" to the public function so it can read the feedback file and start the game
    Private Sub browser_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles browser.FileOk
        Dim file As String
        'file is the string of what you browse for
        file = browser.FileName

        Call GetFileContents(ffile)

        Call Timer_Start()

        game = Process.GetProcessById(Shell(file))

    End Sub
    'This sub saves feedback to the textfile "feedback.txt" and saves it with a date stamp, gives an error message if nessecary. 
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click


        Dim sContents As String
        Dim slashIndex As Integer

        Dim objReader As StreamWriter

        If TextBox1.Text.Contains("Feedback sent!") Or TextBox1.Text.Contains("Enter your feedback here.") = False Then
            Try

                'formatting
                slashIndex = (browser.FileName.LastIndexOf("\") + 1)
                file = browser.FileName.Remove(0, slashIndex)
                sContents = GetFileContents(ffile) + DateAndTime.DateString + " " + TimeOfDay + " " + file + " -- " + TextBox1.Text.Trim()

                ''writes over what's inside the feedback file.
                objReader = New StreamWriter(ffile)
                objReader.WriteLine(sContents)
                objReader.Close()
                TextBox1.Text = "Feedback sent!"
            Catch
                TextBox1.Text = "You must play a game first in order to send feedback!"

            End Try
        End If


    End Sub
    'erase the text if you click on the text box to enter text
    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.Click
        TextBox1.Text = ""

    End Sub
    '' a "secret" way to close the program, because we don't want people closing it when they're done.
    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click
        Me.Close()
    End Sub
    ''every minute the timer ticks, then when it reaches five minutes, it calls to kill the process.
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick


        limit = limit + 1
        If limit = 5 Then

            Call Kill_Process()

        End If



    End Sub
    ''starts the timer and makes each tick last a minute
    Private Sub Timer_Start()
        If Timer1.Enabled = True Then
            Timer1.Interval = 60 * 1000
        End If


    End Sub
    ''kills the process with some error checking in case you close the process before the timer is enabled.
    Private Sub Kill_Process()
        limit = 0
        Dim oops As String = "oops"
        Try
            game.Kill()
            Timer1.Enabled = False


        Catch ex As Exception
            oops = ex.Message

        End Try






    End Sub
    'a secret way to disable the timer. The label is in the top right corner
    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click


        If timerOn = True Then
            timerOn = False
            TextBox1.Text = "Timer disabled"
        Else
            timerOn = True
            TextBox1.Text = "Timer enabled"
        End If
    End Sub
End Class
