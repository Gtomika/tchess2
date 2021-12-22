using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TChess2.Agents
{

    /// <summary>
    /// This agent calls Stockfish chess engine to make a 
    /// move.
    /// </summary>
    public class StockfishAgent : Agent
    {
        public StockfishAgent(string name) : base(name)
        {
        }

        public override Move MakeMove(string fen)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
