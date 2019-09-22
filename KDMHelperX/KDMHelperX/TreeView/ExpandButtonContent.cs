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
    //set what icons shows for expanded/Collapsed/Leafe Nodes or on request node expand icon (when ShowExpandButtonIfEmpty true).
    public class ExpandButtonContent : ContentView
    {

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var node = (BindingContext as TreeViewNode);
            bool isLeafNode = (node.Children == null || node.Children.Count == 0);

            //empty nodes have no icon to expand unless showExpandButtonIfEmpty is et to true which will show the expand
            //icon can click and populated node on demand propably using the expand event.
            if ((isLeafNode) && !node.ShowExpandButtonIfEmpty)
            {
                Content = new ResourceImage
                {
                    Resource = isLeafNode ? "XamarinFormsTreeView.Resource.Blank.png" : "XamarinFormsTreeView.Resource.FolderOpen.png",
                    HeightRequest = 16,
                    WidthRequest = 16
                };
            }
            else
            {
                Content = new ResourceImage
                {
                    Resource = node.IsExpanded ? "XamarinFormsTreeView.Resource.OpenGlyph.png" : "XamarinFormsTreeView.Resource.CollpsedGlyph.png",
                    HeightRequest = 16,
                    WidthRequest = 16
                };
            }
        }

    }
    
}
