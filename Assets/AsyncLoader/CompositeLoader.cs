﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UniRx;

namespace Silphid.AsyncLoader
{
    public class CompositeLoader : ILoader
    {
        private readonly List<ILoader> _children;

        public CompositeLoader(List<ILoader> children)
        {
            _children = children;
        }

        public bool Supports(Uri uri) =>
            _children.Any(x => x.Supports(uri));

        public IObservable<T> Load<T>(Uri uri, Options options  = null)
        {
            var child = _children.FirstOrDefault(x => x.Supports(uri));
            if (child == null)
                throw new NotSupportedException($"URI not supported: {uri}");

            return child.Load<T>(uri, options);
        }
    }
}