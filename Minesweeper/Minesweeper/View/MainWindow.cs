using Minesweeper.Controllers;
using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper {
    public partial class MainWindow : Form {

        private const int CELL_SIZE = 25;
        private const int WIDTH_OFFSET = 16;
        private const int HEIGHT_OFFSET = 8;
        private string difficulty;

        private GameController mainController;
        private Panel mainBox;
        
        public MainWindow() {
            InitializeComponent();
            mainController = new GameController();
        }

        private void MainWindow_Load(object sender, EventArgs e) {

            //Display a radio button menu to enable the user to choose a dificulty that is then passed to initBoard()

            difficulty = "hard";

            mainController.initBoard(difficulty);

            initViewGrid();
            
        }

        private void initViewGrid() {

            int boardSize = mainController.getBoardSize(difficulty);

            mainBox = new Panel();

            //Cell borders ocupy pixel space in the form, so, while the first cell starts at x = 0 and y = 0
            //the second starts at x = 26 and y = 0 despite the side length being = 25
            //This little offset needs to be taken in to account when drawing the cells so besides the ammount of cells * their side length (CELL_SIZE)
            //we also need to add 1 pixel * the number of cells wich is just boardSize
            mainBox.Width = boardSize * CELL_SIZE + boardSize;
            mainBox.Height = boardSize * CELL_SIZE + boardSize;
            mainBox.BackColor = Color.Black;
            mainBox.Name = "mainBox";
            mainBox.BorderStyle = BorderStyle.None;

            Rectangle screenRectangle = RectangleToScreen(ClientRectangle);

            int titleBarHeight = screenRectangle.Top - Top;
            int topOffset = (Height * 15) / 100;

            Width = mainBox.Width + WIDTH_OFFSET;
            Height = mainBox.Height + titleBarHeight + topOffset + HEIGHT_OFFSET;

            mainBox.Top = topOffset;

            createGrid();

            Controls.Add(mainBox);

            Console.WriteLine(Bounds);
        }

        private void createGrid() {

            Color backColor;
            
            foreach(Cell c in mainController.gameBoard) {
                Panel cell = new Panel();

                backColor = Color.Gray;
                int xOffset = CELL_SIZE * c.xCoord;
                int yOffset = CELL_SIZE * c.yCoord;

                cell.Location = new Point(c.xCoord + xOffset, c.yCoord + yOffset);
                cell.Size = new Size(CELL_SIZE, CELL_SIZE);
                cell.BackColor = backColor;
                cell.BorderStyle = BorderStyle.Fixed3D;
                cell.MouseClick += new MouseEventHandler(this.cellClicked);
                cell.Name = c.xCoord + " " + c.yCoord;
                cell.BackgroundImageLayout = ImageLayout.Center;

                mainBox.Controls.Add(cell);

            }
        }

        private void cellClicked(object sender, MouseEventArgs e) {
            Panel clickedCell = (Panel)sender;

            string[] cellCoords = clickedCell.Name.Split(' ');

            int cellX = Convert.ToInt32(cellCoords[0]);
            int cellY = Convert.ToInt32(cellCoords[1]);

            switch (e.Button) {
                case MouseButtons.Right:
                    handleRightClick(clickedCell, mainController.getCellByCoords(cellX, cellY));
                    break;
                case MouseButtons.Left:
                    handleLeftClick(clickedCell, mainController.getCellByCoords(cellX, cellY));
                    break;
            }
        }

        private void handleRightClick(Panel clickedCell, Cell clickedCellObj) {

            if (!clickedCellObj.isOpen) {
                clickedCell.BackgroundImage = Properties.Resources.flag;
            }
        }

        private void handleLeftClick(Panel clickedCell, Cell clickedCellObj) {

            clickedCell.BorderStyle = BorderStyle.None;
            clickedCellObj.isOpen = true;

            if (clickedCellObj.isBomb) {
                gameOverBoard(clickedCell);
            } else {
               
                revealNoBombNeighbours(clickedCellObj);
            }

        }

        private void revealNoBombNeighbours(Cell startingPoint) {

            List<Cell> neighbours = mainController.checkNeighbours(startingPoint);

            int numBombs = mainController.checkForBombs(neighbours, startingPoint);

            if (numBombs == 0) {

                foreach (Cell c in neighbours) {

                    revealCell(c, mainBox, false, numBombs);

                    c.isChecked = true;

                    revealNoBombNeighbours(c);

                }
                
            } else {
                revealCell(startingPoint, mainBox, true, numBombs);
            }


        }

        private void revealCell (Cell c, Panel mainBox, bool hasBombedNeighs, int numBombs) {

            Control[] mainBoxControls = mainBox.Controls.Find(c.xCoord + " " + c.yCoord, false);

            Panel panel = (Panel)mainBoxControls[0];

            panel.BorderStyle = BorderStyle.None;
            c.isOpen = true;

            if (hasBombedNeighs) {
                Label numBombsDisplay = new Label();
                numBombsDisplay.Text = Convert.ToString(numBombs);
                panel.Controls.Add(numBombsDisplay);
            }
        }

        private void gameOverBoard(Panel clickedCell) {
            //Reveal all bombs
            clickedCell.BackgroundImage = Properties.Resources.bomb;

        }

        private void MainWindow_ResizeEnd(object sender, EventArgs e) {
            Console.WriteLine(Bounds);
        }
    }
}
