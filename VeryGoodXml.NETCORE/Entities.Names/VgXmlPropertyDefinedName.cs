using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VeryGoodXml.Entities.Names.Enumerations;

namespace VeryGoodXml.Entities.Names
{
    public class VgXmlPropertyDefinedName : VgXmlName
    {
        public VgXmlPropertyDefinedName(string value) : base(value) { }

        public override VgXmlNameType Type => VgXmlNameType.PropertyDefined;

        public override string GetName(object obj)
        {
            return GetProperty(obj).GetValue(obj)?.ToString();
        }

        public void SetName(object obj, string name)
        {
            GetProperty(obj).SetValue(obj, name);
        }

        public override bool IsNameMatch(string name)
        {
            return true;
        }

        private PropertyInfo GetProperty(object obj)
        {
            return obj.GetType().GetProperty(Value) ??
                throw new ArgumentException($"{obj.GetType().Name} does not contain property for {Value}");
        }
    }
}
