using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace Silphid.AsyncLoader
{
    public class CompositeConverter : IConverter
    {
        private readonly List<IConverter> _children;

        public CompositeConverter(List<IConverter> children)
        {
            _children = children;
        }

        public bool Supports<T>(object obj, ContentType contentType) =>
            _children.Any(x => x.Supports<T>(obj, contentType));

        public T Convert<T>(object obj, ContentType contentType)
        {
            var child = _children.FirstOrDefault(x => x.Supports<T>(obj, contentType));
            if (child == null)
                throw new NotSupportedException($"Conversion not supported from {obj.GetType().Name} with content type {contentType} to {typeof(T).Name}.");

            return child.Convert<T>(obj, contentType);
        }
    }
}