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
                DecryptString(ref tmp);
                return tmp;
            }
            set
            {
                _MdpOperateur = value;
                EncryptString(ref _MdpOperateur);
            }
        }

        public string MdpManager
        {
            get
            {
                string tmp = _MdpManager;
                DecryptString(ref tmp);
                return tmp;
            }
            set
            {
                _MdpManager = value;
                EncryptString(ref _MdpManager);
            }
        }
        public string MdpAdmin
        {
            get
            {
                string tmp = _Mdpadmin;
                DecryptString(ref tmp);
                return tmp;
            }
            set
            {
                _Mdpadmin = value;
                EncryptString(ref _Mdpadmin);
            }
        }

        public void ExportXML(XmlWriter writer)
        {
            writer.WriteStartElement("Parameter");
            writer.WriteElementString("FilePath", FilePath);
            writer.WriteElementString("TotalBucket", TotalBucket.ToString());
            writer.WriteElementString("MdpOperateur", _MdpOperateur);
            writer.WriteElementString("MdpManager", _MdpManager);
            writer.WriteElementString("MdpAdmin", _Mdpadmin);
            writer.WriteEndElement();
        }

        public void ImportXML(XmlReader reader)
        {
            reader.ReadStartElement("Parameter");
            FilePath = reader.ReadElementContentAsString("FilePath", "");
            TotalBucket = reader.ReadElementContentAsInt("TotalBucket", "");
            _MdpOperateur = reader.ReadElementContentAsString("MdpOperateur", "");
            _MdpManager = reader.ReadElementContentAsString("MdpManager", "");
            _Mdpadmin = reader.ReadElementContentAsString("MdpAdmin", "");
            reader.ReadEndElement();

            if (MdpOperateur == "")
                MdpOperateur = "o";

            if (MdpManager == "")
                MdpManager = "m";

            if (MdpAdmin == "")
                MdpAdmin = "a";
        }

        private void EncryptString(ref string mdp)
        {
            StringBuilder tmp = new StringBuilder();
            foreach (char item in mdp)
                tmp.Append((char)(item + 1));
            mdp = tmp.ToString();
        }

        private void DecryptString(ref string mdp)
        {
            StringBuilder tmp = new StringBuilder();
            foreach (char item in mdp)
                tmp.Append((char)(item - 1));
            mdp = tmp.ToString();
        }
    }
}
