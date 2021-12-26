using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TChess2.TChess;

namespace TChess2.Agents
{

    /// <summary>
    /// This agent calls Stockfish chess engine to make a 
    /// move.
    /// </summary>
    public class StockfishAgent : Agent
    {

        private Stockfish.NET.Core.Stockfish Stockfish;

        //The side stockfish is playing on.
        private Player Player;

        //Depth Stockfish goes down to search moves.
        private int Depth;

        public StockfishAgent(string name, Player player) : base(name)
        {
            Player = player;
            //launch stockfish
            var path = Properties.Settings.Default.StockfishPath;
            //TODO: make this this set from the constructor
            //Depth 15 is still incredibly fast
            Depth = 15;
            Stockfish = new Stockfish.NET.Core.Stockfish(path, Depth);
        }

        public override Move MakeMove(string fen)
        {
            Stockfish.SetFenPosition(fen);
            string move = Stockfish.GetBestMove();
            //Console.WriteLine("Move received from Stockfish: " + move);
            if(move.Length == 4)
            {
                //this is not a promotion, such as "e2e4"
                Position from = new Position(TChessUtils.GetFileFromChar(move[0]), int.Parse(move[1].ToString()));
                Position to = new Position(TChessUtils.GetFileFromChar(move[2]), int.Parse(move[3].ToString()));
                return new Move(from, to, Player);
            } else if(move.Length == 5)
            {
                //this is a promotion
                Position from = new Position(TChessUtils.GetFileFromChar(move[0]), int.Parse(move[1].ToString()));
                Position to = new Position(TChessUtils.GetFileFromChar(move[2]), int.Parse(move[3].ToString()));
                char promotion = move[4];
                return new Move(from, to, Player, promotion);
            } else
            {
                //hmm
                throw new ApplicationException($"Unkown move {move} received from Stockfish!");
            }
        }

        public override bool IsHumanControlled()
        {
            return false;
        }

        public override void CloseResources()
        {
            if(Stockfish != null)
            {
                Stockfish.StopStockfish();
            }
        }
    }
}
