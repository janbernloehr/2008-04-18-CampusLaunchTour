using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Valil.Chess.Model;
using Valil.Chess.WinFX.VisualElements.Properties;
using System.Collections.Generic;

namespace Valil.Chess.WinFX.VisualElements
{
    /// <summary>
    /// Implements a 3D game board.
    /// </summary>
    public partial class GameBoard3D : UserControl
    {
        /// <summary>
        /// Random generator used for generating unique names for pieces.
        /// </summary>
        private static readonly Random random = new Random(unchecked((int)DateTime.Now.Ticks));

        /// <summary>
        /// Keeps the name of the 3D visuals.
        /// </summary>
        private static DependencyProperty name = DependencyProperty.Register("Name", typeof(string), typeof(GameBoard3D), new PropertyMetadata(String.Empty));

        /// <summary>
        /// The square size (in world units).
        /// </summary>
        private const double SquareSize = 4;
        
        /// <summary>
        /// Minimum distance from the camera to the world origin.
        /// </summary>
        private const double minDistance = 32.0;
        /// <summary>
        /// Maximum distance from the camera to the world origin.
        /// </summary>
        private const double maxDistance = 90.0;
        
        /// <summary>
        /// True if the scene is in dragging mode, false otherwise.
        /// </summary>
        private bool isDragging;
        /// <summary>
        /// Stores the last hit on the dragging sphere.
        /// </summary>
        private Vector3D initialSphereHitVector;
        /// <summary>
        /// Stores the current hit on the dragging sphere.
        /// </summary>
        private Vector3D currentSphereHitVector;
        /// <summary>
        /// Stores the top most hit test result from the viewable group.
        /// </summary>
        private RayHitTestResult viewableHitResult;
        /// <summary>
        /// The mouse pressed hit test result callback.
        /// </summary>
        private HitTestResultCallback mousePressedHitTestResultCallback;
        /// <summary>
        /// The hit test exclude selection filter callback.
        /// </summary>
        private HitTestFilterCallback hitTestExcludeSelectionFilter;
        /// <summary>
        /// The quaternion at the start of dragging.
        /// </summary>
        private Quaternion initialQuaternion;

        /// <summary>
        /// Keeps the position for the 3D visuals for the chess pieces.
        /// </summary>
        private static DependencyProperty position = DependencyProperty.Register("Position", typeof(int?), typeof(GameBoard3D), new PropertyMetadata((int?)null));

        /// <summary>
        /// The model.
        /// </summary>
        private Game model;

        /// <summary>
        /// Stores the 3D visuals for the chess pieces.
        /// </summary>
        private ModelVisual3D[] visualPieces;

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

                    isDragging = false;

                    // if the control is loaded, draw the board
                    if (IsLoaded)
                    {
                        DrawBoard();
                    }
                }
            }
        }

        public GameBoard3D()
        {
            InitializeComponent();

            visualPieces = new ModelVisual3D[Board.SquareNo];

            mousePressedHitTestResultCallback = new HitTestResultCallback(ProcessMousePressedHitTestResult);
            hitTestExcludeSelectionFilter = new HitTestFilterCallback(ExcludeSelectionFilter);

            // create the animatable materials
            Resources["AnimatableWhiteMaterial"] = (FindResource("WhiteMaterial") as DiffuseMaterial).Clone();
            Resources["AnimatableBlackMaterial"] = (FindResource("BlackMaterial") as DiffuseMaterial).Clone();
            RegisterName("AnimatableWhiteBrush", (FindResource("AnimatableWhiteMaterial") as DiffuseMaterial).Brush);
            RegisterName("AnimatableBlackBrush", (FindResource("AnimatableBlackMaterial") as DiffuseMaterial).Brush);

            // set the camera settings
            camera.Transform = new MatrixTransform3D(Settings.Default.CameraMatrix3D);

            // set the viewable group settings
            viewableGroup.Transform = new RotateTransform3D(new AxisAngleRotation3D(Settings.Default.ViewableGroupRotation3D.Axis, Settings.Default.ViewableGroupRotation3D.Angle));

            // draw the board
            DrawBoard();
        }

        private void ControlLoaded(object sender, EventArgs e)
        {
            // set the focus on this control to receive keyboard events
            Focus();
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

            try
            {
                // save the camera and viewable group settings
                Settings.Default.CameraMatrix3D = (camera.Transform as MatrixTransform3D).Matrix;
                Settings.Default.ViewableGroupRotation3D = new Quaternion(((viewableGroup.Transform as RotateTransform3D).Rotation as AxisAngleRotation3D).Axis, ((viewableGroup.Transform as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle);

                Settings.Default.Save();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Draws the whole board.
        /// </summary>
        private void DrawBoard()
        {
            // no selections
            (pieceSelection.Content as GeometryModel3D).Material = null;
            (squareSelection.Content as GeometryModel3D).Material = null;
            piecePos = null;

            if (model != null && model.IsInitialized)
            {
                // loop through the squares and draw all the pieces
                for (int sqIndex = 0; sqIndex < Board.SquareNo; sqIndex++)
                {
                    // modify the current visual piece only if the corresponding piece from the model has changed
                    if ((visualPieces[sqIndex] != null || model.CurrentBoard[sqIndex] != null) &&
                    (visualPieces[sqIndex] == null || model.CurrentBoard[sqIndex] == null || !visualPieces[sqIndex].GetValue(name).ToString().StartsWith(model.CurrentBoard[sqIndex].GetType().Name)))
                    {
                        // remove the old visual representation
                        if (visualPieces[sqIndex] != null)
                        {
                            viewableGroup.Children.Remove(visualPieces[sqIndex]);
                            visualPieces[sqIndex] = null;
                        }

                        // add the new visual piece and set the visual piece position
                        if (model.CurrentBoard[sqIndex] != null)
                        {
                            visualPieces[sqIndex] = GeneratePieceVisual(model.CurrentBoard[sqIndex].GetType());
                            SetVisualPosition(visualPieces[sqIndex], sqIndex);
                            visualPieces[sqIndex].SetValue(position, (int?)sqIndex);
                        }
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
                        viewableGroup.Children.Remove(visualPieces[sqIndex]);
                        visualPieces[sqIndex] = null;
                    }
                }
            }
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            // use the arrows to rotate the camera around the origin
            // use +/- to change the camera distance from the origin
            // use ,/. to rotate the camera around itself

            // do not allow any modifications to the game board during a move animation
            if (IsAnimating) { return; }

            // the current and the new distance of the camera from the origin
            double currentCameraDistance, newCameraDistance;
            // the rotation axis of the camera
            Vector3D cameraRotationAxis;
            // the transform of the camera
            Transform3D cameraTransform;

            Vector3D cameraRealUpDirection = camera.Transform.Transform(camera.UpDirection);
            Vector3D cameraRealLookDirection = camera.Transform.Transform(camera.LookDirection);
            Point3D cameraRealPosition = camera.Transform.Transform(camera.Position);

            switch (e.Key)
            {
                case Key.Up:
                    // set the camera rotation axis and transform
                    // the axis is a vector perpendicular to the camera look direction and the camera up direction
                    cameraRotationAxis = Vector3D.CrossProduct(cameraRealUpDirection, cameraRealLookDirection);
                    cameraTransform = new RotateTransform3D(new AxisAngleRotation3D(cameraRotationAxis, Settings.Default.Navigation3DStepAngle));

                    (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;

                    e.Handled = true;
                    break;

                case Key.Down:
                    // set the camera rotation axis and transform
                    // the axis is a vector perpendicular to the camera look direction and the camera up direction
                    cameraRotationAxis = Vector3D.CrossProduct(cameraRealUpDirection, cameraRealLookDirection);
                    cameraTransform = new RotateTransform3D(new AxisAngleRotation3D(cameraRotationAxis, -Settings.Default.Navigation3DStepAngle));

                    (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;

                    e.Handled = true;
                    break;

                case Key.Left:
                    // set the camera rotation axis and transform
                    // the axis is the camera up direction
                    cameraRotationAxis = cameraRealUpDirection;
                    cameraTransform = new RotateTransform3D(new AxisAngleRotation3D(cameraRotationAxis, -Settings.Default.Navigation3DStepAngle));

                    (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;

                    e.Handled = true;
                    break;

                case Key.Right:
                    // set the camera rotation axis and transform
                    // the axis is the camera up direction
                    cameraRotationAxis = cameraRealUpDirection;
                    cameraTransform = new RotateTransform3D(new AxisAngleRotation3D(cameraRotationAxis, Settings.Default.Navigation3DStepAngle));

                    (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;

                    e.Handled = true;
                    break;

                case Key.OemPlus:
                case Key.Add:
                    // set the current and the new camera distance from the origin
                    currentCameraDistance = ((Vector3D)cameraRealPosition).Length;
                    newCameraDistance = currentCameraDistance - Settings.Default.Navigation3DStepDistance;

                    // do not go too close to the models
                    if (newCameraDistance > minDistance)
                    {
                        double scale = newCameraDistance / currentCameraDistance;
                        cameraTransform = new ScaleTransform3D(new Vector3D(scale, scale, scale));

                        (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;
                    }

                    e.Handled = true;
                    break;

                case Key.OemMinus:
                case Key.Subtract:
                    // set the current and the new camera distance from the origin
                    currentCameraDistance = ((Vector3D)cameraRealPosition).Length;
                    newCameraDistance = currentCameraDistance + Settings.Default.Navigation3DStepDistance;

                    // do not go too far from the models
                    if (newCameraDistance < maxDistance)
                    {
                        double scale = newCameraDistance / currentCameraDistance;
                        cameraTransform = new ScaleTransform3D(new Vector3D(scale, scale, scale));

                        (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;
                    }

                    e.Handled = true;
                    break;

                case Key.OemComma:
                    // set the camera rotation axis and transform
                    // the axis is the camera look direction
                    cameraRotationAxis = cameraRealLookDirection;
                    cameraTransform = new RotateTransform3D(new AxisAngleRotation3D(cameraRotationAxis, Settings.Default.Navigation3DStepAngle));

                    (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;

                    e.Handled = true;
                    break;

                case Key.OemPeriod:
                    // set the camera rotation axis and transform
                    // the axis is the camera look direction
                    cameraRotationAxis = cameraRealLookDirection;
                    cameraTransform = new RotateTransform3D(new AxisAngleRotation3D(cameraRotationAxis, -Settings.Default.Navigation3DStepAngle));

                    (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;

                    e.Handled = true;
                    break;
            }
        }

        private void MouseWheelRotated(object sender, MouseWheelEventArgs e)
        {
            // use mouse wheel to change the camera distance from the origin

            // do not allow any modifications to the game board during a move animation
            if (IsAnimating) { return; }

            // set the current and the new camera distance from the origin
            double currentCameraDistance = ((Vector3D)camera.Transform.Transform(camera.Position)).Length;
            double newCameraDistance = currentCameraDistance - e.Delta * Settings.Default.Navigation3DStepDistance / 120;

            // do not go too close to or too far from the models
            if (newCameraDistance < maxDistance && newCameraDistance > minDistance)
            {
                double scale = newCameraDistance / currentCameraDistance;
                Transform3D cameraTransform = new ScaleTransform3D(new Vector3D(scale, scale, scale));

                (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;
            }

            e.Handled = true;
        }

        private void MouseLeftButtonPressed(object Sender, MouseButtonEventArgs e)
        {
            // do not allow any modifications to the game board during a move animation
            if (IsAnimating) { return; }

            // process hit test results
            PointHitTestParameters hitParameters = new PointHitTestParameters(e.GetPosition(viewport));
            viewableHitResult = null;
            VisualTreeHelper.HitTest(viewport, hitTestExcludeSelectionFilter, mousePressedHitTestResultCallback, hitParameters);

            isDragging = true;

            // stores the initial rotation
            AxisAngleRotation3D viewableRotation = (viewableGroup.Transform as RotateTransform3D).Rotation as AxisAngleRotation3D;
            initialQuaternion = new Quaternion(viewableRotation.Axis, viewableRotation.Angle);
            initialSphereHitVector = ProjectToTrackball(e.GetPosition(this));

            if (viewableHitResult != null)
            {
                if (viewableHitResult.VisualHit == board)
                {
                    // board is hit
                    Select(((int)Math.Floor(viewableHitResult.PointHit.Z / SquareSize) + (Board.SideSquareNo / 2)) * Board.SideSquareNo + ((int)Math.Floor(viewableHitResult.PointHit.X / SquareSize) + (Board.SideSquareNo / 2)));
                }
                else if (viewableHitResult.VisualHit == boardExtra)
                {
                }
                else
                {
                    // a piece has been hit
                    Select(((int?)viewableHitResult.VisualHit.GetValue(position)).Value);
                }
            }

            // set the focus on this control to receive keyboard events
            Focus();
        }

        private void MouseMoved(object Sender, MouseEventArgs e)
        {
            // check if the scene is in dragging mode
            // do not allow any modifications to the game board during a move animation
            if (!isDragging || IsAnimating) { return; }

            currentSphereHitVector = ProjectToTrackball(e.GetPosition(this));

            // computes the rotation from the initial hit vector and the current one
            Vector3D rotationAxis = Vector3D.CrossProduct(initialSphereHitVector, currentSphereHitVector);
            double rotationAngle = Settings.Default.Navigation3DAngleAccelerationFactor * Vector3D.AngleBetween(initialSphereHitVector, currentSphereHitVector);

            // rotate the viewable group only if the rotation is not negligeable
            if (rotationAngle > 1)
            {
                Quaternion currentQuaternion = new Quaternion(rotationAxis, rotationAngle) * initialQuaternion;

                ((viewableGroup.Transform as RotateTransform3D).Rotation as AxisAngleRotation3D).Axis = currentQuaternion.Axis;
                ((viewableGroup.Transform as RotateTransform3D).Rotation as AxisAngleRotation3D).Angle = currentQuaternion.Angle;
            }
        }

        private void MouseLeftButtonReleased(object Sender, MouseButtonEventArgs e)
        {
            MouseMoved(Sender, e);

            isDragging = false;
        }

        private void MouseLeftControl(object Sender, MouseEventArgs e)
        {
            // do not allow any modifications to the game board during a move animation
            if (IsAnimating) { return; }

            isDragging = false;
        }

        /// <summary>
        /// Stores the first viewable hit test result and the dragging sphere hit.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private HitTestResultBehavior ProcessMousePressedHitTestResult(HitTestResult result)
        {
            viewableHitResult = result as RayHitTestResult;

            return HitTestResultBehavior.Stop;
        }

        /// <summary>
        /// Exclude selections from the hit results.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private HitTestFilterBehavior ExcludeSelectionFilter(DependencyObject target)
        {
            return (target != pieceSelection && target != squareSelection) ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelf;
        }

        private Vector3D ProjectToTrackball(Point point)
        {
            double x = point.X / (ActualWidth / 2);
            double y = point.Y / (ActualHeight / 2);

            x = x - 1;
            y = 1 - y;

            double z2 = 1 - x * x - y * y;
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return camera.Transform.Transform(new Vector3D(x, y, z));
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
                    // exit dragging mode
                    isDragging = false;

                    // do not animate the move, make the move at once
                    showMoveAnimation = false;

                    model.Make(move);
                }

                return;
            }
        }

        /// <summary>
        /// Rotates the camera.
        /// </summary>
        public void Rotate()
        {
            // do not allow any modifications to the game board during a move animation
            if (IsAnimating) { return; }

            // rotates the camera by 180 degrees around the board

            // set the camera rotation axis and transform
            // the axis is the board normal
            Vector3D rotationAxis = viewableGroup.Transform.Transform(new Vector3D(0, 1, 0));
            RotateTransform3D cameraTransform = new RotateTransform3D(new AxisAngleRotation3D(rotationAxis, 180));

            (camera.Transform as MatrixTransform3D).Matrix *= cameraTransform.Value;
        }

        private void model_Modified(object sender, EventArgs e)
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
                (pieceSelection.Content as GeometryModel3D).Material = null;
                (squareSelection.Content as GeometryModel3D).Material = null;
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
        /// Draws a piece selection.
        /// </summary>
        /// <param name="position">The selected position</param>
        private void DrawSelection(int position)
        {
            // hide the square selection
            (squareSelection.Content as GeometryModel3D).Material = null;

            // show and position the piece selection
            (pieceSelection.Content as GeometryModel3D).Material = FindResource("SelectionMaterial") as Material;
            SetVisualPosition(pieceSelection, position);
            piecePos = position;
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
                SetVisualPosition(visualPieces[rookMove.From], rookMove.From);
                visualPieces[rookMove.From].SetValue(position, (int?)rookMove.From);

                // empty the ending square
                visualPieces[rookMove.To] = null;
            }

            // show the piece selection on the starting square
            (pieceSelection.Content as GeometryModel3D).Material = FindResource("SelectionMaterial") as Material;
            SetVisualPosition(pieceSelection, move.From);
            piecePos = move.From;

            if (move is PromotionMove)
            {
                // remove the promotion piece
                viewableGroup.Children.Remove(visualPieces[move.To]);
                visualPieces[move.To] = null;

                // add the promoted pawn
                visualPieces[move.From] = GeneratePieceVisual(model.CurrentBoard[move.From].GetType());
                SetVisualPosition(visualPieces[move.From], move.From);
                visualPieces[move.From].SetValue(position, (int?)move.From);
            }
            else
            {
                // move the visual piece from the ending square to the start square
                visualPieces[move.From] = visualPieces[move.To];
                SetVisualPosition(visualPieces[move.From], move.From);
                visualPieces[move.From].SetValue(position, (int?)move.From);

                // empty the ending square
                visualPieces[move.To] = null;
            }

            // show the square selection on the ending square
            (squareSelection.Content as GeometryModel3D).Material = FindResource("SelectionMaterial") as Material;
            SetVisualPosition(squareSelection, move.To);

            if (move.HasCapture)
            {
                // put back the capture
                int captureTarget = move is EnPassantCaptureMove ? (move as EnPassantCaptureMove).Target : move.To;
                visualPieces[captureTarget] = GeneratePieceVisual(model.CurrentBoard[captureTarget].GetType());
                SetVisualPosition(visualPieces[captureTarget], captureTarget);
                visualPieces[captureTarget].SetValue(position, (int?)captureTarget);
            }
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
                viewableGroup.Children.Remove(visualPieces[captureTarget]);
                visualPieces[captureTarget] = null;
            }

            if (move is CastlingMove)
            {
                Move rookMove = (move as CastlingMove).RookMove;

                // move the rook
                visualPieces[rookMove.To] = visualPieces[rookMove.From];
                SetVisualPosition(visualPieces[rookMove.To], rookMove.To);
                visualPieces[rookMove.To].SetValue(position, (int?)rookMove.To);

                // empty the starting square
                visualPieces[rookMove.From] = null;
            }

            // show the piece selection on the ending square
            (pieceSelection.Content as GeometryModel3D).Material = FindResource("SelectionMaterial") as Material;
            SetVisualPosition(pieceSelection, move.To);
            piecePos = move.To;

            // move the visual piece from the starting square to the ending square
            visualPieces[move.To] = visualPieces[move.From];
            SetVisualPosition(visualPieces[move.To], move.To);
            visualPieces[move.To].SetValue(position, (int?)move.To);

            // empty the starting square
            visualPieces[move.From] = null;

            // show the square selection on the starting square
            (squareSelection.Content as GeometryModel3D).Material = FindResource("SelectionMaterial") as Material;
            SetVisualPosition(squareSelection, move.From);

            if (move is PromotionMove)
            {
                // remove the promoted pawn
                viewableGroup.Children.Remove(visualPieces[move.To]);
                visualPieces[move.To] = null;

                // replace it with the promotion piece
                visualPieces[move.To] = GeneratePieceVisual(model.CurrentBoard[move.To].GetType());
                SetVisualPosition(visualPieces[move.To], move.To);
                visualPieces[move.To].SetValue(position, (int?)move.To);
            }
        }

        /// <summary>
        /// Animates a move.
        /// </summary>
        /// <param name="move"></param>
        private void Animate(Move move)
        {
            // set a reference to the animated move
            animatedMove = move;

            // set the piece translation duration according to the animation parameters
            int translationDuration = Settings.Default.AnimationDurationBaseFactor + (int)(Settings.Default.AnimationDurationScaleFactor * (
                (Board.Rank(move.From) - Board.Rank(move.To)) * (Board.Rank(move.From) - Board.Rank(move.To)) +
                (Board.File(move.From) - Board.File(move.To)) * (Board.File(move.From) - Board.File(move.To))));

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
                // draw the move
                DrawMoved(animatedMove);

                animatedMove = null;
            };

            // the storyboard will not be modified so it will be freezed
            moveStoryboard.Freeze();

            // start the animation
            moveStoryboard.Begin(this, HandoffBehavior.Compose);
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
            // the animations and timelines will not be modified so they will be freezed

            // change the material to the animatable one
            ModelVisual3D capturedPieceVisual = visualPieces[move is EnPassantCaptureMove ? (move as EnPassantCaptureMove).Target : move.To];
            (capturedPieceVisual.Content as GeometryModel3D).Material = FindResource("Animatable" + capturedPieceVisual.GetValue(name).ToString().Substring(0, 5) + "Material") as Material;

            // set the opacity animation
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            opacityAnimation.BeginTime = TimeSpan.FromMilliseconds(Settings.Default.AnimationStartDelay);
            Storyboard.SetTargetName(opacityAnimation, "Animatable" + capturedPieceVisual.GetValue(name).ToString().Substring(0, 5) + "Brush");
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Brush.OpacityProperty));
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
            // the animations and timelines will not be modified so they will be freezed

            // set the X translation animation
            DoubleAnimation xAnimation = new DoubleAnimation();
            xAnimation.To = SquareSize * (Board.File(move.To) - (Board.SideSquareNo / 2) + 0.5);
            xAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            xAnimation.BeginTime = TimeSpan.FromMilliseconds(Settings.Default.AnimationStartDelay);
            Storyboard.SetTargetName(xAnimation, visualPieces[move.From].GetValue(name).ToString() + "Transform");
            Storyboard.SetTargetProperty(xAnimation, new PropertyPath(TranslateTransform3D.OffsetXProperty));
            xAnimation.Freeze();

            // set the Z translation animation
            DoubleAnimation zAnimation = new DoubleAnimation();
            zAnimation.To = SquareSize * (Board.Rank(move.To) - (Board.SideSquareNo / 2) + 0.5);
            zAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            zAnimation.BeginTime = TimeSpan.FromMilliseconds(Settings.Default.AnimationStartDelay);
            Storyboard.SetTargetName(zAnimation, visualPieces[move.From].GetValue(name).ToString() + "Transform");
            Storyboard.SetTargetProperty(zAnimation, new PropertyPath(TranslateTransform3D.OffsetZProperty));
            zAnimation.Freeze();

            // add the animations to the move storyboard
            moveStoryboard.Children.Add(xAnimation);
            moveStoryboard.Children.Add(zAnimation);
        }

        /// <summary>
        /// Sets the visual position to this square on the board.
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="position"></param>
        private void SetVisualPosition(ModelVisual3D visual, int position)
        {
            (visual.Transform as TranslateTransform3D).OffsetX = SquareSize * (Board.File(position) - (Board.SideSquareNo / 2) + 0.5);
            (visual.Transform as TranslateTransform3D).OffsetZ = SquareSize * (Board.Rank(position) - (Board.SideSquareNo / 2) + 0.5);
        }

        /// <summary>
        /// Generates the piece 3D visual according to the piece type.
        /// </summary>
        /// <param name="pieceType"></param>
        /// <returns></returns>
        private ModelVisual3D GeneratePieceVisual(Type pieceType)
        {
            string pieceName = pieceType.Name.Substring(5);
            string colorName = pieceType.Name.Substring(0, 5);

            // get the piece geometry
            MeshGeometry3D pieceGeometry = FindResource(pieceName + "Geometry") as MeshGeometry3D;

            // set the geometry and the material
            GeometryModel3D pieceModel = new GeometryModel3D(pieceGeometry, FindResource(colorName + "Material") as Material);

            // if the piece is black, rotate the model by 180 degrees
            if (colorName == "Black")
            {
                pieceModel.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 180));
            }

            ModelVisual3D pieceVisual = new ModelVisual3D();
            pieceVisual.Content = pieceModel;
            pieceVisual.SetValue(name, pieceType.Name + random.Next(100000000, 1000000000).ToString(CultureInfo.InvariantCulture));

            pieceVisual.Transform = new TranslateTransform3D();

            // add it to the viewable group and register its tranform
            viewableGroup.Children.Add(pieceVisual);
            RegisterName(pieceVisual.GetValue(name).ToString() + "Transform", pieceVisual.Transform);

            return pieceVisual;
        }
    }
}