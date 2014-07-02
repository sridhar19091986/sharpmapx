//==============================================================================
// Copyright 2010-2014 - Fabrizio Vita (www.itacasoft.com)
// This file can be redistributed and/or modified under the terms of the GNU Lesser General Public License.
//===============================================================================

using System;
using System.Text;
using System.Xml;
using System.IO;

namespace SharpMap.Utilities
{
    /// <summary>
    /// XML serializer.
    /// </summary>
    public static class ObjectXMLSerializer
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
        /// Decodes an UTF8 array to string.
        /// </summary>
        /// <param name="characters">UTF8 array.</param>
        /// <returns>string.</returns>
        public static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = "";// encoding.GetString(characters);
            return (constructedString.Substring(1));
        }

        /// <summary>
        /// Encodes a string to a UTF8 array.
        /// </summary>
        /// <param name="pXmlString">string to be encoded.</param>
        /// <returns>array of UTF8 characters.</returns>
        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
    }

    /// <summary>
    /// XML serializer. This is the generic variant of <see cref="ObjectXMLSerializer"/>
    /// </summary>
    /// <typeparam name="T">Class type</typeparam>
    public static class ObjectXMLSerializer<T> where T : class
    {
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = "";// encoding.GetString(characters);
            return (constructedString.Substring(1));
        }

        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="pObject">Object to be serialized.</param>
        /// <returns>XML</returns>
        public static string SerializeObject(T pObject)
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
        public static T DeSerializeObject(string xml)
        {
            Type ObjectType = typeof(T);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(ObjectType);

            System.IO.StringReader read = new StringReader(xml);
            System.Xml.XmlReader reader = XmlReader.Create(read);
            return (T)serializer.Deserialize(reader);
        }
    }
}
