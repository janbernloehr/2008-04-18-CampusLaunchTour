using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.LinqToSql {
    public class Select {

        public void Run() {
            // Der DataContext steuert die Kommunikation mit der Datenbank.
            // Er übersetzt Linq Abfragen in Sql, führt diese gegen die Datenbank
            // aus und erzeugt aus den zurückgegebenen Daten Objekte.
            using (var dbx = new KnowledgeBaseDataContextDataContext()) {
                // using sollte verwendet werden, da der DataContext
                // nicht-verwaltete Ressourcen erzeugt, wie z.b.
                // SqlConnection Objekte.

                // Auch hier können Abfragen gegen eine Datenbank GENAU SO
                // definiert werden wie Abfragen gegen Objekt-Listen oder
                // Xml.

                // In diesem Beispiel werden alle Artikel aus der Datenbank gelesen,
                // die neuer sind als zwei Monate, die Ergebnismenge wird sortiert
                // und nur die obersten 4 Elemente werden zurückgegeben.
                var articles = (from x in dbx.Articles
                                where x.DateCreated >= DateTime.Now.AddMonths(-2)
                                orderby x.Views descending
                                select x).Skip(3).Take(4);

                System.Diagnostics.Debug.WriteLine(articles);

                // Hinweis: Die Abfrage wird komplett nach Sql übersetzt,
                // man findet also sowohl die Where Einschränkung, als auch die
                // sortierung und das Take wieder!

                // Die Abfragen wird übrigens erst ausgeführt, wenn articles mit foreach durchlaufen wird
                // bzw. .Count aufgerufen wird. Diese 'späte' Ausführung wird durch IQueryable ermöglicht.

                // Daten abrufen und ausgeben.
                foreach (var article in articles) {
                    Console.WriteLine(article.Title);
                }
            }

        }

    }
}
