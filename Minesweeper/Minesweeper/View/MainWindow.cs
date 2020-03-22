using Minesweeper.Controllers;
using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper {
    public partial class MainWindow : Form {

        private GameController mainController;
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

            PictureBox mainBox = new PictureBox();
            mainBox.Width = Width;
            mainBox.Height = Height;
            mainBox.Top = (Height * 10) / 100;
            mainBox.BackColor = Color.Black;
            createGrid(mainBox);

            Controls.Add(mainBox);
        }

        private void createGrid(PictureBox mainBox) {

            Color backColor;
            
            foreach(Cell c in mainController.gameBoard) {
                PictureBox cell = new PictureBox();

                backColor = Color.Gray;
                cell.Location = new Point(c.xCoord + (25 * c.xCoord), c.yCoord + (25 * c.yCoord));
                cell.Size = new Size(25, 25);
                cell.BackColor = backColor;
                cell.BorderStyle = BorderStyle.Fixed3D;
                cell.Click += new EventHandler(this.cellClicked);
                cell.Name = c.xCoord + " " + c.yCoord;
                mainBox.Controls.Add(cell);

            }
        }

        private void cellClicked(object sender, EventArgs e) {
            PictureBox clickedCell = (PictureBox)sender;

            string[] cellCoords = clickedCell.Name.Split(' ');

            int cellX = Convert.ToInt32(cellCoords[0]);
            int cellY = Convert.ToInt32(cellCoords[1]);

            Cell clickedCellObj = mainController.gameBoard[cellX, cellY];

            if (clickedCellObj.isBomb) {
                clickedCell.BorderStyle = BorderStyle.None;
                clickedCell.BackColor = Color.Black;
            }
        }
    }
}
