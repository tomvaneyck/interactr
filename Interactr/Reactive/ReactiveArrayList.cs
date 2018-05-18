using System;
using System.Collections.Generic;
using System.Linq;
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

        public override IObservable<IEnumerable<(T Element, int OldIndex, int NewIndex)>> OnMoved => _onMoved;
        private readonly Subject<IEnumerable<(T Element, int OldIndex, int NewIndex)>> _onMoved = new Subject<IEnumerable<(T Element, int OldIndex, int NewIndex)>>();

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
            _onMoved.OnNext(
                Enumerable.Range(index+1, _contents.Count - 1 - index)
                    .Select(i => (_contents[i], i - 1, i))
            );
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
            _onMoved.OnNext(
                Enumerable.Range(index, _contents.Count - index)
                    .Select(i => (_contents[i], i + 1, i))
            );
        }

        /// <see cref="ReactiveList.Move"/>
        public override void Move(T item, int destinationIndex)
        {
            int sourceIndex = IndexOf(item);
            if (sourceIndex == -1)
            {
                throw new ArgumentException("The specified item was not found in the list");
            }

            MoveByIndex(sourceIndex, destinationIndex);
        }

        /// <see cref="ReactiveList.MoveByIndex"/>
        public override void MoveByIndex(int sourceIndex, int destinationIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= this.Count)
            {
                throw new IndexOutOfRangeException($"{sourceIndex} is not in [0;{Count-1}]");
            }

            if (destinationIndex < 0 || destinationIndex >= this.Count)
            {
                throw new IndexOutOfRangeException($"{destinationIndex} is not in [0;{Count - 1}]");
            }

            T item = _contents[sourceIndex];
            _contents.RemoveAt(sourceIndex);
            _contents.Insert(destinationIndex, item);

            IEnumerable<(T, int, int)> movedElementChanges;
            if (sourceIndex < destinationIndex) // Move element from front to back.
            {
                movedElementChanges = Enumerable.Range(sourceIndex, destinationIndex - sourceIndex)
                    .Select(i => (_contents[i], i + 1, i));
            }
            else // Move element from back to front.
            {
                movedElementChanges = Enumerable.Range(destinationIndex + 1, sourceIndex - destinationIndex)
                    .Select(i => (_contents[i], i - 1, i));
            }
            _onMoved.OnNext(new[] { (item, sourceIndex, destinationIndex) }.Concat(movedElementChanges));
        }
        
        public override void ApplyCyclicPermutation(IEnumerable<(int SourceIndex, int DestinationIndex)> changes)
        {
            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

            Dictionary<int, int> changesDictionary = changes.ToDictionary(c => c.SourceIndex, c => c.DestinationIndex);

            if (changesDictionary.Count == 0)
            {
                return;
            }

            // Validate changes
            foreach (var change in changesDictionary)
            {
                int sourceI = change.Key;
                int destI = change.Value;

                // If an element is moved to a new index, the element that was previously at that index
                // must also be moved. Additionally, no other elements may be moved to this index.
                // Also, the source element must have only one destination.
                bool isValid = changesDictionary.ContainsKey(destI) && 
                               changesDictionary.Values.Count(i => i == destI) == 1 &&
                               changesDictionary.Keys.Count(i => i == sourceI) == 1 &&
                               sourceI >= 0 && sourceI < Count &&
                               destI >= 0 && destI < Count;
                if (!isValid)
                {
                    throw new ArgumentException("Invalid move indices. The move must produce a permutation where each " +
                                                "element has 1 unique index and the length of the list is not changed.");
                }
            }

            // Apply changes
            int firstSourceI = changesDictionary.Keys.First();
            int curSourceI = firstSourceI;
            T curValue = this[curSourceI];
            do
            {
                int destI = changesDictionary[curSourceI];
                T oldVal = this[destI];

                this[destI] = curValue;

                curSourceI = destI;
                curValue = oldVal;
            } while (curSourceI != firstSourceI);

            // Emit events
            _onMoved.OnNext(changesDictionary.Select(p => (this[p.Value], p.Key, p.Value)));
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
