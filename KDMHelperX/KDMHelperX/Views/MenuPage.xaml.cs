using KDMHelperX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KDMHelperX.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Browse, Title="Browse" },
                new HomeMenuItem {Id = MenuItemType.About, Title="About" }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                //await RootPage.NavigateFromMenu(id);
                RootPage.NavigateFromMenu(id);
            };

            var assembly = Assembly.GetExecutingAssembly();// IntrospectionExtensions.GetTypeInfo(typeof(LoadResourceText)).Assembly;

            string temp = "";
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                temp += resourceName;
                temp += "\n";
            }

            string ViewResources = "KDMHelperX.Resource.";
            //Stream stream = assembly.GetManifestResourceStream("KDMHelperX.Resource.XamlItemGroups.xml");
            //Stream stream2 = assembly.GetManifestResourceStream("KDMHelperX.XamlItemGroups.xml");

            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "KDMHelper", "Views");
            if (Directory.Exists(folderPath))
            {
                var files = Directory.EnumerateFiles(folderPath);
                foreach (var file in files)
                {
                    // Do your stuff
                }
            }

            ObservableCollection<TreeViewNode> rootNodes = new ObservableCollection<TreeViewNode>();
            rootNodes.Add(new TreeViewNode() {
                Content = new StackLayout {
                    Children = {
                        new ResourceImage
                        {
                            Resource = "KDMHelperX.Resource.Item.png",
                            HeightRequest= 16,
                            WidthRequest = 16
                        },
                        new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            TextColor = Color.Black,
                            Text = "Parent"
                        }
                    },
                    Orientation = StackOrientation.Horizontal
                },
                Children = {
                    new TreeViewNode() {
                        Content = new StackLayout {
                            Children = {
                                new ResourceImage
                                {
                                    Resource = "KDMHelperX.Resource.Item.png",
                                    HeightRequest= 16,
                                    WidthRequest = 16
                                },
                                new Label
                                {
                                    VerticalOptions = LayoutOptions.Center,
                                    TextColor = Color.Black,
                                    Text = "Child"
                                }
                            },
                            Orientation = StackOrientation.Horizontal
                        }
                    }
                }
            });
            TheTreeView.RootNodes = rootNodes;
        }
    }
}