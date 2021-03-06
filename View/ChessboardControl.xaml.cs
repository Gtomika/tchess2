using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessDotNet;
using ChessDotNet.Pieces;
using TChess2;
using TChess2.Agents;
using TChess2.Events;
using TChess2.TChess;

/*
 * https://github.com/thomas-daniels/Chess.NET/blob/master/Samples/BasicUsageSample.cs
 * https://www.youtube.com/watch?v=THV5BW5WW_o
 */

namespace TChess2.View
{
    /// <summary>
    /// Interaction logic for ChessboardControl.xaml
    /// </summary>
    public partial class ChessboardControl : UserControl
    {
        public ChessboardControl()
        {
            InitializeComponent();
            PieceImages = new Dictionary<Position, Image>();
            MoveHelpers = new List<Ellipse>();
            BoardFlip = TChess.BoardFlip.NORMAL;
            //setup board: square backgrounds
            CreateChessboardSquares(BoardFlip);
            //setup board: initial setup of pieces
            ChessGame game = new ChessGame();
            DrawPieces(game, BoardFlip);
            //subscribe to events
            var hub = MessageHub.MessageHub.Instance;
            hub.Subscribe<EventBoardFlipped>(e => OnBoardFlipped(e));
            hub.Subscribe<EventGameStarted>(e => OnGameStarted(e));
            hub.Subscribe<EventMoveMade>(e => OnMoveMade(e));
            hub.Subscribe<EventResignClicked>(e => OnResignClicked(e));
            hub.Subscribe<EventTimeOut>(e => OnTimeOut(e));
            //some initialization
            GameOngoing = false;
            WaitingForGuiMove = false;
            ChessSoundPlayer = new ChessSoundPlayer();
            MoveVoicer = new MoveVoicer();
        }

        //Rectangles that are the backgrounds of each square on the board.
        //Initialized in CreateChessboardSquares.
        private Rectangle[] ChessboardSquareBackgrounds;

        //Stores the displayed piece images.
        private Dictionary<Position, Image> PieceImages;

        //The current chess game (maybe ongoing or over). It is possible
        //that this is null, in this case there is no game started yet.
        private ChessGame CurrentGame;

        //Stores the board flip status.
        private TChess.BoardFlip BoardFlip;

        //stores if there is an active, ongoing game.
        private bool GameOngoing { get; set; }

        //stores if the program is waiting for a human player to make a move.
        private bool WaitingForGuiMove { get; set; }

        //This value is true if a piece is selected for move on the GUI.
        private bool PieceSelected { get; set; }

        //The user has this square currently selected.
        private Position SelectedPosition { get; set; }

        /// <summary>
        /// This agent plays for white. Set when a game is started.
        /// </summary>
        public Agent WhiteAgent { get; set; }

        /// <summary>
        /// This agent plays for black. Set when a game is started.
        /// </summary>
        public Agent BlackAgent { get; set; }

        //plays move sounds
        private ChessSoundPlayer ChessSoundPlayer;

        //says moves out loud
        private MoveVoicer MoveVoicer;

        /// <summary>
        /// Creates and stores the rectangles that are the backgrounds of the squares.
        /// They are also added to the chessboard grid. Then drws the rank and 
        /// file names.
        /// </summary>
        private void CreateChessboardSquares(TChess.BoardFlip boardFlip)
        {
            //draw square backgrounds
            ChessboardSquareBackgrounds = new Rectangle[64];
            var lightSquareBrush = new SolidColorBrush { Color = Color.FromArgb(255, 215, 214, 210) };
            var darkSquareBrush = new SolidColorBrush { Color = Color.FromArgb(255, 189, 125, 19) };
            for (int i = 0; i < 64; i++)
            {
                int row = i / 8, column = i % 8;
                bool isLight = IsLightSquare(row, column);
                Rectangle background = new Rectangle
                {
                    Fill = isLight ? lightSquareBrush : darkSquareBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                Grid.SetColumn(background, column + 1);
                Grid.SetRow(background, row);
                Grid.SetZIndex(background, -1);
                //add click listener
                int iCopy = i;
                background.MouseDown += (source, e) =>
                {
                    OnSquareClicked(iCopy);
                };
                //save
                ChessboardSquareBackgrounds[i] = background;
                //add to grid
                ChessboardGrid.Children.Add(background);
            }
            //draw rank and file names
            if(boardFlip == TChess.BoardFlip.NORMAL)
            {
                TextRank1.Text = "1";
                TextRank2.Text = "2";
                TextRank3.Text = "3";
                TextRank4.Text = "4";
                TextRank5.Text = "5";
                TextRank6.Text = "6";
                TextRank7.Text = "7";
                TextRank8.Text = "8";
                TextFile1.Text = "a";
                TextFile2.Text = "b";
                TextFile3.Text = "c";
                TextFile4.Text = "d";
                TextFile5.Text = "e";
                TextFile6.Text = "f";
                TextFile7.Text = "g";
                TextFile8.Text = "h";
            } else
            {
                TextRank1.Text = "8";
                TextRank2.Text = "7";
                TextRank3.Text = "6";
                TextRank4.Text = "5";
                TextRank5.Text = "4";
                TextRank6.Text = "3";
                TextRank7.Text = "2";
                TextRank8.Text = "1";
                TextFile1.Text = "h";
                TextFile2.Text = "g";
                TextFile3.Text = "f";
                TextFile4.Text = "e";
                TextFile5.Text = "d";
                TextFile6.Text = "c";
                TextFile7.Text = "b";
                TextFile8.Text = "a";
            }
        }

        /// <summary>
        /// Determines if a square is light or dark. This is invariant 
        /// of board flip.
        /// </summary>
        /// <returns>True if square is light.</returns>
        private bool IsLightSquare(int row, int column)
        {
            if(row % 2 == 0)
            {
                return column % 2 == 0;
            }
            else
            {
                return column % 2 != 0;
            }
        }

        /// <summary>
        /// Draws the piece images over the correct squares. First removes 
        /// all currently drawn pieces.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="boardFlip">Specifies if the board is flipped or not.</param>
        private void DrawPieces(ChessGame game, BoardFlip boardFlip)
        {
            //clear board of drawings
            RemoveAllPieceDrawings();
            //check for check...
            bool whiteInCheck = false, blackInCheck = false;
            if(CurrentGame != null)
            {
                whiteInCheck = CurrentGame.IsInCheck(Player.White);
                blackInCheck = CurrentGame.IsInCheck(Player.Black);
            }

            //add drawings back
            for(int i = 0; i < 64; i++)
            {
                Position pos = TChessUtils.PositionFromIndex(i);
                Piece piece = game.GetPieceAt(pos);
                if(piece != null)
                {
                    //load image based on piece
                    Image pieceImage = TChessUtils.GetImageOfPiece(piece, this, whiteInCheck, blackInCheck);
                    //set position, depends on board flip
                    var gpos = TChessUtils.GridPositionFromIndex(i, boardFlip);
                    Grid.SetColumn(pieceImage, gpos.Item1);
                    Grid.SetRow(pieceImage, gpos.Item2);
                    Grid.SetZIndex(pieceImage, 5);
                    //add image to grid.
                    ChessboardGrid.Children.Add(pieceImage);
                    //save image to be able to remove it later
                    PieceImages[pos] = pieceImage;
                }
                
            }
        }

        /// <summary>
        /// Removes all piece images from the chessboard.
        /// </summary>
        private void RemoveAllPieceDrawings()
        {
            foreach(Image pieceImage in PieceImages.Values)
            {
                ChessboardGrid.Children.Remove(pieceImage);
            }
            PieceImages.Clear();
        }
      
        /// <summary>
        /// Called when a board flipped event was received.
        /// </summary>
        /// <param name="e">Board flipped event.</param>
        private void OnBoardFlipped(EventBoardFlipped e)
        {
            
            BoardFlip = BoardFlip == TChess.BoardFlip.NORMAL ?
                TChess.BoardFlip.FLIPPED : TChess.BoardFlip.NORMAL;
            //flip board: redraw squares and rank, file names
            CreateChessboardSquares(BoardFlip);
            //flip board: redraw pieces
            if(CurrentGame != null)
            {
                DrawPieces(CurrentGame, BoardFlip);
            } else
            {
                //draw defaul position
                DrawPieces(new ChessGame(), BoardFlip);
            }
            //remove selected status just in case
            RemoveMoveHelpers();
            SelectedPosition = null;
            PieceSelected = false;
            //the previous move displayers can suddenly get to invalid position
            RemovePreviousMoveHelpers();
            if(CurrentGame != null)
            {
                if(CurrentGame.Moves.Count > 0)
                {
                    DrawPreviousMoveHelpers(CurrentGame.Moves.Last());
                }
            }
        }

        /// <summary>
        /// Called when a game is started. Draws a starting position.
        /// </summary>
        /// <param name="e">Event with details.</param>
        private void OnGameStarted(EventGameStarted e)
        {
            //remove everything, in case anything is shown
            RemoveMoveHelpers();
            RemovePreviousMoveHelpers();
            //initialize new game
            GameOngoing = true;
            CurrentGame = new ChessGame();
            //setup board: initial setup of pieces
            DrawPieces(CurrentGame, BoardFlip);
            //get agents
            WhiteAgent = AgentUtils.AgentFromName(e.WhiteName, Player.White, this);
            BlackAgent = AgentUtils.AgentFromName(e.BlackName, Player.Black, this);
            //start the white agent
            if (WhiteAgent.IsHumanControlled())
            {
                WaitingForGuiMove = true;
            }
            MoveMakerThread.StartMoveMakerThread(WhiteAgent, CurrentGame.GetFen());
        }

        /// <summary>
        /// Called when the agent responsible to make the best move 
        /// has selected a move.
        /// </summary>
        /// <param name="e">Event with details.</param>
        private void OnMoveMade(EventMoveMade e)
        {
            //maybe the game already ended with a time out, but this move came in late
            if(!GameOngoing)
            {
                return;
            }
            //remove previos move helpers, if they are present, cause they are outdated now
            RemovePreviousMoveHelpers();
            //check new move validity
            if(CurrentGame.IsValidMove(e.ChosenMove))
            {
                //the agent made a valid move
                PieceSelected = false;
                WaitingForGuiMove = false;
                SelectedPosition = null;

                //save the piece that moved
                Piece pieceThatMoved = CurrentGame.GetPieceAt(e.ChosenMove.OriginalPosition);

                //play the move in the game object
                MoveType moveType = CurrentGame.MakeMove(e.ChosenMove, alreadyValidated:true);
                /*
                 * update the board. This is not very effective, as it 
                 * updates the whole board, even though only a few squares changed.
                 * However, I avoid some nasty logic about castling and en passant.
                 */
                DrawPieces(CurrentGame, BoardFlip);

                //send the SAN of this move as an event
                MessageHub.MessageHub.Instance.Publish(new EventMoveSan
                {
                    San = CurrentGame.Moves.Last().SAN
                });

                //draw the previous move helpers
                DrawPreviousMoveHelpers(e.ChosenMove);

                //play a sound for this move (if needed)
                PlayMoveSound(moveType);

                //voice this move (if needed)
                VoiceMove(e.ChosenMove, pieceThatMoved, moveType);

                //check if the game is over
                var report = GameStatusReport.CreateGameReport(CurrentGame, this);
                if(report.Status == GameStatus.ONGOING)
                {
                    //the game is still ongoing
                    //tell the other agent to make the next move.
                    var agentToMakeNextMove = CurrentGame.WhoseTurn == Player.White 
                        ? WhiteAgent : BlackAgent;
                    if(agentToMakeNextMove.IsHumanControlled())
                    {
                        WaitingForGuiMove = true;
                    }
                    MoveMakerThread.StartMoveMakerThread(
                        agentToMakeNextMove,
                        CurrentGame.GetFen()
                    );
                }
                else
                {
                    //the game is over
                    OnGameOver(report);
                }
            }
            else
            {
                Console.WriteLine($"{CurrentGame.WhoseTurn} made an invalid move: {e.ChosenMove}. This player is resigned.");
                //the agent made an invalid move, should not happen
                //this agent must resign
                CurrentGame.Resign(CurrentGame.WhoseTurn);
                var report = GameStatusReport.CreateGameReport(CurrentGame, this);
                OnGameOver(report);
            }
        }

        /// <summary>
        /// Called when a game is finished.
        /// </summary>
        /// <param name="gameReport">Contains the result.</param>
        private void OnGameOver(GameStatusReport gameReport)
        {
            //cancel everything
            GameOngoing = false;
            WaitingForGuiMove = false;
            PieceSelected = false;
            SelectedPosition = null;
            RemoveMoveHelpers();

            //publish game over for other components
            var hub = MessageHub.MessageHub.Instance;
            hub.Publish(new EventGameOver { GameReport = gameReport });
        }

        /// <summary>
        /// Called when one of the chessboard squares was clicked.
        /// </summary>
        /// <param name="i">Index of the square.</param>
        private void OnSquareClicked(int i)
        {
            //remove helpers, in case they are present
            RemoveMoveHelpers();
            //find in game position of this square, such as "A4".
            var pos = TChessUtils.PositionFromIndex(i, BoardFlip);
            //Console.WriteLine($"The square {pos} was clicked!");
            if (!PieceSelected) //there is currently no piece selected, user is attempting to select one
            {    
                //if this is a human controlled players's turn to move
                if (GameOngoing && WaitingForGuiMove)
                {
                    var piece = CurrentGame.GetPieceAt(pos);
                    if (piece != null && piece.Owner == CurrentGame.WhoseTurn)
                    {
                        //there is a piece on this square which belongs 
                        //to the player whos turn to move
                        var moves = CurrentGame.GetValidMoves(pos);
                        //Console.WriteLine($"From {pos} {CurrentGame.WhoseTurn} can move to {moves.Count} squares.");
                        //draw helpers
                        if (moves.Count > 0)
                        {
                            //user has legal moves with tihs piece
                            //save this position as selected
                            SelectedPosition = pos;
                            PieceSelected = true;
                            DrawMoveHelpers(moves);
                        }
                       
                    }
                }
            } else //there is a piece selected, user is attempting to make a move with it
            {
                if(GameOngoing && WaitingForGuiMove)
                {
                    var piece = CurrentGame.GetPieceAt(pos);
                    var selectedPiece = CurrentGame.GetPieceAt(SelectedPosition);
                    bool canCastle = CurrentGame.WhoseTurn == Player.White ? (CurrentGame.CanWhiteCastleKingSide || CurrentGame.CanWhiteCastleQueenSide)
                           : (CurrentGame.CanBlackCastleKingSide || CurrentGame.CanBlackCastleQueenSide);
                    bool possibleCastle = (selectedPiece is King) && (piece is Rook) && canCastle;
                    if (piece != null && piece.Owner == CurrentGame.WhoseTurn && pos != SelectedPosition && !possibleCastle)
                    {
                        //user has another piece on this square, which means he reconsidered and wants to make 
                        //a move now with this piece instead
                        PieceSelected = false;
                        SelectedPosition = null;
                        OnSquareClicked(i);
                    } else
                    {
                        //user wants to make a move, to an empty or enemy square
                        //this is the move the user wants to make
                        Move attemptedMove = new Move(SelectedPosition, pos, CurrentGame.WhoseTurn);
                        //is this move a promotion? then user will have to select what to promote to
                        if (selectedPiece is Pawn && TChessUtils.IsPromotionRankOf(CurrentGame.WhoseTurn, pos.Rank))
                        {
                            var dialog = new PromotionDialog();
                            dialog.ShowDialog();
                            attemptedMove.Promotion = dialog.Promotion;
                        }
                        //only allow if legal
                        if (CurrentGame.IsValidMove(attemptedMove))
                        {
                            //publish a move made event
                            var hub = MessageHub.MessageHub.Instance;
                            hub.Publish(new EventMoveMade { ChosenMove = attemptedMove });
                        }
                        else
                        {
                            //this move is not legal
                            SelectedPosition = null;
                            PieceSelected = false;
                        }
                    }
                }
            }
          
        }

        /// <summary>
        /// Plays a sound depending on the move made.
        /// </summary>
        /// <param name="moveType">Type of the move.</param>
        private void PlayMoveSound(MoveType moveType)
        {
            if(!Properties.Settings.Default.PlayMoveSounds)
            {
                return; //user disabled playing sounds
            }
            bool check = CurrentGame.IsInCheck(CurrentGame.WhoseTurn);
            if(check)
            {
                ChessSoundPlayer.PlayCheckSound();
            } else if(moveType.HasFlag(MoveType.Capture))
            {
                ChessSoundPlayer.PlayCaptureSound();
            } else
            {
                ChessSoundPlayer.PlayMoveSound();
            }
        }

        private void VoiceMove(Move move, Piece pieceThatMoved, MoveType moveType)
        {
            var agentWhoPlayedThisMove = CurrentGame.WhoseTurn == Player.White ? BlackAgent : WhiteAgent;
            if(!agentWhoPlayedThisMove.IsHumanControlled() && Properties.Settings.Default.VoiceComputerMoves)
            {
                //this is a computer agent and voicing moves is turned on
                string san = CurrentGame.Moves.Last().SAN;
                MoveVoicer.VoiceMove(san, move, pieceThatMoved, moveType);
            }
        }

        //stores the currently displayed move helpers.
        private List<Ellipse> MoveHelpers;

        /// <summary>
        /// Draws move helpers for the given moves. 
        /// </summary>
        /// <param name="moves">The moves.</param>
        private void DrawMoveHelpers(IReadOnlyCollection<Move> moves)
        {
            if(!Properties.Settings.Default.ShowLegalMoves)
            {
                return; //user disabled showing legal moves, in other words the move helpers
            }
            //semi transparent color for the move helpers
            var brush = new SolidColorBrush { Color = Color.FromArgb(175, 169, 169, 169) };
            foreach (Move move in moves)
            {
                //the move helper must be placed to the moves destination
                var gpos = TChessUtils.GridPositionFromPosition(move.NewPosition, BoardFlip);
                Ellipse moveHelper = new Ellipse
                {
                    IsHitTestVisible = false, //allow click through
                    Fill = brush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 30,
                    Height = 30
                };
                Grid.SetColumn(moveHelper, gpos.Item1);
                Grid.SetRow(moveHelper, gpos.Item2);
                Grid.SetZIndex(moveHelper, 10); //above everything, even pieces
                //save
                MoveHelpers.Add(moveHelper);
                //add to grid
                ChessboardGrid.Children.Add(moveHelper);
            }
        }

        /// <summary>
        /// Clears the move helpers from the board.
        /// </summary>
        private void RemoveMoveHelpers()
        {
            foreach(Ellipse moveHelper in MoveHelpers)
            {
                ChessboardGrid.Children.Remove(moveHelper);
            }
            MoveHelpers.Clear();
        }

        /// <summary>
        /// Called when the user clicked the resign button.
        /// </summary>
        /// <param name="e">Unused.</param>
        private void OnResignClicked(EventResignClicked e)
        {
            //only if this is a GUI based player's turn to move
            if(GameOngoing && WaitingForGuiMove)
            {
                //the current player resigns
                CurrentGame.Resign(CurrentGame.WhoseTurn);
                var report = GameStatusReport.CreateGameReport(CurrentGame, this);
                OnGameOver(report);
            }
        }

        //stores the currently displayed previous move helper rectangles
        private Rectangle[] PreviousMoveHelpers;

        /// <summary>
        /// Draws 2 rectangles to the start and finish square of the given move.
        /// </summary>
        /// <param name="move">The move.</param>
        private void DrawPreviousMoveHelpers(Move move)
        {
            if(!Properties.Settings.Default.ShowPreviousMove)
            {
                return; //this user disabled this
            }
            if(PreviousMoveHelpers == null)
            {
                PreviousMoveHelpers = new Rectangle[2];
            }
            //semi transparent yellow
            var brush = new SolidColorBrush { Color = Color.FromArgb(120, 240, 230, 140) };
            //get grid position of start square
            var gposStart = TChessUtils.GridPositionFromPosition(move.OriginalPosition, BoardFlip);
            //get grid position of destination square
            var gposEnd = TChessUtils.GridPositionFromPosition(move.NewPosition, BoardFlip);

            //create the rectangles
            var startHelper = new Rectangle
            {
                IsHitTestVisible = false, //allow click through
                Fill = brush,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            var endHelper = new Rectangle
            {
                IsHitTestVisible = false, //allow click through
                Fill = brush,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            //set grid properties
            Grid.SetColumn(startHelper, gposStart.Item1);
            Grid.SetRow(startHelper, gposStart.Item2);
            Grid.SetZIndex(startHelper, 1);
            Grid.SetColumn(endHelper, gposEnd.Item1);
            Grid.SetRow(endHelper, gposEnd.Item2);
            Grid.SetZIndex(endHelper, 1);
            //save and add them
            PreviousMoveHelpers[0] = startHelper;
            PreviousMoveHelpers[1] = endHelper;
            ChessboardGrid.Children.Add(startHelper);
            ChessboardGrid.Children.Add(endHelper);
        }

        //deletes the previos move helper rectangles
        private void RemovePreviousMoveHelpers()
        {
            if(PreviousMoveHelpers != null)
            {
                if(PreviousMoveHelpers[0] != null)
                {
                    ChessboardGrid.Children.Remove(PreviousMoveHelpers[0]);
                }
                if (PreviousMoveHelpers[1] != null)
                {
                    ChessboardGrid.Children.Remove(PreviousMoveHelpers[1]);
                }
            }
        }

        /// <summary>
        /// Called when a side times out.
        /// </summary>
        /// <param name="e">Event with details.</param>
        private void OnTimeOut(EventTimeOut e)
        {
            //build report of time out
            var report = new GameStatusReport
            (
                e.PlayerWhoTimedOut == Player.White ? GameStatus.BLACK_WINS : GameStatus.WHITE_WINS,
                (string)FindResource("strTimeOut")
            );
            //call game over
            OnGameOver(report);
        }
    }
}
