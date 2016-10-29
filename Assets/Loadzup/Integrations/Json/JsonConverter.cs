using System.Net.Mime;
using Newtonsoft.Json;

namespace Silphid.Loadzup.Json
{
    public class JsonConverter : IConverter
    {
        public bool Supports<T>(object data, ContentType contentType) =>
            data is string && contentType.MediaType == KnownMediaType.ApplicationJson;

        public T Convert<T>(object data, ContentType contentType) =>
            (T)JsonConvert.DeserializeObject((string)data, typeof(T));
    }
}