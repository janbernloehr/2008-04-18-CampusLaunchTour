using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using Valil.Chess.Model;
using Valil.Chess.WinFX.VisualElements.Properties;
using System.Windows.Shapes;

namespace Valil.Chess.WinFX.VisualElements
{
    /// <summary>
    /// Implements a 2D game board.
    /// </summary>
    public partial class GameBoard2D : UserControl
    {
        /// <summary>
        /// The square dimension in device independent pixels.
        /// </summary>
        public readonly int SquareSize;

        /// <summary>
        /// Random generator used for generating unique names for pieces.
        /// </summary>
        private static readonly Random random = new Random(unchecked((int)DateTime.Now.Ticks));

        /// <summary>
        /// The model.
        /// </summary>
        private Game model;

        /// <summary>
        /// Stores the 2D chess pieces.
        /// </summary>
        private Rectangle[] visualPieces;

        /// <summary>
        /// The position of the current piece.
        /// </summary>
        private int? piecePos;

        /// <summary>
        /// True to if the model loads moves, false otherwise.
        /// </summary>
        private bool isModelLoadingMoves;

        /// <summary>
        /// True to show the moves on the board animated, false otherwise.
        /// </summary>
        private bool showMoveAnimation;
        /// <summary>
        /// The move storyboard.
        /// </summary>
        private Storyboard moveStoryboard;
        /// <summary>
        /// Keeps a reference to the move to be animated.
        /// </summary>
        private Move animatedMove;
        /// <summary>
        /// True if the board is animating a move, false otherwise.
        /// </summary>
        public bool IsAnimating
        {
            get { return animatedMove != null; }
        }

        /// <summary>
        /// The model.
        /// </summary>
        public Game Model
        {
            get { return model; }
            set
            {
                // the model can be set only once
                if (model == null && value != null)
                {
                    model = value;

                    // hook events
                    model.Moving += model_Moving;
                    model.Moved += model_Moved;
                    model.GoingForward += model_GoingForward;
                    model.GoneForward += model_Moved;
                    model.GoingBack += model_GoingBack;
                    model.GoneBack += model_GoneBack;
                    model.Modifying += model_Modifying;
                    model.Modified += model_Modified;
                    model.Loading += model_Modifying;
                    model.BoardConfigurationLoaded += model_Modified;
                    model.GameBoardConfigurationLoaded += model_GameBoardConfigurationLoaded;
                    model.GameMoveSectionLoaded += model_GameMoveSectionLoaded;

                    isModelLoadingMoves = false;

                    showMoveAnimation = true;

                    // if the control is loaded, draw the board
                    if (IsLoaded)
                    {
                        DrawBoard();
                    }
                }
            }
        }

        public GameBoard2D()
        {
            InitializeComponent();

            visualPieces = new Rectangle[Board.SquareNo];

            // the square size is the board width divided by number of squares on one side
            SquareSize = (int)(gameBoard.Width / Board.SideSquareNo);

            // draw the board
            DrawBoard();
        }

        private void ControlUnloaded(object sender, EventArgs e)
        {
            if (model != null)
            {
                // unhook the events
                model.Moving -= model_Moving;
                model.Moved -= model_Moved;
                model.GoingForward -= model_GoingForward;
                model.GoneForward -= model_Moved;
                model.GoingBack -= model_GoingBack;
                model.GoneBack -= model_GoneBack;
                model.Modifying -= model_Modifying;
                model.Modified -= model_Modified;
                model.Loading -= model_Modifying;
                model.BoardConfigurationLoaded -= model_Modified;
                model.GameBoardConfigurationLoaded -= model_GameBoardConfigurationLoaded;
                model.GameMoveSectionLoaded -= model_GameMoveSectionLoaded;
            }
        }

        private void MouseLeftButtonPressed(object sender, MouseButtonEventArgs args)
        {
            // do not allow any modifications to the game board during a move animation
            if (IsAnimating) { return; }

            // transform in game board coordinates
            Point p = args.GetPosition(gameBoard);

            // get the position on the board
            int pos = ((int)p.Y / SquareSize) * Board.SideSquareNo + ((int)p.X / SquareSize);

            // select the position on the board according to the board rotation
            Select(Settings.Default.RotatedBoard2D ? (Board.SquareNo - 1) - pos : pos);
        }

        /// <summary>
        /// Selects a position on the board.
        /// </summary>
        /// <param name="position">The position to be selected</param>
        public void Select(int position)
        {
            // check if the position is OK, if the model is not null, and if the game is not ended
            // do not allow any modifications to the game board during a move animation
            if (position < 0 || position > (Board.SquareNo - 1) || model == null || model.IsEnded || IsAnimating)
            {
                return;
            }

            // if a side to move piece is selected
            if (model.CurrentBoard.IsSideToMovePiece(position) && piecePos != position)
            {
                // draw the piece selection
                DrawSelection(position);
                return;
            }

            // if there is a selected piece and an opposite piece or empty square is selected, try to move
            if (piecePos != null && !model.CurrentBoard.IsSideToMovePiece(position))
            {
                Move move = model.GetMove(piecePos.Value, position, null);

                // if the move is valid, make the move
                if (move != null)
                {
                    // do not animate the move, make the move at once
                    showMoveAnimation = false;

                    model.Make(move);
                }
                return;
            }
        }

        /// <summary>
        /// Rotates the board.
        /// </summary>
        public void Rotate()
        {
            try
            {
                Settings.Default.RotatedBoard2D = !Settings.Default.RotatedBoard2D;
                Settings.Default.Save();
            }
            catch
            {
            }

            DrawBoard();
        }

        /// <summary>
        /// Draws the whole board.
        /// </summary>
        private void DrawBoard()
        {
            // no selections
            pieceSelection.Visibility = Visibility.Hidden;
            squareSelection.Visibility = Visibility.Hidden;
            piecePos = null;

            // set the margin labels
            v1.Text = Settings.Default.RotatedBoard2D ? "1" : "8";
            v2.Text = Settings.Default.RotatedBoard2D ? "2" : "7";
            v3.Text = Settings.Default.RotatedBoard2D ? "3" : "6";
            v4.Text = Settings.Default.RotatedBoard2D ? "4" : "5";
            v5.Text = Settings.Default.RotatedBoard2D ? "5" : "4";
            v6.Text = Settings.Default.RotatedBoard2D ? "6" : "3";
            v7.Text = Settings.Default.RotatedBoard2D ? "7" : "2";
            v8.Text = Settings.Default.RotatedBoard2D ? "8" : "1";
            h1.Text = Settings.Default.RotatedBoard2D ? "H" : "A";
            h2.Text = Settings.Default.RotatedBoard2D ? "G" : "B";
            h3.Text = Settings.Default.RotatedBoard2D ? "F" : "C";
            h4.Text = Settings.Default.RotatedBoard2D ? "E" : "D";
            h5.Text = Settings.Default.RotatedBoard2D ? "D" : "E";
            h6.Text = Settings.Default.RotatedBoard2D ? "C" : "F";
            h7.Text = Settings.Default.RotatedBoard2D ? "B" : "G";
            h8.Text = Settings.Default.RotatedBoard2D ? "A" : "H";

            if (model != null && model.IsInitialized)
            {
                // loop through the squares and draw all the pieces
                for (int sqIndex = 0; sqIndex < Board.SquareNo; sqIndex++)
                {
                    // modify the current visual piece only if the corresponding piece from the model has changed
                    if ((visualPieces[sqIndex] != null || model.CurrentBoard[sqIndex] != null) &&
                    (visualPieces[sqIndex] == null || model.CurrentBoard[sqIndex] == null || !visualPieces[sqIndex].Name.StartsWith(model.CurrentBoard[sqIndex].GetType().Name)))
                    {
                        // remove the old visual representation
                        if (visualPieces[sqIndex] != null)
                        {
                            pieceLayer.Children.Remove(visualPieces[sqIndex]);
                            visualPieces[sqIndex] = null;
                        }

                        // add the new visual piece
                        if (model.CurrentBoard[sqIndex] != null)
                        {
                            visualPieces[sqIndex] = GenerateVisualPiece(model.CurrentBoard[sqIndex].GetType());
                        }
                    }

                    // set the visual piece position
                    if (visualPieces[sqIndex] != null)
                    {
                        SetSpritePosition(visualPieces[sqIndex], sqIndex);
                    }
                }
            }
            else
            {
                // remove all pieces
                for (int sqIndex = 0; sqIndex < Board.SquareNo; sqIndex++)
                {
                    if (visualPieces[sqIndex] != null)
                    {
                        pieceLayer.Children.Remove(visualPieces[sqIndex]);
                        visualPieces[sqIndex] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Draws a piece selection.
        /// </summary>
        /// <param name="position">The selected position</param>
        private void DrawSelection(int position)
        {
            // hide the square selection
            squareSelection.Visibility = Visibility.Hidden;

            // show and position the piece selection
            SetSpritePosition(pieceSelection, position);
            pieceSelection.Visibility = Visibility.Visible;
            piecePos = position;
        }

        /// <summary>
        /// Draws a move.
        /// </summary>
        /// <param name="move">The move</param>
        private void DrawMoved(Move move)
        {
            if (move.HasCapture)
            {
                // remove the capture
                int captureTarget = move is EnPassantCaptureMove ? (move as EnPassantCaptureMove).Target : move.To;
                pieceLayer.Children.Remove(visualPieces[captureTarget]);
                visualPieces[captureTarget] = null;
            }

            if (move is CastlingMove)
            {
                Move rookMove = (move as CastlingMove).RookMove;

                // move the rook
                visualPieces[rookMove.To] = visualPieces[rookMove.From];
                SetSpritePosition(visualPieces[rookMove.To], rookMove.To);

                // empty the starting square
                visualPieces[rookMove.From] = null;
            }

            // show the piece selection on the ending square
            SetSpritePosition(pieceSelection, move.To);
            pieceSelection.Visibility = Visibility.Visible;
            piecePos = move.To;

            // move the visual piece from the starting square to the ending square
            visualPieces[move.To] = visualPieces[move.From];
            SetSpritePosition(visualPieces[move.To], move.To);

            // empty the starting square
            visualPieces[move.From] = null;

            // show the square selection on the starting square
            SetSpritePosition(squareSelection, move.From);
            squareSelection.Visibility = Visibility.Visible;

            if (move is PromotionMove)
            {
                // remove the promoted pawn
                pieceLayer.Children.Remove(visualPieces[move.To]);
                visualPieces[move.To] = null;

                // replace it with the promotion piece
                visualPieces[move.To] = GenerateVisualPiece(model.CurrentBoard[move.To].GetType());
                SetSpritePosition(visualPieces[move.To], move.To);
            }

        }

        /// <summary>
        /// Draws a taken back move.
        /// </summary>
        /// <param name="move">The move</param>
        private void DrawGoneBack(Move move)
        {
            if (move is CastlingMove)
            {
                Move rookMove = (move as CastlingMove).RookMove;

                // move back the rook
                visualPieces[rookMove.From] = visualPieces[rookMove.To];
                SetSpritePosition(visualPieces[rookMove.From], rookMove.From);

                // empty the ending square
                visualPieces[rookMove.To] = null;
            }

            // show the piece selection on the starting square
            SetSpritePosition(pieceSelection, move.From);
            pieceSelection.Visibility = Visibility.Visible;
            piecePos = move.From;

            if (move is PromotionMove)
            {
                // remove the promotion piece
                pieceLayer.Children.Remove(visualPieces[move.To]);
                visualPieces[move.To] = null;

                // add the promoted pawn
                visualPieces[move.From] = GenerateVisualPiece(model.CurrentBoard[move.From].GetType());
                SetSpritePosition(visualPieces[move.From], move.From);
            }
            else
            {
                // move the visual piece from the ending square to the start square
                visualPieces[move.From] = visualPieces[move.To];
                SetSpritePosition(visualPieces[move.From], move.From);

                // empty the ending square
                visualPieces[move.To] = null;
            }

            // show the square selection on the ending square
            SetSpritePosition(squareSelection, move.To);
            squareSelection.Visibility = Visibility.Visible;

            if (move.HasCapture)
            {
                // put back the capture
                int captureTarget = move is EnPassantCaptureMove ? (move as EnPassantCaptureMove).Target : move.To;
                visualPieces[captureTarget] = GenerateVisualPiece(model.CurrentBoard[captureTarget].GetType());
                SetSpritePosition(visualPieces[captureTarget], captureTarget);
            }

        }

        void model_Modified(object sender, EventArgs e)
        {
            // draw the whole board
            DrawBoard();
        }

        private void model_GoingBack(object sender, CancelMoveEventArgs e)
        {
            // do not allow any modifications to the game board during a move animation
            if (IsAnimating)
            {
                e.Cancel = true;
                return;
            }

            // select the piece which is about to move back
            if (piecePos != e.Move.To)
            {
                DrawSelection(e.Move.To);
            }
        }

        private void model_GoneBack(object sender, MoveEventArgs e)
        {
            DrawGoneBack(e.Move);
        }

        private void model_Moving(object sender, CancelMoveEventArgs e)
        {
            // model is loading the moves from a game, the board will be updated at the end
            if (isModelLoadingMoves) { return; }

            // do not allow any modifications to the game board during a move animation
            if (IsAnimating)
            {
                e.Cancel = true;
                return;
            }

            // select the piece which is about to move
            if (piecePos != e.Move.From)
            {
                DrawSelection(e.Move.From);
            }
        }

        private void model_Moved(object sender, MoveEventArgs e)
        {
            // model is loading the moves from a game, the board will be updated at the end
            if (isModelLoadingMoves) { return; }

            if (showMoveAnimation)
            {
                // animate the move
                Animate(e.Move);
            }
            else
            {
                // show the move in one step
                DrawMoved(e.Move);
            }

            // if it's the end of game, no selections
            if (model.IsEnded)
            {
                squareSelection.Visibility = Visibility.Hidden;
                pieceSelection.Visibility = Visibility.Hidden;
                piecePos = null;
            }

            showMoveAnimation = true;
        }

        private void model_GoingForward(object sender, CancelMoveEventArgs e)
        {
            // do not allow any modifications to the game board during a move animation
            if (IsAnimating)
            {
                e.Cancel = true;
                return;
            }

            // select the piece which is about to move
            if (piecePos != e.Move.From)
            {
                DrawSelection(e.Move.From);
            }

            showMoveAnimation = false;
        }

        private void model_Modifying(object sender, CancelEventArgs e)
        {
            // do not allow any modifications to the game board during a move animation
            if (IsAnimating)
            {
                e.Cancel = true;
                return;
            }
        }

        private void model_GameBoardConfigurationLoaded(object sender, EventArgs e)
        {
            // stop updating the board for the loaded moves
            isModelLoadingMoves = true;
        }

        private void model_GameMoveSectionLoaded(object sender, EventArgs e)
        {
            // restore updating the board for when the model moves
            isModelLoadingMoves = false;

            // draw the whole board
            DrawBoard();
        }

        /// <summary>
        /// Generates a visual piece according to the piece type.
        /// </summary>
        /// <param name="pieceType"></param>
        /// <returns></returns>
        private Rectangle GenerateVisualPiece(Type pieceType)
        {
            Rectangle visualPiece = new Rectangle();

            // set the dimension
            visualPiece.Width = SquareSize;
            visualPiece.Height = SquareSize;

            // set the type
            visualPiece.Fill = FindResource(pieceType.Name + "Brush") as Brush;

            // set the unique name as the piece type name plus a random generated number 
            visualPiece.Name = pieceType.Name + random.Next(100000000, 1000000000).ToString(CultureInfo.InvariantCulture);

            visualPiece.RenderTransform = new TranslateTransform();

            // add it to the piece layer and register its name
            pieceLayer.Children.Add(visualPiece);
            pieceLayer.RegisterName(visualPiece.Name, visualPiece);

            return visualPiece;
        }

        /// <summary>
        /// Sets the sprite position to this square on the board.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="position"></param>
        private void SetSpritePosition(Rectangle sprite, int position)
        {
            (sprite.RenderTransform as TranslateTransform).X = Board.File(Settings.Default.RotatedBoard2D ? (Board.SquareNo - 1) - position : position) * SquareSize;
            (sprite.RenderTransform as TranslateTransform).Y = Board.Rank(Settings.Default.RotatedBoard2D ? (Board.SquareNo - 1) - position : position) * SquareSize;
        }

        /// <summary>
        /// Animates a move.
        /// </summary>
        /// <param name="move"></param>
        private void Animate(Move move)
        {
            // set a reference to the animated move 
            animatedMove = move;

            // bring the animated piece to front
            Canvas.SetZIndex(visualPieces[move.From], Canvas.GetZIndex(visualPieces[move.From]) + 1);
            
            // set the piece translation duration according to the animation parameters
            int translationDuration = Settings.Default.AnimationDurationBaseFactor + (int)(Settings.Default.AnimationDurationScaleFactor * Math.Sqrt(
                (Board.File(move.To) - Board.File(move.From)) * (Board.File(move.To) - Board.File(move.From)) +
                (Board.Rank(move.To) - Board.Rank(move.From)) * (Board.Rank(move.To) - Board.Rank(move.From))));

            // set the captured piece fading duration to be less than the translation duration
            int fadeDuration = translationDuration - Settings.Default.AnimationDurationScaleFactor;

            // set the move storyboard
            moveStoryboard = new Storyboard();
            moveStoryboard.FillBehavior = FillBehavior.Stop;

            // set the translation animation for this move
            SetTranslationAnimation(moveStoryboard, move, translationDuration);

            if (move is CastlingMove)
            {
                // set the translation animation for the rook move
                SetTranslationAnimation(moveStoryboard, (move as CastlingMove).RookMove, translationDuration);
            }

            if (move.HasCapture)
            {
                // set the fading animation for the captured piece
                SetOpacityAnimation(moveStoryboard, move, fadeDuration);
            }

            // detect when the animation has terminated
            moveStoryboard.Completed += delegate(object sender, EventArgs e)
            {
                // send the animated piece to back
                Canvas.SetZIndex(visualPieces[move.From], Canvas.GetZIndex(visualPieces[move.From]) - 1);

                // draw the move
                DrawMoved(animatedMove);

                animatedMove = null;
            };

            // the storyboard will not be modified so it will be freezed
            moveStoryboard.Freeze();

            // start the animation
            moveStoryboard.Begin(pieceLayer, HandoffBehavior.Compose);
        }

        /// <summary>
        /// Sets the fading animation for the captured piece.
        /// </summary>
        /// <param name="moveStoryboard"></param>
        /// <param name="move"></param>
        /// <param name="duration"></param>
        private void SetOpacityAnimation(Storyboard moveStoryboard, Move move, int duration)
        {
            // animate the captured piece opacity from the current value (1.0) to 0.0
            // the animations will not be modified so they will be freezed

            // set the opacity animation
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            opacityAnimation.BeginTime = TimeSpan.FromMilliseconds(Settings.Default.AnimationStartDelay);
            Storyboard.SetTargetName(opacityAnimation, visualPieces[move is EnPassantCaptureMove ? (move as EnPassantCaptureMove).Target : move.To].Name);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Rectangle.OpacityProperty));
            opacityAnimation.Freeze();

            // add the animation to the move storyboard
            moveStoryboard.Children.Add(opacityAnimation);
        }

        /// <summary>
        /// Sets the translation animation for the move.
        /// </summary>
        /// <param name="moveStoryboard"></param>
        /// <param name="move"></param>
        /// <param name="duration"></param>
        private void SetTranslationAnimation(Storyboard moveStoryboard, Move move, int duration)
        {
            // animate the piece from the current position to the ending square
            // the animations will not be modified so they will be freezed

            // set the animation along the X axis
            DoubleAnimation xAnimation = new DoubleAnimation();
            xAnimation.To = Board.File(Settings.Default.RotatedBoard2D ? (Board.SquareNo - 1) - move.To : move.To) * SquareSize;
            xAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            xAnimation.BeginTime = TimeSpan.FromMilliseconds(Settings.Default.AnimationStartDelay);
            Storyboard.SetTargetName(xAnimation, visualPieces[move.From].Name);
            Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(0).(1)", Rectangle.RenderTransformProperty, TranslateTransform.XProperty));
            xAnimation.Freeze();

            // set the animation along the Y axis
            DoubleAnimation yAnimation = new DoubleAnimation();
            yAnimation.To = Board.Rank(Settings.Default.RotatedBoard2D ? (Board.SquareNo - 1) - move.To : move.To) * SquareSize;
            yAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            yAnimation.BeginTime = TimeSpan.FromMilliseconds(Settings.Default.AnimationStartDelay);
            Storyboard.SetTargetName(yAnimation, visualPieces[move.From].Name);
            Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(0).(1)", Rectangle.RenderTransformProperty, TranslateTransform.YProperty));
            yAnimation.Freeze();

            // add the animations to the storyboard
            moveStoryboard.Children.Add(xAnimation);
            moveStoryboard.Children.Add(yAnimation);
        }
    }
}