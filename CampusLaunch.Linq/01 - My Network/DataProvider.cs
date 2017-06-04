using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Text;

namespace CampusLaunch.Linq.MyNetwork {

    /// <summary>
    /// DataProvider stellt Informationen über die Netzwerkverbindung und die Netzwerkgeräte
    /// des Computers zur Verfügung.
    /// </summary>
    public class DataProvider {

        /// <summary>
        /// Gibt den Status der Netzwerkverbindung zurück (Online|Offline).
        /// </summary>
        public string Status {
            get {
                if (NetworkInterface.GetIsNetworkAvailable()) {
                    return "Online";
                }
                else {
                    return "Offline";
                }
            }
        }

        /// <summary>
        /// Gibt ein Array eines Anonymen - zur Laufzeit generierten - Typs zurück.
        /// Dieser Typ enthält Informationen über die Netzwerkverbindung.
        /// </summary>
        /// <returns></returns>
        public object[] GetNetworkInterfaces() {
            // basis Abfrage, holt alle Netzwerk Geräte, die IPv4 unterstützen.
            var basic = from ni in NetworkInterface.GetAllNetworkInterfaces()
                        where ni.Supports(NetworkInterfaceComponent.IPv4)
                        select ni;

            // erweiterte Abfrage, sortiert, projeziert und gruppiert diese Netzwerkgeräte
            var query = from ni in basic

                        let speed = ni.Speed / (1024 * 1024)

                        orderby speed descending             // Sortierung nach Geschwindigkeit

                        group new                            // Gruppierung
                        {
                            // Eine Gruppe ist ein Objekt
                            // mit einem Key (Schlüssel), nach dem gruppiert wird
                            // und den Items (Elemente der Gruppe).
                            // Die Items werden hier projeziert,
                            // sie haben die Eigenschaften Name,
                            // Speed, Type und Description.

                            ni.Name,
                            Speed = speed,
                            Type = ni.NetworkInterfaceType,
                            ni.Description
                        }

                            // gruppiert wird nach dem Schlüssel OperationalStatus
                            // alle Gruppen werden dann in die Variable groupedNetworks abgelegt
                        by ni.OperationalStatus into groupedNetworks

                        select new                           // Projektion
                        {
                            // Die Gruppen werden noch einmal projeziert
                            // Aus Key wird Status und die Default Eigenschaft
                            // wird nach Values projeziert.
                            Status = groupedNetworks.Key,
                            Values = groupedNetworks
                        };

            // Das Ergebnis in ein Array umwandeln und zurückgeben.
            return query.ToArray();
        }
    }
}
