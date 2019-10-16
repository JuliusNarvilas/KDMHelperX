using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KDMHelperX.Behaviors
{
    public class FloatEntryBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty IsValidProperty = BindableProperty.Create("IsValid", typeof(bool), typeof(IntEntryBehavior), false);
        public static readonly BindableProperty IsRequiredProperty = BindableProperty.Create("IsRequired", typeof(bool), typeof(IntEntryBehavior), false);
        public static readonly BindableProperty MinValueProperty = BindableProperty.Create("MinValue", typeof(long), typeof(IntEntryBehavior), (long)int.MinValue);
        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create("MaxValue", typeof(long), typeof(IntEntryBehavior), (long)int.MaxValue);

        private bool m_insideChangeCallback = true;

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
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        private bool TestIfConditionsAreValid(string value)
        {
            bool valueIsRequired = IsRequired;
            if (string.IsNullOrEmpty(value))
            {
                return !valueIsRequired;
            }
            else
            {
                if (value[0] == '-')
                {
                    //"-" is not a valid number even for non required values
                    if(value.Length == 1)
                        return false;
                }

                double parsedValue;
                if (double.TryParse(value, out parsedValue))
                {
                    return parsedValue >= MinValue && parsedValue <= MaxValue;
                }
            }
            return false;
        }

        private bool TestIfNumberIsValid(string value)
        {
            bool hadDot = false;
            bool wasValid = true;
            int valueSize = value.Length;

            for (int valueStrIndex = 0; valueStrIndex < valueSize; valueStrIndex++)
            {
                char currentChar = value[valueStrIndex];
                if (!char.IsDigit(currentChar))
                {
                    if (currentChar == '-')
                    {
                        if (valueStrIndex == 0 && value.Length > 1)
                        {
                            continue;
                        }
                    }
                    else if (!hadDot && currentChar == '.')
                    {
                        if(!hadDot && valueStrIndex > 0 && char.IsDigit(value[valueStrIndex-1]))
                        {
                            hadDot = true;
                            continue;
                        }
                    }
                    wasValid = false;
                    break;
                }
            }

            return wasValid;
        }

        private bool TestIfValid(string value)
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
            if (m_insideChangeCallback)
                return;

            m_insideChangeCallback = true;

            if (TestIfValid(e.NewTextValue))
            {
                IsValid = true;
            }
            else
            {
                //return old value
                ((Entry)sender).Text = e.OldTextValue;
            }

            m_insideChangeCallback = false;
        }
    }
}
