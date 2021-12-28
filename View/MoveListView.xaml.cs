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
using TChess2.Events;

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for MoveListView.xaml
    /// </summary>
    public partial class MoveListView : UserControl
    {
        public MoveListView()
        {
            InitializeComponent();
            //subscribe to events
            var hub = MessageHub.MessageHub.Instance;
            hub.Subscribe<EventGameStarted>(e => OnGameStarted(e));
            hub.Subscribe<EventMoveSan>(e => OnMoveSan(e));
        }

        //stores how many moves have been made in a game
        private int moveCounter;

        //a player made a move, and this component receives the SAN of this move.
        public void OnMoveSan(EventMoveSan e)
        {
            //index of the row where this move will go
            //moveCounter = 0 -> row 0 (white)
            //moveCounter = 1 -> row 0 (black)
            //moveCounter = 2 -> row 1 (white)
            //...
            int moveElementIndex = moveCounter / 2;

            if(moveCounter % 2 == 0)
            {
                //the move received is WHITE's move
                //need a new row for this
                var moveElementView = InsertNewMoveElement(moveElementIndex);
                //set white's move
                moveElementView.WhiteMove = e.San;
            } else
            {
                //the move received is BLACK's move
                //there is already a row for this, the last one
                MoveElementView moveElementView = (MoveElementView)StackPanelMoves.Children[StackPanelMoves.Children.Count - 1];
                //set black's move
                moveElementView.BlackMove = e.San;
            }

            moveCounter++;
        }

        //inserts and returns a new move element row
        public MoveElementView InsertNewMoveElement(int moveElementIndex)
        {
            //create move element, this is NOT 0 indexed
            var moveElementView = new MoveElementView(moveElementIndex + 1);
            //insert it
            StackPanelMoves.Children.Add(moveElementView);
                
            return moveElementView;
        }

        //a new game begins
        public void OnGameStarted(EventGameStarted e)
        {
            //delete the previos game's moves
            StackPanelMoves.Children.Clear();
            //reset status
            moveCounter = 0;
        }
    }
}
