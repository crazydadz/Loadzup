using System;
using System.Collections.Generic;
using System.Net;
using Silphid.Extensions;
using Silphid.Loadzup.Http;
using UniRx;
using UnityEngine;

namespace Silphid.Loadzup.Caching
{
    public class CachedRequester : IRequester
    {
        private readonly IRequester _innerRequester;
        private readonly ICache _cache;

        public CachedRequester(IRequester innerRequester, ICache cache)
        {
            _innerRequester = innerRequester;
            _cache = cache;
        }

        public IObservable<Response> Request(Uri uri, Options options = null)
        {
            var policy = options?.CachePolicy ?? CachePolicy.CacheOtherwiseOrigin;

            if (policy == CachePolicy.OriginOnly)
                return LoadFromOrigin(policy, uri, options);

            var responseHeaders = _cache.LoadHeaders(uri);
            if (responseHeaders != null)
            {
                // Cached
                Debug.Log($"#Loadzup# {policy} - Resource found in cache: {uri}");

                if (policy == CachePolicy.CacheOnly || policy == CachePolicy.CacheOtherwiseOrigin)
                    return LoadFromCache(policy, uri, responseHeaders);

                if (policy == CachePolicy.OriginOtherwiseCache)
                    return LoadFromOrigin(policy, uri, options)
                        .Catch<Response, WWWErrorException>(ex =>
                        {
                            Debug.Log($"#Loadzup# {policy} - Failed to retrieve {uri} from origin (error: {ex}), falling back to cached version.");
                            return LoadFromCache(policy, uri, responseHeaders);
                        });

                if (policy == CachePolicy.CacheThenOrigin)
                    return LoadFromCacheThenOrigin(policy, uri, options, responseHeaders);

                if (policy == CachePolicy.OriginIfETagOtherwiseCache || policy == CachePolicy.CacheThenOriginIfETag)
                    return LoadWithETag(policy, uri, options, responseHeaders);

                if (policy == CachePolicy.OriginIfLastModifiedOtherwiseCache || policy == CachePolicy.CacheThenOriginIfLastModified)
                    return LoadWithLastModified(policy, uri, options, responseHeaders);
            }
            else
            {
                // Not cached
                Debug.Log($"#Loadzup# {policy} - Resource not found in cache: {uri}");

                if (policy == CachePolicy.CacheOnly)
                    return Observable.Throw<Response>(new InvalidOperationException($"Policy is {policy} but resource not found in cache"));
            }

            return LoadFromOrigin(policy, uri, options);
        }

        private IObservable<Response> LoadWithETag(CachePolicy policy, Uri uri, Options options, Dictionary<string, string> responseHeaders)
        {
            var eTag = responseHeaders.GetOptionalValue(KnownHttpHeaders.ETag);
            if (eTag == null)
                return LoadFromCacheThenOrigin(policy, uri, options, responseHeaders);

            options.RequestHeaders[KnownHttpHeaders.IfNoneMatch] = eTag;

            if (policy == CachePolicy.OriginIfETagOtherwiseCache)
                return LoadFromOrigin(policy, uri, options)
                    .Catch<Response, WWWErrorException>(ex =>
                    {
                        Debug.Log(
                            $"#Loadzup# {policy} - Failed to retrieve {uri} from origin (error: {ex}), falling back to cached version.");
                        return LoadFromCache(policy, uri, responseHeaders);
                    });

            // CacheThenOriginIfETag
            return LoadFromCacheThenOrigin(policy, uri, options, responseHeaders);
        }

        private IObservable<Response> LoadWithLastModified(CachePolicy policy, Uri uri, Options options, Dictionary<string, string> responseHeaders)
        {
            var lastModified = responseHeaders.GetOptionalValue(KnownHttpHeaders.LastModified);
            if (lastModified == null)
                return LoadFromCacheThenOrigin(policy, uri, options, responseHeaders);

            options.RequestHeaders[KnownHttpHeaders.IfModifiedSince] = lastModified;

            if (policy == CachePolicy.OriginIfETagOtherwiseCache)
                return LoadFromOrigin(policy, uri, options)
                    .Catch<Response, WWWErrorException>(ex =>
                    {
                        Debug.Log(
                            $"#Loadzup# {policy} - Failed to retrieve {uri} from origin (error: {ex}), falling back to cached version.");
                        return LoadFromCache(policy, uri, responseHeaders);
                    });

            // CacheThenOriginIfLastModified
            return LoadFromCacheThenOrigin(policy, uri, options, responseHeaders);
        }

        private IObservable<Response> LoadFromCacheThenOrigin(CachePolicy policy, Uri uri, Options options, Dictionary<string, string> responseHeaders)
        {
            return LoadFromCache(policy, uri, responseHeaders)
                .Concat(Observable.Defer(() => LoadFromOrigin(policy, uri, options)
                    .Catch<Response, WWWErrorException>(ex => (ex.StatusCode == HttpStatusCode.NotModified)
                        ? Observable.Empty<Response>()
                        : Observable.Throw<Response>(ex))));
        }

        private IObservable<Response> LoadFromCache(CachePolicy policy, Uri uri, Dictionary<string, string> responseHeaders)
        {
            Debug.Log($"#Loadzup# {policy} - Loading resource from cache: {uri}");
            return Observable.Return(new Response(() => _cache.Load(uri), () => null, responseHeaders));
        }

        private IObservable<Response> LoadFromOrigin(CachePolicy policy, Uri uri, Options options)
        {
            Debug.Log($"#Loadzup# {policy} - Loading resource from origin: {uri}");
            return _innerRequester
                .Request(uri, options)
                .Do(x => _cache.Save(uri, x.Bytes, x.Headers));
        }
    }
}