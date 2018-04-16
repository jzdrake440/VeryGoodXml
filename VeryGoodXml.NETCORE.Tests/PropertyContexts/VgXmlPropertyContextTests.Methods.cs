using NUnit.Framework;
using System.Linq;

namespace VeryGoodXml.NETCORE.Tests.PropertyContexts
{
    [TestFixture]
    public partial class VgXmlPropertyContextTests
    {
        [Test]
        public void WithTarget_OfPropertyType_ReturnsIVgXmlTargetedPropertyContext()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);
            var targettedContext = context.WithTarget(new TestClass());

            Assert.That(targettedContext, Is.Not.Null);
        }

        [Test]
        public void WithTarget_NotOfPropertyType_ThrowsArgumentException()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(() => context.WithTarget("NotTestClassType"), Throws.ArgumentException);
        }

        [Test]
        public void GetOptions_ManyOptions_ReturnsDefinedOptions()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexStringElement));
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

        [Test]
        public void GetOptionsT_ReturnsTypesDeclaredInGenericArgument()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            //out of the 2 options posted on StringElement; only the StaticName is a generator
            var options = context.GetOptions<IVgXmlNameGenerator>().ToList();

            Assert.That(options, Has.Count.EqualTo(1));
            Assert.That(options, Has.One.InstanceOf<IVgXmlNameGenerator>());
        }

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
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.IsCollection(), Is.False);
        }

        [Test]
        public void GetContentFactory_ReturnsInstanceOfContentFactory()
        {

            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(context.GetContentFactory(), Is.Not.Null);
        }

        [TestCase("RegexName", ExpectedResult = true)]
        [TestCase("RegexNameExtended", ExpectedResult = true)]
        [TestCase("InvalidRegex_Name", ExpectedResult = false)]
        public bool NameMatches_RegexName(string name)
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.RegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            return context.NameMatches(name);
        }

        [TestCase(TestClass.StringElementName_Static, ExpectedResult = true)]
        [TestCase(TestClass.StringElementName_Static2, ExpectedResult = true)]
        [TestCase("InvalidStaticName", ExpectedResult = false)]
        public bool NameMatches_StaticName(string name)
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            return context.NameMatches(name);
        }

        [TestCase(nameof(TestClass.StringAttribute), ExpectedResult = true)]
        [TestCase("InvalidPropertyName", ExpectedResult = false)]
        public bool NameMatches_NoNameDefined_ExpectsNameSameAsPropertyName(string name)
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringAttribute));
            var context = VgXmlPropertyContext.CreateContext(property);

            return context.NameMatches(name);
        }

        [Test]
        public void GenerateName_RegexName_DoesNotReturnFirstFoundRegexName()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.RegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            var result = context.GenerateName(null);

            Assert.That(result, Is.Not.EqualTo(TestClass.StringElementName_Regex));
        }

        [Test]
        public void GenerateName_StaticName_ReturnsFirstFoundStaticName()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            var result = context.GenerateName(null);

            Assert.That(result, Is.EqualTo(TestClass.StringElementName_Static));
        }

        [Test]
        public void GenerateName_NoName_ReturnsPropertyName()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringAttribute));
            var context = VgXmlPropertyContext.CreateContext(property);

            var result = context.GenerateName(null);

            Assert.That(result, Is.EqualTo(nameof(TestClass.StringAttribute)));
        }

        [Test]
        public void GetBestMatchingNameFor_NoNameOptions_ReturnsPropertyName()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StringAttribute));
            var context = VgXmlPropertyContext.CreateContext(property);

            var result = context.GetBestMatchingNameFor("string");

            Assert.That(result, Is.EqualTo(nameof(TestClass.StringAttribute)));
        }

        [Test]
        public void GetBestMatchingNameFor_StaticNameOption_ReturnsFirstStaticName()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            var result = context.GetBestMatchingNameFor("string");

            Assert.That(result, Is.EqualTo(TestClass.StringElementName_Static));
        }

        [Test]
        public void GetBestMatchingNameFor_RegexNameOption_PropertyNameDoesNotMatchPattern_ThrowsInvalidOperationsException()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.RegexStringElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            Assert.That(() => context.GetBestMatchingNameFor("string"), Throws.InvalidOperationException);
        }

        [TestCase("AnyName1")]
        [TestCase("AnyName2")]
        [TestCase("AnyName3")]
        [TestCase("AnyName4")]
        public void GetBestMatchingNameFor_IsNameContainer_NoDefinedOptions_ReturnsNameContainerValue(string nameContainerValue)
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.NameContainerElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            _testObj.StaticRegexNameContainerElement.TagName = nameContainerValue;

            var result = context.GetBestMatchingNameFor(_testObj.StaticRegexNameContainerElement);

            Assert.That(result, Is.EqualTo(nameContainerValue));
        }

        [TestCase(TestClass.StringElementName_Static)]
        [TestCase(TestClass.StringElementName_Static2)]
        [TestCase("RegexName")]
        [TestCase("RegexNameExtended")]
        public void GetBestMatchingNameFor_IsNameContainer_NameOptionsDefined_ContainerValueIsValid_ReturnsContainerValue(string nameContainerValue)
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexNameContainerElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            _testObj.StaticRegexNameContainerElement.TagName = nameContainerValue;

            var result = context.GetBestMatchingNameFor(_testObj.StaticRegexNameContainerElement);

            Assert.That(result, Is.EqualTo(nameContainerValue));
        }

        [Test]
        public void GetBestMatchingNameFor_IsNameContainer_NameOptionsDefined_ContainerValueDoesNotMatch_GetBestMatchingNameFor_RegexNameOption_PropertyNameDoesNotMatchPattern_ThrowsInvalidOperationsException()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.StaticRegexNameContainerElement));
            var context = VgXmlPropertyContext.CreateContext(property);

            _testObj.StaticRegexNameContainerElement.TagName = "NonMatchingName";

            Assert.That(() => context.GetBestMatchingNameFor(_testObj.StaticRegexNameContainerElement), Throws.InvalidOperationException);
        }
    }
}
