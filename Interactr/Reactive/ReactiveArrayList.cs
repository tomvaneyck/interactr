using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Interactr.Reactive
{
    /// <summary>
    /// An implementation of ReactiveList with a List as the backing list.
    /// </summary>
    /// <typeparam name="T">The type of items contained in the list.</typeparam>
    public class ReactiveArrayList<T> : ReactiveList<T>
    {
        private List<T> _contents = new List<T>();

        public override IObservable<(T Element, int Index)> OnAdd => _onAdd;
        private readonly Subject<(T Element, int Index)> _onAdd = new Subject<(T Element, int Index)>();

        public override IObservable<(T Element, int Index)> OnDelete => _onDelete;
        private readonly Subject<(T Element, int Index)> _onDelete = new Subject<(T Element, int Index)>();

        public override T this[int index]
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
        
        public override void Add(T item)
        {
            _contents.Add(item);
            _onAdd.OnNext((item, _contents.Count-1));
        }

        public override void Clear()
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

        /// <see cref="IList.Insert"/>
        public override void Insert(int index, T item)
        {
            _contents.Insert(index, item);
            _onAdd.OnNext((item, index));
        }

        /// <see cref="IList.Remove"/>
        public override bool Remove(T item)
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
        public override void RemoveAt(int index)
        {
            T item = _contents[index];
            _contents.RemoveAt(index);
            _onDelete.OnNext((item, index));
        }

        /// <see cref="ReactiveList.Move"/>
        public override void Move(T item, int destinationIndex)
        {
            int sourceIndex = IndexOf(item);
            MoveByIndex(sourceIndex, destinationIndex);
        }

        /// <see cref="ReactiveList.MoveByIndex"/>
        public override void MoveByIndex(int sourceIndex, int destinationIndex)
        {
            T item = _contents[sourceIndex];
            _contents.RemoveAt(sourceIndex);
            destinationIndex -= sourceIndex < destinationIndex ? 1 : 0;
            _contents.Insert(destinationIndex, item);
        }

        #region DefaultImplementations
        public override int Count => _contents.Count;

        public override bool IsReadOnly => ((ICollection<T>)_contents).IsReadOnly;

        public override bool Contains(T item) => _contents.Contains(item);

        public override void CopyTo(T[] array, int arrayIndex) => _contents.CopyTo(array, arrayIndex);

        public override IEnumerator<T> GetEnumerator() => _contents.GetEnumerator();

        public override int IndexOf(T item) => _contents.IndexOf(item);
        #endregion
    }
}
