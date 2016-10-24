using System.Net.Mime;

namespace Silphid.AsyncLoader
{
    public static class IConverterExtensions
    {
        public static T Convert<T>(this IConverter This, string text, byte[] bytes, ContentType contentType) =>
            This.Supports<T>(text, contentType)
                ? This.Convert<T>(text, contentType)
                : This.Convert<T>(bytes, contentType);
    }
}