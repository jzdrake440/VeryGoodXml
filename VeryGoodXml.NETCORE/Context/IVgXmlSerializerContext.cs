using System.Xml;
using VeryGoodXml.Serializers;

namespace VeryGoodXml.Context
{
    public interface IVgXmlSerializerContext
    {
        XmlReader Reader { get; }
        VgXmlSerializer Serializer { get; }
        XmlWriter Writer { get; }
    }
}