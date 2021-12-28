using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for MoveElementView.xaml
    /// </summary>
    public partial class MoveElementView : UserControl
    {
        public MoveElementView(int moveCounter)
        {
            InitializeComponent();
            MoveCounter = moveCounter;
            //set move counter
            TextBlockMoveCounter.Text = $"{moveCounter}.";
        }

        private string whiteMove;

        public string WhiteMove {
            get
            {
                return whiteMove;
            }
            set
            {
                whiteMove = value; 
                TextBlockWhiteMove.Text = whiteMove;
            }
        }

        private string blackMove;

        public string BlackMove {
            get
            {
                return blackMove;
            }
            set
            {
                blackMove = value;
                TextBlockBlackMove.Text = blackMove;
            }
        }

        public int MoveCounter { get; set; }
    }
}
