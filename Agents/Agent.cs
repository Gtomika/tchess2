using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TChess2.Agents
{
    /// <summary>
    /// Interface for player agent. Implementors of this are 
    /// supposed to be able to make a move given a FEN string.
    /// </summary>
    public abstract class Agent
    {

        //only for subclasses.
        protected Agent(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the agent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Makes a move from the given position.
        /// </summary>
        /// <param name="fen">FEN string.</param>
        /// <returns>The move.</returns>
        public abstract Move MakeMove(string fen);

    }
}
