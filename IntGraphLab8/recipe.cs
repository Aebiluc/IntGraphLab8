using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace IntGraphLab8
{
    public enum tank : byte { A,B,C,D };

    interface ISerialXML
    {
        void ExportXML(XmlWriter writer);
        void ImportXML(XmlReader reader);
    }

    public class Lot : ISerialXML
    {
        public int nbBuckets{get;set;}
        public double[] quantity { get; set; }
        public int id { get; set; }

        public Lot()
        {
            quantity = new double[4];
        }

        public override string ToString()
        {
            double sum = 0;
            foreach (double item in quantity)
                sum += item;

            return string.Format("**************\nid {0} \nNb Bucket : {1}\nBlue : {2}ml\nGreen : {3}ml\nYellow : {4}ml\nOrange : {5}ml\n\n Sum = {6}\n**************\n", id, nbBuckets,quantity[0], quantity[1], quantity[2], quantity[3],sum);
        }

        public void ExportXML(XmlWriter writer)
        {
            writer.WriteStartElement("Lot");

            writer.WriteElementString("ID", id.ToString());
            writer.WriteElementString("nBucket", nbBuckets.ToString());

            writer.WriteElementString("blue", quantity[0].ToString());
            writer.WriteElementString("green", quantity[1].ToString());
            writer.WriteElementString("yellow", quantity[2].ToString());
            writer.WriteElementString("orange", quantity[3].ToString());

            writer.WriteEndElement();
        }

        public void ImportXML(XmlReader reader)
        {
            reader.ReadStartElement("Lot");

            id = reader.ReadElementContentAsInt("ID", "");
            nbBuckets = reader.ReadElementContentAsInt("nBucket", "");

            quantity[0] = reader.ReadElementContentAsDouble("blue", "");
            quantity[1] = reader.ReadElementContentAsDouble("green", "");
            quantity[2] = reader.ReadElementContentAsDouble("yellow", "");
            quantity[3] = reader.ReadElementContentAsDouble("orange", "");

            reader.ReadEndElement();
        }
    }

    public class Recipe : ISerialXML
    {
        private List<Lot> _listLot = new List<Lot>();
        private int _currentId;
        public IEnumerable items { get { return _listLot; } }

        public int NbLot
        {
            get { return _listLot.Count; }
        }
        public int CurrentId
        {
            get { return _currentId; }  
        }

        public Recipe()
        {
            _currentId = 0;
        }

        public void Add(Lot lot)
        {
            lot.id = _currentId++;
            _listLot.Add(lot);
        }

        public bool Swap(int indexA, int indexB)
        {
            if (indexA < 0 || indexB < 0 || indexA > _listLot.Count - 1 || indexB > _listLot.Count - 1)
                return false;

            var tmp = _listLot[indexA];
            _listLot[indexA] = _listLot[indexB];
            _listLot[indexB] = tmp;

            return true;
        }

        public void Remove(int index)
        {
            _listLot.RemoveAt(index);
        }

        public void Insert(int index, Lot lot)
        {
            _listLot.Insert(index, lot);
            lot.id = _currentId++;
        }

        public Lot GetItem(int index)
        {
            return _listLot[index];
        }

        public void Clear()
        {
            _listLot.Clear();
            _currentId = 0;
        }

        public void ExportXML(XmlWriter writer)
        {
            writer.WriteStartElement("recipe");
            writer.WriteElementString("nblot", _listLot.Count.ToString());
            writer.WriteElementString("currentID", CurrentId.ToString());
            foreach (Lot element in _listLot)
                element.ExportXML(writer);

            writer.WriteEndElement();
        }

        public void ImportXML(XmlReader reader)
        {
            int nbLot;
            if (_listLot.Count > 0)
                _listLot.Clear();

            reader.ReadStartElement("recipe");
            nbLot = reader.ReadElementContentAsInt("nblot", "");
            
            _currentId = reader.ReadElementContentAsInt("currentID", "");
            for (int i = 0; i < nbLot; i++)
            {
                Lot lot = new Lot();
                lot.ImportXML(reader);
                _listLot.Add(lot);
            }
            reader.ReadEndElement();
        }
    }
}
