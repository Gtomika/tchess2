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
using TChess2.TChess;

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for GameResultView.xaml
    /// </summary>
    public partial class GameResultView : UserControl
    {
        public GameResultView()
        {
            InitializeComponent();
            //subscribe to events
            var hub = MessageHub.MessageHub.Instance;
            hub.Subscribe<EventGameStarted>(e => OnGameStarted(e));
            hub.Subscribe<EventGameOver>(e => OnGameOver(e));
        }

        private string WhiteName { get; set; }
        private string BlackName { get; set; }

        //when game is started, clears the current result and shows ongoing.
        private void OnGameStarted(EventGameStarted e)
        {
            TextBlockOutcome.Text = (string)FindResource("strOngoing");
            TextBlockReason.Text = "";
            //save names
            WhiteName = e.WhiteName;
            BlackName = e.BlackName;
        }

        //when a game ends, displays the result
        private void OnGameOver(EventGameOver e)
        {
            string outcome = GetOutcomeString(e.GameReport.Status);
            TextBlockOutcome.Text = outcome;
            TextBlockReason.Text = e.GameReport.Reason;
        }

        //converts game status to string
        private string GetOutcomeString(GameStatus status)
        {
            switch(status)
            {
                case GameStatus.WHITE_WINS:
                    string outcomeW = (string)FindResource("strWhiteWon");
                    outcomeW = outcomeW.Replace("x", WhiteName);
                    return outcomeW;
                case GameStatus.BLACK_WINS:
                    string outcomeB = (string)FindResource("strBlackWon");
                    outcomeB = outcomeB.Replace("x", BlackName);
                    return outcomeB;
                case GameStatus.DRAW:
                    return (string)FindResource("strDraw");
                default:
                    //game status ongoing should not arrive when game is over...
                    throw new ApplicationException("Game status was ongoing in game over event!");
            }
        }
    }
}
