using System.Xml;

namespace VeryGoodXml
{
    public interface IVgXmlSerializerContext
    {
        XmlReader Reader { get; }
        VgXmlSerializer Serializer { get; }
        XmlWriter Writer { get; }
    }
}