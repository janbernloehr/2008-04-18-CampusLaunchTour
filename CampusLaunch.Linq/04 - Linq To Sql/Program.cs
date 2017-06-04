using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.LinqToSql {
    class Program {
        static void Main(string[] args) {

            // *** Einfache Datenbank Abfrage ***
            var code = new Select();
            code.Run();

            // *** Zusammengesetzte Datenbank Abfrage ***
            // var code = new JoinSelect();
            // code.Run();

            // *** Datenbank Update ***
            // var code = new Update();
            // code.Run();

            Console.ReadLine();
        }
    }
}
