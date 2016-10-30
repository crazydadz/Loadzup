using System.Collections.Generic;
using System.Net.Mime;
using Silphid.Loadzup.Caching;

namespace Silphid.Loadzup
{
    public class Options
    {
        public ContentType ContentType;
        public CachePolicy? CachePolicy;
        public Dictionary<string, string> RequestHeaders;
    }
}