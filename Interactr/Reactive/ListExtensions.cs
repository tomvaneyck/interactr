using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Reactive
{
    public static class ListExtensions
    {
        /// <summary>
        /// Moves elements in the list according to the specified permutation.
        /// Note that the permutation must be complete. For example, to permute {A, B, C} to {B, C, A},
        /// you must supply the following change tuples: {(0, 2), (1, 0), (2, 1)}
        /// The order of the tuples does not matter.
        /// </summary>
        /// <param name="changes">
        /// An enumeration of change tuples, each containing the current index of the element
        /// to move and the new index to move the element to.
        /// </param>
        public static void ApplyPermutation<T>(this IList<T> list, IEnumerable<(int SourceIndex, int DestinationIndex)> changes)
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
                               sourceI >= 0 && sourceI < list.Count &&
                               destI >= 0 && destI < list.Count;
                if (!isValid)
                {
                    throw new ArgumentException("Invalid move indices. The move must produce a permutation where each " +
                                                "element has 1 unique index and the length of the list is not changed.");
                }
            }

            // Apply changes by apply each cycle in the permutation
            int curSourceI = -1;
            T curValue = default(T);
            do
            {
                // If curSourceI is -1, we are not yet in a cycle, so we pick an arbitrary change to apply
                // Otherwise, if curSourceI is not in the dictionary, we completed a cycle and
                // should also pick an an arbitrary change from a next cycle to apply.
                if (curSourceI == -1 || !changesDictionary.ContainsKey(curSourceI))
                {
                    curSourceI = changesDictionary.Keys.First();
                    curValue = list[curSourceI];
                }

                // Move element to new index and store the element that was at that position
                int destI = changesDictionary[curSourceI];
                T oldVal = list[destI];
                list[destI] = curValue;

                // Delete the current change since it has now been applied and set the next element to handle
                changesDictionary.Remove(curSourceI);
                curSourceI = destI;
                curValue = oldVal;
            } while (changesDictionary.Count > 0);
        }
    }
}
