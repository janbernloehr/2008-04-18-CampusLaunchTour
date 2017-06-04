using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.BasicQueries {
    /// <summary>
    /// Demo Code für das ausführen einer einfachen LinqToObjects query.
    /// </summary>
    public class Queries {

        /// <summary>
        /// Führt den Demo Code aus.
        /// </summary>
        public void Run() {
            // daten string erstellen
            var text = "Arma virumque cano Troiae qui primus ab oris ...";

            // string in worte teilen
            var words = text.SplitIntoWords();


            // Jetzt gehts los :)
            // die Abfrage gibt vom String-Array words
            // genau die Strings zurück, die länger sind
            // als 5 zeichen, sortiert diese absteigend nach
            // der Länge und führt eine Projektion durch,
            // bei der die ausgewählten Strings 
            // als upper-case zurückgegeben werden.
            var selection = from word in words
                            where word.Length > 5
                            orderby word.Length
                            select word.ToUpper();


            // Ergebnis iterieren und auf der Console ausgeben.
            foreach (var word in selection) {
                Console.WriteLine(word);
            }
        }
    }
}
