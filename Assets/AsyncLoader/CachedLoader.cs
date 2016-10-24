using System;
using System.Collections.Generic;
using UniRx;

namespace Silphid.AsyncLoader
{
    public class CachedLoader : ILoader
    {
        protected readonly ILoader _innerLoader;
        protected readonly Dictionary<Uri, object> _cache = new Dictionary<Uri, object>();

        public CachedLoader(ILoader innerLoader)
        {
            _innerLoader = innerLoader;
        }

        public bool Supports(Uri uri) =>
            _innerLoader.Supports(uri);

        public IObservable<T> Load<T>(Uri uri, Options options = null)
        {
            object obj;
            if (_cache.TryGetValue(uri, out obj))
                return Observable.Return((T) obj);

            return _innerLoader
                .Load<T>(uri, options)
                .Do(x => _cache[uri] = x);
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}