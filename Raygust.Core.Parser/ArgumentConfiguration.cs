using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Raygust.Core.Parser
{
    public class ArgumentConfiguration
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name;

        [XmlAttribute(AttributeName = "required")]
        public bool IsRequired;

        [XmlAttribute(AttributeName = "list")]
        public bool IsList;
    }
}
