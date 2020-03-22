using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.Model {
    class Cell {

        //Auto properties create an anonymous backing field that can only be accessed through get and set
        public int numAdjacentBombs { get; set; }

        //The above declaration is the same as the one below

        //private int _numAdjacentBombs;
        //public int numAdjacentBombs{
        //  get {return _numAdjacentBombs;}
        //  set {_numAdjacentBombs = value}
        //}

        public int xCoord { get; set; }
        public int yCoord { get; set; }

        public bool isOpen { get; set; }
        public bool isBomb { get; set; }
        public bool hasFlag { get; set; }

        public Cell(int x, int y, bool open, bool bomb, bool flag) {
            xCoord = x;
            yCoord = y;
            isOpen = open;
            isBomb = bomb;
            hasFlag = flag;
        }
    }
}
