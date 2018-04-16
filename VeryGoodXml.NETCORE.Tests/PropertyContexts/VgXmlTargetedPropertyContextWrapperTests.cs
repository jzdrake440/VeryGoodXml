using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace VeryGoodXml.NETCORE.Tests.PropertyContexts
{
    [TestFixture]
    public class VgXmlTargetedPropertyContextWrapperTests
    {
        public class Tesclass
        {
            [VgXmlElement]
            [VgXmlStaticName("Element")]
            [VgXmlRegexName(@"Element\d*")]
            public string StringElement { get; set; }

            [VgXmlElement]
            public List<string> StringCollectionElement { get; set; }
        }

        private Tesclass _testObj;
        private PropertyInfo _stringElementProperty = typeof(Tesclass).GetProperty(nameof(Tesclass.StringElement));
        private PropertyInfo _stringCollectionElementProperty = typeof(Tesclass).GetProperty(nameof(Tesclass.StringCollectionElement));

        [SetUp]
        public void SetUp()
        {
            _testObj = new Tesclass();
        }

        [Test]
        public void Constructor_TargetIsNotSameTypeAsDeclaredInContext_ThrowArgumentException()
        {
            var mock = new Mock<IVgXmlPropertyContext>();
            mock.SetupGet(c => c.Property).Returns(_stringElementProperty);

            Assert.That(() => new VgXmlTargetedPropertyContextWrapper(mock.Object, "NotTestClass"), Throws.ArgumentException);
        }

        [Test]
        public void SetValue_ValueIsAppropriateType_ValueIsSet()
        {
            var context = VgXmlPropertyContext.CreateContext(_stringElementProperty);
            var targetedContext = context.WithTarget(_testObj);
            var newStringValue = "NewStringValue";

            targetedContext.SetValue(newStringValue);

            Assert.That(_testObj.StringElement, Is.EqualTo(newStringValue));
        }

        [Test]
        public void SetValue_ValueIsNotAppropriateType_ThrowsArgumentException()
        {
            var context = VgXmlPropertyContext.CreateContext(_stringElementProperty);
            var targetedContext = context.WithTarget(_testObj);

            Assert.That(() => targetedContext.SetValue(new { value = "NotStringValue" }), Throws.ArgumentException);
        }

        [Test]
        public void GetValue_ReturnsTargetsValue()
        {
            var context = VgXmlPropertyContext.CreateContext(_stringElementProperty);
            var targetedContext = context.WithTarget(_testObj);

            _testObj.StringElement = "StringValue";
            var result = targetedContext.GetValue();

            Assert.That(result, Is.EqualTo(_testObj.StringElement));
        }

        [Test]
        public void AddToCollection_PropertyIsNotACollection_ThrowsInvalidOperationException()
        {
            var context = VgXmlPropertyContext.CreateContext(_stringElementProperty);
            var targetedContext = context.WithTarget(_testObj);

            Assert.That(() => targetedContext.AddToCollection("object"), Throws.InvalidOperationException);
        }

        [Test]
        public void AddToCollection_CollectionIsNull_InstantiateCollectionOnTarget()
        {
            var context = VgXmlPropertyContext.CreateContext(_stringCollectionElementProperty);
            var targetedContext = context.WithTarget(_testObj);

            targetedContext.AddToCollection("string");

            Assert.That(_testObj.StringCollectionElement, Is.Not.Null);
        }

        [Test]
        public void AddToCollection_ValueIsAddedToCollection()
        {
            var context = VgXmlPropertyContext.CreateContext(_stringCollectionElementProperty);
            var targetedContext = context.WithTarget(_testObj);

            _testObj.StringCollectionElement = new List<string>();
            var value1 = "stringValue1";
            var value2 = "stringValue1";
            var value3 = "stringValue1";
            targetedContext.AddToCollection(value1);
            targetedContext.AddToCollection(value2);
            targetedContext.AddToCollection(value3);

            Assert.That(_testObj.StringCollectionElement, Has.Count.EqualTo(3));
            Assert.That(_testObj.StringCollectionElement, Has.Member(value1));
            Assert.That(_testObj.StringCollectionElement, Has.Member(value2));
            Assert.That(_testObj.StringCollectionElement, Has.Member(value3));
        }

        [Test]
        public void AllWrappedMethods_FallThroughToBaseContext()
        {
            var mock = new Mock<IVgXmlPropertyContext>();
            mock.SetupGet(c => c.Property).Returns(_stringElementProperty);

            var targetedContext = new VgXmlTargetedPropertyContextWrapper(mock.Object, _testObj);

            targetedContext.GetOptions();
            mock.Verify(c => c.GetOptions());

            targetedContext.GetOptions<IVgXmlNameGenerator>();
            mock.Verify(c => c.GetOptions<IVgXmlNameGenerator>());

            targetedContext.NameMatches("value");
            mock.Verify(c => c.NameMatches("value"));

            targetedContext.GenerateName(_testObj);
            mock.Verify(c => c.GenerateName(_testObj));

            targetedContext.IsCollection();
            mock.Verify(c => c.IsCollection());

            targetedContext.GetBestMatchingNameFor(_testObj);
            mock.Verify(c => c.GetBestMatchingNameFor(_testObj));

            targetedContext.GetContentFactory();
            mock.Verify(c => c.GetContentFactory());
        }

        [Test]
        public void AllWrappedProperties_FallThroughToBaseContext()
        {
            var mock = new Mock<IVgXmlPropertyContext>();
            mock.SetupGet(c => c.Property).Returns(_stringElementProperty);

            var targetedContext = new VgXmlTargetedPropertyContextWrapper(mock.Object, _testObj);

            mock.ResetCalls();

            var property = targetedContext.Property;
            mock.VerifyGet(c => c.Property);

            var entityType = targetedContext.EntityType;
            mock.VerifyGet(c => c.EntityType);

            var propertyType = targetedContext.PropertyType;
            mock.VerifyGet(c => c.PropertyType);
        }
    }
}
