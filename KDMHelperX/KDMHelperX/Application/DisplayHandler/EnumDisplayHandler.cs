using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Model;
using Game.Properties;

namespace Game.DisplayHandler
{
    public class EnumDisplayHandler : ValueDisplayHandler
    {
        private string m_name;

        private string m_val;


        private string m_type;


        private string m_content;

        public override void SetValue(NamedProperty val)
        {
            string nameStr = string.Empty;
            string valStr = string.Empty;
            string typeStr = string.Empty;
            string contentStr = string.Empty;
            KDMEnumProperty enumProp = null;


            if (val != null)
            {
                nameStr = val.GetName();
                enumProp = val.Property as KDMEnumProperty;
                if (enumProp != null)
                {
                    valStr = enumProp.GetValue().ToString();
                    typeStr = enumProp.GetValue().Factory.Name;
                    contentStr = enumProp.GetValue().Factory.Name;
                }
            }

            //if (m_name != null)
            //    m_name.text = nameStr;

            //if (valStr != null)
            //    m_name.text = valStr;

            //if (m_type != null)
            //    m_type.text = typeStr;

            //if (m_content != null)
            //    m_content.text = contentStr;
        }
    }
}
