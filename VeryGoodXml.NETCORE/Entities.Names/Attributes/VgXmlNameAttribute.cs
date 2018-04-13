using System;
using System.Collections.Generic;
using System.Text;
using VeryGoodXml.Entities.Names.Enumerations;

namespace VeryGoodXml.Entities.Names.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VgXmlNameAttribute : Attribute
    {
        public string Value { get; private set; }
        public VgXmlNameType Type { get; private set; }

        public VgXmlNameAttribute(string value, VgXmlNameType type = VgXmlNameType.Static)
        {
            Value = value;
            Type = type;
        }

        public VgXmlName ToVgXmlName()
        {
            return VgXmlName.Build(Value, Type);
        }
    }
}
