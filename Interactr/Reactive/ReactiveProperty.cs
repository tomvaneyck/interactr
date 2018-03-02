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
    /// 
    /// </summary> 
    public class ReactiveProperty<T>
    {
        private T _value;
        private Subject<T> _changed;

        public IObservable<T> Changed
        {
            get
            {
                if (_changed == null)
                {
                    _changed = new Subject<T>();
                }

                return _changed.StartWith(Value);
            }
        }

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