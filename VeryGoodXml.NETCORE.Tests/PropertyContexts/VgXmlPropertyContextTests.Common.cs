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
            public const string StringElementName_Regex = "RegexName*";

            [VgXmlElement]
            [VgXmlStaticName(StringElementName_Static)]
            [VgXmlRegexName(StringElementName_Regex)]
            public string StringElement { get; set; }

            [VgXmlAttribute]
            public string StringAttribute { get; set; }

            [VgXmlRawElement]
            public string StringRawElement { get; set; }

            [VgXmlRawElement]
            public List<string> CollectionElement { get; set; }
        }
    }
}
