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
using ChessDotNet;
using TChess2;
using TChess2.Events;

/*
 * https://github.com/thomas-daniels/Chess.NET/blob/master/Samples/BasicUsageSample.cs
 * https://www.youtube.com/watch?v=THV5BW5WW_o
 */

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for ChessboardControl.xaml
    /// </summary>
    public partial class ChessboardControl : UserControl
    {
        public ChessboardControl()
        {
            InitializeComponent();
            PieceImages = new Dictionary<Position, Image>();
            BoardFlip = TChess.BoardFlip.NORMAL;
            //setup board: square backgrounds
            CreateChessboardSquares(BoardFlip);
            //setup board: initial setup of pieces
            ChessGame game = new ChessGame();
            DrawPieces(game, BoardFlip);
            //subscribe to events
            var hub = MessageHub.MessageHub.Instance;
            hub.Subscribe<EventBoardFlipped>(e => OnBoardFlipped(e));
        }

        //Rectangles that are the backgrounds of each square on the board.
        //Initialized in CreateChessboardSquares.
        private Rectangle[] ChessboardSquareBackgrounds;

        //Stores the displayed piece images.
        private Dictionary<Position, Image> PieceImages;

        //The current chess game (maybe ongoing or over). It is possible
        //that this is null, in this case there is no game started yet.
        private ChessGame CurrentGame;

        //Stores the board flip status.
        private TChess.BoardFlip BoardFlip;

        /// <summary>
        /// Creates and stores the rectangles that are the backgrounds of the squares.
        /// They are also added to the chessboard grid. Then drws the rank and 
        /// file names.
        /// </summary>
        private void CreateChessboardSquares(TChess.BoardFlip boardFlip)
        {
            //draw square backgrounds
            ChessboardSquareBackgrounds = new Rectangle[64];
            var lightSquareBrush = new SolidColorBrush { Color = Color.FromArgb(255, 215, 214, 210) };
            var darkSquareBrush = new SolidColorBrush { Color = Color.FromArgb(255, 189, 125, 19) };
            for (int i = 0; i < 64; i++)
            {
                int row = i / 8, column = i % 8;
                bool isLight = IsLightSquare(row, column);
                Rectangle background = new Rectangle
                {
                    Fill = isLight ? lightSquareBrush : darkSquareBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                Grid.SetColumn(background, column + 1);
                Grid.SetRow(background, row);
                Grid.SetZIndex(background, -1);
                //save
                ChessboardSquareBackgrounds[i] = background;
                //add to grid
                ChessboardGrid.Children.Add(background);
            }
            //draw rank and file names
            if(boardFlip == TChess.BoardFlip.NORMAL)
            {
                TextRank1.Text = "1";
                TextRank2.Text = "2";
                TextRank3.Text = "3";
                TextRank4.Text = "4";
                TextRank5.Text = "5";
                TextRank6.Text = "6";
                TextRank7.Text = "7";
                TextRank8.Text = "8";
                TextFile1.Text = "a";
                TextFile2.Text = "b";
                TextFile3.Text = "c";
                TextFile4.Text = "d";
                TextFile5.Text = "e";
                TextFile6.Text = "f";
                TextFile7.Text = "g";
                TextFile8.Text = "h";
            } else
            {
                TextRank1.Text = "8";
                TextRank2.Text = "7";
                TextRank3.Text = "6";
                TextRank4.Text = "5";
                TextRank5.Text = "4";
                TextRank6.Text = "3";
                TextRank7.Text = "2";
                TextRank8.Text = "1";
                TextFile1.Text = "h";
                TextFile2.Text = "g";
                TextFile3.Text = "f";
                TextFile4.Text = "e";
                TextFile5.Text = "f";
                TextFile6.Text = "c";
                TextFile7.Text = "b";
                TextFile8.Text = "a";
            }
        }

        /// <summary>
        /// Determines if a square is light or dark. This is invariant 
        /// of board flip.
        /// </summary>
        /// <returns>True if square is light.</returns>
        private bool IsLightSquare(int row, int column)
        {
            if(row % 2 == 0)
            {
                return column % 2 == 0;
            }
            else
            {
                return column % 2 != 0;
            }
        }

        /// <summary>
        /// Draws the piece images over the correct squares. First removes 
        /// all currently drawn pieces.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="boardFlip">Specifies if the board is flipped or not.</param>
        private void DrawPieces(ChessGame game, TChess.BoardFlip boardFlip)
        {
            //clear board of drawings
            RemoveAllPieceDrawings();
            //add drawings back
            for(int i = 0; i < 64; i++)
            {
                Position pos = TChess.TChessUtils.PositionFromIndex(i);
                Piece piece = game.GetPieceAt(pos);
                if(piece != null)
                {
                    //load image based on piece
                    Image pieceImage = TChess.TChessUtils.GetImageOfPiece(piece, this);
                    //set position, depends on board flip
                    if(boardFlip == TChess.BoardFlip.NORMAL)
                    {
                        int row = 7 - (i / 8), column = i % 8;
                        Grid.SetColumn(pieceImage, column + 1);
                        Grid.SetRow(pieceImage, row);
                    } else
                    {
                        int row = i / 8, column = 7 - (i % 8);
                        Grid.SetColumn(pieceImage, column + 1);
                        Grid.SetRow(pieceImage, row);
                    }
                    Grid.SetZIndex(pieceImage, 1);
                    //add image to grid.
                    ChessboardGrid.Children.Add(pieceImage);
                    //save image to be able to remove it later
                    PieceImages[pos] = pieceImage;
                }
                
            }
        }

        /// <summary>
        /// Removes all piece images from the chessboard.
        /// </summary>
        private void RemoveAllPieceDrawings()
        {
            foreach(Image pieceImage in PieceImages.Values)
            {
                ChessboardGrid.Children.Remove(pieceImage);
            }
            PieceImages.Clear();
        }
      
        /// <summary>
        /// Called when a board flipped event was received.
        /// </summary>
        /// <param name="e">Board flipped event.</param>
        private void OnBoardFlipped(EventBoardFlipped e)
        {
            BoardFlip = BoardFlip == TChess.BoardFlip.NORMAL ?
                TChess.BoardFlip.FLIPPED : TChess.BoardFlip.NORMAL;
            //flip board: redraw squares and rank, file names
            CreateChessboardSquares(BoardFlip);
            //flip board: redraw pieces
            if(CurrentGame != null)
            {
                DrawPieces(CurrentGame, BoardFlip);
            } else
            {
                //draw defaul position
                DrawPieces(new ChessGame(), BoardFlip);
            }
           
        }
    }
}
