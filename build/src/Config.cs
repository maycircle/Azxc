using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Azxc
{
    public class Config
    {
        private XmlDocument _doc;
        private string _filePath;

        public Config(string filePath)
        {
            _filePath = filePath;
            _doc = new XmlDocument();
            if (!File.Exists(filePath))
                CreateRoot(filePath);
            _doc.Load(filePath);
        }

        private void CreateRoot(string filePath)
        {
            XmlWriter writer = XmlWriter.Create(filePath);
            writer.WriteStartDocument();
            writer.WriteStartElement("ModConfig");
            writer.WriteEndElement();
            writer.Close();
        }

        public void Add(string elementName, string value)
        {
            XmlNode element = _doc.CreateElement(elementName);
            _doc.DocumentElement.AppendChild(element);
            element.InnerText = value;
            _doc.Save(_filePath);
        }

        public XmlNode GetSingle(string elementName)
        {
            return _doc.GetElementsByTagName(elementName)?.Item(0);
        }

        public string TryGetSingle(string elementName, string defaultValue)
        {
            XmlNodeList nodeList = _doc.GetElementsByTagName(elementName);
            if (nodeList.Count == 0)
                return defaultValue;
            return nodeList.Item(0).InnerText ?? defaultValue;
        }

        public void TrySet(string elementName, string value)
        {
            XmlNode element = GetSingle(elementName);
            if (element != null)
                element.InnerText = value;
            else
                Add(elementName, value);
            _doc.Save(_filePath);
        }
    }
}
