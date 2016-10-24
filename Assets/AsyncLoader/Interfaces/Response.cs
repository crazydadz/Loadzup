using System.Collections.Generic;
using System.Net.Mime;
using Silphid.Extensions;

namespace Silphid.AsyncLoader
{
    public class Response
    {
        private ContentType _contentType;

        public byte[] Bytes;
        public string Text;
        public Dictionary<string, string> Headers;

        public ContentType ContentType
        {
            get
            {
                if (_contentType != null)
                    return _contentType;

                var str = Headers.GetOptionalValue(KnownHttpHeaders.ContentType);
                if (str != null)
                    _contentType = new ContentType(str);

                return _contentType;
            }
        }
    }
}