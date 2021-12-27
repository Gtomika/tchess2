using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TChess2.TChess
{

    /// <summary>
    /// Possible states of a game.
    /// </summary>
    public enum GameStatus
    {
        ONGOING, WHITE_WINS, BLACK_WINS, DRAW
    }

    /// <summary>
    /// Contains information about the game status, which can be 
    /// used to decide if the game is over or still ongoing. In case 
    /// of a finished game, a reason is also given why it is finished.
    /// </summary>
    public class GameStatusReport
    {

        public GameStatusReport(GameStatus status, string reason)
        {
            Status = status;
            Reason = reason;
        }

        public GameStatus Status { get; set; }

        public string Reason { get; set; }

        /// <summary>
        /// Utility method to make a report about a game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="control">Used to access resources.</param>
        /// <returns>Game status report.</returns>
        public static GameStatusReport CreateGameReport(ChessGame game, UserControl control)
        {
            if(game.IsCheckmated(Player.White))
            {
                string mate = (string)control.FindResource("strCheckmate");
                return new GameStatusReport(GameStatus.BLACK_WINS, mate);
            }
            if (game.IsCheckmated(Player.Black))
            {
                string mate = (string)control.FindResource("strCheckmate");
                return new GameStatusReport(GameStatus.WHITE_WINS, mate);
            }
            if (game.IsStalemated(Player.White) || game.IsStalemated(Player.Black))
            {
                string smate = (string)control.FindResource("strStalemate");
                return new GameStatusReport(GameStatus.DRAW, smate);
            }
            if (game.IsInsufficientMaterial())
            {
                string msg = (string)control.FindResource("strInsufficientMaterial");
                return new GameStatusReport(GameStatus.DRAW, msg);
            }
            //this library only support 50 move rule, not repetition
            //this means draw because of 50 move rule
            if(game.DrawCanBeClaimed)
            {
                string msg = (string)control.FindResource("strFiftyMoveRule");
                game.ClaimDraw(msg);
                return new GameStatusReport(GameStatus.DRAW, msg);
            }
            var resigned = game.Resigned;
            if(resigned == Player.Black)
            {
                string msg = (string)control.FindResource("strResignation");
                return new GameStatusReport(GameStatus.WHITE_WINS, msg);
            }
            if (resigned == Player.White)
            {
                string msg = (string)control.FindResource("strResignation");
                return new GameStatusReport(GameStatus.BLACK_WINS, msg);
            }
            //TODO: handle draw by repetition

            //the game is not over
            return new GameStatusReport(GameStatus.ONGOING, "");
        }

    }
}
