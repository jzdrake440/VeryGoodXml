using System;
using System.Collections.Generic;
using System.Text;
using VeryGoodXml.Entities.Names.Enumerations;

namespace VeryGoodXml.Entities.Names
{
    public class VgXmlStaticName : VgXmlName
    {
        public VgXmlStaticName(string value) : base(value) { }

        public override VgXmlNameType Type => VgXmlNameType.Static;

        public override string GetName(object obj)
        {
            return Value;
        }

        public override bool IsNameMatch(string name)
        {
            return Value.Equals(name);
        }
    }
}
