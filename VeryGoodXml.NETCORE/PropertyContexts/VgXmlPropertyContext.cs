using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VeryGoodXml
{
    public class VgXmlPropertyContext : IVgXmlPropertyContext
    {
        //maintain a cache of contexts as long as they are being referenced elsewhere (mainly the lifetime of the serialize/deserialize method call)
        private static Dictionary<PropertyInfo, WeakReference<VgXmlPropertyContext>> _softCache { get; }
            = new Dictionary<PropertyInfo, WeakReference<VgXmlPropertyContext>>();

        private Lazy<IEnumerable<IVgXmlEntityOption>> _options;
        private VgXmlStaticNameAttribute _defaultName;
        private Lazy<VgXmlEntityAttribute> _entityAttribute;
        private Lazy<bool> _isCollection;
        private Lazy<IVgXmlContentFactory> _contentFactory;
        private Lazy<PropertyInfo> _nameContainerProperty;

        public PropertyInfo Property { get; private set; }
        public VgXmlEntityType EntityType => _entityAttribute.Value.Type;
        public Type PropertyType => Property.PropertyType;

        private VgXmlPropertyContext(PropertyInfo property)
        {
            Property = property;

            _options = new Lazy<IEnumerable<IVgXmlEntityOption>>(
                () => property.GetCustomAttributes()
                .Where(a => a is IVgXmlEntityOption)
                .Select(a => a as IVgXmlEntityOption));

            _defaultName = new VgXmlStaticNameAttribute(property.Name);

            _entityAttribute = new Lazy<VgXmlEntityAttribute>(property.GetCustomAttribute<VgXmlEntityAttribute>);

            _isCollection = new Lazy<bool>(() => PropertyType.HasInterface(typeof(ICollection<>)));

            _contentFactory = new Lazy<IVgXmlContentFactory>(_entityAttribute.Value.GetFactoryInstance);

            _nameContainerProperty = new Lazy<PropertyInfo>(
                () => PropertyType.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<VgXmlNameContainerAttribute>() != null));
        }

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

        public IVgXmlTargetedPropertyContext WithTarget(object target) => new VgXmlTargetedPropertyContextWrapper(this, target);
        public IEnumerable<IVgXmlEntityOption> GetOptions() => _options.Value;
        public IEnumerable<T> GetOptions<T>() where T : IVgXmlEntityOption => GetOptions().Where(o => o is T).Cast<T>();
        public bool IsCollection() => _isCollection.Value;
        public IVgXmlContentFactory GetContentFactory() => _contentFactory.Value;

        public bool NameMatches(string name) =>
            GetOptions<IVgXmlNameMatcher>()
            .Where(o => o is IVgXmlNameMatcher)
            .DefaultIfEmpty(_defaultName)
            .Any(nm => nm.IsMatch(name));

        public string GenerateName(object subEntity) =>
            GetOptions<IVgXmlNameGenerator>()
            .DefaultIfEmpty(_defaultName)
            .Select(ng => ng.GenerateName(subEntity))
            .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));


        public string GetBestMatchingNameFor(object subEntity)
        {
            var nameContainerValue = _nameContainerProperty.Value?.GetValue(subEntity) as string;
            if (_nameContainerProperty.Value != null &&
                !string.IsNullOrWhiteSpace(nameContainerValue))
            {
                //no defined names, so just use w/e is stored in container
                if (GetOptions<IVgXmlNameGenerator>().Count() == 0)
                    return nameContainerValue;

                //with defined names, the container value must match
                if (NameMatches(nameContainerValue))
                    return nameContainerValue;

                throw new InvalidOperationException($"The Type {PropertyType.Name} for Property {Property.Name} has defined " +
                    $"{nameof(VgXmlNameContainerAttribute)} for a property, but it's value failed to match.");
            }

            return GenerateName(subEntity) ??
                throw new InvalidOperationException("Could not determine appropriate name.");
        }
    }
}
