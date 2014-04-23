'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/10/2012
' Time: 5:26 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class CompReportViewer
	Inherits System.Windows.Forms.Form
	
	''' <summary>
	''' Designer variable used to keep track of non-visual components.
	''' </summary>
	Private components As System.ComponentModel.IContainer
	
	''' <summary>
	''' Disposes resources used by the form.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If components IsNot Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub
	
	''' <summary>
	''' This method is required for Windows Forms designer support.
	''' Do not change the method contents inside the source code editor. The Forms designer might
	''' not be able to load this method if it was changed manually.
	''' </summary>
	Private Sub InitializeComponent()
		Me.reportBrowser = New System.Windows.Forms.WebBrowser
		Me.SuspendLayout
		'
		'reportBrowser
		'
		Me.reportBrowser.AllowNavigation = false
		Me.reportBrowser.AllowWebBrowserDrop = false
		Me.reportBrowser.Dock = System.Windows.Forms.DockStyle.Fill
		Me.reportBrowser.Location = New System.Drawing.Point(0, 0)
		Me.reportBrowser.MinimumSize = New System.Drawing.Size(20, 20)
		Me.reportBrowser.Name = "reportBrowser"
		Me.reportBrowser.Size = New System.Drawing.Size(764, 502)
		Me.reportBrowser.TabIndex = 0
		Me.reportBrowser.WebBrowserShortcutsEnabled = false
		'
		'CompReportViewer
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(764, 502)
		Me.Controls.Add(Me.reportBrowser)
		Me.Name = "CompReportViewer"
		Me.ShowInTaskbar = false
		Me.Text = "Compilation Report"
		Me.ResumeLayout(false)
	End Sub
	Private reportBrowser As System.Windows.Forms.WebBrowser
End Class
