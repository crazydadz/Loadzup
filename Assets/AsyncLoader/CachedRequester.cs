using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Silphid.Extensions;
using UniRx;
using UnityEngine;

namespace Silphid.AsyncLoader
{
    public class CachedRequester : IRequester
    {
        private readonly IRequester _innerRequester;

        public CachedRequester(IRequester innerRequester)
        {
            _innerRequester = innerRequester;
        }

        public IObservable<Response> Request(Uri uri, Options options = null)
        {
            var cachePolicy = options?.CachePolicy ?? CachePolicy.TryCacheThenOrigin;
            var cacheFile = GetCacheFile(uri);
            var headers = GetHeadersFromCache(cacheFile);
            if (headers != null)
            {
                if (cachePolicy == CachePolicy.TryCacheThenOrigin)
                {
                    Debug.Log($"#AsyncLoader# Returning cached version of resource {uri} (policy: {cachePolicy})");
                    return GetFromCache(cacheFile, headers);
                }

                if (cachePolicy == CachePolicy.TryOriginThenCache)
                {
                    Debug.Log($"#AsyncLoader# Resource exists in cache, but first trying to retrieve it from {uri} (policy: {cachePolicy})");
                    return Request(uri, cacheFile)
                        .Catch<Response, Exception>(ex =>
                        {
                            Debug.Log($"#AsyncLoader# Failed to retrieve {uri}, falling back to cached version.");
                            return GetFromCache(cacheFile, headers);
                        });
                }

                if (cachePolicy == CachePolicy.CheckETag || cachePolicy == CachePolicy.CheckLastModified)
                    throw new NotImplementedException();
            }

            Debug.Log($"#AsyncLoader# Resource not found in cache, trying to retrieve it from {uri} (policy: {cachePolicy})");
            return Request(uri, cacheFile);
        }

        private IObservable<Response> GetFromCache(string cacheFile, Dictionary<string, string> headers)
        {
            return Observable.Return(
                new Response
                {
                    Bytes = GetBytesFromCache(cacheFile),
                    Headers = headers
                });
        }

        private IObservable<Response> Request(Uri uri, string cacheFile) =>
            _innerRequester
                .Request(uri)
                .Do(x => SaveToCache(cacheFile, x.Bytes, x.Headers));

        private string GetCacheFile(Uri uri) =>
            GetCacheDir() + Path.DirectorySeparatorChar + GetEscapedFileName(uri);

        private string GetHeadersFile(string dataFile) =>
            dataFile + ".Headers";

        private void SaveToCache(string cacheFile, byte[] bytes, IDictionary<string, string> headers)
        {
            File.WriteAllBytes(cacheFile, bytes);
            File.WriteAllLines(GetHeadersFile(cacheFile), headers.Select(x => $"{x.Key}: {x.Value}").ToArray());
        }

        private Dictionary<string, string> GetHeadersFromCache(string cacheFile)
        {
            if (!File.Exists(GetHeadersFile(cacheFile)))
                return null;

            return (from line in File.ReadAllLines(GetHeadersFile(cacheFile))
                    let separatorIndex = GetSeparatorIndex(line)
                    select new
                    {
                        Key = line.Left(separatorIndex).Trim(),
                        Value = line.Substring(separatorIndex + 2).Trim()
                    })
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private int GetSeparatorIndex(string line)
        {
            var index = line.IndexOf(": ", StringComparison.InvariantCulture);
            if (index == -1)
                throw new InvalidOperationException("Malformed cache headers file");
            return index;
        }

        private byte[] GetBytesFromCache(string cacheFile) =>
            File.ReadAllBytes(cacheFile);

        private string GetCacheDir()
        {
            var path = Application.temporaryCachePath + Path.DirectorySeparatorChar + "Silphid.AsyncLoader.Cache";
            Debug.Log($"Cache path: {path}");
            Directory.CreateDirectory(path);
            return path;
        }

        private string GetEscapedFileName(Uri uri)
        {
            var invalidCharacters = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + ":");
            var regex = new Regex($"[{invalidCharacters}]");
            return regex.Replace(uri.AbsoluteUri, "_");
        }

        public void ClearCache()
        {
            Directory
                .GetFiles(GetCacheDir())
                .ForEach(File.Delete);
        }
    }
}