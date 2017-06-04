using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace CampusLaunch.Wpf.Demo06_DataBinding
{
    public class Int32ValidationRule : ValidationRule
    {
        public int Maximum { get; set; }

        public int Minimum { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string svalue = value as string;
            int valueType;
            bool isValid;

            if (svalue == null) { return new ValidationResult(false, "The databound value should be of type string."); }

            isValid = Int32.TryParse(svalue, out valueType);

            if (isValid)
            {
                if (Minimum <= valueType && valueType <= Maximum)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false, string.Format("The value should be bettween {0} and {1}.", Minimum, Maximum));
                }

            }
            else
            {
                return new ValidationResult(false, "The value should be of type integer.");
            }
        }
    }
}
