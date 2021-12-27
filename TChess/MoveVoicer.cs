using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using ChessDotNet;
using System.Text.RegularExpressions;

namespace TChess2.TChess
{
    class MoveVoicer
    {

        public MoveVoicer()
        {
            Synthesizer = new SpeechSynthesizer();
        }

        private SpeechSynthesizer Synthesizer;

        /// <summary>
        /// Prints available voices. Hungarian is not supported :(
        /// </summary>
        public void PrintVoices()
        {
            foreach(var voice in Synthesizer.GetInstalledVoices())
            {
                Console.WriteLine(voice.VoiceInfo.Description);
            }
        }

        /// <summary>
        /// Says a move out loud.
        /// </summary>
        /// <param name="san">Short algebraic notation of the move.</param>
        /// <param name="move">The move.</param>
        /// <param name="moveType">Type of the move such as capture, etc.</param>
        public void VoiceMove(string san, Move move, Piece pieceThatMoved, MoveType moveType)
        {
           //deal with castles
           if(san == "O-O")
            {
                Say("Short castle");
                return;
            }
           if(san == "O-O-O")
            {
                Say("Long castle");
                return;
            }
            StringBuilder builder = new StringBuilder();

            //first must say what piece moved
            string pieceWord = GetPieceWordOf(pieceThatMoved.GetFenCharacter());
            if(pieceWord != "Pawn")
            {
                //pawn is not said, other pieces are said
                builder.Append(pieceWord + " ");
            } else if(pieceWord == "Pawn" && moveType.HasFlag(MoveType.Capture))
            {
                //in case of pawn capture, the name of the file is said, such as "d takes c4"
                builder.Append(move.OriginalPosition.File.ToString() + " ");
            }
  
            //determine if there was an ambiguity, such as both 
            //knight were able to make this move
            if(san.Length >= 4)
            {
                Regex ambRegex = new Regex(@"[QRBN][abcdefgh12345678][abcdefgh][12345678]");
                string sanStart = san.Substring(0, 4);
                if (ambRegex.IsMatch(sanStart))
                {
                    //this move is ambigous. we need to say the disambiguing file or rank name
                    builder.Append(san[1] + " ");
                }
            }

            //is this a capture or not
            if(moveType.HasFlag(MoveType.Capture))
            {
                builder.Append("takes ");
            }

            //say the destination square
            string squareWord = GetSquareWordOf(move.NewPosition);
            builder.Append(squareWord + " ");

            //say the promoted piece if this is a promotion
            if(moveType.HasFlag(MoveType.Promotion))
            {
                string promotionWord = GetPieceWordOf((char)move.Promotion);
                builder.Append(promotionWord + " ");
            }

            //say check and checkmate
            if(san.EndsWith("+"))
            {
                builder.Append("check");
            } else if(san.EndsWith("#"))
            {
                builder.Append("checkmate");
            }

            //say the constructed move string
            string moveString = builder.ToString();
            //Console.WriteLine("Saying this move: " + moveString);
            Say(moveString);
        }

        private string GetPieceWordOf(char c)
        {
            if(c == 'K' || c == 'k')
            {
                return "King";
            }
            if (c == 'Q' || c == 'q')
            {
                return "Queen";
            }
            if (c == 'R' || c == 'r')
            {
                return "Rook";
            }
            if (c == 'B' || c == 'b')
            {
                return "Bishop";
            }
            if (c == 'N' || c == 'n')
            {
                return "Knight";
            }
            if (c == 'P' || c == 'p')
            {
                return "Pawn";
            }
            throw new ArgumentException($"Unknown piece {c}");
        }

        private string GetSquareWordOf(Position square)
        {
            return square.File.ToString() + square.Rank.ToString();
        }

        private void Say(string msg)
        {
            Synthesizer.SpeakAsync(msg);
        }
    }
}
