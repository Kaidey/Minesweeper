using Minesweeper.Controllers;
using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper {
    public partial class MainWindow : Form {

        private GameController mainController;
        public MainWindow() {
            InitializeComponent();
            mainController = new GameController();
        }

        private void MainWindow_Load(object sender, EventArgs e) {

            //Display a radio button menu to enable the user to choose a dificulty that is then passed to initBoard()

            mainController.initBoard("easy");

            foreach(Cell x in mainController.gameBoard) {
                Console.WriteLine(x.isBomb);
            }
        }
    }
}
