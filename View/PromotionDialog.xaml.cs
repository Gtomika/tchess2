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

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for PromotionDialog.xaml
    /// </summary>
    public partial class PromotionDialog : Window
    {
        public PromotionDialog()
        {
            InitializeComponent();
            //queen is selected by default
            Promotion = 'Q';
        }

        //character of what the user selected to promote to, such as 'q'
        public char Promotion { get; set; }

        private void OnPromotionSelected(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            string name = button.Content as string;
            if(name == (string)FindResource("strQueen"))
            {
                Promotion = 'Q';
            }
            else if (name == (string)FindResource("strRook"))
            {
                Promotion = 'R';
            }
            else if (name == (string)FindResource("strBishop"))
            {
                Promotion = 'B';
            }
            else if (name == (string)FindResource("strKnight"))
            {
                Promotion = 'N';
            }

        }

        private void OnPromoteClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
