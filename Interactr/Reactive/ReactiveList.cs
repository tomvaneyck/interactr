using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Reactive
{
    /// <summary>
    /// An implementation of IList with observables.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the list.</typeparam>
    public class ReactiveList<T> : IList<T>
    {
        private List<T> _contents = new List<T>();

        /// <summary>
        /// Observable that emits any item that is added to the list.
        /// The element is emitted after it is added.
        /// </summary>
        public IObservable<(T Element, int Index)> OnAdd => _onAdd;
        private readonly Subject<(T Element, int Index)> _onAdd = new Subject<(T Element, int Index)>();

        /// <summary>
        /// Observable that emits any item that is remove from the list.
        /// The element is emitted after it is removed.
        /// </summary>
        public IObservable<(T Element, int Index)> OnDelete => _onDelete;
        private readonly Subject<(T Element, int Index)> _onDelete = new Subject<(T Element, int Index)>();

        /// <summary>
        /// Apply the specified observableSelector to every item that is added to the list,
        /// and automatically unsubscribes the resulting observable when the item is removed from the list.
        /// </summary>
        /// <typeparam name="V">The value produced by the observable returned by observableSelector.</typeparam>
        /// <param name="observableSelector">A function that maps each element on an observable.</param>
        /// <returns>An observable of the elements that are emitted along with the item that produced it.</returns>
        public IObservable<(T Element, V Value)> ObserveEach<V>(Func<T, IObservable<V>> observableSelector)
        {
            // Take all items that are currently in the list (values and corresponding index), 
            // including all that will be added in the future.
            var currentContents = Observable.Zip(_contents.ToObservable(), Observable.Range(0, _contents.Count), (e, i) => (e, i));
            IObservable<(T Element, int Index)> items = currentContents.Concat(OnAdd);

            // Select the target observable using observableSelector and return
            // values from it until the item is removed from this list.
            return items.SelectMany(newElem =>
                observableSelector(newElem.Element)
                    .TakeUntil(
                        OnDelete.Where(deletedElem => Object.Equals(deletedElem, newElem))
                    )
                    .Select(val => (newElem.Element, val))
            );
        }


        public T this[int index]
        {
            get => _contents[index];
            set
            {
                // Cache element in item and then replace the element in
                // the list with value
                T item = _contents[index];
                _contents[index] = value;
                // Emit an event that the elements value has changed
                _onDelete.OnNext((item, index));
                _onAdd.OnNext((value, index));
            }
        }

        /// <summary>
        /// Add an item to a list
        /// </summary>
        public void Add(T item)
        {
            _contents.Add(item);
            _onAdd.OnNext((item, _contents.Count-1));
        }

        /// <summary>
        /// Add multiple items to a list.
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            //Clear _contents by making new list but keep items for emitting _onDelete.
            List<T> temp = _contents;
            _contents = new List<T>();

            // Delete the contents items in reverse order.
            for (var i = temp.Count-1; i >= 0; i--)
            {
                T item = temp[i];
                _onDelete.OnNext((item, i));
            }

            temp.Clear();
        }

        /// <inheritdoc cref="IList.Insert"/>
        public void Insert(int index, T item)
        {
            _contents.Insert(index, item);
            _onAdd.OnNext((item, index));
        }

        /// <see cref="IList.Remove"/>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <see cref="IList.RemoveAt"/> 
        public void RemoveAt(int index)
        {
            T item = _contents[index];
            _contents.RemoveAt(index);
            _onDelete.OnNext((item, index));
        }

        #region DefaultImplementations
        public int Count => _contents.Count;

        public bool IsReadOnly => ((ICollection<T>)_contents).IsReadOnly;

        public bool Contains(T item) => _contents.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _contents.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _contents.GetEnumerator();

        public int IndexOf(T item) => _contents.IndexOf(item);

        IEnumerator IEnumerable.GetEnumerator() => _contents.GetEnumerator();
        #endregion
    }
}
