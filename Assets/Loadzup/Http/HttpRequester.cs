using System;
using System.Collections.Generic;
using System.Linq;
using Silphid.Extensions;
using UniRx;

namespace Silphid.Loadzup.Http
{
    public class HttpRequester : IRequester
    {
        private static readonly string[] MeaningfulHeaders =
        {
            KnownHttpHeaders.ContentType,
            KnownHttpHeaders.LastModified,
            KnownHttpHeaders.ETag,
        };

        public IObservable<Response> Request(Uri uri, Options options = null) =>
            ObservableWWW
                .GetWWW(uri.AbsoluteUri, options?.RequestHeaders)
                .Select(www => new Response(() => www.bytes, () => www.text, GetMeaningfulHeaders(www.responseHeaders)));

        private Dictionary<string, string> GetMeaningfulHeaders(IDictionary<string, string> allHeaders)
        {
            return MeaningfulHeaders
                .Select(x => new KeyValuePair<string, string>(x, allHeaders.GetOptionalValue(x)))
                .Where(x => x.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}