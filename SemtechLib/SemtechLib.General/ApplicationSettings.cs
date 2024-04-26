using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml;

namespace SemtechLib.General
{
	public sealed class ApplicationSettings : IDisposable
	{
		private const string FileName = "ApplicationSettings.xml";

		private const string RootElement = "ApplicationSettings";

		private const string SettingElement = "Setting";

		private const string PathSeperator = "/";

        public XmlDocument XmlDocument { get; private set; }

        public ApplicationSettings()
		{
			XmlDocument = OpenDocument();
		}

		public bool SetValue(string Name, string Value)
		{
			var xmlNodeList = XmlDocument.SelectNodes("/ApplicationSettings/Setting");
			foreach (XmlNode item in xmlNodeList)
			{
				if (!item.Attributes["Name"].Value.Equals(Name)) continue;
				item.Attributes["Value"].Value = Value;
				return false;
			}
			var xmlNode2 = XmlDocument.SelectSingleNode("/ApplicationSettings");
			XmlNode xmlNode3 = XmlDocument.CreateElement("Setting");
			xmlNode3.Attributes.Append(XmlDocument.CreateAttribute("Name"));
			xmlNode3.Attributes.Append(XmlDocument.CreateAttribute("Value"));
			xmlNode3.Attributes["Name"].Value = Name;
			xmlNode3.Attributes["Value"].Value = Value;
			xmlNode2.AppendChild(xmlNode3);
			return true;
		}

		public bool RemoveValue(string Name)
		{
			var xmlNodeList = XmlDocument.SelectNodes("/ApplicationSettings/Setting");
			foreach (XmlNode item in xmlNodeList)
			{
				if (!item.Attributes["Name"].Value.Equals(Name)) continue;
				item.ParentNode.RemoveChild(item);
				return true;
			}
			return false;
		}

		public string GetValue(string Name)
		{
			var xmlNodeList = XmlDocument.SelectNodes("/ApplicationSettings/Setting");
			foreach (XmlNode item in xmlNodeList)
			{
				if (item.Attributes["Name"].Value.Equals(Name))
				{
					return item.Attributes["Value"].Value;
				}
			}
			return null;
		}

		public void ClearSettings()
		{
			XmlDocument = CreateDocument();
		}

		public Hashtable GetSettings()
		{
			var xmlNodeList = XmlDocument.SelectNodes("/ApplicationSettings/Setting");
			var hashtable = new Hashtable(xmlNodeList.Count);
			foreach (XmlNode item in xmlNodeList)
			{
				hashtable.Add(item.Attributes["Name"].Value, item.Attributes["Value"].Value);
			}
			return hashtable;
		}

		public void SaveConfiguration()
		{
			SaveDocument(XmlDocument, "ApplicationSettings.xml");
		}

		private static XmlDocument OpenDocument()
		{
			IsolatedStorageFileStream isolatedStorageFileStream;
			try
			{
				isolatedStorageFileStream = new IsolatedStorageFileStream("ApplicationSettings.xml", FileMode.Open, FileAccess.Read);
			}
			catch (FileNotFoundException)
			{
				return CreateDocument();
			}
			var xmlDocument = new XmlDocument();
			var xmlTextReader = new XmlTextReader(isolatedStorageFileStream);
			xmlDocument.Load(xmlTextReader);
			xmlTextReader.Close();
			isolatedStorageFileStream.Close();
			return xmlDocument;
		}

		private static XmlDocument CreateDocument()
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.CreateXmlDeclaration("1.0", null, "yes");
			var newChild = xmlDocument.CreateElement("ApplicationSettings");
			xmlDocument.AppendChild(newChild);
			return xmlDocument;
		}

		private static void SaveDocument(XmlDocument document, string filename)
		{
			var isolatedStorageFileStream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
			isolatedStorageFileStream.SetLength(0L);
            var xmlTextWriter = new XmlTextWriter(isolatedStorageFileStream, new UnicodeEncoding())
            {
                Formatting = Formatting.Indented
            };
            document.Save(xmlTextWriter);
			xmlTextWriter.Close();
			isolatedStorageFileStream.Close();
		}

		public void Dispose()
		{
			SaveDocument(XmlDocument, "ApplicationSettings.xml");
		}
	}
}
