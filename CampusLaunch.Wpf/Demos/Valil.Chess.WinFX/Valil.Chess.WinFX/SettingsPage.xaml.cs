using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Valil.Chess.Opponents;

namespace Valil.Chess.WinFX
{
    public partial class SettingsPage : Page
    {
        /// <summary>
        /// True if the settings are saved, false otherwise.
        /// </summary>
        private bool settingsSaved;

        private void PageLoaded(object sender, EventArgs e)
        {
            // set the data context
            panel.DataContext = OpponentsSettings.Default;

            OpponentsSettings.Default.SettingChanging += setting_Changing;
            OpponentsSettings.Default.SettingsSaving += settings_Saving;

            settingsSaved = true;
        }

        private void PageUnloaded(object sender, EventArgs e)
        {
            // if the settings are not saved reload the last saved ones
            if (!settingsSaved)
            {
                OpponentsSettings.Default.Reload();
            }
        }

        private void ResetClick(object sender, EventArgs e)
        {
            // reset the settings
            OpponentsSettings.Default.Reset();

            settingsSaved = true;
            saveBtn.IsEnabled = false;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            // try to save the settings
            try
            {
                OpponentsSettings.Default.Save();
            }
            catch
            {
                settingsSaved = true;
                saveBtn.IsEnabled = false;
            }
        }

        private void settings_Saving(object sender, CancelEventArgs e)
        {
            settingsSaved = true;
            saveBtn.IsEnabled = false;
        }

        private void setting_Changing(object sender, SettingChangingEventArgs e)
        {
            settingsSaved = false;
            saveBtn.IsEnabled = true;
        }
    }
}