using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Interactr.Reactive
{
    public abstract class ReactiveList<T> : IReadOnlyReactiveList<T>, IList<T>
    {
        /// <summary>
        /// Observable that emits any item that is added to the list.
        /// The element is emitted after it is added.
        /// </summary>
        public abstract IObservable<(T Element, int Index)> OnAdd { get; }

        /// <summary>
        /// Observable that emits any item that is remove from the list.
        /// The element is emitted after it is removed.
        /// </summary>
        public abstract IObservable<(T Element, int Index)> OnDelete { get; }

        public abstract T this[int index] { get; set; }

        public abstract void Add(T item);

        /// <summary>
        /// Appends each element of the enumerable to the list
        /// </summary>
        /// <param name="items">The items to add to this list</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        public abstract void Clear();

        public abstract bool Contains(T item);

        public abstract bool Remove(T item);

        public void RemoveAll(IEnumerable<T> items)
        {
            items = items.ToList();
            foreach (T item in items)
            {
                Remove(item);
            }
        }

        public abstract void CopyTo(T[] array, int arrayIndex);

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract int IndexOf(T item);

        public abstract void Insert(int index, T item);

        public abstract void RemoveAt(int index);

        public abstract void Move(T item, int destinationIndex);

        public abstract void MoveByIndex(int sourceIndex, int destinationIndex);

        public abstract int Count { get; }

        int IReadOnlyCollection<T>.Count => Count;

        public abstract bool IsReadOnly { get; }
    }
}