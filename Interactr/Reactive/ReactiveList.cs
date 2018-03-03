using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public IObservable<T> OnAdd => _onAdd;
        private readonly Subject<T> _onAdd = new Subject<T>();

        /// <summary>
        /// Observable that emits any item that is remove from the list.
        /// The element is emitted after it is removed.
        /// </summary>
        public IObservable<T> OnDelete => _onDelete;
        private readonly Subject<T> _onDelete = new Subject<T>();

        public T this[int index]
        {
            get => _contents[index];
            set
            {
                T item = _contents[index];
                _contents[index] = value;
                _onDelete.OnNext(item);
                _onAdd.OnNext(value);
            }
        }
        
        public void Add(T item)
        {
            _contents.Add(item);
            _onAdd.OnNext(item);
        }

        public void Clear()
        {
            //Clear _contents by making new list but keep items for emitting _onDelete.
            List<T> temp = _contents;
            _contents = new List<T>();

            foreach (T item in temp)
            {
                _onDelete.OnNext(item);
            }

            temp.Clear();
        }

        public void Insert(int index, T item)
        {
            _contents.Insert(index, item);
            _onAdd.OnNext(item);
        }

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

        public void RemoveAt(int index)
        {
            T item = _contents[index];
            _contents.RemoveAt(index);
            _onDelete.OnNext(item);
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
