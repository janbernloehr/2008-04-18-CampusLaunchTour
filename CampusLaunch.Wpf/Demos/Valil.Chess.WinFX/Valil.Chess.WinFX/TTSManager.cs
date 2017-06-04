using System;
using System.Speech.Synthesis;
using Valil.Chess.Model;
using Valil.Chess.Opponents;

namespace Valil.Chess.WinFX
{
    /// <summary>
    /// Manages text-to-speech.
    /// </summary>
    public class TTSManager
    {
        #region The messages

        private static readonly string moveSpeechTemplate = "Move from {0} to {1}.";
        private static readonly string checkSpeech = "Check!";
        private static readonly string checkmateSpeech = "Checkmate!";
        private static readonly string draw50MoveSpeech = "Draw: 50-move rule!";
        private static readonly string drawInsufficientMaterialSpeech = "Draw: insufficient material!";
        private static readonly string drawRepetitionSpeech = "Draw: repetition!";
        private static readonly string stalemateSpeech = "Stalemate!";

        #endregion

        /// <summary>
        /// The speech synthesizer.
        /// </summary>
        private SpeechSynthesizer synth;

        /// <summary>
        /// The model.
        /// </summary>
        private Game model;

        /// <summary>
        /// The opponents info.
        /// </summary>
        private OpponentsInfo oppInfo;

        /// <summary>
        /// True to if the model loads moves, false otherwise.
        /// </summary>
        private bool isModelLoadingMoves;

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

                    synth = new SpeechSynthesizer();

                    // hook events
                    model.Moving += model_Moving;
                    model.Moved += model_Moved;
                    model.GameBoardConfigurationLoaded += model_GameBoardConfigurationLoaded;
                    model.GameMoveSectionLoaded += model_GameMoveSectionLoaded;

                    isModelLoadingMoves = false;
                }
            }
        }

        /// <summary>
        /// The opponents info.
        /// </summary>
        public OpponentsInfo OppInfo
        {
            get { return oppInfo; }
            set { oppInfo = value; }
        }

        private void model_Moving(object sender, CancelMoveEventArgs e)
        {
            // no talking during loading
            if (isModelLoadingMoves) { return; }

            // say the move if it is engine turn
            if (oppInfo != null && oppInfo.AITurn)
            {
                synth.SpeakAsync(String.Format(moveSpeechTemplate, Utils.GetNotation(e.Move.From), Utils.GetNotation(e.Move.To)));
            }
        }

        private void model_Moved(object sender, MoveEventArgs e)
        {
            // no talking during loading
            if (isModelLoadingMoves) { return; }

            // say any change in game status
            switch (model.Status)
            {
                case GameStatus.Check:
                    synth.SpeakAsync(checkSpeech);
                    break;
                case GameStatus.Checkmate:
                    synth.SpeakAsync(checkmateSpeech);
                    break;
                case GameStatus.Draw50Move:
                    synth.SpeakAsync(draw50MoveSpeech);
                    break;
                case GameStatus.DrawInsufficientMaterial:
                    synth.SpeakAsync(drawInsufficientMaterialSpeech);
                    break;
                case GameStatus.DrawRepetition:
                    synth.SpeakAsync(drawRepetitionSpeech);
                    break;
                case GameStatus.Stalemate:
                    synth.SpeakAsync(stalemateSpeech);
                    break;
            }
        }

        private void model_GameBoardConfigurationLoaded(object sender, EventArgs e)
        {
            // stop talking during loading
            isModelLoadingMoves = true;
        }

        private void model_GameMoveSectionLoaded(object sender, EventArgs e)
        {
            // resume talking 
            isModelLoadingMoves = false;
        }

        public void Dispose()
        {
            if (model != null)
            {
                // unhook events
                model.Moving -= model_Moving;
                model.Moved -= model_Moved;
                model.GameBoardConfigurationLoaded -= model_GameBoardConfigurationLoaded;
                model.GameMoveSectionLoaded -= model_GameMoveSectionLoaded;
            }

            synth.Dispose();
        }
    }
}
