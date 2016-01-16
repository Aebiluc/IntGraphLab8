using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Collections.ObjectModel;

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
        public int NbBuckets{get;set;}
        public double[] Quantity { get; set; }
        public int Id { get; set; }

        public Lot()
        {
            Quantity = new double[4];
        }

        public override string ToString()
        {
            double sum = 0;
            foreach (double item in Quantity)
                sum += item;

            return string.Format("**************\nid {0} \nNb Bucket : {1}\nBlue : {2}ml\nGreen : {3}ml\nYellow : {4}ml\nOrange : {5}ml\n\n Sum = {6}\n**************", Id, NbBuckets, Quantity[0], Quantity[1], Quantity[2], Quantity[3],sum);
        }

        public void ExportXML(XmlWriter writer)
        {
            writer.WriteStartElement("Lot");

            writer.WriteElementString("ID", Id.ToString());
            writer.WriteElementString("nBucket", NbBuckets.ToString());

            writer.WriteElementString("blue", Quantity[0].ToString());
            writer.WriteElementString("green", Quantity[1].ToString());
            writer.WriteElementString("yellow", Quantity[2].ToString());
            writer.WriteElementString("orange", Quantity[3].ToString());

            writer.WriteEndElement();
        }

        public void ImportXML(XmlReader reader)
        {
            reader.ReadStartElement("Lot");

            Id = reader.ReadElementContentAsInt("ID", "");
            NbBuckets = reader.ReadElementContentAsInt("nBucket", "");

            Quantity[0] = reader.ReadElementContentAsDouble("blue", "");
            Quantity[1] = reader.ReadElementContentAsDouble("green", "");
            Quantity[2] = reader.ReadElementContentAsDouble("yellow", "");
            Quantity[3] = reader.ReadElementContentAsDouble("orange", "");

            reader.ReadEndElement();
        }
    }

    public class Recipe : ISerialXML
    {
        private ObservableCollection<Lot> _listLot = new ObservableCollection<Lot>();
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
            lot.Id = _currentId++;
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
            lot.Id = _currentId++;
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
