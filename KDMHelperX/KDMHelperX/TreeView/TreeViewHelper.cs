using Adapt.PresentationSamples.Standard.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Xamarin.Forms
{
    public class TreeViewHelper
    {
        public static void ProcessXamlItems(TreeViewNode node, XamlItemGroup xamlItemGroup)
        {
            var children = new ObservableCollection<TreeViewNode>();
            foreach (var xamlItem in xamlItemGroup.XamlItems.OrderBy(xi => xi.Key))
            {
                CreateXamlItem(children, xamlItem);
            }
            node.Children = children;
        }

        public static void CreateXamlItem(IList<TreeViewNode> children, XamlItem xamlItem)
        {
            var label = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Black
            };
            label.SetBinding(Label.TextProperty, "Key");

            var xamlItemTreeViewNode = CreateTreeViewNode(xamlItem, label, true);
            children.Add(xamlItemTreeViewNode);
        }

        public static TreeViewNode CreateTreeViewNode(object bindingContext, Label label, bool isItem)
        {
            var node = new TreeViewNode
            {
                BindingContext = bindingContext,
                Content = new StackLayout
                {
                    Children =
                        {
                            new ResourceImage
                            {
                                Resource = isItem? "XamarinFormsTreeView.Resource.Item.png" :"XamarinFormsTreeView.Resource.FolderOpen.png" ,
                                HeightRequest= 16,
                                WidthRequest = 16
                            },
                            label
                        },
                    Orientation = StackOrientation.Horizontal
                }
            };

            //set DataTemplate for expand button content
            node.ExpandButtonTemplate = new DataTemplate(() => new ExpandButtonContent { BindingContext = node });

            return node;
        }

        private static ObservableCollection<TreeViewNode> ProcessXamlItemGroups(XamlItemGroup xamlItemGroups)
        {
            var rootNodes = new ObservableCollection<TreeViewNode>();

            foreach (var xamlItemGroup in xamlItemGroups.Children.OrderBy(xig => xig.Name))
            {

                var label = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.Black
                };
                label.SetBinding(Label.TextProperty, "Name");

                var groupTreeViewNode = TreeViewHelper.CreateTreeViewNode(xamlItemGroup, label, false);

                rootNodes.Add(groupTreeViewNode);

                groupTreeViewNode.Children = ProcessXamlItemGroups(xamlItemGroup);

                foreach (var xamlItem in xamlItemGroup.XamlItems)
                {
                    TreeViewHelper.CreateXamlItem(groupTreeViewNode.Children, xamlItem);
                }

            }

            return rootNodes;
        }

        public static object DeserialiseObject(string source, Type targetType)
        {
            var serializer = new XmlSerializer(targetType);
            var stream = new StringReader(source);
            return serializer.Deserialize(stream);
        }
    }
}
