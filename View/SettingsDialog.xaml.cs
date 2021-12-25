﻿using System;
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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();
            //load stockfish path if exists
            TextBlockStockfishPath.Text = Properties.Settings.Default.StockfishPath;
        }

        private void OnSelectStockfishClicked(object sender, RoutedEventArgs e)
        {
            var fileChooser = new Microsoft.Win32.OpenFileDialog
            {
                Title = (string)FindResource("strSelectStockfishExe"),
                Filter = "Exe files (.exe)|*.exe",
                Multiselect = false,
            };
            if(fileChooser.ShowDialog() == true)
            {
                //display and save
                TextBlockStockfishPath.Text = fileChooser.FileName;
                Properties.Settings.Default.StockfishPath = fileChooser.FileName;
            }
        }

        private void OnTestEngineClicked(object sender, RoutedEventArgs e)
        {
            string path = Properties.Settings.Default.StockfishPath;
            if(path != null && path.Length > 0)
            {
                //there is something selected
                var stockfish = new Stockfish.NET.Core.Stockfish(path);
                try
                {
                    //attempt to get a response
                    var game = new ChessDotNet.ChessGame();
                    stockfish.SetFenPosition(game.GetFen());
                    var move = stockfish.GetBestMoveTime(500);
                    stockfish.StopStockfish();

                    var brush = new SolidColorBrush { Color = Color.FromArgb(255, 0, 255, 0) };
                    TextBlockTestResult.Foreground = brush;
                    TextBlockTestResult.Text = (string)FindResource("strEngineWorking");
                } catch (Exception)
                {
                    var brush = new SolidColorBrush { Color = Color.FromArgb(255, 255, 0, 0) };
                    TextBlockTestResult.Foreground = brush;
                    TextBlockTestResult.Text = (string)FindResource("strEngineNotWorking");
                }
            }
            else
            {
                var brush = new SolidColorBrush { Color = Color.FromArgb(255, 255, 0, 0) };
                TextBlockTestResult.Foreground = brush;
                TextBlockTestResult.Text = (string)FindResource("strEngineNotSelected");
            }
        }

        private void OnCloseSettingsClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClearSelectionClicked(object sender, RoutedEventArgs e)
        {
            TextBlockStockfishPath.Text = "";
            Properties.Settings.Default.StockfishPath = "";
            TextBlockTestResult.Text = "";
        }
    }
}