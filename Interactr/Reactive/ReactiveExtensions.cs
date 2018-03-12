using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Reactive
{
    public static class ReactiveExtensions
    {
        /// <summary>
        /// Take IObservable&lt;T&gt;, project each element of type T on an IObservable&lt;R&gt; 
        /// and return the values of type R emitted by the most recently emitted T.
        /// If a null value is encountered for T, this value is simply ignored.
        /// </summary>
        /// <typeparam name="T">The source observable type</typeparam>
        /// <typeparam name="R">The observable type nested inside T</typeparam>
        /// <param name="obs">The source observable</param>
        /// <param name="subObsSelector">Projection function of T onto an IObservable&lt;R&gt;</param>
        /// <returns>A new observable of type R.</returns>
        public static IObservable<R> ObserveNested<T, R>(this IObservable<T> obs, Func<T, IObservable<R>> subObsSelector)
        {
            return obs.Where(t => t != null).Select(subObsSelector).Switch();
        }

        /// <summary>
        /// Create a new list that mirrors the contents of the source list (automatically handles addition and removal of elements), 
        /// but maps each element to a different function using a selectorFunc.
        /// </summary>
        /// <typeparam name="TInput">The type of elements in the source list.</typeparam>
        /// <typeparam name="TResult">The type of elements in the target list.</typeparam>
        /// <param name="sourceList">The list from which to take elements.</param>
        /// <param name="selectorFunc">The function that maps elements to a different type.</param>
        /// <returns>Returns the resulting list and an IDisposable that can be used to stop the updating of the derived list.</returns>
        public static (IDisposable Binding, ReactiveList<TResult> ResultList) CreateDerivedList<TInput, TResult>
            (this ReactiveList<TInput> sourceList, Func<TInput, TResult> selectorFunc)
        {
            return CreateDerivedListBinding(Observable.Return(sourceList), selectorFunc);
        }

        /// <summary>
        /// Create a new list that mirrors the contents of the latest source list of the observable,
        /// but maps each element to a different function using a selectorFunc.
        /// Automatically handles addition and removal of list elements, as well as (un-)subscribing to list events.
        /// </summary>
        /// <typeparam name="TInput">The type of elements in the source lists.</typeparam>
        /// <typeparam name="TResult">The type of elements in the target list.</typeparam>
        /// <param name="sourceListObservable">An observable of reactive lists from which to take elements.</param>
        /// <param name="selectorFunc">The function that maps elements to a different type.</param>
        /// <returns>Returns the resulting list and an IDisposable that can be used to stop the updating of the derived list.</returns>
        public static (IDisposable Binding, ReactiveList<TResult> ResultList) CreateDerivedListBinding<TInput, TResult>
            (this IObservable<ReactiveList<TInput>> sourceListObservable, Func<TInput, TResult> selectorFunc)
        {
            ReactiveList<TResult> targetList = new ReactiveList<TResult>();

            CompositeDisposable disposable = new CompositeDisposable(
                sourceListObservable.Subscribe(newList =>
                {
                    targetList.Clear();
                    targetList.AddRange(newList.Select(selectorFunc));
                }),
                sourceListObservable.ObserveNested(list => list.OnAdd)
                    .Select(added => (Index: added.Index, Element: selectorFunc(added.Element)))
                    .Subscribe(mapped => targetList.Insert(mapped.Index, mapped.Element)),
                sourceListObservable.ObserveNested(list => list.OnDelete).Subscribe(e => targetList.RemoveAt(e.Index))
            );
            return (disposable, targetList);
        }
    }
}
