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
using System.Windows.Shapes;
using TChess2.TChess;

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for CustomTimeControlDialog.xaml
    /// </summary>
    public partial class CustomTimeControlDialog : Window
    {
        public CustomTimeControlDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public TimeControl SelectedTimeControl { get; set; }

        public int WhiteHours { get; set; }
        public int WhiteMinutes { get; set; }
        public int WhiteSeconds { get; set; }

        public int BlackHours { get; set; }
        public int BlackMinutes { get; set; }
        public int BlackSeconds { get; set; }

        public int IncrementSeconds { get; set; }

        //ok button is clicked, validate inputs
        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            string invalid = (string)FindResource("strInvalidTimeControl");
            if (WhiteHours < 0 || WhiteMinutes < 0 || WhiteSeconds < 0 ||
                BlackHours < 0 || BlackMinutes < 0 || BlackSeconds < 0 ||
                    IncrementSeconds < 0)
            {
                TextBlockErrorMessage.Text = invalid;
                return;
            }
            if(WhiteHours > 10 || BlackHours > 10)
            {
                TextBlockErrorMessage.Text = (string)FindResource("strTimeTooMuch");
                return;
            }
            if(IncrementSeconds > 60 * 10)
            {
                TextBlockErrorMessage.Text = (string)FindResource("strIncrementTooMuch");
                return;
            }

            //build time control object
            long whiteTime = WhiteHours * 60 * 60 * 1000 + WhiteMinutes * 60 * 1000 + WhiteSeconds * 1000;
            long blackTime = BlackHours * 60 * 60 * 1000 + BlackMinutes * 60 * 1000 + BlackSeconds * 1000;
            long increment = IncrementSeconds * 1000;

            if (whiteTime == 0 || blackTime == 0)
            {
                TextBlockErrorMessage.Text = invalid;
                return;
            }

            SelectedTimeControl = new TimeControl(whiteTime, blackTime, increment);

            //close window
            Close();
        }
    }
}
