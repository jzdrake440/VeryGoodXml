using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VeryGoodXml.Attributes;
using VeryGoodXml.ContentFactories;
using VeryGoodXml.Entities.Enumerations;
using VeryGoodXml.Entities.Names;
using VeryGoodXml.Entities.Names.Attributes;
using VeryGoodXml.Entities.Names.Enumerations;
using VeryGoodXml.Exntensions;

namespace VeryGoodXml.Context
{
    public class VgXmlPropertyContext : IVgXmlPropertyContext
    {
        //maintain a cache of contexts as long as they are being referenced elsewhere (mainly the lifetime of the serialize/deserialize method call)
        private static Dictionary<PropertyInfo, WeakReference<VgXmlPropertyContext>> _softCache { get; }
            = new Dictionary<PropertyInfo, WeakReference<VgXmlPropertyContext>>();

        private Lazy<IReadOnlyCollection<VgXmlName>> _names;
        private Lazy<VgXmlName> _propertyDefinedName;
        private Lazy<VgXmlEntityAttribute> _entityAttribute;
        private Lazy<bool> _isCollection;
        private Lazy<IVgXmlContentFactory> _contentFactory;

        public PropertyInfo Property { get; private set; }
        public VgXmlEntityType EntityType => _entityAttribute.Value.Type;
        public Type PropertyType => Property.PropertyType;

        private VgXmlPropertyContext(PropertyInfo property)
        {
            Property = property;

            _names = new Lazy<IReadOnlyCollection<VgXmlName>>(() =>
            {
                var names = new List<VgXmlName>();

                names.AddRange(from a in Property.GetCustomAttributes<VgXmlNameAttribute>()
                               where a.Type != VgXmlNameType.PropertyDefined
                               select a.ToVgXmlName());

                //Add a default name if no names declared
                if (names.Count == 0 && _propertyDefinedName.Value != null)
                    names.Add(VgXmlName.Build(Property.Name, VgXmlNameType.Static));

                return names.AsReadOnly();
            });

            _propertyDefinedName = new Lazy<VgXmlName>(() =>
                (from a in Property.GetCustomAttributes<VgXmlNameAttribute>()
                 where a.Type == VgXmlNameType.PropertyDefined
                 select a.ToVgXmlName()).FirstOrDefault());

            _entityAttribute = new Lazy<VgXmlEntityAttribute>(property.GetCustomAttribute<VgXmlEntityAttribute>);

            _isCollection = new Lazy<bool>(() => PropertyType.HasInterface(typeof(ICollection<>)));

            _contentFactory = new Lazy<IVgXmlContentFactory>(_entityAttribute.Value.GetFactoryInstance);
        }

        public IVgXmlTargetedPropertyContext WithTarget(object target) => new TargetedPropertyContextWrapper(this, target);
        public IEnumerable<VgXmlName> GetNames() => _names.Value;

        public static VgXmlPropertyContext CreateContext(PropertyInfo property)
        {
            if (_softCache.TryGetValue(property, out var weakReference) &&
                weakReference.TryGetTarget(out var cachedProperty))
                return cachedProperty;

            var context = new VgXmlPropertyContext(property);

            if (_softCache.ContainsKey(property))
                _softCache[property].SetTarget(context);
            else
                _softCache[property] = new WeakReference<VgXmlPropertyContext>(context);

            return context;
        }

        /// <summary>Determines if <paramref name="name"/> matches this property based on decorating <c>VgXmlNameAttribute</c>s.</summary>
        /// <param name="name">Name to Match</param>
        /// <returns>
        ///     <para><c>True</c> if the only defined named is of type <c>PropertyDefined</c></para>
        ///     <para><c>True</c> if <paramref name="name"/> matches any <c>VgXmlName.IsNameMatch</c> (other than <c>PropertyDefined</c>)</para>
        ///     <para><c>False</c> otherwise</para>
        /// </returns>
        public bool NameMatches(string name) => (_propertyDefinedName.Value != null && GetNames().Count() == 0) || GetNames().Any(n => n.IsNameMatch(name));

        public bool IsCollection() => _isCollection.Value;

        public string GetBestMatchingNameFor(object target)
        {
            var propertyDefinedValue = _propertyDefinedName.Value?.GetName(target);
            if (NameMatches(propertyDefinedValue))
                return propertyDefinedValue;

            return GetNames().FirstOrDefault(n => !string.IsNullOrWhiteSpace(n.GetName(target))).GetName(target) ??
                throw new InvalidOperationException("Could not determine appropriate name.");
        }

        public IVgXmlContentFactory GetContentFactory() => _contentFactory.Value;


        //wrapper class for use with targets
        private class TargetedPropertyContextWrapper : IVgXmlTargetedPropertyContext
        {
            private VgXmlPropertyContext _context;

            public object Target { get; private set; }
            public PropertyInfo Property => _context.Property;
            public VgXmlEntityType EntityType => _context.EntityType;
            public Type PropertyType => Property.PropertyType;

            public TargetedPropertyContextWrapper(VgXmlPropertyContext context, object target)
            {
                _context = context;
                Target = target;
            }

            public IEnumerable<VgXmlName> GetNames() => _context.GetNames();
            public bool NameMatches(string name) => _context.NameMatches(name);
            public void SetValue(object value) => Property.SetValue(Target, value);
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

                PropertyType.GetMethod(nameof(ICollection<object>.Add)).Invoke(Target, new[] { value });
            }
            public bool IsCollection() => _context.IsCollection();
            public string GetBestMatchingNameFor(object target) => _context.GetBestMatchingNameFor(target);
            public IVgXmlContentFactory GetContentFactory() => _context.GetContentFactory();
        }
    }


}
