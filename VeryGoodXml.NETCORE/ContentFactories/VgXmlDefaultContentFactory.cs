using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using VeryGoodXml.Context;
using VeryGoodXml.Entities.Enumerations;

namespace VeryGoodXml.ContentFactories
{
    public class VgXmlDefaultContentFactory : IVgXmlContentFactory
    {
        public void ReadIntoProperty(IVgXmlSerializerContext sContext, IVgXmlTargetedPropertyContext pContext)
        {
            if (pContext.IsCollection())
            {
                var entityValue = ReadEntity(sContext, pContext.PropertyType.GetGenericArguments().First(), pContext.EntityType);
                if (entityValue == null)
                    return;

                pContext.AddToCollection(entityValue);
            }
            else
                pContext.SetValue(ReadEntity(sContext, pContext.PropertyType, pContext.EntityType));
        }

        private object ReadEntity(IVgXmlSerializerContext sContext, Type entityValueType, VgXmlEntityType entityType)
        {
            switch (entityType)
            {
                case VgXmlEntityType.Attribute:
                    return StringToObject(sContext.Reader.ReadContentAsString(), entityValueType);
                case VgXmlEntityType.Element:
                    if (!(entityValueType.IsPrimitive || entityValueType == typeof(string)))
                        return sContext.Serializer.Deserialize(sContext.Reader, entityValueType);
                    else
                        return StringToObject(sContext.Reader.ReadString(), entityValueType);
                case VgXmlEntityType.RawElement:
                    return sContext.Reader.ReadOuterXml();
            }

            throw new NotImplementedException(entityType.ToString());
        }

        public void WriteFromProperty(IVgXmlSerializerContext sContext, IVgXmlTargetedPropertyContext pContext)
        {
            if (pContext.GetValue() == null)
                return;

            if (pContext.IsCollection())
            {
                var collection = pContext.GetValue() as IEnumerable;
                foreach (var item in collection)
                    WriteEntity(sContext, item, pContext.GetBestMatchingNameFor(item), pContext.EntityType);
            }
            else
                WriteEntity(sContext, pContext.GetValue(), pContext.GetBestMatchingNameFor(pContext.GetValue()), pContext.EntityType);

        }

        //separate the PropertyContext from the entity name/value for handling collections
        private void WriteEntity(IVgXmlSerializerContext sContext, object entityValue, string entityName, VgXmlEntityType entityType)
        {

            switch (entityType)
            {
                case VgXmlEntityType.Attribute:
                    sContext.Writer.WriteAttributeString(entityName, ObjectToString(entityValue));
                    break;

                case VgXmlEntityType.Element:
                    sContext.Writer.WriteStartElement(entityName);

                    if (!(entityValue.GetType().IsPrimitive || entityValue is string))
                        sContext.Serializer.Serialize(sContext.Writer, entityValue);
                    else
                        sContext.Writer.WriteValue(ObjectToString(entityValue));

                    sContext.Writer.WriteEndElement();
                    break;

                case VgXmlEntityType.RawElement:
                    sContext.Writer.WriteStartElement(entityName);
                    sContext.Writer.WriteRaw(ObjectToString(entityValue));
                    sContext.Writer.WriteEndElement();
                    break;
            }
        }

        private static object StringToObject(string value, Type type)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (type == typeof(string))
                return value;

            if (value.GetType().GetInterfaces().Any(i => i == typeof(IConvertible)))
                return Convert.ChangeType(value, type);

            throw new NotImplementedException(type.Name);
        }

        private static string ObjectToString(object value)
        {
            if (value == null)
                return null;

            if (value is string sValue)
                return sValue;

            if (value.GetType().GetInterfaces().Any(i => i == typeof(IConvertible)))
                return Convert.ToString(value);

            return value.ToString();
        }
    }
}
