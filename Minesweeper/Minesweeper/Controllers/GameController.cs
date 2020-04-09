using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.Controllers {
    class GameController {

        private readonly Dictionary<string, Tuple<int, int>> DIFICULTY = new Dictionary<string, Tuple<int, int>>{
            {"easy", new Tuple<int,int>(9,10)},
            {"medium", new Tuple<int,int>(16,40)},
            {"hard", new Tuple<int, int>(30,100)},
            {"test", new Tuple<int, int>(4,1) }
        };

        private int _revealedCells;

        private Cell _clicedCell;

        private readonly Form _mainWindow;

        private Panel _gameBoard;

        private Label _mineCountDisplay;

        private Label _timer;

        private DateTime _startTime;

        public double ElapsedTime;

        private Timer _gameTimer { get; set; }

        public bool GameOver {get; set; }

        public int IdentifiedMines { get; set; }

        public string Dificulty { get; set; }

        public GameController(Form mw) {
            _revealedCells = 0;
            _mainWindow = mw;
            _gameTimer = new Timer();
            _gameTimer.Interval = 1000;
            _gameTimer.Tick += new EventHandler(TickHandler);
        }

        //MultiDimensional array declaration. Cell[][] would be the declaration for a jagged array
        public Cell[,] GameBoard { get; set; }

        public void InitBoard() {

            int numBombs = DIFICULTY[Dificulty].Item2;
            int numCells = DIFICULTY[Dificulty].Item1;

            GameBoard = new Cell[numCells, numCells];

            for(int i = 0; i < numCells; i++) {
                for(int k = 0; k < numCells; k++) {
                    GameBoard[i, k] = new Cell(i, k, false, false, false);
                } 
            }


            Random generator = new Random();

            for (int j = 0; j < numBombs; j++) {
                int x = generator.Next(0, numCells);
                int y = generator.Next(0, numCells);


                if (!GameBoard[x, y].isBomb) {
                    GameBoard[x, y].isBomb = true;
                }

            }

            IdentifiedMines = DIFICULTY[Dificulty].Item2;

        }

        public void UpdateUiElements() {
            Control[] board = _mainWindow.Controls.Find("gameBoard", false);

            _gameBoard = (Panel)board[0];

            Control[] count = _mainWindow.Controls.Find("mineCountDisplay", false);

            _mineCountDisplay = (Label)count[0];

            Control[] timer = _mainWindow.Controls.Find("timer", false);

            _timer = (Label)timer[0];
        }

        public int GetBoardSize(string difficulty) {

            return DIFICULTY[difficulty].Item1;
        }

        private List<Cell> GetNeighbours(Cell cell) {

            List<Cell> neighs = new List<Cell>();

            foreach (Cell c in GameBoard) {

                if (!c.isChecked) {
                    int xDiff = Math.Abs(cell.xCoord - c.xCoord);
                    int yDiff = Math.Abs(cell.yCoord - c.yCoord);

                    //Get diagonal neighbours
                    if (xDiff == 1 && yDiff == 1) {
                        neighs.Add(c);
                        //Get up, down, right, left neighbours
                    } else if (xDiff == 1 && yDiff == 0 || xDiff == 0 && yDiff == 1) {
                        neighs.Add(c);
                    }
                }

            }

            return neighs;

        }

        private Cell GetCellObjectFromPanel(Panel panel) {

            string[] cellCoords = panel.Name.Split(' ');

            int cellX = Convert.ToInt32(cellCoords[0]);
            int cellY = Convert.ToInt32(cellCoords[1]);

            return GameBoard[cellX, cellY];
        }

        private Panel GetPanelFromCellObj(Cell cellObj, Panel _gameBoard) {

            Control[] controls = _gameBoard.Controls.Find(cellObj.xCoord + " " + cellObj.yCoord, false);

            return (Panel)controls[0];

        }

        private void TickHandler(object sender, EventArgs e) {


            TimeSpan newTime = DateTime.Now - _startTime;

            ElapsedTime = Math.Floor(newTime.TotalSeconds);

            _timer.Invalidate();
        }

        public void StartTimer() {

            _startTime = DateTime.Now;
            _gameTimer.Start();
        }

        public bool CellIsOpen(Panel _clicedCell) {

            return GetCellObjectFromPanel(_clicedCell).isOpen;
        }

        public void ClickHandler(Panel clickC, MouseEventArgs e) {

            _clicedCell = GetCellObjectFromPanel(clickC);
            _mineCountDisplay.Invalidate();

            if (!_clicedCell.isOpen) {

                switch (e.Button) {
                    case MouseButtons.Right:
                        HandleRightClick(clickC);
                        break;
                    case MouseButtons.Left:
                        HandleLeftClick(clickC);
                        break;
                }

            }else if (e.Button == MouseButtons.Middle) {
                HandleMiddleClick();
            }

        }

        public void HandleRightClick(Panel clickC) {

            _clicedCell = GetCellObjectFromPanel(clickC);

            if (_clicedCell.hasFlag) {
                clickC.BackgroundImage = null;
                _clicedCell.hasFlag = false;
                IdentifiedMines++;
            } else {
                clickC.BackgroundImage = Properties.Resources.flag;
                _clicedCell.hasFlag = true;
                IdentifiedMines--;
            }

            _mineCountDisplay.Invalidate();

        }

        public void HandleLeftClick(Panel clickC) {

            int cellAmmount = (int)Math.Pow(DIFICULTY[Dificulty].Item1, 2);
            int bombAmmount = DIFICULTY[Dificulty].Item2;


            if((cellAmmount - bombAmmount) == _revealedCells) {
                //Game Won Screen
                _gameTimer.Stop();
                Application.Exit();
            }

            if (_clicedCell.isBomb) {
                RevealCell(_clicedCell);
                GameOverBoard( _clicedCell);
                GameOver = true;
            } else {

                if (_clicedCell.hasFlag) {
                    _clicedCell.hasFlag = false;
                    clickC.BackgroundImage = null;
                }

                ExpandCell(_clicedCell);
            }

        }

        private void ExpandCell(Cell currentCell) {

            List<Cell> neighbours = GetNeighbours(currentCell);

            //LINQ
            IEnumerable<Cell> query = from neighbor in neighbours where neighbor.isBomb select neighbor;

            currentCell.numAdjacentBombs = query.Count();

            if (currentCell.numAdjacentBombs == 0) {

                foreach(Cell neigh in neighbours) {

                    neigh.isChecked = true;
                    ExpandCell(neigh);

                }

            }

            if (!currentCell.isOpen) {
                RevealCell(currentCell);
            }
        }

        public void HandleMiddleClick() {

            List<Cell> neighs = GetNeighbours(_clicedCell);

            IEnumerable<Cell> bombedNeighs = from neighour in neighs where neighour.isBomb select neighour;

            IEnumerable<Cell> notFlagged = from bNeigh in bombedNeighs where !bNeigh.hasFlag select bNeigh;

            if(notFlagged.Count() > 0) {
                GameOverBoard(null);
                notFlagged.ToList().ForEach(cell => GetPanelFromCellObj(cell, _gameBoard).BackColor = Color.Red);
                GameOver = true;
            } else {

                IEnumerable<Cell> notBombedNeighs = neighs.Except(bombedNeighs);

                foreach(Cell c in notBombedNeighs) {
                    RevealCell(c);
                }
            }

        }

        private void RevealCell(Cell toReveal) {

            Panel toRevealPanel = GetPanelFromCellObj(toReveal, _gameBoard);

            _revealedCells++;
            
            toRevealPanel.BorderStyle = BorderStyle.None;
            toRevealPanel.BackColor = Color.Gray;
            toReveal.isOpen = true;

            switch (toReveal.numAdjacentBombs) {

                case 1:
                    toRevealPanel.BackgroundImage = Properties.Resources._1;
                    break;
                case 2:
                    toRevealPanel.BackgroundImage = Properties.Resources._2;
                    break;
                case 3:
                    toRevealPanel.BackgroundImage = Properties.Resources._3;
                    break;
                case 4:
                    toRevealPanel.BackgroundImage = Properties.Resources._4;
                    break;
                case 5:
                    toRevealPanel.BackgroundImage = Properties.Resources._5;
                    break;
                case 6:
                    toRevealPanel.BackgroundImage = Properties.Resources._6;
                    break;
                case 7:
                    toRevealPanel.BackgroundImage = Properties.Resources._7;
                    break;
                case 8:
                    toRevealPanel.BackgroundImage = Properties.Resources._8;
                    break;
                default:
                    break;
            }
        }

        private void GameOverBoard(Cell bombClicked) {

            foreach (Cell c in GameBoard) {

                if (c.isBomb) {

                    Panel bomb = GetPanelFromCellObj(c, _gameBoard);

                    bomb.BackgroundImage = Properties.Resources.bomb;
                    bomb.BorderStyle = BorderStyle.None;

                    if (c == bombClicked) {
                        bomb.BackColor = Color.Red;

                    } else {

                        bomb.BackColor = Color.Gray;
                    }
                }

            }

        }

    }
}
