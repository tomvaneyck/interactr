using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Reactive
{
    public static class ReactiveExtensions
    {
        /// <summary>
        /// Takes IObservable&lt;T&gt;, projects each element of type T on an IObservable&lt;R&gt; 
        /// and returns the values of type R emitted by the most recently emitted T.
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
    }
}
