using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KDMHelperX.Behaviors
{

    public class IntEntryBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty IsValidProperty = BindableProperty.Create("IsValid", typeof(bool), typeof(IntEntryBehavior), false);
        public static readonly BindableProperty IsRequiredProperty = BindableProperty.Create("IsRequired", typeof(bool), typeof(IntEntryBehavior), false);
        public static readonly BindableProperty MinDigitCountProperty = BindableProperty.Create("MinDigitCount", typeof(int), typeof(IntEntryBehavior), 0);
        public static readonly BindableProperty MaxDigitCountProperty = BindableProperty.Create("MaxDigitCount", typeof(int), typeof(IntEntryBehavior), 0);
        public static readonly BindableProperty MinValueProperty = BindableProperty.Create("MinValue", typeof(long), typeof(IntEntryBehavior), (long)int.MinValue);
        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create("MaxValue", typeof(long), typeof(IntEntryBehavior), (long)int.MaxValue);

        public int MinDigitCount
        {
            get { return (int)GetValue(MinDigitCountProperty); }
            set { SetValue(MinDigitCountProperty, value); }
        }
        public int MaxDigitCount
        {
            get { return (int)GetValue(MaxDigitCountProperty); }
            set { SetValue(MaxDigitCountProperty, value); }
        }
        public long MinValue
        {
            get { return (long)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public long MaxValue
        {
            get { return (long)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }
        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }


        public bool TestIfConditionsAreValid(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return !IsRequired;
            }
            else
            {
                double parsedValue;
                if (double.TryParse(value, out parsedValue))
                {
                    return parsedValue >= MinValue && parsedValue <= MaxValue;
                }
            }
            return false;
        }

        /// <summary>
        /// Validates a string that should represent a number of double type
        /// </summary>
        /// <remarks>
        /// This only allows for decimal notation strings without display seperattors. Empty string is also valid.
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Returns <code>true</code> for a valid number string and false otherwise.</returns>
        public bool TestIfNumberIsValid(string value)
        {
            bool wasValid = true;
            int valueSize = value.Length;

            for (int valueStrIndex = 0; valueStrIndex < valueSize; valueStrIndex++)
            {
                char currentChar = value[valueStrIndex];
                if (!char.IsDigit(currentChar))
                {
                    if (currentChar == '-')
                    {
                        //"-" without a number is not a valid number string
                        if (valueStrIndex != 0 || valueSize <= 1)
                        {
                            wasValid = false;
                            break;
                        }
                    }
                    else
                    {
                        wasValid = false;
                        break;
                    }
                }
            }

            return wasValid;
        }

        public bool TestIfValid(string value)
        {
            bool finalIsValid = true;
            if (!string.IsNullOrEmpty(value))
            {
                finalIsValid = TestIfNumberIsValid(value);
            }
            if (finalIsValid)
            {
                finalIsValid = TestIfConditionsAreValid(value);
            }

            return finalIsValid;
        }



        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.Keyboard = Keyboard.Numeric;
            IsValid = TestIfValid(bindable.Text);
            bindable.TextChanged += bindable_TextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;
        }

        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsValid = TestIfValid(e.NewTextValue);
        }
    }
}
