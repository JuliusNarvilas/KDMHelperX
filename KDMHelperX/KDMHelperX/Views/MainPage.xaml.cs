using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using KDMHelperX.Models;

namespace KDMHelperX.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);
        }

        //public async Task NavigateFromMenu(int id)
        public void NavigateFromMenu(int id)
        {
            NavigationPage newPage;
            if(!MenuPages.TryGetValue(id, out newPage))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        newPage = new NavigationPage(new ItemsPage());
                        break;
                    case (int)MenuItemType.About:
                        newPage = new NavigationPage(new AboutPage());
                        break;
                    default:
                        newPage = null;
                        break;
                }
                if (newPage != null)
                {
                    MenuPages.Add(id, newPage);
                }
            }

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                //if (Device.RuntimePlatform == Device.Android)
                //    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}