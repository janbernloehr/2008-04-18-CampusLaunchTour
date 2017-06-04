using System;
using System.Windows;
using System.Windows.Navigation;
using System.Configuration;
using System.ComponentModel;
using Valil.Chess.Model;
using Valil.Chess.Opponents;

namespace Valil.Chess.WinFX
{
    public partial class App : Application
    {
        private void AppStartup(object sender, StartupEventArgs args)
        {
            // put all the necessary objects in application properties
            // so they could be available in all windows

            // set the model
            Game model = new Game();
            Properties["Model"] = model;

            // set the opponent info
            OpponentsInfo oppInfo = new OpponentsInfo();
            oppInfo.Model = model;
            Properties["OpponentsInfo"] = oppInfo;

            // set the game description proxy
            GameStringDescriptionProxy desc = new GameStringDescriptionProxy();
            desc.Model = model;
            Properties["GameDescriptionProxy"] = desc;

            // set the autosave manager
            GameLoadSaveOperationsManager gameLoadSaveManager = new GameLoadSaveOperationsManager();
            gameLoadSaveManager.Proxy = desc;
            Properties["GameLoadSaveOperationsManager"] = gameLoadSaveManager;

            // set the speech recognizer manager
            SpeechRecognitionManager speechRecognitionManager = new SpeechRecognitionManager();
            Properties["SpeechRecognitionManager"] = speechRecognitionManager;

            // set the text-to-speech manager
            TTSManager ttsManager = new TTSManager();
            ttsManager.Model = model;
            ttsManager.OppInfo = oppInfo;
            Properties["TTSManager"] = ttsManager;

            
        }

        private void AppLoadCompleted(object sender1, EventArgs args1)
        {
            // set the window closing handler only once
            if (Properties["IsClosingHandlerSetup"] == null)
            {
                MainWindow.Closing += MainWindow_Closing;

                Properties["IsClosingHandlerSetup"] = new object();
            }
            
        }

        private void AppSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // remove window closing handler when the session ends
            MainWindow.Closing -= MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // ask the user if he/she wants to quit the game when he/she closes this window
            if (MessageBox.Show(MainWindow, Valil.Chess.WinFX.Properties.Resources.AskUserQuitMsg, Valil.Chess.WinFX.Properties.Resources.QuitAppTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
        }

        private void AppExit(object sender, EventArgs args)
        {
            (Properties["SpeechRecognitionManager"] as SpeechRecognitionManager).Dispose();
            (Properties["TTSManager"] as TTSManager).Dispose();
            (Properties["GameLoadSaveOperationsManager"] as GameLoadSaveOperationsManager).Dispose();
            (Properties["GameDescriptionProxy"] as GameStringDescriptionProxy).Dispose();
            (Properties["OpponentsInfo"] as OpponentsInfo).Dispose();
            (Properties["Model"] as Game).Dispose();
        }
    }
}