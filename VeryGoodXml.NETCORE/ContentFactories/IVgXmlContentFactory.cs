namespace VeryGoodXml
{
    public interface IVgXmlContentFactory
    {
        void ReadIntoProperty(IVgXmlSerializerContext sContext, IVgXmlTargetedPropertyContext pContext);
        void WriteFromProperty(IVgXmlSerializerContext sContext, IVgXmlTargetedPropertyContext pContext);
    }
}
