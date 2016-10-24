using System.Net.Mime;

namespace Silphid.AsyncLoader
{
    public interface IConverter
    {
        bool Supports<T>(object obj, ContentType contentType);
        T Convert<T>(object obj, ContentType contentType);
    }
}
