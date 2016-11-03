using System;
using System.IO;
using System.Net.Mime;
using Silphid.Extensions;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silphid.Loadzup.Resource
{
    public class ResourceLoader : ILoader
    {
        public const string Scheme = "res";

        private readonly IConverter _converter;

        public ResourceLoader(IConverter converter)
        {
            _converter = converter;
        }

        public bool Supports(Uri uri) =>
            uri.Scheme == Scheme;

        public IObservable<T> Load<T>(Uri uri, Options options)
        {
            var contentType = options?.ContentType;
            var path = GetPathAndContentType(uri, ref contentType);

            return Resources
                .LoadAsync<Object>(path)
                .AsObservable<Object>()
                .Select(x => Convert<T>(x, contentType))
                .DoOnCompleted(() => Debug.Log($"#Loadzup# Asset '{path}' loaded from resources."))
                .DoOnError(error => Debug.LogError($"#Loadzup# Failed to load asset '{path}' from resources: {error}"));
        }

        private string GetPathAndContentType(Uri uri, ref ContentType contentType)
        {
            // Rebuild path while removing scheme component
            var path = string.IsNullOrEmpty(uri.AbsolutePath.RemovePrefix("/"))
                ? uri.Host.RemovePrefix("/")
                : uri.Host.RemovePrefix("/") + uri.AbsolutePath;

            // Any extension detected?
            var extension = Path.GetExtension(path);
            if (extension.IsNullOrWhiteSpace())
                return path;

            // Remove extension, because Unity doesn't expect it when looking up resources
            path = path.Left(path.LastIndexOf(".", StringComparison.Ordinal));

            if (contentType == null)
            {
                // Try to determine content type from extension
                var mediaType = KnownMediaType.FromExtension(extension);
                if (mediaType != null)
                    contentType = new ContentType(mediaType);
            }

            return path;
        }

        private T Convert<T>(Object obj, ContentType contentType)
        {
            if (obj is T)
                return (T) (object) obj;

            if (obj is TextAsset)
            {
                var textAsset = (TextAsset) obj;
                return _converter.Convert<T>(textAsset.text, textAsset.bytes, contentType);
            }

            throw new NotSupportedException("Conversion not supported from {obj.GetType().Name} to {typeof(T).Name}.");
        }
    }
}