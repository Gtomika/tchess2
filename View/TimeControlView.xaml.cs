using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessDotNet;
using MessageHub;
using TChess2.Events;
using TChess2.TChess;

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for TimeControlView.xaml
    /// </summary>
    public partial class TimeControlView : UserControl
    {
        public TimeControlView()
        {
            InitializeComponent();
            DataContext = this;
            WhiteName = (string)FindResource("strWhite"); //starter names, will be replaced on game start
            BlackName = (string)FindResource("strBlack");
            TextBlockWhiteName.Text = WhiteName;
            TextBlockBlackName.Text = BlackName;
            WhiteTimeString = "00:10:00"; //just some random time, will be replaces on game start
            BlackTimeString = "00:10:00";
            TextBlockWhiteTime.Text = WhiteTimeString;
            TextBlockBlackTime.Text = BlackTimeString;

            //subscribe to events
            var hub = MessageHub.MessageHub.Instance;
            hub.Subscribe<EventSelectedPlayerChanged>(e => OnSelectedPlayerChanged(e));

            hub.Subscribe<EventTimeControlChanged>(e => OnTimeControlChanged(e));

            hub.Subscribe<EventGameStarted>(e => OnGameStarted(e));

            hub.Subscribe<EventMoveMade>(e => OnMoveMade(e));

            hub.Subscribe<EventGameOver>(e => OnGameOver(e));
        }

        //amount of time between timer ticks
        private static readonly int INTERVAL = 500;

        public TimeControl GameTimeControl { get; set; }
        
        public string WhiteName { get; set; }

        public string BlackName { get; set; }

        public string WhiteTimeString { get; set; }

        public string BlackTimeString { get; set; }

        //counts time
        public Timer GameTimer;

        //stores whose time is currently ticking
        private Player CurrentPlayer;

        /// <summary>
        /// Called when the selected player was changed. This is called 
        /// pre game.
        /// </summary>
        /// <param name="e">Event with the details.</param>
        private void OnSelectedPlayerChanged(EventSelectedPlayerChanged e)
        {
            //preview the selected player name
            if (e.SelectedSide == TChess.Side.WHITE)
            {
                WhiteName = e.Name;
                TextBlockWhiteName.Text = WhiteName;
            }
            else
            {
                BlackName = e.Name;
                TextBlockBlackName.Text = BlackName;
            }
        }

        /// <summary>
        /// Called when the selected time control was changed. This is 
        /// called pre game.
        /// </summary>
        /// <param name="e">Event with the details.</param>
        private void OnTimeControlChanged(EventTimeControlChanged e)
        {
            //preview the selected time control
            WhiteTimeString = e.SelectedTimeControl.GetTimeString(Player.White);
            BlackTimeString = e.SelectedTimeControl.GetTimeString(Player.Black);
            TextBlockWhiteTime.Text = WhiteTimeString;
            TextBlockBlackTime.Text = BlackTimeString;
        }

        /// <summary>
        /// Called when the game is started.
        /// </summary>
        /// <param name="e">Event with the details.</param>
        private void OnGameStarted(EventGameStarted e)
        {
            //save data
            GameTimeControl = e.GameTimeControl;
            WhiteTimeString = e.GameTimeControl.GetTimeString(Player.White);
            BlackTimeString = e.GameTimeControl.GetTimeString(Player.Black);
            WhiteName = e.WhiteName;
            BlackName = e.BlackName;
            //set UI elements
            TextBlockWhiteName.Text = WhiteName;
            TextBlockBlackName.Text = BlackName;
            TextBlockWhiteTime.Text = WhiteTimeString;
            TextBlockBlackTime.Text = BlackTimeString;
            //initialize timer
            CurrentPlayer = Player.White;
            GameTimer = new Timer
            {
                Interval = INTERVAL //update every this much milliseconds
            };
            GameTimer.Elapsed += new ElapsedEventHandler(OnCountDownTimeElapsed);
            GameTimer.Start();
        }

        //called when the timer completes an interval
        private void OnCountDownTimeElapsed(object source, ElapsedEventArgs e)
        {
            //call this on the MAIN thread
            Application.Current.Dispatcher.Invoke(new Action(() => {
                //remove time from the current player
                GameTimeControl.RemoveTime(CurrentPlayer, INTERVAL);
                //update UI
                if (CurrentPlayer == Player.White)
                {
                    WhiteTimeString = GameTimeControl.GetTimeString(CurrentPlayer);
                    TextBlockWhiteTime.Text = WhiteTimeString;
                }
                else
                {
                    BlackTimeString = GameTimeControl.GetTimeString(CurrentPlayer);
                    TextBlockBlackTime.Text = BlackTimeString;
                }
                //check if they have time
                if (GameTimeControl.IsTimedOut(CurrentPlayer))
                {
                    //this player timed out, publish an event of this
                    var hub = MessageHub.MessageHub.Instance;
                    hub.Publish(new EventTimeOut
                    {
                        PlayerWhoTimedOut = CurrentPlayer
                    });
                }
            }));
        }

        /// <summary>
        /// Called when a side has made their move. Starts counting the other sides time.
        /// </summary>
        /// <param name="e">Event with the details.</param>
        private void OnMoveMade(EventMoveMade e)
        {
            CurrentPlayer = ChessUtilities.GetOpponentOf(CurrentPlayer);
        }

        //called when game is over, stops counter
        private void OnGameOver(EventGameOver e)
        {
            if (GameTimer != null)
            {
                GameTimer.Stop();
            }
        }

    }
}
