/*
 * Created by SharpDevelop.
 * User: Hugh
 * Date: 12/04/2010
 * Time: 1:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Font_Editor
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.charEditor = new GameOfLife.Board();
			this.prevChar = new System.Windows.Forms.Button();
			this.nextChar = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.currChar = new System.Windows.Forms.Label();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.buttonSave = new System.Windows.Forms.Button();
			this.dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.dialogSaveFile = new System.Windows.Forms.SaveFileDialog();
			this.buttonExport = new System.Windows.Forms.Button();
			this.dialogExportFile = new System.Windows.Forms.SaveFileDialog();
			this.ModeGroup = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.modeLCD = new System.Windows.Forms.RadioButton();
			this.modeSmall = new System.Windows.Forms.RadioButton();
			this.ModeGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// charEditor
			// 
			this.charEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.charEditor.BoardSize = 8;
			this.charEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.charEditor.Location = new System.Drawing.Point(144, 12);
			this.charEditor.Name = "charEditor";
			this.charEditor.Size = new System.Drawing.Size(284, 280);
			this.charEditor.TabIndex = 0;
			this.charEditor.CellClick += new GameOfLife.CellClickEventHandler(this.CharEditorCellClick);
			// 
			// prevChar
			// 
			this.prevChar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.prevChar.Location = new System.Drawing.Point(12, 336);
			this.prevChar.Name = "prevChar";
			this.prevChar.Size = new System.Drawing.Size(75, 23);
			this.prevChar.TabIndex = 1;
			this.prevChar.Text = "Prev";
			this.prevChar.UseVisualStyleBackColor = true;
			this.prevChar.Click += new System.EventHandler(this.PrevCharClick);
			// 
			// nextChar
			// 
			this.nextChar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.nextChar.Location = new System.Drawing.Point(353, 336);
			this.nextChar.Name = "nextChar";
			this.nextChar.Size = new System.Drawing.Size(75, 23);
			this.nextChar.TabIndex = 2;
			this.nextChar.Text = "Next";
			this.nextChar.UseVisualStyleBackColor = true;
			this.nextChar.Click += new System.EventHandler(this.NextCharClick);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(12, 306);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Character:";
			// 
			// currChar
			// 
			this.currChar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.currChar.Location = new System.Drawing.Point(73, 306);
			this.currChar.Name = "currChar";
			this.currChar.Size = new System.Drawing.Size(100, 23);
			this.currChar.TabIndex = 4;
			this.currChar.Text = "label2";
			// 
			// buttonLoad
			// 
			this.buttonLoad.Location = new System.Drawing.Point(35, 174);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(77, 23);
			this.buttonLoad.TabIndex = 5;
			this.buttonLoad.Text = "Open ...";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.ButtonLoadClick);
			// 
			// buttonSave
			// 
			this.buttonSave.Location = new System.Drawing.Point(35, 203);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(77, 23);
			this.buttonSave.TabIndex = 6;
			this.buttonSave.Text = "Save ...";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.ButtonSaveClick);
			// 
			// dialogOpenFile
			// 
			this.dialogOpenFile.Filter = "Text Files|*.txt|All Files|*.*";
			this.dialogOpenFile.Title = "Open";
			this.dialogOpenFile.FileOk += new System.ComponentModel.CancelEventHandler(this.DialogOpenFileFileOk);
			// 
			// dialogSaveFile
			// 
			this.dialogSaveFile.Filter = "Text Files|*.txt|All Files|*.*";
			this.dialogSaveFile.Title = "Save";
			this.dialogSaveFile.FileOk += new System.ComponentModel.CancelEventHandler(this.DialogSaveFileFileOk);
			// 
			// buttonExport
			// 
			this.buttonExport.Location = new System.Drawing.Point(35, 232);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(77, 23);
			this.buttonExport.TabIndex = 7;
			this.buttonExport.Text = "Export ...";
			this.buttonExport.UseVisualStyleBackColor = true;
			this.buttonExport.Click += new System.EventHandler(this.ButtonExportClick);
			// 
			// dialogExportFile
			// 
			this.dialogExportFile.Filter = "GCBASIC Library (.h)|*.h|GCSim LCD Font (.dat)|*.dat|All Files|*.*";
			this.dialogExportFile.Title = "Export";
			this.dialogExportFile.FileOk += new System.ComponentModel.CancelEventHandler(this.DialogExportFileFileOk);
			// 
			// ModeGroup
			// 
			this.ModeGroup.Controls.Add(this.label3);
			this.ModeGroup.Controls.Add(this.label2);
			this.ModeGroup.Controls.Add(this.modeLCD);
			this.ModeGroup.Controls.Add(this.modeSmall);
			this.ModeGroup.Location = new System.Drawing.Point(10, 16);
			this.ModeGroup.Name = "ModeGroup";
			this.ModeGroup.Size = new System.Drawing.Size(116, 124);
			this.ModeGroup.TabIndex = 8;
			this.ModeGroup.TabStop = false;
			this.ModeGroup.Text = "Mode";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 95);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 24);
			this.label3.TabIndex = 3;
			this.label3.Text = "(8 x 8, 128 chars)";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "(5 x 7, 32 chars)";
			// 
			// modeLCD
			// 
			this.modeLCD.Location = new System.Drawing.Point(10, 74);
			this.modeLCD.Name = "modeLCD";
			this.modeLCD.Size = new System.Drawing.Size(55, 18);
			this.modeLCD.TabIndex = 1;
			this.modeLCD.Text = "LCD";
			this.modeLCD.UseVisualStyleBackColor = true;
			this.modeLCD.CheckedChanged += new System.EventHandler(this.ModeLCDCheckedChanged);
			// 
			// modeSmall
			// 
			this.modeSmall.Checked = true;
			this.modeSmall.Location = new System.Drawing.Point(10, 26);
			this.modeSmall.Name = "modeSmall";
			this.modeSmall.Size = new System.Drawing.Size(55, 18);
			this.modeSmall.TabIndex = 0;
			this.modeSmall.TabStop = true;
			this.modeSmall.Text = "Small";
			this.modeSmall.UseVisualStyleBackColor = true;
			this.modeSmall.CheckedChanged += new System.EventHandler(this.ModeSmallCheckedChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(440, 371);
			this.Controls.Add(this.ModeGroup);
			this.Controls.Add(this.buttonExport);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonLoad);
			this.Controls.Add(this.currChar);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nextChar);
			this.Controls.Add(this.prevChar);
			this.Controls.Add(this.charEditor);
			this.Name = "MainForm";
			this.Text = "Font Editor";
			this.ModeGroup.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RadioButton modeSmall;
		private System.Windows.Forms.RadioButton modeLCD;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox ModeGroup;
		private System.Windows.Forms.SaveFileDialog dialogExportFile;
		private System.Windows.Forms.Button buttonExport;
		private System.Windows.Forms.SaveFileDialog dialogSaveFile;
		private System.Windows.Forms.OpenFileDialog dialogOpenFile;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Button buttonLoad;
		private System.Windows.Forms.Label currChar;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button nextChar;
		private System.Windows.Forms.Button prevChar;
		private GameOfLife.Board charEditor;
	}
}
