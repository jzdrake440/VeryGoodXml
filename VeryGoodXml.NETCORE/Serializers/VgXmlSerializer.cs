using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using VeryGoodXml.Attributes;
using VeryGoodXml.Context;
using VeryGoodXml.ContentFactories;
using VeryGoodXml.Entities.Enumerations;
using VeryGoodXml.Entities.Names;
using VeryGoodXml.Entities.Names.Attributes;
using VeryGoodXml.Entities.Names.Enumerations;

namespace VeryGoodXml.Serializers
{
    public sealed class VgXmlSerializer
    {
        public void Serialize(XmlWriter writer, object element)
        {
            var sContext = VgXmlSerializerContext.CreateWriterContext(this, writer);

            if (writer.WriteState == WriteState.Start)
            {
                SerializeRoot(writer, element, element.GetType().Name);
                return;
            }

            var pContexts = CreatePropertyContexts(element.GetType());

            //write attributes first
            foreach (var pContext in pContexts.Where(p => p.EntityType == VgXmlEntityType.Attribute))
                pContext.GetContentFactory().WriteFromProperty(sContext, pContext.WithTarget(element));

            //then write other entities
            foreach (var pContext in pContexts.Where(p => p.EntityType != VgXmlEntityType.Attribute))
                pContext.GetContentFactory().WriteFromProperty(sContext, pContext.WithTarget(element));

            writer.Flush();
        }

        public void SerializeRoot(XmlWriter writer, object element, string rootName)
        {
            bool isRoot = false;
            if (writer.WriteState == WriteState.Start)
            {
                isRoot = true;
                writer.WriteStartElement(rootName);
            }

            Serialize(writer, element);

            if (isRoot)
                writer.WriteEndElement();

            writer.Flush();
        }

        public object Deserialize(XmlReader reader, Type type)
        {
            var sContext = VgXmlSerializerContext.CreateReaderContext(this, reader);

            var retElement = Activator.CreateInstance(type);

            var isEmpty = reader.IsEmptyElement;

            var pContexts = CreatePropertyContexts(type);

            //Read Attributes
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                var pContext = pContexts.FirstOrDefault(p => p.EntityType == VgXmlEntityType.Attribute && p.NameMatches(reader.Name)) ??
                    throw new InvalidOperationException($"No matching Property found on type {type.Name} for Attribute {reader.Name}.");

                pContext.GetContentFactory().ReadIntoProperty(sContext, pContext.WithTarget(retElement));
            }

            reader.ReadStartElement();

            if (isEmpty)
                return retElement;

            while (reader.IsStartElement())
            {
                var pContext = pContexts.FirstOrDefault(p => p.EntityType == VgXmlEntityType.Element && p.NameMatches(reader.Name)) ??
                    throw new InvalidOperationException($"No matching Property found on type {type.Name} for Element {reader.Name}.");

                pContext.GetContentFactory().ReadIntoProperty(sContext, pContext.WithTarget(retElement));
            }

            reader.ReadEndElement();

            return retElement;
        }

        private static IEnumerable<VgXmlPropertyContext> CreatePropertyContexts(Type type)
        {
            return from p in type.GetProperties()
                   where p.GetCustomAttribute<VgXmlEntityAttribute>() != null
                   select VgXmlPropertyContext.CreateContext(p);
        }
    }
}
