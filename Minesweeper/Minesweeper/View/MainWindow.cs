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

        private GameController mainController;
        private Panel mainBox;
        public MainWindow() {
            InitializeComponent();
            mainController = new GameController();
        }

        private void MainWindow_Load(object sender, EventArgs e) {

            //Display a radio button menu to enable the user to choose a dificulty that is then passed to initBoard()

            mainController.initBoard("easy");

            /*foreach(Cell x in mainController.gameBoard) {
                Console.WriteLine(x.isBomb);
            }*/

            initViewGrid();
        }

        private void initViewGrid() {

            mainBox = new Panel();
            mainBox.Width = Width;
            mainBox.Height = Height;
            mainBox.Top = (Height * 10) / 100;
            mainBox.BackColor = Color.Black;
            mainBox.Name = "mainBox";
            createGrid();

            Controls.Add(mainBox);
        }

        private void createGrid() {

            Color backColor;
            
            foreach(Cell c in mainController.gameBoard) {
                Panel cell = new Panel();

                backColor = Color.Gray;
                cell.Location = new Point(c.xCoord + (25 * c.xCoord), c.yCoord + (25 * c.yCoord));
                cell.Size = new Size(25, 25);
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

            if(e.Button == MouseButtons.Right) {
                handleRightClick(clickedCell, mainController.getCellByCoords(cellX, cellY));
            } else {
                handleLeftClick(clickedCell, mainController.getCellByCoords(cellX, cellY));
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
    }
}
