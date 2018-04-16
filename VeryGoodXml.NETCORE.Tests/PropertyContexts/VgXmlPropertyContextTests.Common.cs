using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeryGoodXml.NETCORE.Tests.PropertyContexts
{
    [TestFixture]
    public partial class VgXmlPropertyContextTests
    {
        public class TestClass
        {
            public const string StringElementName_Static = "StaticName";
            public const string StringElementName_Static2 = "StaticName2";
            public const string StringElementName_Regex = "RegexName.*";

            [VgXmlElement]
            [VgXmlStaticName(StringElementName_Static)]
            [VgXmlRegexName(StringElementName_Regex)]
            public string StaticRegexStringElement { get; set; }

            [VgXmlAttribute]
            public string StringAttribute { get; set; }

            [VgXmlRawElement]
            public string StringRawElement { get; set; }

            [VgXmlRawElement]
            public List<string> CollectionElement { get; set; }
            
            [VgXmlElement]
            [VgXmlStaticName(StringElementName_Static)]
            [VgXmlStaticName(StringElementName_Static2)]
            public string StaticStringElement { get; set; }

            [VgXmlElement]
            [VgXmlRegexName(StringElementName_Regex)]
            public string RegexStringElement { get; set; }

            [VgXmlElement]
            public TestSubElement NameContainerElement { get; set; }

            [VgXmlElement]
            [VgXmlStaticName(StringElementName_Static)]
            [VgXmlStaticName(StringElementName_Static2)]
            [VgXmlRegexName(StringElementName_Regex)]
            public TestSubElement StaticRegexNameContainerElement { get; set; }
        }

        public class TestSubElement
        {
            [VgXmlNameContainer]
            public string TagName { get; set; }
        }

        private TestClass _testObj;

        [SetUp]
        public void Setup()
        {
            _testObj = new TestClass
            {
                NameContainerElement = new TestSubElement(),
                StaticRegexNameContainerElement = new TestSubElement()
            };
        }
    }
}
