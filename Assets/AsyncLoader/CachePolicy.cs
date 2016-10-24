namespace Silphid.AsyncLoader
{
    public enum CachePolicy
    {
        /// <summary>
        /// Specifies that cache should be checked for given resource first, and only if not found there should it be
        /// loaded from its original location.  This is the fastest approach, but does not allow updates on the server
        /// to be retrieved.
        /// </summary>
        TryCacheThenOrigin,


        /// <summary>
        /// Specifies that cache should be checked only after resource failed to download from its original location.
        /// This is typically used for offline scenarios, where latest version of resource should ideally be downloaded,
        /// but application wants to fallback to version in cache if connectivity is not available.  This approach
        /// provides no speed gain, only offline support, and allows updates on the server to be retrieved.
        /// </summary>
        TryOriginThenCache,

        /// <summary>
        /// Specifies that cached resource should only be used if its ETag is the same as the one in content headers
        /// retrieved from original location.  Otherwise, latest resource is retrieved completely and cache gets
        /// updated.  If content headers failed to be retrieved, cache will be used as fallback, as for
        /// UseCacheAsFallback.  This approach is slightly slower than UseCacheAsFallback, because server is always
        /// queried for headers, but is a reasonable compromise in terms of performance in order to allow updates on the
        /// server to be retrieved. Note that, for retrieving many small files, the overhead of retrieving headers can
        /// become quite taxing.
        /// </summary>
        CheckETag,

        /// <summary>
        /// Specifies that system should first check with server for a more recent version of the resource, based
        /// on its last modified date
        /// cached resource should only be used if its last modified date is the same as the one in
        /// content headers retrieved from original location.  Otherwise, latest resource is retrieved completely and
        /// cache gets updated.  If content headers failed to be retrieved, cache will be used as fallback, as for
        /// UseCacheAsFallback.  This approach is slightly slower than UseCacheAsFallback, because server is always
        /// queried for headers, but is a reasonable compromise in terms of performance in order to allow updates on the
        /// server to be retrieved. Note that, for retrieving many small files, the overhead of retrieving headers can
        /// become quite taxing.
        /// </summary>
        CheckLastModified
    }
}