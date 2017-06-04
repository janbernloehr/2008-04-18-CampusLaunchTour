using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Valil.Chess.Model;
using Valil.Chess.WinFX.Properties;

namespace Valil.Chess.WinFX
{
    /// <summary>
    /// Manages the load and save operations of the game form.
    /// </summary>
    public partial class GameLoadSaveOperationsManager : Component
    {
        /// <summary>
        /// The game description proxy.
        /// </summary>
        private GameStringDescriptionProxy proxy;

        /// <summary>
        /// Keeps the PGN string for saving later in case the application is autosaving.
        /// </summary>
        private string autosaveStore;

        /// <summary>
        /// The game description proxy.
        /// </summary>
        public GameStringDescriptionProxy Proxy
        {
            get { return proxy; }
            set
            {
                if (value != null)
                {
                    proxy = value;

                    // set the autosave handler
                    proxy.Save = new AutoSavePGNHandler(AutoSavePGN);
                }
            }
        }

        public GameLoadSaveOperationsManager()
        {
            InitializeComponent();
        }

        public GameLoadSaveOperationsManager(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Loads a game from a PGN file.
        /// </summary>
        /// <param name="filePath">PGN file path</param>
        private void LoadPGN(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // initialize the game
                    proxy.LoadGame(sr);
                }
            }
            catch
            {
                // show the error
                MessageBox.Show(Resources.ErrorOpeningPGNMsg, Resources.OpeningPGNTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }


        /// <summary>
        /// Loads the last played game.
        /// Does nothing if the proxy is null.
        /// </summary>
        internal void LoadLastGame()
        {
            if (proxy == null) { return; }

            // get the isolated store for this user
            IsolatedStorageFile isoStore =
                ApplicationDeployment.IsNetworkDeployed ?
                IsolatedStorageFile.GetUserStoreForApplication() :
                IsolatedStorageFile.GetUserStoreForAssembly();

            // if the file does not exists, set a new game
            if (isoStore.GetFileNames(Settings.Default.AutosaveGameRelPath).Length == 0)
            {
                proxy.LoadNewGame();

                return;
            }

            try
            {
                using (StreamReader sr = new StreamReader(new IsolatedStorageFileStream(Settings.Default.AutosaveGameRelPath, FileMode.Open, FileAccess.Read, FileShare.Read, isoStore)))
                {
                    // initialize the game
                    proxy.LoadGame(sr);
                }
            }
            catch
            {
                // load a new game
                proxy.LoadNewGame();
            }
        }

        /// <summary>
        /// Saves a game in a PGN file.
        /// </summary>
        /// <param name="filePath">PGN file path</param>
        private void SavePGN(string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    proxy.WritePGNTo(sw);
                }
            }
            catch
            {
                MessageBox.Show(Resources.ErrorSavingPGNMsg, Resources.SavingPGNTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Autosave the game in PGN.
        /// </summary>
        private void AutoSavePGN()
        {
            if (autosaveBackgroundWorker.IsBusy)
            {
                // the application is still autosaving
                // save the PGN in the buffer
                using (StringWriter sw = new StringWriter())
                {
                    proxy.WritePGNTo(sw);
                    autosaveStore = sw.ToString();
                }
            }
            else
            {
                // start autosaving
                using (StringWriter sw = new StringWriter())
                {
                    proxy.WritePGNTo(sw);
                    autosaveBackgroundWorker.RunWorkerAsync(sw.ToString());
                }
            }
        }

        /// <summary>
        /// Loads a game from a FEN file.
        /// </summary>
        /// <param name="filePath">The FEN file path</param>
        private void LoadFEN(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // initialize the game
                    proxy.LoadBoardConfiguration(sr.ReadToEnd());
                }
            }
            catch (ArgumentException exc)
            {
                MessageBox.Show(exc.Message, Resources.OpeningFENTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            catch
            {
                MessageBox.Show(Resources.ErrorOpeningFENMsg, Resources.OpeningFENTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// Shows a open dialog and loads the game configuration from the chosen file.
        /// Does nothing if the proxy is null.
        /// </summary>
        internal void LoadGame()
        {
            if (proxy == null) { return; }

            // open a game in FEN or PGN format

            openGameDialog.Title = Resources.OpenGameTitle;
            openGameDialog.InitialDirectory = GamesLocationHandler.GamesFolderPath;
            openGameDialog.Filter = Resources.PGNFilterTxt + "(*.pgn)|*.pgn|" + Resources.FENFilterTxt + "(*.fen)|*.fen";

            if (openGameDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // open PGN, show errors
                if (openGameDialog.FilterIndex == 1)
                {
                    LoadPGN(openGameDialog.FileName);
                }
                // open FEN
                else if (openGameDialog.FilterIndex == 2)
                {
                    LoadFEN(openGameDialog.FileName);
                }
            }
        }

        /// <summary>
        /// Shows a save dialog and saves the game configuration in the chosen file.
        /// Does nothing if the proxy is null.
        /// </summary>
        internal void SaveGame()
        {
            if (proxy == null) { return; }

            // save a game in FEN or PGN format

            saveGameDialog.Title = Resources.SaveGameTitle;
            saveGameDialog.InitialDirectory = GamesLocationHandler.GamesFolderPath;
            saveGameDialog.Filter = Resources.PGNFilterTxt + "(*.pgn)|*.pgn|" + Resources.FENFilterTxt + "(*.fen)|*.fen";

            if (saveGameDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // save PGN, show errors
                if (saveGameDialog.FilterIndex == 1)
                {
                    SavePGN(saveGameDialog.FileName);
                }
                //save FEN
                else if (saveGameDialog.FilterIndex == 2)
                {
                    SaveFEN(saveGameDialog.FileName);
                }
            }

        }

        /// <summary>
        /// Saves the current board in a FEN file.
        /// </summary>
        /// <param name="filePath">The FEN file path</param>
        private void SaveFEN(string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(proxy.GetBoardConfiguration());
                }
            }
            catch
            {
                MessageBox.Show(Resources.ErrorSavingFENMsg, Resources.SavingFENTitle, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void autosaveBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get the isolated store for this user
            IsolatedStorageFile isoStore =
                ApplicationDeployment.IsNetworkDeployed ?
                IsolatedStorageFile.GetUserStoreForApplication() :
                IsolatedStorageFile.GetUserStoreForAssembly();

            try
            {
                // save the PGN
                using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(Settings.Default.AutosaveGameRelPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, isoStore)))
                {
                    sw.Write(e.Argument as string);
                    sw.Flush();
                }
            }
            catch
            {
            }
        }

        private void autosaveBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (autosaveStore != null)
            {
                // start saving what was in the save buffer
                autosaveBackgroundWorker.RunWorkerAsync(autosaveStore);
                autosaveStore = null;
            }
        }
    }
}
