using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Globalization;

namespace CampusLaunch.Wpf.Demo06_DataBinding
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private List<Person> _Persons = new List<Person>();
        public List<Person> Persons { get { return _Persons; } }

        public Window1()
        {
            _Persons.Add(new Person { Name = "Sahra Baum", Age = 21, Department = Department.Management });
            _Persons.Add(new Person { Name = "Eva Fröhlich", Age = 28, Department = Department.IT });
            _Persons.Add(new Person { Name = "Silvia Halanzy", Age = 41, Department = Department.Marketing, IsWorkingPartTime = true });
            _Persons.Add(new Person { Name = "Heinrich Kühn", Age = 51, Department = Department.Sales });
            _Persons.Add(new Person { Name = "Egon Walter", Age = 23, Department = Department.IT, IsWorkingAtHome = true });

            InitializeComponent();
        }

        private void OnHeaderClick(object sender, EventArgs e)
        {
            GridViewColumnHeader header = (GridViewColumnHeader)sender;
            string headerName = (string)header.Content;
            CollectionViewSource source = (CollectionViewSource)FindResource("persons");
            SortDescription description = source.SortDescriptions[0];

            switch (headerName)
            {
                case "Name":
                    if (description.PropertyName == "Name" && description.Direction == ListSortDirection.Ascending)
                    {
                        source.SortDescriptions.Clear();
                        source.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
                    }
                    else
                    {
                        source.SortDescriptions.Clear();
                        source.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    }
                    break;
                case "Age":
                    if (description.PropertyName == "Age" && description.Direction == ListSortDirection.Ascending)
                    {
                        source.SortDescriptions.Clear();
                        source.SortDescriptions.Add(new SortDescription("Age", ListSortDirection.Descending));
                    }
                    else
                    {
                        source.SortDescriptions.Clear();
                        source.SortDescriptions.Add(new SortDescription("Age", ListSortDirection.Ascending));
                    }
                    break;
                case "Department":
                    if (description.PropertyName == "Department" && description.Direction == ListSortDirection.Ascending)
                    {
                        source.SortDescriptions.Clear();
                        source.SortDescriptions.Add(new SortDescription("Department", ListSortDirection.Descending));
                    }
                    else
                    {
                        source.SortDescriptions.Clear();
                        source.SortDescriptions.Add(new SortDescription("Department", ListSortDirection.Ascending));
                    }
                    break;
                default:
                    break;
            }

        }
    }
}