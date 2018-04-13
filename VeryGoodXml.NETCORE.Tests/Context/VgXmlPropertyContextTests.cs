using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VeryGoodXml.Attributes;
using VeryGoodXml.Context;
using VeryGoodXml.Entities.Attributes;
using VeryGoodXml.Entities.Enumerations;
using VeryGoodXml.Entities.Names;
using VeryGoodXml.Entities.Names.Attributes;
using VeryGoodXml.Entities.Names.Enumerations;

namespace VeryGoodXml.NETCORE.Tests.Context
{
    [TestFixture]
    public class VgXmlPropertyContextTests
    {
        public class TestClass
        {
            public const string StringElementName_Static = "StaticName";
            public const string StringElementName_Regex = "RegexName*";
            public const string StringElementName_PropertyDefined = "PropertyDefinedName";

            [VgXmlElement]
            [VgXmlName(StringElementName_Static, VgXmlNameType.Static)]
            [VgXmlName(StringElementName_Regex, VgXmlNameType.Regex)]
            [VgXmlName(StringElementName_PropertyDefined, VgXmlNameType.PropertyDefined)]
            public string StringElement { get; set; }

            [VgXmlAttribute]
            public string StringAttribute { get; set; }

            [VgXmlRawElement]
            public string StringRawElement { get; set; }

            [VgXmlRawElement]
            public List<string> CollectionElement { get; set; }
        }

        [Test]
        public void CreateContext_Property_Property_EqualsInputProperty()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.Property, Is.EqualTo(property));
        }

        [Test]
        public void CreateContext_Property_PropertyType_ReturnsPropertyInfoPropertyType()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.PropertyType, Is.EqualTo(property.PropertyType));
        }

        [TestCase(nameof(TestClass.StringElement), VgXmlEntityType.Element)]
        [TestCase(nameof(TestClass.StringAttribute), VgXmlEntityType.Attribute)]
        [TestCase(nameof(TestClass.StringRawElement), VgXmlEntityType.RawElement)]
        public void CreateContext_Property_EntityType_ReturnsEntityTypeDeclaredInPropertyDecoration(string propertyName, VgXmlEntityType expectedEntityType)
        {
            var property = typeof(TestClass).GetProperty(propertyName);
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.EntityType, Is.EqualTo(expectedEntityType));
        }

        [Test]
        public void GetNames_AllNameTypesIncluded_OnlyReturnsStaticAndRegexNameType()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            var names = context.GetNames();

            Assert.That(names, Has.One.Property(nameof(VgXmlName.Type)).EqualTo(VgXmlNameType.Static).With.
                                       Property(nameof(VgXmlName.Value)).EqualTo(TestClass.StringElementName_Static));
            Assert.That(names, Has.One.Property(nameof(VgXmlName.Type)).EqualTo(VgXmlNameType.Regex).With.
                                       Property(nameof(VgXmlName.Value)).EqualTo(TestClass.StringElementName_Regex));

            Assert.That(names, Has.None.Property(nameof(VgXmlName.Type)).EqualTo(VgXmlNameType.PropertyDefined).With.
                                        Property(nameof(VgXmlName.Value)).EqualTo(TestClass.StringElementName_PropertyDefined));
        }

        [Test]
        public void IsCollection_PropertyIsCollection_ReturnTrue()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.CollectionElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.IsCollection(), Is.True);
        }
    }
}
