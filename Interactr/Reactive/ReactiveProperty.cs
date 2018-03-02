using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Reactive
{
    ///<summary>
    /// A property that includes a value and an IObservable.
    ///Changes to the value are indicated in the changed observable.
    /// </summary> 
    public class ReactiveProperty<T>
    {
        private T _value;
        private readonly Subject<T> _changed;

        public ReactiveProperty(T value)
        {
            _value = value;
            _changed = new Subject<T>();
        }

        /// <summary>
        /// A a stream of changed values. 
        /// </summary>
        /// <remarks>
        /// Stream is readonly.
        /// </remarks>
        public IObservable<T> Changed
        {
            get { return _changed.StartWith(Value); }
        }

        /// <summary>
        /// Value of Type T. 
        /// </summary>
        /// <remarks>
        /// ReactiveProperty should always have a value set.
        /// </remarks>
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _changed?.OnNext(_value);
            }
        }
    }
}