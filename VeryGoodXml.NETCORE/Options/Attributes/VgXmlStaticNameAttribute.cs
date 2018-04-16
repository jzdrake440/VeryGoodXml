using System;
using System.Collections.Generic;
using System.Text;

namespace VeryGoodXml
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VgXmlStaticNameAttribute : Attribute, IVgXmlNameGenerator, IVgXmlNameMatcher
    {
        public string Name { get; private set; }
        private StringComparison _comparisonType;

        public VgXmlStaticNameAttribute(string name, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            Name = name;
            _comparisonType = comparisonType;
        }

        public string GenerateName(object target) => Name;
        public bool IsMatch(string name) => Name.Equals(name, _comparisonType);
    }
}
