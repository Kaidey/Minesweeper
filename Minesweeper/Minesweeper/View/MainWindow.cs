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

        private const int INFO_BAR_OFFSET = 15;
        private const int CELL_SIZE = 25;
        private const int WIDTH_OFFSET = 16;
        private const int HEIGHT_OFFSET = 8;
        private string difficulty;

        private readonly GameController mainController;
        private Panel _gameBoard;
        private Label _mineCountDisplay;
        private Label _timer;

        public MainWindow() {
            InitializeComponent();
            mainController = new GameController(this);
        }

        private void MainWindow_Load(object sender, EventArgs e) {

            //Display a radio button menu to enable the user to choose a dificulty that is then passed to initBoard()

            difficulty = "medium";

            mainController.Dificulty = difficulty;

            mainController.InitBoard();

            InitViewGrid();
            CreateMineCountDisplay();
            CreateTimer();

            mainController.UpdateUiElements();
            mainController.StartTimer();

        }

        private void CreateTimer() {

            _timer = new Label() {
                Size = new Size(35, 35),
                BackColor = Color.Black,
                Name = "timer",
                Left = 5,
                Top = 5,
                BorderStyle = BorderStyle.Fixed3D,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10),
                ForeColor = Color.Red
            };

            _timer.Invalidated += new InvalidateEventHandler((object sender, InvalidateEventArgs e) => {
                Label display = (Label)sender;
                display.Text = Convert.ToString(mainController.ElapsedTime);
            });

            _timer.Location = new Point(Width - 75, 0);

            Controls.Add(_timer);
        }

        private void CreateMineCountDisplay() {

            _mineCountDisplay = new Label {
                Size = new Size(35, 35),
                BackColor = Color.Black,
                Name = "mineCountDisplay",
                Left = 5,
                Top = 5,
                BorderStyle = BorderStyle.Fixed3D,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10),
                ForeColor = Color.Red

            };

            _mineCountDisplay.Invalidated += new InvalidateEventHandler((object sender, InvalidateEventArgs e) => { 
                Label display = (Label)sender;
                display.Text = Convert.ToString(mainController.IdentifiedMines);
            });

            Controls.Add(_mineCountDisplay);
        }

        private void InitViewGrid() {

            int boardSize = mainController.GetBoardSize(difficulty);

            _gameBoard = new Panel {

                //Cell borders ocupy pixel space in the form, so, while the first cell starts at x = 0 and y = 0
                //the second starts at x = 26 and y = 0 despite the side length being = 25
                //This little offset needs to be taken in to account when drawing the cells so besides the ammount of cells * their side length (CELL_SIZE)
                //we also need to add 1 pixel * the number of cells wich is just boardSize
                Width = boardSize * CELL_SIZE + boardSize,
                Height = boardSize * CELL_SIZE + boardSize,
                BackColor = Color.Transparent,
                Name = "gameBoard",
                BorderStyle = BorderStyle.None
            };

            Rectangle screenRectangle = RectangleToScreen(ClientRectangle);

            int titleBarHeight = screenRectangle.Top - Top;
            int topOffset = (Height * INFO_BAR_OFFSET) / 100;

            Width = _gameBoard.Width + WIDTH_OFFSET;
            Height = _gameBoard.Height + titleBarHeight + topOffset + HEIGHT_OFFSET;

            _gameBoard.Top = topOffset;

            CreateGrid();

            Controls.Add(_gameBoard);
        }

        private void CreateGrid() {

            foreach (Cell c in mainController.GameBoard) {

                int xOffset = CELL_SIZE * c.xCoord;
                int yOffset = CELL_SIZE * c.yCoord;

                Panel cell = new Panel {
                    Location = new Point(c.xCoord + xOffset, c.yCoord + yOffset),
                    Size = new Size(CELL_SIZE, CELL_SIZE),
                    BackColor = Color.SlateGray,
                    BorderStyle = BorderStyle.FixedSingle,
                    Name = c.xCoord + " " + c.yCoord,
                    BackgroundImageLayout = ImageLayout.Center
                };

                cell.MouseClick += new MouseEventHandler(CellClicked);

                _gameBoard.Controls.Add(cell);

            }
        }

        private void CellClicked(object sender, MouseEventArgs e) {

            if (!mainController.GameOver) {
                mainController.ClickHandler((Panel)sender, e);
            }
        }
    }
}
