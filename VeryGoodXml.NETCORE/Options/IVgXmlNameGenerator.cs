namespace VeryGoodXml
{
    public interface IVgXmlNameGenerator : IVgXmlEntityOption
    {
        string GenerateName(object target);
    }
}
