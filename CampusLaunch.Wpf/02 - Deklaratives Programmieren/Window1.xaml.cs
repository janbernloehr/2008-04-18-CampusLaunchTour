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
using System.Xml;
using System.Windows.Markup;

namespace WPF_Demo_1 {
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window {
        public Window1() {
            InitializeComponent();

            ElementByCode();

            ElementFromXamlFile();

            SaveElementTree();
        }

        private void ElementByCode() {   // Element erzeugen und hinzufügen.
            var zeile4 = new ListBoxItem();
            zeile4.Content = "Zeile 4 (programmiert)";
            listbox.Items.Add(zeile4);
        }

        private void ElementFromXamlFile() {   // Element laden und hinzufügen.
            var xmlReader = new XmlTextReader("Button.xaml");

            var button = (Button) XamlReader.Load(xmlReader);
            var zeile5 = new ListBoxItem();
            zeile5.Content = button;
            listbox.Items.Add(zeile5);
        }

        private void SaveElementTree() {   // ListBox speichern.
            var xmlWriter = new XmlTextWriter("ListBox.xaml", Encoding.Unicode);
            XamlWriter.Save(listbox, xmlWriter);
        }

        private void listbox_MouseDown(object sender, MouseButtonEventArgs e) {

        }
    }
}
