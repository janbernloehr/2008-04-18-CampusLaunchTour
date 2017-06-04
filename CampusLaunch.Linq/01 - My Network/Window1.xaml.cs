using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CampusLaunch.Linq.MyNetwork {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window {
        public Window1() {
            InitializeComponent();
        }
    }

    /// <summary>
    /// Converter zum Umwandeln von Long Geschwindigkeits Werten in 
    /// String.
    /// 100 --> 100 Mb/s
    /// 1000 --> 1.000 Mb/s
    /// </summary>
    public class SpeedConverter : IValueConverter {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            // Konvertierung vom Objekt zum Wpf Element
            long speed = (long) value;

            return string.Format("{0:#,##0} Mb/s", speed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            // Rückkonvertierung vom Wpf Element zum Objekt
            // Hier: Nicht implementiert.

            throw new NotImplementedException();
        }

        #endregion
    }
}
