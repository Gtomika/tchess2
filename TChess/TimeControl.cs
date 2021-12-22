using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TChess2.TChess
{
    /// <summary>
    /// Represents time control of a chess game.
    /// </summary>
    public class TimeControl
    {
        /// <summary>
        /// Remaining time of the white player, in milliseconds.
        /// </summary>
        public long WhiteTime { get; set; }

        /// <summary>
        /// Remaining time of the black player, in milliseconds.
        /// </summary>
        public long BlackTime { get; set; }

        /// <summary>
        /// Increment of the players, in milliseconds. This is the amount of time 
        /// they gain after a move.
        /// </summary>
        public long Increment { get; set; }

        public TimeControl(long whiteTime, long blackTime, long increment)
        {
            this.WhiteTime = whiteTime;
            this.BlackTime = blackTime;
            this.Increment = increment;
        }
        
        /// <summary>
        /// Gets time as a formatted string.
        /// <param name="side">The side whose time will be returned.</param>
        /// </summary>
        public string GetTimeString(Side side)
        {
            string timeString;
            long seconds = (side == Side.WHITE) ? (WhiteTime / 1000) : (BlackTime / 1000);
            //calculate hours and remove from time
            long hours = seconds / 3600;
            seconds = seconds - hours * 3600;
            //calculate minutes and remove from time
            long minutes = seconds / 60;
            seconds = seconds - minutes * 60;
            //actual seconds remain
            if(hours > 99)
            {
                timeString = $"99:{minutes.ToString().PadLeft(2, '0')}:{seconds.ToString().PadLeft(2, '0')}";
            } else
            {
                timeString = $"{hours.ToString().PadLeft(2, '0')}:{minutes.ToString().PadLeft(2, '0')}:{seconds.ToString().PadLeft(2, '0')}";
            }
            return timeString;
        }

        /// <summary>
        /// Converts a string that the user selected from the time control combo box into a time 
        /// control object.
        /// </summary>
        /// <param name="selString">The selected string, like 'Classical (hour). This is a resource.</param>
        /// <param name="userControl">Object to access the resources.</param>
        /// <returns>Time control object.</returns>
        public static TimeControl FromSelectionString(string selString, System.Windows.Controls.UserControl userControl)
        {
            string classical1h = (string)userControl.FindResource("strClassical1h");
            if (selString == classical1h)
            {
                return new TimeControl(60 * 60 * 1000, 60 * 60 * 1000, 0);
            } 
            else if(selString == (string)userControl.FindResource("strClassical30m"))
            {
                return new TimeControl(30 * 60 * 1000, 30 * 60 * 1000, 0);
            }
            else if (selString == (string)userControl.FindResource("strRapid"))
            {
                return new TimeControl(10 * 60 * 1000, 10 * 60 * 1000, 0);
            }
            else if (selString == (string)userControl.FindResource("strBlitz"))
            {
                return new TimeControl(3 * 60 * 1000, 3 * 60 * 1000, 2000);
            }
            else if (selString == (string)userControl.FindResource("strBullet"))
            {
                return new TimeControl(60 * 1000, 60 * 1000, 0);
            } else
            {
                throw new ArgumentException(selString + " is not a valid time control string!");
            }
        }

    }
}
