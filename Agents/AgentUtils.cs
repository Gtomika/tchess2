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
        /// <param name="control">Allows access to resources.</param>
        /// <returns>The agent with that name.</returns>
        public static Agent AgentFromName(string name, UserControl control)
        {
            if(name.Equals((string)control.FindResource("strPlayer")))
            {
                return new PlayerAgent(name);
            } 
            else if(name.Equals((string)control.FindResource("strStockfish")))
            {
                return new StockfishAgent(name);
            } 
            else
            {
                throw new ArgumentException(name + " is not a valid agent name!");
            }
        }

    }
}
