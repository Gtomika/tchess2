using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TChess2.Agents
{
    public abstract class AgentUtils
    {

        /// <summary>
        /// Finds the appropriate Agent for a name.
        /// </summary>
        /// <param name="name">The name, such as "Stockfish".</param>
        /// <param name="player">Side of the agent.</param>
        /// <param name="control">Allows access to resources.</param>
        /// <returns>The agent with that name.</returns>
        public static Agent AgentFromName(string name, Player player, UserControl control)
        {
            if(name.Equals((string)control.FindResource("strPlayer")))
            {
                return new PlayerAgent(name);
            } 
            else if(name.Equals((string)control.FindResource("strStockfish5")))
            {
                return new StockfishAgent(name, player, 5);
            }
            else if (name.Equals((string)control.FindResource("strStockfish10")))
            {
                return new StockfishAgent(name, player, 10);
            }
            else if (name.Equals((string)control.FindResource("strStockfish15")))
            {
                return new StockfishAgent(name, player, 15);
            }
            else if (name.Equals((string)control.FindResource("strStockfish20")))
            {
                return new StockfishAgent(name, player, 20);
            }
            else
            {
                throw new ArgumentException(name + " is not a valid agent name!");
            }
        }

    }
}
