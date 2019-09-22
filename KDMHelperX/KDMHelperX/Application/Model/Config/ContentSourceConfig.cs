using Common;
using Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Xamarin.Forms;

namespace Assets.Game.Model.Config
{
    [Serializable]
    public class ContentSourceRecord
    {
        public string Name;
        public string Path;
    }

    [Serializable]
    public class ContentSourceConfig
    {
        public int Version;

        public List<ContentSourceRecord> Content = new List<ContentSourceRecord>();

        public List<ContentSourceRecord> Images = new List<ContentSourceRecord>();

        public List<ContentSourceRecord> Layouts = new List<ContentSourceRecord>();


        public bool SaveConfigFile()
        {
            return false;
        }
    }




    [Serializable]
    public class ContentResourceRecord
    {
        public string Name;
        public string Content;
    }


    [Serializable]
    public class ContentResourceImageRecord
    {
        public string Name;
        public Image Asset;
    }


    [Serializable]
    public class ContentResource
    {
        public int Version;

        public List<ContentResourceRecord> Content = new List<ContentResourceRecord>();

        public List<ContentResourceImageRecord> Images = new List<ContentResourceImageRecord>();

        public List<ContentResourceRecord> Layouts = new List<ContentResourceRecord>();



        public ContentSourceConfig GetConfig()
        {
            var result = new ContentSourceConfig();
            result.Version = Version;

            foreach (var record in Content)
            {
                var configRecord = new ContentSourceRecord();
                configRecord.Name = record.Name;
                configRecord.Path = string.Format("{0}.csv", record.Name);

                result.Content.Add(configRecord);
            }

            return result;
        }

        public void SaveContent()
        {
        }
    }
}
