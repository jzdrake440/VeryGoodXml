using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VeryGoodXml.Entities.Names.Enumerations;

namespace VeryGoodXml.Entities.Names
{
    public abstract class VgXmlName
    {
        public virtual string Value { get; set; }
        public abstract VgXmlNameType Type { get; }

        public VgXmlName(string value)
        {
            Value = value;
        }

        public abstract bool IsNameMatch(string name);
        public abstract string GetName(object obj);

        public static VgXmlName Build(string value, VgXmlNameType type)
        {
            switch (type)
            {
                case VgXmlNameType.PropertyDefined:
                    return new VgXmlPropertyDefinedName(value);
                case VgXmlNameType.Regex:
                    return new VgXmlRegexName(value);
                case VgXmlNameType.Static:
                    return new VgXmlStaticName(value);
            }

            throw new NotImplementedException(type.ToString());
        }
    }
}
