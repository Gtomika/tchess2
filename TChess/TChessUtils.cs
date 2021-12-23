﻿using System;
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
        public static Position PositionFromIndex(int i)
        {
            int file = i % 8;
            int row = i / 8;
            return new Position((File)file, row + 1);
        }

        /// <summary>
        /// Returns the square position, e. g "E4" from the index of the square. 
        /// Indexing begins at the top left corner (A8->B8->...).
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>Position of the index</returns>
        public static Position PositionFromIndex(int i, BoardFlip boardFlip)
        {
            if(boardFlip == BoardFlip.NORMAL)
            {
                int file = i % 8;
                int row = 7 - (i / 8);
                return new Position((File)file, row + 1);
            } else
            {
                int file = i % 8;
                int row = i / 8;
                return new Position((File)file, row + 1);
            }
        }

        /// <summary>
        /// Finds the board position from a square index.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <param name="boardFlip">Current board flip.</param>
        /// <returns>Tuple, first component is column, second is row.</returns>
        public static Tuple<int, int> GridPositionFromIndex(int i, BoardFlip boardFlip)
        {
            int column, row;
            if (boardFlip == TChess.BoardFlip.NORMAL)
            {
                row = 7 - (i / 8);
                column = i % 8;
            }
            else
            {
                row = i / 8; 
                column = 7 - (i % 8);
            }
            column += 1;
            return new Tuple<int, int>(column, row);
        }

        /// <summary>
        /// From a position such as "A4" finds the location of this square 
        /// on the grid.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="boardFlip">Board flip status.</param>
        /// <returns>Tuple, first component is the column, second is the row.</returns>
        public static Tuple<int, int> GridPositionFromPosition(Position position, BoardFlip boardFlip)
        {
            int column, row;
            if(boardFlip == BoardFlip.NORMAL)
            {
                //a8 -> rank = 8, expected = 1
                row = 8 - position.Rank;
                //a8 -> file = a (numeric 0), expected = 1
                column = (int)position.File + 1;
            } else
            {
                //a8 -> rank = 8, expected = 8
                row = position.Rank;
                //a8 -> file = a (numeric 0), expected = 8
                column = 8 - (int)position.File;
            }
            return new Tuple<int, int>(column, row);
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
                IsHitTestVisible = false, //allows click through
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };
            return pieceImage;
        }

    }
}
