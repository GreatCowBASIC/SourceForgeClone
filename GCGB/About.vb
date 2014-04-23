Imports System
Imports System.Drawing
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class AboutForm
		Inherits System.Windows.Forms.Form
		Private contributorsGroup As System.Windows.Forms.GroupBox
		Private contributorsBox As System.Windows.Forms.TextBox
		Private DisplayVersion As System.Windows.Forms.TextBox
		Private versionGroup As System.Windows.Forms.GroupBox
		Private linksGroup As System.Windows.Forms.GroupBox
		Private licenseGroup As System.Windows.Forms.GroupBox
		Private forumLink As System.Windows.Forms.LinkLabel
		Private closeButton As System.Windows.Forms.Button
		Private licenseText As System.Windows.Forms.Label
		Private gcbLink As System.Windows.Forms.LinkLabel
		Private CowLogo As System.Windows.Forms.PictureBox
		Private readLicense As System.Windows.Forms.Button
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.DisplayVersion.Text = "Version " + MainForm.ProgVersion
			
			CowLogo.Select
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(AboutForm))
			Me.readLicense = New System.Windows.Forms.Button
			Me.CowLogo = New System.Windows.Forms.PictureBox
			Me.gcbLink = New System.Windows.Forms.LinkLabel
			Me.licenseText = New System.Windows.Forms.Label
			Me.closeButton = New System.Windows.Forms.Button
			Me.forumLink = New System.Windows.Forms.LinkLabel
			Me.licenseGroup = New System.Windows.Forms.GroupBox
			Me.linksGroup = New System.Windows.Forms.GroupBox
			Me.versionGroup = New System.Windows.Forms.GroupBox
			Me.DisplayVersion = New System.Windows.Forms.TextBox
			Me.contributorsBox = New System.Windows.Forms.TextBox
			Me.contributorsGroup = New System.Windows.Forms.GroupBox
			Me.licenseGroup.SuspendLayout
			Me.linksGroup.SuspendLayout
			Me.versionGroup.SuspendLayout
			Me.contributorsGroup.SuspendLayout
			Me.SuspendLayout
			'
			'readLicense
			'
			Me.readLicense.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.readLicense.Location = New System.Drawing.Point(32, 96)
			Me.readLicense.Name = "readLicense"
			Me.readLicense.Size = New System.Drawing.Size(168, 23)
			Me.readLicense.TabIndex = 1
			Me.readLicense.Text = "Read the license ..."
			AddHandler Me.readLicense.Click, AddressOf Me.ReadLicenseClick
			'
			'CowLogo
			'
			Me.CowLogo.BackColor = System.Drawing.Color.White
			Me.CowLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me.CowLogo.Image = CType(resources.GetObject("CowLogo.Image"),System.Drawing.Image)
			Me.CowLogo.Location = New System.Drawing.Point(16, 16)
			Me.CowLogo.Name = "CowLogo"
			Me.CowLogo.Size = New System.Drawing.Size(216, 128)
			Me.CowLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
			Me.CowLogo.TabIndex = 3
			Me.CowLogo.TabStop = false
			'
			'gcbLink
			'
			Me.gcbLink.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.gcbLink.Location = New System.Drawing.Point(8, 24)
			Me.gcbLink.Name = "gcbLink"
			Me.gcbLink.Size = New System.Drawing.Size(216, 16)
			Me.gcbLink.TabIndex = 0
			Me.gcbLink.TabStop = true
			Me.gcbLink.Text = "Great Cow BASIC Web Site"
			AddHandler Me.gcbLink.LinkClicked, AddressOf Me.GcbLinkLinkClicked
			'
			'licenseText
			'
			Me.licenseText.Location = New System.Drawing.Point(8, 16)
			Me.licenseText.Name = "licenseText"
			Me.licenseText.Size = New System.Drawing.Size(216, 80)
			Me.licenseText.TabIndex = 0
			Me.licenseText.Text = "Great Cow Graphical BASIC comes with ABSOLUTELY NO WARRANTY. This is free softwar"& _ 
"e, and you are welcome to redistribute it under certain conditions; please read "& _ 
"the license for details."
			Me.licenseText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
			'
			'closeButton
			'
			Me.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.closeButton.Location = New System.Drawing.Point(264, 328)
			Me.closeButton.Name = "closeButton"
			Me.closeButton.Size = New System.Drawing.Size(200, 24)
			Me.closeButton.TabIndex = 11
			Me.closeButton.Text = "Close"
			'
			'forumLink
			'
			Me.forumLink.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.forumLink.Location = New System.Drawing.Point(8, 48)
			Me.forumLink.Name = "forumLink"
			Me.forumLink.Size = New System.Drawing.Size(216, 16)
			Me.forumLink.TabIndex = 1
			Me.forumLink.TabStop = true
			Me.forumLink.Text = "Great Cow BASIC Forums"
			AddHandler Me.forumLink.LinkClicked, AddressOf Me.ForumLinkLinkClicked
			'
			'licenseGroup
			'
			Me.licenseGroup.Controls.Add(Me.readLicense)
			Me.licenseGroup.Controls.Add(Me.licenseText)
			Me.licenseGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.licenseGroup.Location = New System.Drawing.Point(8, 224)
			Me.licenseGroup.Name = "licenseGroup"
			Me.licenseGroup.Size = New System.Drawing.Size(232, 128)
			Me.licenseGroup.TabIndex = 8
			Me.licenseGroup.TabStop = false
			Me.licenseGroup.Text = "License"
			'
			'linksGroup
			'
			Me.linksGroup.Controls.Add(Me.forumLink)
			Me.linksGroup.Controls.Add(Me.gcbLink)
			Me.linksGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.linksGroup.Location = New System.Drawing.Point(248, 248)
			Me.linksGroup.Name = "linksGroup"
			Me.linksGroup.Size = New System.Drawing.Size(232, 72)
			Me.linksGroup.TabIndex = 9
			Me.linksGroup.TabStop = false
			Me.linksGroup.Text = "Links"
			'
			'versionGroup
			'
			Me.versionGroup.Controls.Add(Me.DisplayVersion)
			Me.versionGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.versionGroup.Location = New System.Drawing.Point(8, 160)
			Me.versionGroup.Name = "versionGroup"
			Me.versionGroup.Size = New System.Drawing.Size(232, 56)
			Me.versionGroup.TabIndex = 7
			Me.versionGroup.TabStop = false
			Me.versionGroup.Text = "Version"
			'
			'DisplayVersion
			'
			Me.DisplayVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.DisplayVersion.BorderStyle = System.Windows.Forms.BorderStyle.None
			Me.DisplayVersion.Location = New System.Drawing.Point(8, 21)
			Me.DisplayVersion.Multiline = true
			Me.DisplayVersion.Name = "DisplayVersion"
			Me.DisplayVersion.ReadOnly = true
			Me.DisplayVersion.Size = New System.Drawing.Size(216, 16)
			Me.DisplayVersion.TabIndex = 1
			Me.DisplayVersion.Text = "Version not set"
			Me.DisplayVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
			'
			'contributorsBox
			'
			Me.contributorsBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.contributorsBox.BorderStyle = System.Windows.Forms.BorderStyle.None
			Me.contributorsBox.HideSelection = false
			Me.contributorsBox.Location = New System.Drawing.Point(8, 16)
			Me.contributorsBox.Multiline = true
			Me.contributorsBox.Name = "contributorsBox"
			Me.contributorsBox.ReadOnly = true
			Me.contributorsBox.Size = New System.Drawing.Size(216, 208)
			Me.contributorsBox.TabIndex = 0
			Me.contributorsBox.Text = "Great Cow Graphical BASIC:"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Hugh Considine (Lead Developer)"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Great Cow BASIC:"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)& _ 
"(Used by GCGB to compile programs)"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Hugh Considine (Lead Developer)"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Stefano Bon"& _ 
"omi (2 bit LCD routines)"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Kent Schafer (Peripheral routines)"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Thank you also t"& _ 
"o all of those who've helped out by reporting bugs, giving suggestions, answerin"& _ 
"g questions on the forums, or just spreading the word - GCBASIC wouldn't be the "& _ 
"same without it!"
			'
			'contributorsGroup
			'
			Me.contributorsGroup.Controls.Add(Me.contributorsBox)
			Me.contributorsGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.contributorsGroup.Location = New System.Drawing.Point(248, 8)
			Me.contributorsGroup.Name = "contributorsGroup"
			Me.contributorsGroup.Size = New System.Drawing.Size(232, 232)
			Me.contributorsGroup.TabIndex = 10
			Me.contributorsGroup.TabStop = false
			Me.contributorsGroup.Text = "Contributors"
			'
			'AboutForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.closeButton
			Me.ClientSize = New System.Drawing.Size(490, 360)
			Me.Controls.Add(Me.closeButton)
			Me.Controls.Add(Me.contributorsGroup)
			Me.Controls.Add(Me.linksGroup)
			Me.Controls.Add(Me.licenseGroup)
			Me.Controls.Add(Me.versionGroup)
			Me.Controls.Add(Me.CowLogo)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "AboutForm"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Tag = ""
			Me.Text = "About Great Cow Graphical BASIC"
			Me.licenseGroup.ResumeLayout(false)
			Me.linksGroup.ResumeLayout(false)
			Me.versionGroup.ResumeLayout(false)
			Me.contributorsGroup.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ReadLicenseClick(sender As System.Object, e As System.EventArgs)
			System.Diagnostics.Process.Start(MainForm.InstallDir + MainForm.FilenameLicense)
			
		End Sub
		
		Private Sub GcbLinkLinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
			System.Diagnostics.Process.Start(MainForm.URL_GCB)
			
		End Sub
		
		Private Sub ForumLinkLinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
			System.Diagnostics.Process.Start(MainForm.URL_Forum)
			
		End Sub
		
	End Class
''End Namespace
