using System;
using System.ComponentModel;
using System.Configuration;

namespace Valil.Chess.Opponents
{
    /// <summary>
    /// The opponents settings.
    /// </summary>
    public class OpponentsSettings : ApplicationSettingsBase
    {
        private static OpponentsSettings defaultInstance = new OpponentsSettings();

        /// <summary>
        /// Unique instance.
        /// </summary>
        public static OpponentsSettings Default
        {
            get { return defaultInstance; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool AIOpponentPlaysWhite
        {
            get { return ((bool)(this["AIOpponentPlaysWhite"])); }
            set { this["AIOpponentPlaysWhite"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("3")]
        public int AIOpponentLevel
        {
            get { return ((int)(this["AIOpponentLevel"])); }
            set { this["AIOpponentLevel"] = value; }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool TwoPlayersMode
        {
            get { return ((bool)(this["TwoPlayersMode"])); }
            set
            {
                if (value)
                {
                    if (WebServiceOpponent) { WebServiceOpponent = false; }
                }

                this["TwoPlayersMode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool WebServiceOpponent
        {
            get { return ((bool)(this["WebServiceOpponent"])); }
            set
            {
                if (value)
                {
                    if (TwoPlayersMode) { TwoPlayersMode = false; }
                }

                this["WebServiceOpponent"] = value;
            }
        }

    }
}
