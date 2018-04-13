using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using VeryGoodXml.Attributes;
using VeryGoodXml.Entities.Names.Attributes;
using VeryGoodXml.Entities.Names.Enumerations;
using VeryGoodXml.Serializers;

namespace VeryGoodXml.NETCORE.Tests
{
    [TestFixture]
    class GeneralTests
    {
        [Test]
        public void GeneralTest()
        {
            var serializer = new VgXmlSerializer();
            var testObject = new TestClass
            {
                StaticElement = "Value",
                PRElement = new PropertyDefinedElement
                {
                    TagName = "PropertyDefined",
                    StaticElement = "Value2"
                }
            };

            using (var stringWriter = new StringWriter())
            using (var writer = XmlWriter.Create(stringWriter,
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    IndentChars = "  "
                }))
            {
                serializer.Serialize(writer, testObject);
                writer.Flush();
                TestContext.Write(stringWriter.ToString());
            }
        }
    }

    public class TestClass
    {
        [VgXmlElement]
        [VgXmlName("element1")]
        [VgXmlName("element2")]
        public string StaticElement { get; set; }

        [VgXmlElement]
        [VgXmlName(nameof(PropertyDefinedElement.TagName), VgXmlNameType.PropertyDefined)]
        public PropertyDefinedElement PRElement { get; set; }

        [VgXmlElement]
        [VgXmlName("regexElement.*", VgXmlNameType.Regex)]
        public string RegexElement { get; set; }
    }

    public class PropertyDefinedElement
    {
        public string TagName { get; set; }

        [VgXmlElement]
        [VgXmlName("element1")]
        public string StaticElement { get; set; }
    }
}
