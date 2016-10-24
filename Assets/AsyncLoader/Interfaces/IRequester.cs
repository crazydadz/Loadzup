using System;
using UniRx;

namespace Silphid.AsyncLoader
{
    public interface IRequester
    {
         IObservable<Response> Request(Uri uri, Options options = null);
    }
}