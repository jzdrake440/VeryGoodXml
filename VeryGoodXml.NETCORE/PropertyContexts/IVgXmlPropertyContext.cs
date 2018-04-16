using System;
using System.Collections.Generic;
using System.Reflection;

namespace VeryGoodXml
{
    public interface IVgXmlPropertyContext
    {
        PropertyInfo Property { get; }
        VgXmlEntityType EntityType { get; }
        IEnumerable<IVgXmlEntityOption> GetOptions();
        IEnumerable<T> GetOptions<T>() where T : IVgXmlEntityOption;
        bool NameMatches(string name);
        string GenerateName(object subEntity);
        string GetBestMatchingNameFor(object target);
        bool IsCollection();
        IVgXmlContentFactory GetContentFactory();

        //proxy methods for PropertyInfo
        Type PropertyType { get; }
    }


}
