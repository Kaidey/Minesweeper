using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Cell getCellByCoords(int x, int y) {
            return gameBoard[x, y];
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

    }
}
