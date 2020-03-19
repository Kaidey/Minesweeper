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
                    gameBoard[i, k] = new Cell(false, false, false);
                } 
            }
            

            Random generator = new Random();

            for (int j = 0 ; j < numBombs ; j++) {
                int x = generator.Next(0, numCells);
                int y = generator.Next(0, numCells);

                gameBoard[x, y].isBomb = true;
            }

        }

    }
}
