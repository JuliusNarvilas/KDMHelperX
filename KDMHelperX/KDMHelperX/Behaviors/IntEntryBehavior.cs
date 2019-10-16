using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KDMHelperX.Behaviors
{

    public class IntEntryBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty IsValidProperty = BindableProperty.Create("IsValid", typeof(bool), typeof(IntEntryBehavior), false);
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

        private bool TestIfValid(string value)
        {
            int minDigits = MinDigitCount;
            if (string.IsNullOrEmpty(value))
            {
                return minDigits == 0;
            }
            else
            {
                int valDigits = value.Length;
                if (value[0] == '-')
                {
                    //"-" is not a valid number even for 0 min digit count
                    if (value.Length == 1)
                        return false;

                    --valDigits;
                }

                int maxDigits = MaxDigitCount;
                if(valDigits >= minDigits && (maxDigits == 0 || valDigits <= maxDigits))
                {
                    long parsedValue;
                    if(long.TryParse(value, out parsedValue))
                    {
                        return parsedValue >= MinValue && parsedValue <= MaxValue;
                    }
                }
            }
            return false;
        }




        protected override void OnAttachedTo(Entry bindable)
        {
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
