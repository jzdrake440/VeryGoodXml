using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VeryGoodXml.NETCORE.Tests.PropertyContexts
{
    [TestFixture]
    public partial class VgXmlPropertyContextTests
    {
        [Test]
        public void WithTarget_OfPropertyType_ReturnsIVgXmlTargetedPropertyContext()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);
            var targettedContext = context.WithTarget(new TestClass());

            Assert.That(targettedContext, Is.Not.Null);
            Assert.That(targettedContext, Is.InstanceOf<IVgXmlTargetedPropertyContext>());
        }

        [Test]
        public void WithTarget_NotOfPropertyType_ThrowsArgumentException()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(() => context.WithTarget("NotTestClassType"), Throws.ArgumentException);
        }

        [Test]
        public void GetOptions_ManyOptions_ReturnsDefinedOptions()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            var options = context.GetOptions().ToList();

            Assert.That(options, Has.Count.EqualTo(2));
            Assert.That(options, Has.One.InstanceOf(typeof(VgXmlStaticNameAttribute)).With.
                Property(nameof(VgXmlStaticNameAttribute.Name)).EqualTo(TestClass.StringElementName_Static));

            Assert.That(options, Has.One.InstanceOf(typeof(VgXmlRegexNameAttribute)).With.
                Property(nameof(VgXmlRegexNameAttribute.Pattern)).EqualTo(TestClass.StringElementName_Regex));
        }

        [Test]
        public void GetOptions_NoNamesDefined_ReturnsEmpty()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringRawElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            var options = context.GetOptions().ToList();

            Assert.That(options, Is.Empty);
        }

        //GetOptions<T>

        [Test]
        public void IsCollection_PropertyIsCollection_ReturnTrue()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.CollectionElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.IsCollection(), Is.True);
        }

        [Test]
        public void IsCollection_PropertyIsNotCollection_ReturnFalse()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.IsCollection(), Is.False);
        }

        //GetContentFactory
        //GetBestMatchingNameFor
    }
}
