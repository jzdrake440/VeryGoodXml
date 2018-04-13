using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VeryGoodXml.ContentFactories;
using VeryGoodXml.Entities.Enumerations;
using VeryGoodXml.Serializers;

namespace VeryGoodXml.Attributes
{
    [AttributeUsage(
        AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public abstract class VgXmlEntityAttribute : Attribute
    {
        public virtual Type ContentFactory { get; set; } = typeof(VgXmlDefaultContentFactory);
        public abstract VgXmlEntityType Type { get; }

        public virtual IVgXmlContentFactory GetFactoryInstance()
        {
            if (ContentFactory == null)
                return null;

            if (!ContentFactory.GetInterfaces().Any(i => i == typeof(IVgXmlContentFactory)))
                throw new ArgumentException($"{nameof(ContentFactory)} must be type that implements {nameof(IVgXmlContentFactory)}.");

            var constructor = ContentFactory.GetConstructor(System.Type.EmptyTypes) ??
                throw new ArgumentException($"{nameof(ContentFactory)} must have default constructor.");

            return constructor.Invoke(new object[] { }) as IVgXmlContentFactory;
        }
    }
}
