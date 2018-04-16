using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VeryGoodXml
{
    public static class VgXmlTypeExtensions
    {
        public static bool HasInterface(this Type thisType, Type interfaceType)
        {
            return thisType.GetInterfaces().Any(
                t => interfaceType == t || t.IsEquivalentToGenericType(interfaceType)
            );
        }

        public static bool IsEquivalentToGenericType(this Type thisType, Type genericType)
        {
            if (!(thisType.IsGenericType && genericType.IsGenericType))
                return false;

            if (thisType.IsGenericTypeDefinition ^ genericType.IsGenericTypeDefinition)
            {
                if (thisType.IsGenericType && !thisType.IsGenericTypeDefinition)
                    return IsEquivalentToGenericType(thisType.GetGenericTypeDefinition(), genericType);

                if (genericType.IsGenericType && !genericType.IsGenericTypeDefinition)
                    return IsEquivalentToGenericType(thisType, genericType.GetGenericTypeDefinition());
            }

            return thisType == genericType;
        }
    }
}
