using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace I95Dev.Connector.UI.Controls
{
    public partial class DateTimePicker
    {
        private enum Direction
        {
            Previous = -1,
            Next = 1
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimePicker"/> class.
        /// </summary>
        public DateTimePicker()
        {
            InitializeComponent();
            CheckBoxSelected.Visibility = ShowCheckBox ? Visibility.Visible : Visibility.Collapsed;
            CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
            DateDisplay.PreviewMouseUp += DateDisplay_PreviewMouseUp;
            DateDisplay.LostFocus += DateDisplay_LostFocus;
            DateDisplay.PreviewKeyDown += DateTimePicker_PreviewKeyDown;
            DateDisplay.TextChanged += DateDisplay_TextChanged;
            PopUpCalendarButton.IsEnabled = DateDisplay.IsEnabled = !ShowCheckBox;
        }

        #region "Properties"

        /// <summary>
        /// Gets or sets the selected date.
        /// </summary>
        /// <value>
        /// The selected date.
        /// </value>
        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat
        {
            get { return Convert.ToString(GetValue(DateFormatProperty), CultureInfo.CurrentCulture); }
            set { SetValue(DateFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show calendar button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show calendar button]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCalendarButton
        {
            get { return PopUpCalendarButton.Visibility == Visibility.Visible; }
            set { PopUpCalendarButton.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); }
        }

        private string inputDateFormat;

        /// <summary>
        /// Inputs the date format.
        /// </summary>
        /// <returns></returns>
        private string InputDateFormat()
        {
            if (inputDateFormat != null) return inputDateFormat;
            string df = DateFormat;
            if (!df.Contains("MMM"))
                df = df.Replace("MM", "M");
            if (!df.Contains("ddd"))
                df = df.Replace("dd", "d");
            inputDateFormat = df.Replace("hh", "h").Replace("HH", "H").Replace("mm", "m").Replace("ss", "s");
            return inputDateFormat;
        }

        /// <summary>
        /// Gets or sets the minimum date.
        /// </summary>
        /// <value>
        /// The minimum date.
        /// </value>
        public DateTime MinimumDate
        {
            get { return Convert.ToDateTime(GetValue(MinimumDateProperty), CultureInfo.CurrentCulture); }
            set { SetValue(MinimumDateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum date.
        /// </summary>
        /// <value>
        /// The maximum date.
        /// </value>
        public DateTime MaximumDate
        {
            get { return Convert.ToDateTime(GetValue(MaximumDateProperty), CultureInfo.CurrentCulture); }
            set { SetValue(MaximumDateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show CheckBox].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show CheckBox]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCheckBox
        {
            get { return Convert.ToBoolean(GetValue(ShowCheckBoxProperty), CultureInfo.CurrentCulture); }
            set { SetValue(ShowCheckBoxProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsCheckBoxChecked
        {
            get { return Convert.ToBoolean(GetValue(IsCheckBoxCheckedProperty), CultureInfo.CurrentCulture); }
            set { SetValue(IsCheckBoxCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckBoxCheckedProperty = DependencyProperty.Register("IsCheckBoxChecked", typeof(bool), typeof(DateTimePicker), new FrameworkPropertyMetadata(false, OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = (DateTimePicker)d;
            me.CheckBoxSelected.IsChecked = (bool)e.NewValue;
            me.DateDisplay.IsEnabled = me.PopUpCalendarButton.IsEnabled = (bool)e.NewValue;
        }

        #endregion "Properties"

        #region "Events"

        /// <summary>
        /// Occurs when [date changed].
        /// </summary>
        public event RoutedEventHandler DateChanged
        {
            add { AddHandler(DateChangedEvent, value); }
            remove { RemoveHandler(DateChangedEvent, value); }
        }

        /// <summary>
        /// The date changed event
        /// </summary>
        public static readonly RoutedEvent DateChangedEvent = EventManager.RegisterRoutedEvent("DateChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DateTimePicker));

        /// <summary>
        /// Occurs when [date format changed].
        /// </summary>
        public event RoutedEventHandler DateFormatChanged
        {
            add { AddHandler(DateFormatChangedEvent, value); }
            remove { RemoveHandler(DateFormatChangedEvent, value); }
        }

        /// <summary>
        /// The date format changed event
        /// </summary>
        public static readonly RoutedEvent DateFormatChangedEvent = EventManager.RegisterRoutedEvent("DateFormatChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DateTimePicker));

        #endregion "Events"

        #region "DependencyProperties"

        /// <summary>
        /// The date format property
        /// </summary>
        public static readonly DependencyProperty DateFormatProperty = DependencyProperty.Register("DateFormat", typeof(string), typeof(DateTimePicker), new FrameworkPropertyMetadata("yyyy-MM-dd HH:mm:ss ttt", OnDateFormatChanged));

        /// <summary>
        /// The show CheckBox property
        /// </summary>
        public static readonly DependencyProperty ShowCheckBoxProperty = DependencyProperty.Register("ShowCheckBox", typeof(bool), typeof(DateTimePicker), new FrameworkPropertyMetadata(false, OnShowCheckBoxChanged));

        /// <summary>
        /// The maximum date property
        /// </summary>
        public static readonly DependencyProperty MaximumDateProperty = DependencyProperty.Register("MaximumDate", typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(Convert.ToDateTime("3000-01-01 00:00", CultureInfo.CurrentCulture), null, CoerceMaxDate));

        /// <summary>
        /// The minimum date property
        /// </summary>
        public static readonly DependencyProperty MinimumDateProperty = DependencyProperty.Register("MinimumDate", typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(Convert.ToDateTime("1900-01-01 00:00", CultureInfo.CurrentCulture), null, CoerceMinDate));

        /// <summary>
        /// The selected date property
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(DateTime.Now, OnSelectedDateChanged, CoerceDate));

        /// <summary>true when user is busy editing DateDisplay and the SelectedDate
        /// becomes different from the date shown on the text box.</summary>
        public static readonly DependencyProperty DateTextIsWrongProperty = DependencyProperty.Register("DateTextIsWrong", typeof(bool), typeof(DateTimePicker), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether [date text is wrong].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [date text is wrong]; otherwise, <c>false</c>.
        /// </value>
        protected bool DateTextIsWrong
        {
            get { return (bool)GetValue(DateTextIsWrongProperty); }
            set { SetValue(DateTextIsWrongProperty, value); }
        }

        #endregion "DependencyProperties"

        #region "EventHandlers"

        private bool forceTextUpdateNow = true;

        private void CalDisplay_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            PopUpCalendarButton.IsChecked = false;
            TimeSpan timeOfDay = SelectedDate.TimeOfDay;
            if (CalDisplay.SelectedDate != null) SelectedDate = CalDisplay.SelectedDate.Value.Date + timeOfDay;
        }

        private void DateDisplay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DateDisplay.SelectionLength == 0)
                FocusOnDatePart(DateDisplay.SelectionStart);
        }

        private bool IsDateInExpectedFormat()
        {
            return ParseDateText(false) != null;
        }

        private DateTime? ParseDateText(bool flexible)
        {
            DateTime selectedDate;

            if (!DateTime.TryParseExact(DateDisplay.Text, InputDateFormat(), null, DateTimeStyles.AllowWhiteSpaces, out selectedDate) && (!flexible || !DateTime.TryParse(DateDisplay.Text, out selectedDate)))
                return null;
            return selectedDate;
        }

        private void ReformatDateText()
        {
            // Changes DateDisplay.Text to match the current DateFormat
            DateTime? date = ParseDateText(true);
            if (date == null) return;
            string newText = date.Value.ToString(DateFormat, CultureInfo.CurrentCulture);
            if (DateDisplay.Text != newText)
                DateDisplay.Text = newText;
        }

        private void DateDisplay_LostFocus(object sender, RoutedEventArgs e)
        {
            DateDisplay.Text = SelectedDate.ToString(DateFormat, CultureInfo.CurrentCulture);
            // When the user selects a field again, then the box loses focus, then
            // the user clicks the same field again, the selection is cleared,
            // causing the arrows not to appear. To fix, clear selection in advance.
            try
            {
                DateDisplay.SelectionLength = 0;
            }
            catch (NullReferenceException)
            {
            }
        }

        private void DateDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime? date = ParseDateText(true);
            if (date != null)
                SelectedDate = date.Value;
        }

        private void DateTimePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int selstart = DateDisplay.SelectionStart;

            if (!IsDateInExpectedFormat())
                return;

            switch (e.Key)
            {
                case Key.Up:
                    OnUpDown(+1);
                    e.Handled = true;
                    break;

                case Key.Down:
                    OnUpDown(-1);
                    e.Handled = true;
                    break;

                case Key.Left:
                    if (Keyboard.Modifiers != ModifierKeys.None)
                        return;
                    SelectPosition(selstart, Direction.Previous);
                    e.Handled = true;
                    break;

                case Key.Right:
                    if (Keyboard.Modifiers != ModifierKeys.None)
                        return;
                    SelectPosition(selstart, Direction.Next);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    var dir = Direction.Next;
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                        dir = Direction.Previous;
                    e.Handled = SelectPosition(selstart, dir);
                    break;

                default:
                    char nextChar = '\0';
                    if (selstart < DateDisplay.Text.Length)
                        nextChar = DateDisplay.Text[selstart];

                    if ((e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.OemQuestion || e.Key == Key.Divide) &&
                        (nextChar == '/' || nextChar == '-') ||
                        e.Key == Key.Space && nextChar == ' ' ||
                        e.Key == Key.OemSemicolon && nextChar == ':')
                        SelectPosition(selstart, Direction.Next);
                    else
                        return;
                    e.Handled = true;
                    break;
            }
        }

        private void OnUpDown(int increment)
        {
            int selstart = DateDisplay.SelectionStart;
            forceTextUpdateNow = true;
            SelectedDate = Increase(selstart, increment);
            FocusOnDatePart(selstart);
        }

        private static object CoerceDate(DependencyObject d, object value)
        {
            DateTimePicker me = (DateTimePicker)d;
            DateTime current = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
            if (current < me.MinimumDate)
                current = me.MinimumDate;
            if (current > me.MaximumDate)
                current = me.MaximumDate;
            return current;
        }

        private static object CoerceMinDate(DependencyObject d, object value)
        {
            DateTimePicker me = (DateTimePicker)d;
            DateTime current = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
            if (current >= me.MaximumDate)
                throw new ArgumentException("MinimumDate can not be equal to, or more than maximum date");

            if (current > me.SelectedDate)
                me.SelectedDate = current;

            return current;
        }

        private static object CoerceMaxDate(DependencyObject d, object value)
        {
            DateTimePicker me = (DateTimePicker)d;
            DateTime current = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
            if (current <= me.MinimumDate)
                throw new ArgumentException("MaximimumDate can not be equal to, or less than MinimumDate");

            if (current < me.SelectedDate)
                me.SelectedDate = current;

            return current;
        }

        /// <summary>
        /// Called when [date format changed].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDateFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var me = (DateTimePicker)obj;
            me.inputDateFormat = null; // will be recomputed on-demand
            me.DateDisplay.Text = me.SelectedDate.ToString(me.DateFormat, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Called when [show CheckBox changed].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowCheckBoxChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var me = (DateTimePicker)obj;
            me.CheckBoxSelected.Visibility = me.ShowCheckBox ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Called when [selected date changed].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnSelectedDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var me = (DateTimePicker)obj;

            var date = (DateTime)args.NewValue;
            me.CalDisplay.SelectedDate = date;
            me.CalDisplay.DisplayDate = date;
            if (me.DateDisplay.IsFocused && !me.forceTextUpdateNow)
            {
                DateTime? oldDate = me.ParseDateText(true);
                if (oldDate != null)
                    me.DateTextIsWrong = date != oldDate.Value;
            }
            else
            {
                me.DateTextIsWrong = false;
                me.forceTextUpdateNow = false;
                me.DateDisplay.Text = date.ToString(me.DateFormat, CultureInfo.CurrentCulture);
            }
        }

        #endregion "EventHandlers"

        // Selects next or previous date value, depending on the incrementor value
        // Alternatively moves focus to previous control or the calender button
        private bool SelectPosition(int selstart, Direction direction)
        {
            selstart = CalcPosition(selstart, direction);
            return selstart > -1 && FocusOnDatePart(selstart);
        }

        // Gets location of next/previous date field, depending on the incrementor value.
        // Returns -1 if there is no next/previous field.
        private int CalcPosition(int selStart, Direction direction)
        {
            string df = DateFormat;
            if (selStart >= df.Length)
                selStart = df.Length - 1;
            char startChar = df[selStart];
            int i = selStart;

            for (; ; )
            {
                i += (int)direction;
                if ((uint)i >= (uint)df.Length)
                    return -1;
                if (df[i] == startChar)
                    continue;
                if (char.IsLetter(df[i]))
                    break;
                startChar = '\0'; // to handle cases like "yyyy-MM-dd (ddd)" correctly
            }

            if (direction >= 0) return i;
            while (i > 0 && df[i - 1] == df[i])
                i--;

            return i;
        }

        private bool FocusOnDatePart(int selStart)
        {
            ReformatDateText();

            // Find beginning of field to select
            string df = DateFormat;
            if (selStart > df.Length - 1)
                selStart = df.Length - 1;
            char firstchar = df[selStart];
            while (!char.IsLetter(firstchar) && selStart + 1 < df.Length)
            {
                selStart++;
                firstchar = df[selStart];
            }
            while (selStart > 0 && df[selStart - 1] == firstchar)
                selStart--;

            int selLength = 1;
            while (selStart + selLength < df.Length && df[selStart + selLength] == firstchar)
                selLength++;

            // don't select AM/PM: we have no interface to change it.
            if (firstchar == 't')
                return false;

            DateDisplay.Focus();
            DateDisplay.Select(selStart, selLength);
            return true;
        }

        private DateTime Increase(int selstart, int value)
        {
            DateTime retval = (ParseDateText(false) ?? SelectedDate);

            try
            {
                switch (DateFormat.Substring(selstart, 1))
                {
                    case "h":
                    case "H":
                        retval = retval.AddHours(value);
                        break;

                    case "y":
                        retval = retval.AddYears(value);
                        break;

                    case "M":
                        retval = retval.AddMonths(value);
                        break;

                    case "m":
                        retval = retval.AddMinutes(value);
                        break;

                    case "d":
                        retval = retval.AddDays(value);
                        break;

                    case "s":
                        retval = retval.AddSeconds(value);
                        break;
                }
            }
            catch
            {
                //Catch dates with year over 9999 etc, don't throw
            }

            return retval;
        }
    }
}