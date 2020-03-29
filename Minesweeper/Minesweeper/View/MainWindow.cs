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

        private readonly GameController mainController;
        private Panel mainBox;

        public MainWindow() {
            InitializeComponent();
            mainController = new GameController();
        }

        private void MainWindow_Load(object sender, EventArgs e) {

            //Display a radio button menu to enable the user to choose a dificulty that is then passed to initBoard()

            difficulty = "easy";

            mainController.Dificulty = difficulty;

            mainController.InitBoard();

            InitViewGrid();

        }

        private void InitViewGrid() {

            int boardSize = mainController.GetBoardSize(difficulty);

            mainBox = new Panel();

            //Cell borders ocupy pixel space in the form, so, while the first cell starts at x = 0 and y = 0
            //the second starts at x = 26 and y = 0 despite the side length being = 25
            //This little offset needs to be taken in to account when drawing the cells so besides the ammount of cells * their side length (CELL_SIZE)
            //we also need to add 1 pixel * the number of cells wich is just boardSize
            mainBox.Width = boardSize * CELL_SIZE + boardSize;
            mainBox.Height = boardSize * CELL_SIZE + boardSize;
            mainBox.BackColor = Color.Transparent;
            mainBox.Name = "mainBox";
            mainBox.BorderStyle = BorderStyle.None;

            Rectangle screenRectangle = RectangleToScreen(ClientRectangle);

            int titleBarHeight = screenRectangle.Top - Top;
            int topOffset = (Height * 15) / 100;

            Width = mainBox.Width + WIDTH_OFFSET;
            Height = mainBox.Height + titleBarHeight + topOffset + HEIGHT_OFFSET;

            mainBox.Top = topOffset;

            CreateGrid();

            Controls.Add(mainBox);
        }

        private void CreateGrid() {

            foreach (Cell c in mainController.GameBoard) {
                Panel cell = new Panel();

                int xOffset = CELL_SIZE * c.xCoord;
                int yOffset = CELL_SIZE * c.yCoord;

                cell.Location = new Point(c.xCoord + xOffset, c.yCoord + yOffset);
                cell.Size = new Size(CELL_SIZE, CELL_SIZE);
                cell.BackColor = Color.SlateGray;
                cell.BorderStyle = BorderStyle.FixedSingle;
                cell.Name = c.xCoord + " " + c.yCoord;
                cell.BackgroundImageLayout = ImageLayout.Center;

                cell.MouseClick += new MouseEventHandler(CellClicked);

                mainBox.Controls.Add(cell);

            }
        }

        private void CellClicked(object sender, MouseEventArgs e) {

            if (!mainController.GameOver) {
                mainController.ClickHandler((Panel)sender, e, mainBox);
            }

        }
    }
}
