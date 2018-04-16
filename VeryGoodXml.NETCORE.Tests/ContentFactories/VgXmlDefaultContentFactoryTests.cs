using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace VeryGoodXml.NETCORE.Tests.ContentFactories
{
    [TestFixture]
    public class VgXmlDefaultContentFactoryTests
    {
        private string TestXml = "" +
            "<TestRoot>" +
            "  <TestElement>ElementValue</Element>" +
            "</TestRoot>";

        private class TestRoot
        {
            [VgXmlElement]
            [VgXmlStaticName("TestElement")]
            public List<string> Elements { get; set; }
        }

        [Test]
        public void ReadIntoProperty_PropertyIsCollection_EntityAddedToCollection()
        {
            var serializer = new VgXmlSerializer();

            using (var stringReader = new StringReader(TestXml))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                var ret = serializer.Deserialize(xmlReader, typeof(TestRoot)) as TestRoot;

                Assert.That(ret.Elements, Has.Member("ElementValue"));
            }
        }
    }
}
