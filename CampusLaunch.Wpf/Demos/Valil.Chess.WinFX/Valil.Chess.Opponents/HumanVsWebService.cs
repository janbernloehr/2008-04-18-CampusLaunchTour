using System;
using System.ComponentModel;
using Valil.Chess.Model;
using Valil.Chess.Opponents.Properties;
using Valil.Chess.Opponents.ChessEngineWebService;

namespace Valil.Chess.Opponents
{
    /// <summary>
    /// A human player playing against the chess web service.
    /// </summary>
    class HumanVsWebService : HumanVsAI
    {
        /// <summary>
        /// The Web Service.
        /// </summary>
        private Service engineService;

        public HumanVsWebService()
        {
            // initialize the web service
            engineService = new Service();
        }

        /// <summary>
        /// Abort engine thinking.
        /// </summary>
        protected override void AbortAIMove()
        {
            // if the engine is thinking, abort the engine move
            engineWorker.CancelAsync();
        }

        /// <summary>
        /// Starts engine thinking.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void engineWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            EngineInputParameters inputParams = e.Argument as EngineInputParameters;

            // call the web service engine synchronously
            string canMove = engineService.GetNextMove(inputParams.FEN, inputParams.RepetitiveMoveCandidate, inputParams.DepthLevel);
            e.Result = Utils.GetCANMove(model, canMove);
        }

        /// <summary>
        /// Dispose the engine resources.
        /// </summary>
        public override void Dispose()
        {
            // abort engine thinking
            AbortAIMove();

            //dispose the service and the worker
            if (engineService != null) { engineService.Dispose(); }
            if (engineWorker != null) { engineWorker.Dispose(); }

            base.Dispose();
        }
    }
}
