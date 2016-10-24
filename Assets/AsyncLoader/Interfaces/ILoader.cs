using System;
using UniRx;

namespace Silphid.AsyncLoader
{
    public interface ILoader
    {
        bool Supports(Uri uri);
        IObservable<T> Load<T>(Uri uri, Options options = null);
    }
}