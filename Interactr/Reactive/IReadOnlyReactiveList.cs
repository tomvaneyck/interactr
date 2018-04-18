using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Interactr.Reactive
{
    /// <summary>
    /// A read-only view of a list with observables for monitoring changes.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the list.</typeparam>
    public interface IReadOnlyReactiveList<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Observable that emits any item that is added to the list.
        /// The element is emitted after it is added.
        /// </summary>
        IObservable<(T Element, int Index)> OnAdd { get; }

        /// <summary>
        /// Observable that emits any item that is remove from the list.
        /// The element is emitted after it is removed.
        /// </summary>
        IObservable<(T Element, int Index)> OnDelete { get; }
    }

    public static class ReadOnlyReactiveListExtensions
    {
        /// <summary>
        /// Apply the specified observableSelector to every item that is added to the list,
        /// and automatically unsubscribes the resulting observable when the item is removed from the list.
        /// </summary>
        /// <typeparam name="V">The value produced by the observable returned by observableSelector.</typeparam>
        /// <param name="observableSelector">A function that maps each element on an observable.</param>
        /// <returns>An observable of the elements that are emitted along with the item that produced it.</returns>
        public static IObservable<(T Element, V Value)> ObserveEach<T, V>(this IReadOnlyReactiveList<T> list, Func<T, IObservable<V>> observableSelector)
        {
            return list.ObserveWhere(observableSelector, t => true);
        }

        /// <summary>
        /// Apply the specified observableSelector to every item that is added to the list and matches filter,
        /// and automatically unsubscribes the resulting observable when the item is removed from the list.
        /// </summary>
        /// <typeparam name="V">The value produced by the observable returned by observableSelector.</typeparam>
        /// <param name="observableSelector">A function that maps each matching element on an observable.</param>
        /// <param name="filter">A predicate that specifies whether or not this specific element should be observed</param>
        /// <returns>An observable of the elements that are emitted along with the item that produced it.</returns>
        public static IObservable<(T Element, V Value)> ObserveWhere<T, V>(this IReadOnlyReactiveList<T> list, Func<T, IObservable<V>> observableSelector, Func<T, bool> filter)
        {
            if (observableSelector == null)
            {
                throw new ArgumentNullException(nameof(observableSelector));
            }
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            // Take all items that are currently in the list (values and corresponding index), 
            // including all that will be added in the future.
            var currentContents = Observable.Zip(list.ToObservable(), Observable.Range(0, list.Count), (e, i) => (e, i));
            IObservable<(T Element, int Index)> items = currentContents.Concat(list.OnAdd);

            // Select the target observable using observableSelector and return
            // values from it until the item is removed from this list.
            return items.Where(e => filter(e.Element)).SelectMany(newElem =>
                observableSelector(newElem.Element)
                    .TakeUntil(
                        list.OnDelete.Where(deletedElem => Object.Equals(deletedElem, newElem))
                    )
                    .Select(val => (newElem.Element, val))
            );
        }
    }
}
