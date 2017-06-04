using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CampusLaunch.Wpf.Demo06_DataBinding
{
    public class Person : System.ComponentModel.INotifyPropertyChanged
    {
        private string name;
        private int age;
        private Department department;
        private bool isWorkingPartTime;
        private bool isWorkingAtHome;

        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }

        public int Age { get { return age; } set { age = value; OnPropertyChanged("Age"); } }

        public Department Department { get { return department; } set { department = value; OnPropertyChanged("Department"); } }

        public bool IsWorkingPartTime { get { return isWorkingPartTime; } set { isWorkingPartTime = value; OnPropertyChanged("IsWorkingPartTime"); } }

        public bool IsWorkingAtHome { get { return isWorkingAtHome; } set { isWorkingAtHome = value; OnPropertyChanged("IsWorkingAtHome"); } }

        #region INotifyPropertyChanged Members

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public enum Department
    {
        Management, Sales, IT, Marketing
    }
}
