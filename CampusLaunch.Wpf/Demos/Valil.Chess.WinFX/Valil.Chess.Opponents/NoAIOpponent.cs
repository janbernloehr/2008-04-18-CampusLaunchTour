using System;
using Valil.Chess.Model;

namespace Valil.Chess.Opponents
{
    /// <summary>
    /// Two-players mode.
    /// </summary>
    class NoAIOpponent : IOpponents
    {
        /// <summary>
        /// The model.
        /// </summary>
        protected Game model;

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value></value>
        public Game Model
        {
            get { return model; }
            set
            {
                // the model can be set only once
                if (model == null && value != null)
                {
                    model = value;
                }
            }
        }

        /// <summary>
        /// Dispose the engine resources.
        /// </summary>
        public void Dispose() { }
    }
}
