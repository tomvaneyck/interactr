﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Interactr.Reactive
{
    /// <summary>
    /// A property that includes a value and an IObservable.
    /// Changes to the value are indicated in the changed observable.
    /// </summary> 
    public class ReactiveProperty<T>
    {
        private T _value;
        private readonly Subject<T> _changed;

        public ReactiveProperty()
        {
            _changed = new Subject<T>();
        }

        /// <summary>
        /// A a stream of changed values. 
        /// </summary>
        /// <remarks>
        /// Stream is readonly.
        /// </remarks>
        public IObservable<T> Changed => _changed.StartWith(Value);

        /// <summary>
        /// Represents the value of the property, the value has a type T. 
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (!object.Equals(_value, value))
                {
                    _value = value;
                    _changed.OnNext(_value);
                }
            }
        }
    }
}