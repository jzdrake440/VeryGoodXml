using System;
using System.Collections.Generic;
using System.Reflection;

namespace VeryGoodXml
{
    public class VgXmlTargetedPropertyContextWrapper : IVgXmlTargetedPropertyContext
    {
        private IVgXmlPropertyContext _context;
        public object Target { get; private set; }

        public VgXmlTargetedPropertyContextWrapper(IVgXmlPropertyContext context, object target)
        {
            if (!target.GetType().IsAssignableFrom(context.Property.DeclaringType))
                throw new ArgumentException($"{nameof(target)} ({target.GetType()}) is not of same type as {nameof(context)}.{nameof(context.Property)}.{nameof(context.Property.DeclaringType)} ({context.Property.DeclaringType.Name}).");

            _context = context;
            Target = target;
        }

        public void SetValue(object value)
        {
            if (!PropertyType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException($"{nameof(value)} is not of type {PropertyType.Name}.", nameof(value));

            Property.SetValue(Target, value);
        }

        public object GetValue() => Property.GetValue(Target);

        public void AddToCollection(object value)
        {
            if (!IsCollection()) throw new InvalidOperationException("Property is not collection.");

            var collection = GetValue();

            if (collection == null)
            {
                collection = Activator.CreateInstance(PropertyType);
                SetValue(collection);
            }

            PropertyType.GetMethod(nameof(ICollection<object>.Add)).Invoke(collection, new[] { value });
        }

        //fall-through properties
        public PropertyInfo Property => _context.Property;
        public VgXmlEntityType EntityType => _context.EntityType;
        public Type PropertyType => _context.PropertyType;

        //fall-through methods
        public IEnumerable<IVgXmlEntityOption> GetOptions() => _context.GetOptions();
        public IEnumerable<T> GetOptions<T>() where T : IVgXmlEntityOption => _context.GetOptions<T>();
        public bool NameMatches(string name) => _context.NameMatches(name);
        public string GenerateName(object subEntity) => _context.GenerateName(subEntity);
        public bool IsCollection() => _context.IsCollection();
        public string GetBestMatchingNameFor(object target) => _context.GetBestMatchingNameFor(target);
        public IVgXmlContentFactory GetContentFactory() => _context.GetContentFactory();
    }
}
