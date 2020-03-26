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
        }

        private void createGrid() {

            Color backColor;

            foreach (Cell c in mainController.gameBoard) {
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

            switch (e.Button) {
                case MouseButtons.Right:
                    mainController.handleRightClick(clickedCell, mainBox);
                    break;
                case MouseButtons.Left:
                    mainController.handleLeftClick(clickedCell, mainBox);
                    break;
            }
        }
    }
}
