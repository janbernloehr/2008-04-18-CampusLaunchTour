using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.BasicQueries {

    /// <summary>
    /// Demo Code für die neuen Sprach Features von C# 3.0.
    /// </summary>
    public class LanuageFeatures {

        /// <summary>
        /// Führt den Demo Code aus.
        /// </summary>
        public void Run() {
            // Sprachfeature: Type inference
            // Type inference bedeutet, dass der Compiler bei einer lokalen
            // Variable automatisch erkennt um welchen Typ es sich handelt.
            // Zur Compiletime nicht zur Runtime!

            // früher:
            // string text = "Arma virumque cano Troiae qui primus ab oris ...";

            // neu:
            var text = "Arma virumque cano Troiae qui primus ab oris ...";


            // Sprachfeature: Extension Methods
            // Bestehende Typen können um Funktionen erweitert werden.
            // Diese Funktionen sind in statische Klassen ausgelagert
            // und erfordern als ersten Parameter eine Variable von dem Typ
            // der erweitert werden soll.

            // früher:
            // string[] words = StringExtensions.SplitIntoWords(text);

            // neu:
            var words = text.SplitIntoWords();


            // Sprachfeature: Lambda Expressions
            // Inline Funktionen

            // früher:
            // string[] lowerWords = words.Select(ApplyTransform);

            // neu:
            var lowerWords = words.Select(o => o.ToLower());


            // Ergebnis ausgeben
            foreach (var word in lowerWords) {
                Console.WriteLine(word);
            }
        }

        private string ApplyTransform(string value) {
            return value.ToLower();
        }
    }
}
