using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool isOpen { get; set; }
        public bool isBomb { get; set; }
        public bool hasFlag { get; set; }

        public Cell(bool open, bool bomb, bool flag) {
            isOpen = open;
            isBomb = bomb;
            hasFlag = flag;
        }
    }
}
