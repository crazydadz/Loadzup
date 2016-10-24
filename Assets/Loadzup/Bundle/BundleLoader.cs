﻿using System;
using UniRx;

namespace Silphid.Loadzup.Bundle
{
    public class BundleLoader : ILoader
    {
        public const string Scheme = "bundle";

        public bool Supports(Uri uri) =>
            uri.Scheme == Scheme;

        public IObservable<T> Load<T>(Uri uri, Options options = null)
        {
            throw new NotImplementedException();
        }
    }
}