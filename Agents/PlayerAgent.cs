using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TChess2.Agents
{
    /// <summary>
    /// Simple class for manually controlled player.
    /// </summary>
    public class PlayerAgent : Agent
    {
        public PlayerAgent(string name) : base(name)
        {
        }

        /// <summary>
        /// Does not make any move. The players moves are received 
        /// from the GUI.
        /// </summary>
        /// <param name="fen">Not used.</param>
        /// <returns>Not used.</returns>
        public override Move MakeMove(string fen)
        {
            return null;
        }

        public override bool IsHumanControlled()
        {
            return true;
        }

        public override void CloseResources()
        {
            //does not need resources
        }
    }
}
