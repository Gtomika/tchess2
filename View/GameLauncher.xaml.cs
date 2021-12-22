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
using TChess2.TChess;
using MessageHub;
using TChess2.Events;

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for GameLauncher.xaml
    /// </summary>
    public partial class GameLauncher : UserControl
    {

        public GameLauncher()
        {
            InitializeComponent();
        }

        //white player
        public string WhiteSelection { get; set; }

        //black player
        public string BlackSelection { get; set; }

        //game time control
        public TimeControl SelectedTimeControl { get; set; }

        /// <summary>
        /// Builds description string for time control.
        /// </summary>
        /// <param name="name">Name of the time control.</param>
        /// <param name="timeControl">Time control object.</param>
        private void SetTimeControlDescription(string name, TimeControl timeControl)
        {
            string desc = name + "\n";
            //time
            if (timeControl.WhiteTime == timeControl.BlackTime)
            {
                desc += $"Both players have {timeControl.GetTimeString(Side.WHITE)} time.\n";
            }
            else
            {
                desc += $"White has {timeControl.GetTimeString(Side.WHITE)} time.\n" +
                    $"Black has {timeControl.GetTimeString(Side.BLACK)} time.\n";
            }
            //increment
            if (timeControl.Increment == 0)
            {
                desc += "There is no increment.";
            } else
            {
                desc += $"There is {timeControl.Increment / 1000} seconds of increment.";
            }
            TextBlockTimeControlDescription.Text = desc;
        }

        private void ComboBoxWhiteChanged(object sender, SelectionChangedEventArgs e)
        {
            WhiteSelection = ComboBoxWhite.SelectedValue.ToString();
            var hub = MessageHub.MessageHub.Instance;
            var _event = new EventSelectedPlayerChanged 
            {
                SelectedSide = Side.WHITE,
                Name = WhiteSelection
            };
            hub.Publish(_event);
            Console.WriteLine("Sent selected player changed event!");
        }

        private void ComboBoxBlackChanged(object sender, SelectionChangedEventArgs e)
        {
            BlackSelection = ComboBoxBlack.SelectedValue.ToString();
            var hub = MessageHub.MessageHub.Instance;
            var _event = new EventSelectedPlayerChanged
            {
                SelectedSide = Side.BLACK,
                Name = WhiteSelection
            };
            hub.Publish(_event);
        }

        private void ComboBoxTimeControlChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = ComboBoxTimeControl.SelectedValue.ToString();
            string name;
            //custom was selected
            if(selected.Equals((string)FindResource("strCustom")))
            {
                CustomTimeControlDialog dialog = new CustomTimeControlDialog();
                dialog.ShowDialog();
                if (dialog.SelectedTimeControl != null)
                {
                    SelectedTimeControl = dialog.SelectedTimeControl;
                }
                else
                {
                    return;
                }
                name = (string)FindResource("strCustomTimeControl");
            }
            //known time control from the list
            else
            {
                SelectedTimeControl = TimeControl.FromSelectionString(selected, this);
                name = selected;
            }
            //set the desctiption
            SetTimeControlDescription(name, SelectedTimeControl);
            //publish event
            var hub = MessageHub.MessageHub.Instance;
            hub.Publish(new EventTimeControlChanged
            {
                SelectedTimeControl = SelectedTimeControl
            }); 
        }

        /// <summary>
        /// Called when start button is clicked. Validates selected 
        /// inputs first. Publishes game start event when all inputs 
        /// are valid.
        /// </summary>
        private void GameStartClicked(object sender, RoutedEventArgs e)
        {
            string error = (string)FindResource("strError");
            if(WhiteSelection == null || BlackSelection == null)
            {
                string msg = (string)FindResource("strErrorSelectPlayers");
                MessageBox.Show(msg, error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(SelectedTimeControl == null)
            {
                string msg = (string)FindResource("strErrorSelectTimeControl");
                MessageBox.Show(msg, error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //game can start
            LockControls();
            var hub = MessageHub.MessageHub.Instance;
            hub.Publish(new EventGameStarted
            { 
                WhiteName = WhiteSelection,
                BlackName = BlackSelection,
                GameTimeControl = SelectedTimeControl
            });
        }

        /// <summary>
        /// Locks the game launcher controls. This is useful when a game 
        /// is ongoing.
        /// </summary>
        private void LockControls()
        {
            ComboBoxWhite.IsEnabled = false;
            ComboBoxBlack.IsEnabled = false;
            ComboBoxTimeControl.IsEnabled = false;
            ButtonStart.IsEnabled = false;
        }

        /// <summary>
        /// Unlocks game launcher controls. This can be used when a 
        /// game ends a new one can be launched.
        /// </summary>
        private void UnlockControls()
        {
            ComboBoxWhite.IsEnabled = true;
            ComboBoxBlack.IsEnabled = true;
            ComboBoxTimeControl.IsEnabled = true;
            ButtonStart.IsEnabled = true;
        }
    }
}
