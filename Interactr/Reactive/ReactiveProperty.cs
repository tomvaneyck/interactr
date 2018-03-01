using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Interactr.Reactive
{
    public class ReactiveProperty<T>
    {
        private T _value;
        
        private readonly Subject<T> _changed = new Subject<T>();
        public IObservable<T> Changed => _changed;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _changed.OnNext(_value);
            }
        }
    }
}
