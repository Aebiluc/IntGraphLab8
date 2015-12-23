using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IntGraphLab8
{
    class ProgrammeConfig : ISerialXML
    {
        public string FilePath { get; set; }

        public void ExportXML(XmlWriter writer)
        {
            writer.WriteStartElement("Parameter");
            writer.WriteElementString("FilePath", FilePath);
            writer.WriteEndElement();
        }

        public void ImportXML(XmlReader reader)
        {
            reader.ReadStartElement("Parameter");
            FilePath = reader.ReadElementContentAsString("FilePath", "");
            reader.ReadEndElement();
        }
    }
}
