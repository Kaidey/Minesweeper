using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.Controllers {
    class GameController {

        private readonly Dictionary<string, Tuple<int,int>> DIFICULTY = new Dictionary<string, Tuple<int,int>>{
            {"easy", new Tuple<int,int>(9,10)},
            {"medium", new Tuple<int,int>(16,40)},
            {"hard", new Tuple<int, int>(30,100)}
        };

        private Cell clickedCell;
        public bool gameOver = false;

        //MultiDimensional array declaration. Cell[][] would be the declaration for a jagged array
        public Cell[,] gameBoard { get; set; }

        public void InitBoard(string mode) {

            int numBombs = DIFICULTY[mode].Item2;
            int numCells = DIFICULTY[mode].Item1;

            gameBoard = new Cell[numCells, numCells];

            for(int i = 0; i < numCells; i++) {
                for(int k = 0; k < numCells; k++) {
                    gameBoard[i, k] = new Cell(i, k, false, false, false);
                } 
            }
            

            Random generator = new Random();

            for (int j = 0; j < numBombs; j++) {
                int x = generator.Next(0, numCells);
                int y = generator.Next(0, numCells);


                if (!gameBoard[x, y].isBomb) {
                    gameBoard[x, y].isBomb = true;
                }

            }

        }

        public int GetBoardSize(string difficulty) {

            return DIFICULTY[difficulty].Item1;
        }

        private List<Cell> GetNeighbours(Cell cell) {

            List<Cell> neighs = new List<Cell>();

            foreach (Cell c in gameBoard) {

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

            return gameBoard[cellX, cellY];
        }

        private Panel GetPanelFromCellObj(Cell cellObj, Panel mainBox) {

            Control[] controls = mainBox.Controls.Find(cellObj.xCoord + " " + cellObj.yCoord, false);

            return (Panel)controls[0];

        }

        public bool CellIsOpen(Panel clickedCell) {

            return GetCellObjectFromPanel(clickedCell).isOpen;
        }

        public void ClickHandler(Panel clickC, MouseEventArgs e, Panel mainBox) {

            clickedCell = GetCellObjectFromPanel(clickC);

            if (!clickedCell.isOpen) {

                switch (e.Button) {
                    case MouseButtons.Right:
                        HandleRightClick(clickC);
                        break;
                    case MouseButtons.Left:
                        HandleLeftClick(clickC, mainBox);
                        break;
                }

            }else if (e.Button == MouseButtons.Middle) {
                HandleMiddleClick( mainBox);
            }

        }

        public void HandleRightClick(Panel clickC) {

            clickC.BackgroundImage = Properties.Resources.flag;
            clickedCell.hasFlag = true;
        }

        public void HandleLeftClick(Panel clickC, Panel mainBox) {

            if (clickedCell.isBomb) {
                RevealCell(clickedCell, mainBox);
                clickC.BackColor = Color.Red;
                GameOverBoard(mainBox);
                gameOver = true;
            } else {

                if (clickedCell.hasFlag) {
                    clickedCell.hasFlag = false;
                    clickC.BackgroundImage = null;
                }

                ExpandCell(clickedCell, mainBox);
            }

        }

        private void ExpandCell(Cell currentCell, Panel mainBox) {

            List<Cell> neighbours = GetNeighbours(currentCell);

            //LINQ
            IEnumerable<Cell> query = from neighbor in neighbours where neighbor.isBomb select neighbor;

            currentCell.numAdjacentBombs = query.Count();

            if (currentCell.numAdjacentBombs == 0) {

                foreach(Cell neigh in neighbours) {

                    neigh.isChecked = true;
                    ExpandCell(neigh, mainBox);
                    RevealCell(currentCell, mainBox);

                }

            } else {

                RevealCell(currentCell, mainBox);

            }

        }

        public void HandleMiddleClick(Panel mainBox) {

            List<Cell> neighs = GetNeighbours(clickedCell);

            IEnumerable<Cell> bombedNeighs = from neighour in neighs where neighour.isBomb select neighour;

            IEnumerable<Cell> notFlagged = from bNeigh in bombedNeighs where !bNeigh.hasFlag select bNeigh;

            if(notFlagged.Count() > 0) {
                GameOverBoard(mainBox);
                notFlagged.ToList().ForEach(cell => GetPanelFromCellObj(cell, mainBox).BackColor = Color.Red);
                gameOver = true;
            } else {

                IEnumerable<Cell> notBombedNeighs = neighs.Except(bombedNeighs);

                foreach(Cell c in notBombedNeighs) {
                    RevealCell(c, mainBox);
                }
            }

        }

        private void RevealCell(Cell toReveal, Panel mainBox) {

            Panel toRevealPanel = GetPanelFromCellObj(toReveal, mainBox);

            
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

        private void GameOverBoard(Panel mainBox) {
            
            foreach(Cell c in gameBoard) {

                if (c.isBomb) {

                    Panel bomb = GetPanelFromCellObj(c, mainBox);

                    bomb.BackgroundImage = Properties.Resources.bomb;

                }

            }

        }

    }
}
