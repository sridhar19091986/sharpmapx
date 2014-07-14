using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SharpMap
{
    public class TypeUtils
    {
        /// <summary>
        /// Returns the object type as string.
        /// </summary>
        /// <param name="xml">Input XML</param>
        /// <returns>The type of the object</returns>
        public static string GetObjectTypeAsString(string xml)
        {
            System.IO.StringReader read = new StringReader(xml);

            var settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            System.Xml.XmlReader reader = XmlReader.Create(read, settings);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return reader.LocalName;
                }
            }

            return "";
        }

        /// <summary>
        /// Check if the given xml contains an object of a type.
        /// </summary>
        /// <param name="xml">xml to be searched</param>
        /// <param name="tipo">Type to be checked.</param>
        /// <returns>True if the type is found.</returns>
        public static bool CheckObjectType(string xml, Type tipo)
        {
            string str = GetObjectTypeAsString(xml);
            if (tipo.ToString().ToLower().Contains(str.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the given xml contains an object of a type.
        /// </summary>
        /// <param name="xml">xml to be searched</param>
        /// <param name="tipo">Type to be checked.</param>
        /// <returns>True if the type is found.</returns>
        public static bool CheckObjectType(string xml, string tipo)
        {
            string str = GetObjectTypeAsString(xml);
            if (tipo.ToLower().Contains(str.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="pObject">Object to be serialized.</param>
        /// <returns>XML</returns>
        public static string SerializeObject<T>(T pObject)
        {
            Type ObjectType = typeof(T);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(ObjectType);
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = false;
            StringBuilder output = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(output, ws);
            serializer.Serialize(xmlWriter, pObject);
            return output.ToString();
        }

        /// <summary>
        /// Deserializes an xml string.
        /// </summary>
        /// <param name="xml">xml to be deserialized.</param>
        /// <returns>The object.</returns>
        public static T DeSerializeObject<T>(string xml)
        {
            Type ObjectType = typeof(T);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(ObjectType);

            System.IO.StringReader read = new StringReader(xml);
            System.Xml.XmlReader reader = XmlReader.Create(read);
            return (T)serializer.Deserialize(reader);
        }

    }
}
