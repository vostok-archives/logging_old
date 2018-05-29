namespace Vostok.Logging.Core.Configuration.Parsing
{
    internal interface IInlineParser
    {
        bool TryParse(string value, out object result);
    }
}