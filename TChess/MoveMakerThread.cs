using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TChess2.Agents;
using TChess2.Events;

namespace TChess2.TChess
{
    /// <summary>
    /// A background thread that uses an agent, which makes a move 
    /// in the background, and then fires the move made event.
    /// This is so that while a move is being searched, the UI is 
    /// not frozen.
    /// </summary>
    public abstract class MoveMakerThread
    {

        /// <summary>
        /// Make a move in the background. A special case is where a human player 
        /// is the agent. In this case no move searching is needed (because the 
        /// player will select a move on the GUI) and this method will simply return.
        /// </summary>
        /// <param name="agent">The agent that must make a move.</param>
        /// <param name="fen">FEN of the chess position.</param>
        public static void StartMoveMakerThread(Agents.Agent agent, string fen)
        {
            //return for human players
            if (agent is PlayerAgent) return;
            //this agent needs to calculate a move
            Thread moveMakerThread = new Thread(() =>
            {
                //make move
                Move move = agent.MakeMove(fen);
                var hub = MessageHub.MessageHub.Instance;
                //publish an event of this move ON THE MAIN THREAD

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    hub.Publish(new EventMoveMade
                    {
                        ChosenMove = move
                    });
                }));
            });
            moveMakerThread.Priority = ThreadPriority.AboveNormal;
            moveMakerThread.Name = $"{agent.Name} move maker thread";
            //same as daemon in Java
            moveMakerThread.IsBackground = true;
            moveMakerThread.Start();
        }

    }
}
