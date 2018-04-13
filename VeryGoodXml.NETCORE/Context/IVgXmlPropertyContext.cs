using System;
using System.Collections.Generic;
using System.Reflection;
using VeryGoodXml.ContentFactories;
using VeryGoodXml.Entities.Enumerations;
using VeryGoodXml.Entities.Names;

namespace VeryGoodXml.Context
{
    public interface IVgXmlPropertyContext
    {
        PropertyInfo Property { get; }
        VgXmlEntityType EntityType { get; }
        IEnumerable<VgXmlName> GetNames();
        bool NameMatches(string name);
        string GetBestMatchingNameFor(object target);
        bool IsCollection();
        IVgXmlContentFactory GetContentFactory();

        //proxy methods for PropertyInfo
        Type PropertyType { get; }
    }


}
