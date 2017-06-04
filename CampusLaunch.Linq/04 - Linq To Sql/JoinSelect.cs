using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.LinqToSql {

    /// <summary>
    ///  Demo Code für ein Join Select mit Linq To Sql.
    /// </summary>
    public class JoinSelect {

        public void Run() {
            // Es sollen nur Artikel aus folgender Kategorie abgerufen werden:
            var categoryTitle = "Visual Basic.NET";

            // Wie immer muss zunächst der DataContext instanziert werden.
            using (var dbx = new KnowledgeBaseDataContextDataContext()) {
                // Der DataContext kann sein Logging auf der Console ausgeben.
                dbx.Log = Console.Out;

                // Die Basis Abfrage solle alle Artikel einer Kategorie mit einem 
                // bestimmten Titel zurückgeben.
                var articles = from mapping in dbx.ArticleCategoryMappings
                               where mapping.Category.Title == categoryTitle
                               select mapping.Article;

                // Das Ergebnis soll nach dem Artikel-Titel aufsteigend sortiert werden
                var ordered = from a in articles
                              orderby a.Title ascending
                              select a;

                // Und zuletzt interessieren nur die ersten 4 Elemente
                var firstFour = ordered.Take(4);

                // Erst hier wird die Linq Query in Sql übersetzt und ausgeführt
                // (Siehe Looging auf der Console)
                // Alle drei Query (Basis, Sortierung, Take) werden in ein einziges Sql zusammengeführt.
                foreach (var article in firstFour) {
                    Console.WriteLine(article.Title);
                }
            }
        }

    }
}
