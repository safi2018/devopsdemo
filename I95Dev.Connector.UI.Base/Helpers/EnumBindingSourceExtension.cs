using System;
using System.Windows.Markup;

namespace I95Dev.Connector.UI.Base.Helpers
{
    /// <summary>
    /// Enum binding source
    /// </summary>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type enumType;

        /// <summary>
        /// Gets or sets the type of the enum.
        /// </summary>
        /// <value>
        /// The type of the enum.
        /// </value>
        /// <exception cref="ArgumentException">Type must be for an Enum.</exception>
        public Type EnumType
        {
            get { return enumType; }
            set
            {
                if (value == enumType) return;
                if (null != value)
                {
                    Type enumType1 = Nullable.GetUnderlyingType(value) ?? value;

                    if (!enumType1.IsEnum)
                        throw new ArgumentException("Type must be for an Enum.");
                }

                enumType = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumBindingSourceExtension"/> class.
        /// </summary>
        public EnumBindingSourceExtension() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumBindingSourceExtension"/> class.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The EnumType must be specified.</exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == enumType)
                throw new InvalidOperationException("The EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == enumType)
                return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}