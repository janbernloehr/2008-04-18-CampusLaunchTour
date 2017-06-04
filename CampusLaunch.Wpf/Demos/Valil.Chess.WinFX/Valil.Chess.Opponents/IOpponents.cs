using System;
using Valil.Chess.Model;

namespace Valil.Chess.Opponents
{
    /// <summary>
    /// The type of opponents in a game.
    /// </summary>
    interface IOpponents : IDisposable
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value></value>
        Game Model { get; set; }
    }

    /// <summary>
    /// The opponents type.
    /// </summary>
    public enum OpponentsType { HumanVsWebService, TwoPlayerMode };
}
