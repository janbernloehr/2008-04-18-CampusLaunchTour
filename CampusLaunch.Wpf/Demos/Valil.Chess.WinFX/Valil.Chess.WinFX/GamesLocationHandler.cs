using System;
using System.Deployment.Application;
using System.IO;
using System.Windows.Forms;
using Valil.Chess.WinFX.Properties;

namespace Valil.Chess.WinFX
{
    /// <summary>
    /// Helper class, handles games location.
    /// </summary>
    internal static class GamesLocationHandler
    {
        /// <summary>
        /// Games folder path.
        /// </summary>
        private static string gamesFolderPath;

        static GamesLocationHandler()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                // the application is deployed using ClickOnce

                // set the games folder in the user application data folder
                gamesFolderPath = Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + Settings.Default.GamesFolderRelPath;
            }
            else
            {
                // the application is not deployed using ClickOnce

                // set the games folder in the application directory
                gamesFolderPath = Application.StartupPath + Path.DirectorySeparatorChar + Settings.Default.GamesFolderRelPath;
            }

            // create the games directory if it does not exist
            if (!Directory.Exists(gamesFolderPath))
            {
                Directory.CreateDirectory(gamesFolderPath);
            }
        }

        /// <summary>
        /// Games folder path.
        /// </summary>
        public static string GamesFolderPath
        {
            get { return gamesFolderPath; }
        }
    }
}
