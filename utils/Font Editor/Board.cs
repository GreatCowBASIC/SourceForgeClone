/*
 * Created by SharpDevelop.
 * User: Hugh
 * Date: 25/05/2009
 * Time: 3:39 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
	
	/// <summary>
	/// Description of Board.
	/// </summary>
	public partial class Board : UserControl
	{
		
		/// <summary>
		/// Event that is fired when a cell on the Board is clicked
		/// </summary>
		public event CellClickEventHandler CellClick;
		
		/// <summary>
		/// Triggers the CellClick event
		/// </summary>
		/// <param name="x">X location of clicked cell</param>
		/// <param name="y">Y location of clicked cell</param>
		public virtual void OnCellClick(int x, int y){
			//If handler defined, call it
			if (CellClick != null){
				CellClick(this, new CellClickEventArgs(x, y));
			}
		}
		
		private Color[,] boardState;
		private int boardSize;
		/// <summary>
		/// Stores the size of the board. Will resize the board as needed.
		/// </summary>
		public int BoardSize{
			get{return boardSize;}
			set{
				int oldSize = boardSize;
				boardSize = value;
				
				//Check new value
				if (boardSize == 0) boardSize = 1;
				if (boardSize < 0) boardSize = boardSize * -1;
				
				//Create new board if size changed
				if (oldSize != boardSize){
					Color[,] oldState = boardState;
					newBoard();
					setCells(oldState);
				}
			}
		}
		
		/// <summary>
		/// Creates a new board, clearing all existing cells.
		/// </summary>
		private void newBoard(){
			boardState = new Color[boardSize, boardSize];
			for (int x = 0; x < boardSize; x ++){
				for (int y = 0; y < boardSize; y ++){
					boardState[x, y] = Color.White;
				}
			}
			
			this.Invalidate();
		}
		
		/// <summary>
		/// Sets the colour of an individual cell
		/// </summary>
		/// <param name="x">X location of cell to set</param>
		/// <param name="y">Y location of cell to set</param>
		/// <param name="newColour">Colour to set cell to</param>
		/// <exception cref="IndexOutOfRangeException">Thrown when x or y is too big or too small</exception>
		public void setCell(int x, int y, Color newColour){
			//Check x and y
			if (x < 0 || y < 0 || x >= boardSize || y >= boardSize) {
				throw new IndexOutOfRangeException();
			}
			
			//Set the cell
			boardState[x, y] = newColour;
			
			//Redraw
			this.Invalidate();
		}
		
		/// <summary>
		/// Sets the colours of all cells on the board to those contained in the array.
		/// If the array is not the correct size, then the largest number of cells will
		/// be copied. If newColours is too large, then only the top left will be shown.
		/// If newColours is too small, then only the area of the board that it overlaps
		/// will be updated, and the rest of the board will remain unchanged.
		/// </summary>
		/// <param name="newColours">Array of new Colors to display. Must be square.</param>
		public void setCells(Color[,] newColours){
			//If boardState is null, copy
			if (boardState == null){
				boardState = newColours;
				return;
			//If new state is null, exit without copying
			} else if (newColours == null){
				return;
			}
			
			//If size is same, copy reference
			if (boardState.Length == newColours.Length){
				boardState = newColours;
			} else {
				//Get smallest board
				int oldSize = (int)Math.Sqrt(boardState.Length);
				int newSize = (int)Math.Sqrt(newColours.Length);
				int smallSize = oldSize;
				if (newSize < smallSize) smallSize = newSize;
				
				//Then copy cells
				for (int x = 0; x < smallSize; x ++){
					for (int y = 0; y < smallSize; y ++){
						boardState[x, y] = newColours[x, y];
					}
				}
				
			}
			this.Invalidate();
		}
		
		/// <summary>
		/// Gets the colour of the selected cell
		/// </summary>
		/// <param name="x">X location of cell to read</param>
		/// <param name="y">Y location of cell to read</param>
		/// <returns>Colour of the cell</returns>
		public Color getCell(int x, int y){
			//Check x and y
			if (x < 0 || y < 0 || x >= boardSize || y >= boardSize) {
				throw new IndexOutOfRangeException();
			}
			
			//Return colour
			return boardState[x, y];
			
		}
		
		/// <summary>
		/// Clears the board, setting all cells to Color.White
		/// </summary>
		public void clearBoard(){
			//Clear board
			for (int x = 0; x < boardSize; x ++){
				for (int y = 0; y < boardSize; y ++){
					boardState[x, y] = Color.White;
				}
			}
			
			//Redraw
			this.Invalidate();
		}
		
		/// <summary>
		/// Constructor for Board. Size will default to 10 x 10 squares.
		/// </summary>
		public Board()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//Set default board size
			BoardSize = 10;
		}
		
		/// <summary>
		/// Custom paint method. Draws the cells on the board.
		/// </summary>
		/// <param name="e"></param>
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			//Get Graphics object to paint
			Graphics g = e.Graphics;
			
			double rectX, rectY;
			rectX = this.Size.Width / (double)boardSize;
			rectY = this.Size.Height / (double)boardSize;
			
			//Paint the rectangles
			for (int x = 0; x < boardSize; x ++){
				for (int y = 0; y < boardSize; y ++){
					g.FillRectangle(new SolidBrush(boardState[x, y]),
					                new Rectangle((int)(x * rectX), (int)(y * rectY),
					                              (int)rectX, (int)rectY));
				}
			}
			
			//Paint the dividing lines
			for (int x = 1; x < boardSize; x ++){
				g.DrawLine(new Pen(Color.Gray), (int)(x * rectX - 1), 0, (int)(x * rectX - 1), this.Size.Height);
			}
			for (int y = 1; y < boardSize; y ++){
				g.DrawLine(new Pen(Color.Gray), 0, (int)(y * rectY - 1), this.Size.Width, (int)(y * rectY - 1));
			}
			
		}
		
		
		void BoardResize(object sender, EventArgs e)
		{
			//Force redraw on resize
			this.Invalidate();
		}
		
		
		void BoardMouseClick(object sender, MouseEventArgs e)
		{
			//Get the cell that has been clicked
			//Get size of each cell
			double rectX, rectY;
			rectX = this.Size.Width / (double)boardSize;
			rectY = this.Size.Height / (double)boardSize;
			//Then get cell
			int clickedX, clickedY;
			clickedX = (int)((e.X + 1)/ rectX);
			clickedY = (int)((e.Y + 1) / rectY);
			
			//Fire event
			OnCellClick(clickedX, clickedY);
		}
		
	}
	
	/// <summary>
	/// Stores location of clicked cell
	/// </summary>
	public class CellClickEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor for Cell Click Event Args
		/// </summary>
		/// <param name="x">X location (column) of cell that has been clicked</param>
		/// <param name="y">Y location (row) of cell that has been clicked</param>
		public CellClickEventArgs(int x, int y){
			this.x = x;
			this.y = y;
		}
		
		private int x;
		/// <summary>
		/// X location (column)
		/// </summary>
		public int X{
			get {return x;}
			set {x = value;}
		}
		
		private int y;
		/// <summary>
		/// Y location (row)
		/// </summary>
		public int Y{
			get {return y;}
			set {y = value;}
		}
	}
	
	/// <summary>
	/// Delegate sub to handle CellClick event.
	/// </summary>
	public delegate void CellClickEventHandler(object sender, CellClickEventArgs args);
	
}
