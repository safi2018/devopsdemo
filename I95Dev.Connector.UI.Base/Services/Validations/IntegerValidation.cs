using System.Globalization;
using System.Windows.Controls;

namespace I95Dev.Connector.UI.Base.Services.Validations
{
    /// <summary>
    /// Integer validations
    /// </summary>
    /// <seealso cref="System.Windows.Controls.ValidationRule" />
    public class IntegerValidation : ValidationRule
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
            int number;
            bool noIllegalChars;
            noIllegalChars = int.TryParse(value.ToString(), out number);

            if (noIllegalChars == false)
            {
                return new ValidationResult(false, "Only Integers allowed");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}