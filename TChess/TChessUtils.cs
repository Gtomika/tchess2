using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ChessDotNet;

namespace TChess2.TChess
{
    class TChessUtils
    {

        /// <summary>
        /// Returns the square position, e. g "E4" from the index of the square. 
        /// Indexing begins at the top left corner (A8->B8->...).
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>Position of the index</returns>
        public static Position PositionFromIndex(int i)
        {
            int file = i % 8;
            int row = i / 8;
            return new Position((File)file, row + 1);
        }

        /// <summary>
        /// Finds the image resource of a piece.
        /// </summary>
        /// <param name="piece">The piece.</param>
        /// <param name="control">Control to access resources.</param>
        /// <returns>Image view that has the piece image loaded.</returns>
        public static Image GetImageOfPiece(Piece piece, UserControl control)
        {
            BitmapImage bitmap;
            //upper case is white, lower case is black
            switch (piece.GetFenCharacter())
            {
                case 'K':
                    bitmap = (BitmapImage)control.FindResource("imgWhiteKing");
                    break;
                case 'k':
                    bitmap = (BitmapImage)control.FindResource("imgBlackKing");
                    break;
                case 'Q':
                    bitmap = (BitmapImage)control.FindResource("imgWhiteQueen");
                    break;
                case 'q':
                    bitmap = (BitmapImage)control.FindResource("imgBlackQueen");
                    break;
                case 'P':
                    bitmap = (BitmapImage)control.FindResource("imgWhitePawn");
                    break;
                case 'p':
                    bitmap = (BitmapImage)control.FindResource("imgBlackPawn");
                    break;
                case 'N':
                    bitmap = (BitmapImage)control.FindResource("imgWhiteKnight");
                    break;
                case 'n':
                    bitmap = (BitmapImage)control.FindResource("imgBlackKnight");
                    break;
                case 'B':
                    bitmap = (BitmapImage)control.FindResource("imgWhiteBishop");
                    break;
                case 'b':
                    bitmap = (BitmapImage)control.FindResource("imgBlackBishop");
                    break;
                case 'R':
                    bitmap = (BitmapImage)control.FindResource("imgWhiteRook");
                    break;
                case 'r':
                    bitmap = (BitmapImage)control.FindResource("imgBlackRook");
                    break;
                default:
                    throw new ArgumentException("Unknown piece: " + piece.ToString());
            }
            Image pieceImage = new Image
            {
                Source = bitmap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };
            return pieceImage;
        }


    }
}
