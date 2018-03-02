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
    /// This is an implementation of IList with observables.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the list.</typeparam>
    public class ReactiveList<T> : IList<T>
    {

        private List<T> _contents;

        private Subject<T> _onAdd;
        public IObservable<T> OnAdd => _onAdd;

        private Subject<T> _onDelete;
        public IObservable<T> OnDelete => _onDelete;

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
            // Clearing _contents by making new list but keeping items for notifying.
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
            if (_contents.Remove(item))
            {
                _onDelete.OnNext(item);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            T item = _contents[index];
            this.Remove(item);
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
