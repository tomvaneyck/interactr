using System;

namespace Interactr.View.Framework
{
    /// <summary>
    /// A property that can be set on arbitrary UIElements.
    /// Allows configuring of UIElements through composition.
    /// </summary>
    /// <example>
    /// For example: a DragLayout component wants to know which of its
    /// children can be dragged around and which cannot. The class
    /// can expose a IsDraggable AttachedProperty and users of DragLayout
    /// can attach IsDraggable to the children of the layout.
    /// This approach scales well because each element class can expose their
    /// own attached properties without cluttering the UIElement class.
    /// </example>
    public class AttachedProperty
    {
        /// <summary>
        /// Type of the property values.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The default value for this property.
        /// If the property is not explicitly set, this value will be used.
        /// </summary>
        public object DefaultValueObject { get; }

        /// <summary>
        /// Creates a new attached property.
        /// </summary>
        /// <param name="type">The type of value stored in this property.</param>
        /// <param name="defaultValueObject">The default value to be set for this property.</param>
        public AttachedProperty(Type type, object defaultValueObject)
        {
            if (!type.IsInstanceOfType(defaultValueObject) && defaultValueObject != null)
            {
                throw new ArgumentException("Invalid default value");
            }

            Type = type;
            this.DefaultValueObject = defaultValueObject;
        }

        /// <summary>
        /// Retrieve the value for this property on the given element, 
        /// or DefaultValueObject if none has been set.
        /// </summary>
        /// <param name="elem">The element to retrieve the value from.</param>
        /// <returns>The property value.</returns>
        public object GetValueObject(UIElement elem)
        {
            if (elem.AttachedProperties.TryGetValue(this, out object value))
            {
                return value;
            }

            return DefaultValueObject;
        }

        /// <summary>
        /// Set the value for this property on the given element.
        /// </summary>
        /// <param name="elem">
        /// The new value for this property on the given object. 
        /// The value type must match the type of this property.
        /// </param>
        /// <param name="value">The value to set.</param>
        public void SetValueObject(UIElement elem, object value)
        {
            if (!Type.IsInstanceOfType(value) && value != null)
            {
                throw new ArgumentException("Invalid value");
            }

            elem.AttachedProperties[this] = value;
        }
    }

    /// <summary>
    /// An attachedproperty with a generic type for the property value
    /// </summary>
    /// <typeparam name="T">The type of the value contained in this property</typeparam>
    public class AttachedProperty<T> : AttachedProperty
    {
        /// <summary>
        /// The default value for this property.
        /// If the property is not explicitly set, this value will be used.
        /// </summary>
        public T DefaultValue => (T) DefaultValueObject;

        public AttachedProperty(T defaultValueObject) : base(typeof(T), defaultValueObject)
        {}

        /// <summary>
        /// Retrieve the value for this property on the given element, 
        /// or DefaultValueObject if none has been set.
        /// </summary>
        /// <param name="elem">The element to retrieve the value from.</param>
        /// <returns>The property value.</returns>
        public T GetValue(UIElement elem)
        {
            return (T)GetValueObject(elem);
        }

        /// <summary>
        /// Set the value for this property on the given element.
        /// </summary>
        /// <param name="elem">
        /// The new value for this property on the given object. 
        /// The value type must match the type of this property.
        /// </param>
        /// <param name="value">The value to set.</param>
        public void SetValue(UIElement elem, T value)
        {
            elem.AttachedProperties[this] = value;
        }
    }
}
