using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using VeryGoodXml.Entities.Names.Enumerations;

namespace VeryGoodXml.Entities.Names
{
    public class VgXmlRegexName : VgXmlName
    {
        public VgXmlRegexName(string value) : base(value) { }

        public override VgXmlNameType Type => VgXmlNameType.Regex;

        public override string GetName(object obj)
        {
            return null;
        }

        public override bool IsNameMatch(string name)
        {
            return Regex.IsMatch(name, Value);
        }
    }
}
