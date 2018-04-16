using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeryGoodXml.NETCORE.Tests.PropertyContexts
{
    [TestFixture]
    public partial class VgXmlPropertyContextTests
    {
        [Test]
        public void Property_EqualsInputProperty()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.Property, Is.EqualTo(property));
        }

        [Test]
        public void PropertyType_ReturnsPropertyInfoPropertyType()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.PropertyType, Is.EqualTo(property.PropertyType));
        }

        [TestCase(nameof(TestClass.StringElement), VgXmlEntityType.Element)]
        [TestCase(nameof(TestClass.StringAttribute), VgXmlEntityType.Attribute)]
        [TestCase(nameof(TestClass.StringRawElement), VgXmlEntityType.RawElement)]
        public void EntityType_ReturnsEntityTypeDeclaredInPropertyDecoration(string propertyName, VgXmlEntityType expectedEntityType)
        {
            var property = typeof(TestClass).GetProperty(propertyName);
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.EntityType, Is.EqualTo(expectedEntityType));
        }
    }
}
