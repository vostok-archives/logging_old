namespace Vostok.Logging.Tests
{
    internal interface IInlineParser
    {
        bool TryParse(string value, out object result);
    }
}