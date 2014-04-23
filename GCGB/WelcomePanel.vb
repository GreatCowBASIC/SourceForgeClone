'
' Created by SharpDevelop.
' User: Hugh
' Date: 26/01/2011
' Time: 3:27 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class WelcomePanel
		Inherits System.Windows.Forms.UserControl
		Private welcomeMessage As System.Windows.Forms.RichTextBox
		
		Public Sub New()
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
			WelcomeMessage.Rtf = "{\rtf1\ansi " + _
			                     "\pard \qc {\b Welcome to Great Cow Graphical BASIC!} \par " + _
			                     "\par " + _
			                     "\pard To get started, tell GCGB what chip you're programming. You can do this with {{\v <http://hardware/}Hardware Settings{\v >}}, or by {{\v <http://libraries/}adding a library{\v >}} if there is one for your device. \par " + _
			                     "\par " + _
			                     "\pard Next, add an icon. Most of the ones you need will be in {{\v <http://catFlowControl/}Flow Control{\v >}} and {{\v <http://catInputOutput/}Input/Output{\v >}}. Drag and drop the icon into the program. \par " + _
			                     "\par " + _
			                     "\pard When an icon is added, any settings it has will appear here. Change them to make the icon do what you want. \par " + _
			                     "\par " + _
			                     "\pard When this message disappears, you can get it back by clicking on the Great Cow at the start of the program. \par " + _
			                     "\par " + _
			                     "\pard Finally, attach your microcontroller chip to the computer with a suitable programmer, and {{\v <http://download/}Download{\v >}} the program. \par " + _
			                     "}"
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.welcomeMessage = New System.Windows.Forms.RichTextBox
			Me.SuspendLayout
			'
			'welcomeMessage
			'
			Me.welcomeMessage.BackColor = System.Drawing.SystemColors.Window
			Me.welcomeMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
			Me.welcomeMessage.Dock = System.Windows.Forms.DockStyle.Fill
			Me.welcomeMessage.Location = New System.Drawing.Point(0, 0)
			Me.welcomeMessage.Name = "welcomeMessage"
			Me.welcomeMessage.ReadOnly = true
			Me.welcomeMessage.Size = New System.Drawing.Size(200, 360)
			Me.welcomeMessage.TabIndex = 0
			Me.welcomeMessage.Text = "Welcome to Great Cow Graphical BASIC!"
			AddHandler Me.welcomeMessage.LinkClicked, AddressOf Me.WelcomeMessageLinkClicked
			'
			'WelcomePanel
			'
			Me.Controls.Add(Me.welcomeMessage)
			Me.Name = "WelcomePanel"
			Me.Size = New System.Drawing.Size(200, 360)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub WelcomeMessageLinkClicked(sender As System.Object, e As System.Windows.Forms.LinkClickedEventArgs)
			'Deal with links being clicked in the message
			
			'Links are in format http://address/link text
			'Need to get address
			Dim LinkAddress As String
			If e.LinkText.StartsWith("http://") Then
				LinkAddress = e.LinkText.Substring(7).ToLower
				If LinkAddress.IndexOf("/") <> -1 Then
					LinkAddress = LinkAddress.Substring(0, LinkAddress.IndexOf("/"))
					
					'Perform action for link
					Select Case LinkAddress
						Case "hardware":
							Dim CurrProgram As GCBProgram = MainForm.GetCurrentProgram
							If Not CurrProgram Is Nothing Then
								dim fChipConf As New ChipSettings(CurrProgram)
								fChipConf.ShowDialog()
								
								MainForm.MainFormInstance.GetCurrentContainer.SyncProgramToEditor
							End If
							
						Case "libraries":
							dim LibChoose As New LibraryWindow(MainForm.GetCurrentProgram)
							LibChoose.ShowDialog()
							
						Case "catflowcontrol":
							MainForm.MainFormInstance.SetIconCategory("Flow Control")
							
						Case "catinputoutput":
							MainForm.MainFormInstance.SetIconCategory("Input/Output")
							
						Case "download":
							MainForm.StartProgramDownload
							
					End Select
					
				End If
				
			End If
			
		End Sub
		
	End Class
'End Namespace
