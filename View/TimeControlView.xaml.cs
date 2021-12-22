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
            hub.Subscribe<EventSelectedPlayerChanged>(e =>
                OnSelectedPlayerChanged(e));

            hub.Subscribe<EventTimeControlChanged>(e =>
                OnTimeControlChanged(e));

            hub.Subscribe<EventGameStarted>(e =>
                OnGameStarted(e));
        }

        public TimeControl GameTimeControl { get; set; }
        
        public string WhiteName { get; set; }

        public string BlackName { get; set; }

        public string WhiteTimeString { get; set; }

        public string BlackTimeString { get; set; }

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
            WhiteTimeString = e.SelectedTimeControl.GetTimeString(Side.WHITE);
            BlackTimeString = e.SelectedTimeControl.GetTimeString(Side.BLACK);
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
            WhiteTimeString = e.GameTimeControl.GetTimeString(Side.WHITE);
            BlackTimeString = e.GameTimeControl.GetTimeString(Side.BLACK);
            WhiteName = e.WhiteName;
            BlackName = e.BlackName;
            //set UI elements
            TextBlockWhiteName.Text = WhiteName;
            TextBlockBlackName.Text = BlackName;
            TextBlockWhiteTime.Text = WhiteTimeString;
            TextBlockBlackTime.Text = BlackTimeString;
            //TODO: start counting white time
        }
    }
}
