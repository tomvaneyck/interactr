﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

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
        public static (IDisposable Binding, IReadOnlyReactiveList<TResult> ResultList) CreateDerivedList<TInput, TResult>(
            this IReadOnlyReactiveList<TInput> sourceList, 
            Func<TInput, TResult> selectorFunc, 
            Func<TInput, bool> filterFunc = null
        )
        {
            return CreateDerivedListBinding(Observable.Return(sourceList), selectorFunc, filterFunc);
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
        public static (IDisposable Binding, IReadOnlyReactiveList<TResult> ResultList) CreateDerivedListBinding<TInput, TResult>(
            this IObservable<IReadOnlyReactiveList<TInput>> sourceListObservable, 
            Func<TInput, TResult> selectorFunc, 
            Func<TInput, bool> filterFunc = null
        )
        {
            ReactiveList<TResult> targetList = new ReactiveArrayList<TResult>();
            IList<bool> isFilteredIn = new List<bool>();

            void InsertElement(int index, TInput element)
            {
                if (filterFunc == null)
                {
                    targetList.Insert(index, selectorFunc(element));
                }
                else
                {
                    if (filterFunc(element))
                    {
                        isFilteredIn.Insert(index, true);
                        int indexInTargetList = SourceListIndexToTargetListIndex(index);
                        targetList.Insert(indexInTargetList, selectorFunc(element));
                    }
                    else
                    {
                        isFilteredIn.Insert(index, false);
                    }
                }
            }

            void RemoveElement(int index)
            {
                if (filterFunc == null)
                {
                    targetList.RemoveAt(index);
                }
                else
                {
                    int indexInList = isFilteredIn[index] ? SourceListIndexToTargetListIndex(index) : -1;
                    isFilteredIn.RemoveAt(index);
                    if (indexInList != -1)
                    {
                        targetList.RemoveAt(indexInList);
                    }
                }
            }

            int SourceListIndexToTargetListIndex(int sourceIndex)
            {
                return isFilteredIn.Take(sourceIndex + 1).Count(isIn => isIn) - 1;
            }

            CompositeDisposable disposable = new CompositeDisposable(
                sourceListObservable.Subscribe(newList =>
                {
                    targetList.Clear();
                    for (int i = 0; i < newList.Count; i++)
                    {
                        InsertElement(i, newList[i]);
                    }
                }),
                sourceListObservable.ObserveNested(list => list.OnAdd)
                    .Subscribe(e => InsertElement(e.Index, e.Element)),
                sourceListObservable.ObserveNested(list => list.OnDelete)
                    .Subscribe(e => RemoveElement(e.Index))
            );
            return (disposable, targetList);
        }
    }
}