using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KDMHelperX.Views.Property
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntEditView : ContentView
    {
        public IntEditView()
        {
            InitializeComponent();
        }

        void OnIncrement(object sender, EventArgs args)
        {
            long result = 0;
            if(long.TryParse(NumberInput.Text, out result))
            {
                result++;
                NumberInput.Text = result.ToString();
            }
            else
            {
                NumberInput.Text = "0";
            }
        }

        void OnDecrement(object sender, EventArgs args)
        {
            long result = 0;
            if (long.TryParse(NumberInput.Text, out result))
            {
                result--;
                NumberInput.Text = result.ToString();
            }
            else
            {
                NumberInput.Text = "0";
            }
        }
    }
}