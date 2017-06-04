using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.BasicQueries {

    /// <summary>
    /// Modul für String Erweiterungen.
    /// 
    /// Früher: StringExtensions.SplitIntoWords(text);
    /// Jetzt:  text.SplitIntoWords();
    /// 
    /// Der Compiler erkennt die hier definierte Methode als Extension Method.
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Teilt den Übergebenen String in Wort Blöcke und gibt diese als String Array zurück.
        /// </summary>
        public static string[] SplitIntoWords(this string value) {
            // seperator chars.
            char[] seperators = { ' ' };

            // eigentlicher Code
            return value.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
