using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VeryGoodXml
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VgXmlRegexNameAttribute : Attribute, IVgXmlNameMatcher
    {
        public string Pattern { get; private set; }
        public RegexOptions Options { get; private set; }

        //Default options set to the default for the Regex class
        //https://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regex.options(v=vs.110).aspx
        public VgXmlRegexNameAttribute(string pattern, RegexOptions options = RegexOptions.None)
        {
            Pattern = pattern;
            Options = options;
        }

        public bool IsMatch(string name) => Regex.IsMatch(name, Pattern, Options);
    }
}
