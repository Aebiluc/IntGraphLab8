using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IntGraphLab8
{
    public class ProgrammeConfig : ISerialXML
    {
        public string FilePath { get; set; }
        public int TotalBucket { get; set; }

        public void ExportXML(XmlWriter writer)
        {
            writer.WriteStartElement("Parameter");
            writer.WriteElementString("FilePath", FilePath);
            writer.WriteElementString("TotalBucket", TotalBucket.ToString());
            writer.WriteEndElement();
        }

        public void ImportXML(XmlReader reader)
        {
            reader.ReadStartElement("Parameter");
            FilePath = reader.ReadElementContentAsString("FilePath", "");
            TotalBucket = reader.ReadElementContentAsInt("TotalBucket", "");
            reader.ReadEndElement();
        }
    }
}
