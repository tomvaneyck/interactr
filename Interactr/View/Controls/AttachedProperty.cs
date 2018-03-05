using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactr.Reactive;

namespace Interactr.View.Controls
{
    /// <summary>
    /// A property that can be set on arbitrary UIElements.
    /// Allows configuring of UIElements through composition.
    /// 
    /// For example: a DragLayout component wants to know which of its
    /// children can be dragged around and which cannot. The class
    /// can expose a IsDraggable AttachedProperty and users of DragLayout
    /// can attach IsDraggable to the children of the layout.
    /// This approach scales well because each element class can expose their
    /// own attached properties without cluttering the UIElement class.
    /// </summary>
    public class AttachedProperty
    {
        public Type Type { get; }
        public object DefaultValue { get; }

        public AttachedProperty(Type type, object defaultValue)
        {
            if (!type.IsInstanceOfType(defaultValue) && defaultValue != null)
            {
                throw new ArgumentException("Invalid default value");
            }

            Type = type;
            this.DefaultValue = defaultValue;
        }

        public object GetValueObject(UIElement elem)
        {
            if (elem.AttachedProperties.TryGetValue(this, out object value))
            {
                return value;
            }

            return DefaultValue;
        }

        public void SetValueObject(UIElement elem, object value)
        {
            if (!Type.IsInstanceOfType(value) && value != null)
            {
                throw new ArgumentException("Invalid value");
            }

            elem.AttachedProperties[this] = value;
        }
    }

    public class AttachedProperty<T> : AttachedProperty
    {
        public AttachedProperty(T defaultValue) : base(typeof(T), defaultValue)
        {}

        public T GetValue(UIElement elem)
        {
            return (T)GetValueObject(elem);
        }

        public void SetValue(UIElement elem, T value)
        {
            elem.AttachedProperties[this] = value;
        }
    }
}
