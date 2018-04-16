namespace VeryGoodXml
{
    public interface IVgXmlNameMatcher : IVgXmlEntityOption
    {
        bool IsMatch(string name);
    }
}
