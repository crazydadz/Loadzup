using System;
using System.Collections.Generic;
using System.Net.Mime;
using Silphid.Extensions;

namespace Silphid.Loadzup
{
    public class Response
    {
        private readonly Func<byte[]> _bytesFunc;
        private readonly Func<string> _textFunc;
        private ContentType _contentType;

        public byte[] Bytes => _bytesFunc();
        public string Text => _textFunc();
        public Dictionary<string, string> Headers;

        public Response(Func<byte[]> bytesFunc, Func<string> textFunc, Dictionary<string, string> headers)
        {
            _bytesFunc = bytesFunc;
            _textFunc = textFunc;
            Headers = headers;
        }

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