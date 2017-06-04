namespace Valil.Chess.WinFX
{
    partial class GameLoadSaveOperationsManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openGameDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveGameDialog = new System.Windows.Forms.SaveFileDialog();
            this.autosaveBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            // 
            // openGameDialog
            // 
            this.openGameDialog.RestoreDirectory = true;
            // 
            // saveGameDialog
            // 
            this.saveGameDialog.RestoreDirectory = true;
            // 
            // autosaveBackgroundWorker
            // 
            this.autosaveBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.autosaveBackgroundWorker_DoWork);
            this.autosaveBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.autosaveBackgroundWorker_RunWorkerCompleted);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openGameDialog;
        private System.Windows.Forms.SaveFileDialog saveGameDialog;
        private System.ComponentModel.BackgroundWorker autosaveBackgroundWorker;
    }
}
