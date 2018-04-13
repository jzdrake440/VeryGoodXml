using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using VeryGoodXml.Context;
using VeryGoodXml.Serializers;

namespace VeryGoodXml.ContentFactories
{
    public interface IVgXmlContentFactory
    {
        void ReadIntoProperty(IVgXmlSerializerContext sContext, IVgXmlTargetedPropertyContext pContext);
        void WriteFromProperty(IVgXmlSerializerContext sContext, IVgXmlTargetedPropertyContext pContext);
    }
}
