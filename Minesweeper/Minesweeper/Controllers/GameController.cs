using Minesweeper.Model;
using System;
using System.Collections.Generic;
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

        //MultiDimensional array declaration. Cell[][] would be the declaration for a jagged array
        public Cell[,] gameBoard { get; set; }

        public void initBoard(string mode) {

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

        public int getBoardSize(string difficulty) {

            return DIFICULTY[difficulty].Item1;
        }

        public List<Cell> checkNeighbours(Cell cell) {

            List<Cell> neighs = new List<Cell>();

            /*new List<Cell> {
                gameBoard[cell.xCoord, cell.yCoord - 1],

                gameBoard[cell.xCoord, cell.yCoord + 1],

                gameBoard[cell.xCoord - 1, cell.yCoord],

                gameBoard[cell.xCoord + 1, cell.yCoord],

                gameBoard[cell.xCoord - 1, cell.yCoord - 1],

                gameBoard[cell.xCoord + 1, cell.yCoord - 1],

                gameBoard[cell.xCoord - 1, cell.yCoord + 1],

                gameBoard[cell.xCoord + 1, cell.yCoord + 1]
            };*/

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

        public int checkForBombs(List<Cell> neighbours, Cell clicked) {

            int bombCounter = 0;

            foreach(Cell c in neighbours) {
                if (c.isBomb) {
                    bombCounter++;
                }
            }

            return bombCounter;

        }

        public void handleRightClick(Panel clickedCell, Panel mainBox) {

            string[] cellCoords = clickedCell.Name.Split(' ');

            int cellX = Convert.ToInt32(cellCoords[0]);
            int cellY = Convert.ToInt32(cellCoords[1]);

            if (!gameBoard[cellX, cellY].isOpen) {
                clickedCell.BackgroundImage = Properties.Resources.flag;
            }
        }

        public void handleLeftClick(Panel clickedCell, Panel mainBox) {

            string[] cellCoords = clickedCell.Name.Split(' ');

            int cellX = Convert.ToInt32(cellCoords[0]);
            int cellY = Convert.ToInt32(cellCoords[1]);

            clickedCell.BorderStyle = BorderStyle.None;
            gameBoard[cellX, cellY].isOpen = true;

            if (gameBoard[cellX, cellY].isBomb) {
                gameOverBoard(clickedCell);
            } else {

                revealNoBombNeighbours(gameBoard[cellX, cellY], mainBox);
            }

        }

        private void revealNoBombNeighbours(Cell startingPoint, Panel mainBox) {

            List<Cell> neighbours = checkNeighbours(startingPoint);

            int numBombs = checkForBombs(neighbours, startingPoint);

            if (numBombs == 0) {

                foreach (Cell c in neighbours) {

                    revealCell(c, mainBox, false, numBombs);

                    c.isChecked = true;

                    revealNoBombNeighbours(c, mainBox);

                }

            } else {
                revealCell(startingPoint, mainBox, true, numBombs);
            }


        }

        private void revealCell(Cell c, Panel mainBox, bool hasBombedNeighs, int numBombs) {

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
