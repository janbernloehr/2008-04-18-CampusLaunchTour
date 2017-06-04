using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.LinqToSql {
    /// <summary>
    /// Demo Code für Linq To Sql Datenbank Updates.
    /// </summary>
    public class Update {
        public void Run() {
            // Der DataContext überwacht auch Änderungen an Objekten!
            // Somit ist es möglich ohne zusätzlichen Aufwand 
            // das Change Set gegen die Datenbank zu schicken.
            using (var dbx = new KnowledgeBaseDataContextDataContext()) {
                // Logging aktivieren
                dbx.Log = Console.Out;

                // Artikel mit Id 218 holen.
                var article = (from x in dbx.Articles
                               where x.Id == 218
                               select x).Single();

                // Titel und DateEdit ändern.
                article.Title = "Hallo Karlsruhe!";
                article.DateEdit = DateTime.Now;

                // Änderungen speichern.
                // Hinweis: Bei mehrmaligem Ausführen hat das Setzten des Titels kein Effekt mehr,
                // da dieser bereits "Hallo Karlsruhe!" ist. Der DataContext merkt das und sendet
                // nur 'wahre' Änderungen an die Datenbank.
                dbx.SubmitChanges();
            }

            // Nochmal den Artikel aus der Datenbank holen und die geänderten Felder anzeigen.
            using (var dbx = new KnowledgeBaseDataContextDataContext()) {
                var article = (from x in dbx.Articles
                               where x.Id == 218
                               select x).Single();

                Console.WriteLine("Title: {0}\nChanged: {1}", article.Title, article.DateEdit);
            }
        }
    }
}
