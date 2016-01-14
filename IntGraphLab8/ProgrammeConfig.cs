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

        private string _MdpOperateur;
        private string _MdpManager;
        private string _Mdpadmin;

        public string MdpOperateur
        {
            get
            {
                string tmp = _MdpOperateur;
                DecryptString(tmp);
                return tmp;
            }
            set
            {
                _MdpOperateur = value;
                EncryptString(_MdpOperateur);
            }
        }

        public string MdpManager
        {
            get
            {
                string tmp = _MdpManager;
                DecryptString(tmp);
                return tmp;
            }
            set
            {
                _MdpManager = value;
                EncryptString(_MdpManager);
            }
        }
        public string MdpAdmin
        {
            get
            {
                string tmp = _Mdpadmin;
                DecryptString(tmp);
                return tmp;
            }
            set
            {
                _Mdpadmin = value;
                EncryptString(_Mdpadmin);
            }
        }

        public void ExportXML(XmlWriter writer)
        {
            writer.WriteStartElement("Parameter");
            writer.WriteElementString("FilePath", FilePath);
            writer.WriteElementString("TotalBucket", TotalBucket.ToString());
            writer.WriteElementString("MdpOperateur", MdpOperateur);
            writer.WriteElementString("MdpManager", MdpManager);
            writer.WriteElementString("MdpAdmin", MdpAdmin);
            writer.WriteEndElement();
        }

        public void ImportXML(XmlReader reader)
        {
            reader.ReadStartElement("Parameter");
            FilePath = reader.ReadElementContentAsString("FilePath", "");
            TotalBucket = reader.ReadElementContentAsInt("TotalBucket", "");
            MdpOperateur = reader.ReadElementContentAsString("MdpOperateur", "");
            MdpManager = reader.ReadElementContentAsString("MdpManager", "");
            MdpAdmin = reader.ReadElementContentAsString("MdpAdmin", "");
            reader.ReadEndElement();

            if (MdpOperateur == "")
                MdpOperateur = "o";

            if (MdpManager == "")
                MdpManager = "m";

            if (MdpAdmin == "")
                MdpAdmin = "a";
        }

        private void EncryptString(string mdp)
        {
            /*
            StringBuilder tmp = new StringBuilder();
            foreach (char item in mdp)
                tmp.Append( item + 1);
            mdp = tmp.ToString();*/
        }

        private void DecryptString(string mdp)
        {
            /*
            StringBuilder tmp = new StringBuilder();
            foreach (char item in mdp)
                tmp.Append(item - 1);
            mdp = tmp.ToString();*/
        }
    }
}
