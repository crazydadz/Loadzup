using System;
using UniRx;
using UniRx.Operators;

namespace Silphid.Extensions
{
    public static class IObservableExtensions
    {
        public static IObservable<TRet> Then<T, TRet>(this IObservable<T> observable, Func<IObservable<TRet>> selector) =>
            observable.AsSingleUnitObservable().ContinueWith(_ => selector());

        public static IObservable<TRet> Then<T, TRet>(this IObservable<T> observable, IObservable<TRet> other) =>
            observable.AsSingleUnitObservable().ContinueWith(_ => other);

        public static IObservable<TRet> ThenReturn<T, TRet>(this IObservable<T> observable, TRet value) =>
            observable.Then(() => Observable.Return(value));

        internal class AutoDetachObservable<T> : OperatorObservableBase<T>
        {
            private readonly IObservable<T> _source;

            public AutoDetachObservable(IObservable<T> source)
                : base(source.IsRequiredSubscribeOnCurrentThread())
            {
                _source = source;
            }

            protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel) =>
                _source.Subscribe(Observer.CreateAutoDetachObserver(observer, cancel));
        }

        public static IObservable<T> AutoDetach<T>(this IObservable<T> This) =>
            new AutoDetachObservable<T>(This);

        public static IDisposable SubscribeCompletion<T>(this IObservable<T> This, Action onCompleted) =>
            This.AutoDetach().Subscribe(Observer.Create<T>(_ => {}, ex => { throw ex; }, onCompleted));

        public static IObservable<bool> WhereTrue(this IObservable<bool> This) =>
            This.Where(x => x);

        public static IObservable<bool> WhereFalse(this IObservable<bool> This) =>
            This.Where(x => !x);

        public static IObservable<bool> Negate(this IObservable<bool> This) =>
            This.Select(x => !x);

        public static IObservable<Tuple<TSource, TSource>> PairWithPrevious<TSource>(this IObservable<TSource> source)
            =>
            source.Scan(
                Tuple.Create(default(TSource), default(TSource)),
                (acc, current) => Tuple.Create(acc.Item2, current));
    }
}