using System;
using System.Configuration;
using System.ComponentModel;
using System.Reflection;
using Valil.Chess.Model;
using Valil.Chess.Opponents.Properties;

namespace Valil.Chess.Opponents
{
    /// <summary>
    /// Stores information about the game opponents.
    /// </summary>
    public class OpponentsInfo : Component, INotifyPropertyChanged
    {
        /// <summary>
        /// The model.
        /// </summary>
        private Game model;

        /// <summary>
        /// The opponents.
        /// </summary>
        private IOpponents opponents;

        /// <summary>
        /// The opponents info text.
        /// </summary>
        private string opponentsInfoText;

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

                    opponents.Model = model;
                }
            }
        }

        /// <summary>
        /// The opponents info text.
        /// </summary>
        /// <value></value>
        public string OpponentsInfoText
        {
            get { return opponentsInfoText; }
            private set
            {
                if (opponentsInfoText != value)
                {
                    opponentsInfoText = value;
                    OnPropertyChanged("OpponentsInfoText");
                }
            }
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises property changed event.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpponentsInfo()
        {
            // set the opponents
            if (OpponentsSettings.Default.TwoPlayersMode)
            {
                opponents = new NoAIOpponent();
            }
            else if (OpponentsSettings.Default.WebServiceOpponent)
            {
                opponents = new HumanVsWebService();
            }

            // set the opponents info text using settings values
            SetOpponentsInfoText();

            // listen for events
            OpponentsSettings.Default.PropertyChanged += property_Changed;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="container">The container which will contain this component.</param>
        public OpponentsInfo(IContainer container)
            : this()
        {
            container.Add(this);
        }

        private void property_Changed(object sender, PropertyChangedEventArgs e)
        {
            // if the opponents type property has changed
            if (e.PropertyName == "TwoPlayersMode" && OpponentsSettings.Default.TwoPlayersMode && !(opponents is NoAIOpponent))
            {
                // dispose the old opponents
                opponents.Dispose();

                // set the opponents
                opponents = new NoAIOpponent();

                // set the model
                opponents.Model = model;

                // set the opponents info text using settings values
                SetOpponentsInfoText();
            }
            else if (e.PropertyName == "WebServiceOpponent" && OpponentsSettings.Default.WebServiceOpponent && !(opponents is HumanVsWebService))
            {
                // dispose the old opponents
                opponents.Dispose();

                // set the opponents
                opponents = new HumanVsWebService();

                // set the model
                opponents.Model = model;

                // set the opponents info text using settings values
                SetOpponentsInfoText();
            }
            else if (e.PropertyName == "AIOpponentPlaysWhite")
            {
                // set the opponents info text using settings values
                SetOpponentsInfoText();
            }
        }

        /// <summary>
        /// Sets the opponents info text.
        /// </summary>
        private void SetOpponentsInfoText()
        {
            if (OpponentsSettings.Default.TwoPlayersMode)
            {
                OpponentsInfoText = Resources.TwoPlayerModeText;
            }
            else if (OpponentsSettings.Default.WebServiceOpponent)
            {
                OpponentsInfoText = OpponentsSettings.Default.AIOpponentPlaysWhite ?
                    Resources.WSVsYouText :
                    Resources.YouVsWSText;
            }
        }

        /// <summary>
        /// Force the opponent engine to move, if the game is not in two-players mode.
        /// </summary>
        public void ForceAIMove()
        {
            if (opponents is HumanVsAI) { (opponents as HumanVsAI).ForceAIMove(); }
        }

        /// <summary>
        /// True if it's engine turn, false otherwise.
        /// </summary>
        public bool AITurn
        {
            get { return opponents is HumanVsAI && (opponents as HumanVsAI).AITurn; }
        }

        /// <summary>
        /// Release the resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (opponents != null) { opponents.Dispose(); }

            base.Dispose(disposing);
        }
    }

}
