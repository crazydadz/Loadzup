using System.Collections.Generic;
using System.Net.Mime;

namespace Silphid.AsyncLoader
{
    public class Options
    {
        public ContentType ContentType;
        public CachePolicy? CachePolicy;
        public Dictionary<string, string> RequestHeaders;
    }
}