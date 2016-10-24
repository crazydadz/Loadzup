using System.Collections.Generic;
using System.Net.Mime;

namespace Silphid.Loadzup
{
    public class Options
    {
        public ContentType ContentType;
        public CachePolicy? CachePolicy;
        public Dictionary<string, string> RequestHeaders;
    }
}