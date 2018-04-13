namespace VeryGoodXml.Context
{
    public interface IVgXmlTargetedPropertyContext : IVgXmlPropertyContext
    {
        object Target { get; }

        void SetValue(object value);
        object GetValue();
        void AddToCollection(object value);
    }


}
