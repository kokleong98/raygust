using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Raygust.Core.Parser
{
    [XmlRoot("configuration")]
    public class ArgumentConfigurations
    {
        [XmlArray(ElementName = "params")]
        [XmlArrayItem(ElementName = "param")]
        public List<ArgumentConfiguration> Items = new List<ArgumentConfiguration>();

        public void Load(string xmlData)
        {
            using (StringReader sr = new StringReader(xmlData))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ArgumentConfigurations));
                ArgumentConfigurations tmp = (ArgumentConfigurations)xs.Deserialize(sr);
                Items.Clear();
                foreach (ArgumentConfiguration item in tmp.Items)
                {
                    Items.Add(item);
                }
            }
        }
    }
}
