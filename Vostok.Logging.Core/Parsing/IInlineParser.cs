namespace Vostok.Logging.Core.Parsing
{
    internal interface IInlineParser
    {
        bool TryParse(string value, out object result);
    }
}