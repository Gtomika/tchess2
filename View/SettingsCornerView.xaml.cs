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
    /// Interaction logic for SettingsCornerView.xaml
    /// </summary>
    public partial class SettingsCornerView : UserControl
    {
        public SettingsCornerView()
        {
            InitializeComponent();
        }

        private void OnFlipBoardClicked(object sender, RoutedEventArgs e)
        {
            var hub = MessageHub.MessageHub.Instance;
            hub.Publish(new EventBoardFlipped());
        }
    }
}
