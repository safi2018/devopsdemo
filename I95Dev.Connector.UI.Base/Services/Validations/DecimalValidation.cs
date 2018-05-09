using System.Globalization;
using System.Windows.Controls;

namespace I95Dev.Connector.UI.Base.Services.Validations
{
    public class DecimalValidation : ValidationRule
    {
        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult" /> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal number;
            bool noIllegalChars;
            noIllegalChars = decimal.TryParse(value.ToString(), out number);

            if (noIllegalChars == false)
            {
                return new ValidationResult(false, "Only Decimal allowed");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}