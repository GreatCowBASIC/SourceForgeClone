/*
 * Created by SharpDevelop.
 * User: Hugh
 * Date: 12/04/2010
 * Time: 1:30 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Font_Editor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		
		private enum Mode{
			SMALL, LCD
		}
		
		private Font_Editor.MainForm.Mode mode = Mode.SMALL;
		
		private int currentChar = 1;
		private int[,] charData = new int[129, 8];
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			RedrawChar();
		}
		
		void PrevCharClick(object sender, EventArgs e)
		{
			if (currentChar > 1){
				currentChar -= 1;
				RedrawChar();
			}
		}
		
		void NextCharClick(object sender, EventArgs e)
		{
			if ((mode == Mode.SMALL && currentChar < 38) || (mode == Mode.LCD && currentChar < 127)){
				currentChar += 1;
				RedrawChar();
			}
		}
		
		void CharEditorCellClick(object sender, GameOfLife.CellClickEventArgs args)
		{
			
			//Small mode, 5 x 7 characters
			if (mode == Mode.SMALL) {
				//Disallow anything outside of 5 x 7
				if (args.X < 3 || args.Y > 6) return;
				
				//Toggle colour
				if (charEditor.getCell(args.X, args.Y) == Color.White){
					//Set
					charEditor.setCell(args.X, args.Y, Color.Black);
					charData[currentChar, args.X] = charData[currentChar, args.X] | (int)Math.Pow(2, args.Y);
				} else {
					//Clear
					charEditor.setCell(args.X, args.Y, Color.White);
					charData[currentChar, args.X] = charData[currentChar, args.X] & ~(int)Math.Pow(2, args.Y);
				}
			}
			
			//LCD mode, 8 x 8 characters
			if (mode == Mode.LCD) {
				//Toggle colour
				if (charEditor.getCell(args.X, args.Y) == Color.White){
					//Set
					charEditor.setCell(args.X, args.Y, Color.Black);
					charData[currentChar, args.X] = charData[currentChar, args.X] | (int)Math.Pow(2, args.Y);
				} else {
					//Clear
					charEditor.setCell(args.X, args.Y, Color.White);
					charData[currentChar, args.X] = charData[currentChar, args.X] & ~(int)Math.Pow(2, args.Y);
				}
			}
		}
		
		void ButtonLoadClick(object sender, EventArgs e)
		{
			dialogOpenFile.ShowDialog();
		}
		
		void ButtonSaveClick(object sender, EventArgs e)
		{
			dialogSaveFile.ShowDialog();
		}
		
		void DialogOpenFileFileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			LoadData(dialogOpenFile.FileName);
		}
		
		void DialogSaveFileFileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveData(dialogSaveFile.FileName);
		}
		
		void SaveData(string fileName){
			//Write string to file
			StreamWriter writeFile = new StreamWriter(fileName);
			//writeFile.WriteLine(outData);
			
			int maxChar = 39;
			if (mode == Mode.LCD) maxChar = 128;
			
			for (int currChar = 1; currChar < maxChar; currChar ++){
				writeFile.WriteLine("Character: " + getLetter(currChar) + " (" + currChar.ToString() + ")");
				for (int currCol = 0; currCol < 8; currCol ++){
					writeFile.WriteLine(charData[currChar, currCol]);
				}
			}
						
			writeFile.Close();
		}
		
		void LoadData(string fileName){
			//Open file, init variables
			StreamReader readFile = new StreamReader(fileName);
	   		string tempData;
	   		
	   		mode = Mode.SMALL;
	   		
			//Read File
			for (int currChar = 1; currChar <= 127; currChar ++){
				//writeFile.WriteLine("Character: " + getLetter(currChar) + " (" + currChar.ToString() + ")");
				if (readFile.Peek() != -1) readFile.ReadLine().Trim().ToLower();
				
				for (int currCol = 0; currCol < 8; currCol ++){
					//writeFile.WriteLine(charData[currChar, currCol]);
					if (readFile.Peek() != -1){
						tempData = readFile.ReadLine().Trim().ToLower();
						charData[currChar, currCol] = Convert.ToInt32(tempData);
						if (currChar > 38) mode = Mode.LCD;
					}
				}
			}
			
			readFile.Close();
			
			//Redraw
			RedrawChar();
		}
		
		string getLetter(int charNumber){
			
			if (mode == Mode.SMALL){
				if (charNumber > 0 && charNumber < 27){
					return ((char)('A' + charNumber - 1)).ToString();
				} else if (charNumber > 26 && charNumber < 37){
					return ((char)('0' + charNumber - 27)).ToString();
				} else if (charNumber == 37){
					return "!";
				} else if (charNumber == 38){
					return ".";
				}
			
			} else {
				if (charNumber >= 32 && charNumber <= 127){
					return ((char)charNumber).ToString();
				} else {
					return "";
				}
			}
			
			return "";
		}
		
		void RedrawChar(){
			//Show char number
			currChar.Text = getLetter(currentChar) + " (" + currentChar.ToString() + ")";
			
			//Set mode display
			if (mode == Mode.SMALL){
				modeSmall.Checked = true;
				
			}else if (mode == Mode.LCD){
				modeLCD.Checked = true;
			}
			
			//Clear character
			charEditor.clearBoard();
			
			//Reload character
			for (int x = 0; x < 8; x ++){
				for (int y = 0; y < 8; y ++){
					
					if ((charData[currentChar, x] & (int)Math.Pow(2, y)) != 0){
						//Set
						charEditor.setCell(x, y, Color.Black);
					} else {
						//Clear
						charEditor.setCell(x, y, Color.White);
					}
					
				}
			}
			
		}
		
		void ButtonExportClick(object sender, EventArgs e)
		{
			dialogExportFile.ShowDialog();
		}
		
		void DialogExportFileFileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int[] rows = new int[8];
			
			//Find saved file type
			//Should be either dat (simulator font) or h (GCBASIC library)
			String fileType = dialogExportFile.FileName;
			if (fileType.LastIndexOf('.') != -1){
				fileType = fileType.Substring(fileType.LastIndexOf('.') + 1).Trim().ToLower();
			}
			
			//Need to create tables
			//Write string to file
			StreamWriter writeFile = new StreamWriter(dialogExportFile.FileName);
			
			//Number of characters to write
			int outCharStart = 0;
			int outCharCount = 39;
			int outColStart = 0;
			if (mode == Mode.LCD){
				outCharStart = 16;
				outCharCount = 128;
				outColStart = 3;
			}
			
			//if (mode == Mode.SMALL){
			if (fileType == "h"){
				writeFile.WriteLine(";Library for font character bitmaps");
				for (int currCol = outColStart; currCol < 8; currCol ++){
					writeFile.WriteLine();
					writeFile.WriteLine("Table CharCol" + currCol.ToString());
					for (int currChar = outCharStart; currChar < outCharCount; currChar ++){
						writeFile.WriteLine(charData[currChar, currCol]);
					}
					writeFile.WriteLine("End Table");
				}
			
			//} else if (mode == Mode.LCD) {
			} else if (fileType == "dat") {
				writeFile.WriteLine("'LCD font for simulator");
				writeFile.WriteLine();
				for (int currChar = outCharStart; currChar < outCharCount; currChar ++){
					writeFile.WriteLine("[Character=" + currChar.ToString() + "]");
					//Transpose columns to rows
					for (int currRow = 0; currRow < 8; currRow ++){
						rows[currRow] = 0;
						for (int currCol = 0; currCol < 8; currCol ++){
							if ((charData[currChar, currCol] & (int)Math.Pow(2, currRow)) != 0){
								rows[currRow] |= (int)Math.Pow(2, 7 - currCol);
							}
						}
					}
					
					//Write rows
					for (int currRow = 0; currRow < 8; currRow ++){
						writeFile.WriteLine(rows[currRow]);
					}
					writeFile.WriteLine();
				}
			}
			
			writeFile.Close();
			
		}
		
		void ModeSmallCheckedChanged(object sender, EventArgs e)
		{
			mode = Mode.SMALL;
			
			//Check current character
			if (currentChar > 38){
				currentChar = 38;
				RedrawChar();
			}
			
			currChar.Text = getLetter(currentChar) + " (" + currentChar.ToString() + ")";
		}
		
		void ModeLCDCheckedChanged(object sender, EventArgs e)
		{
			mode = Mode.LCD;
			
			//Check current character
			
			currChar.Text = getLetter(currentChar) + " (" + currentChar.ToString() + ")";
		}
	}
}
