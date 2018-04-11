namespace Vostok.Logging.Configuration.Parsing
{
    internal interface IInlineParser
    {
        bool TryParse(string value, out object result);
    }
}