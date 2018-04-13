using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using VeryGoodXml.Serializers;

namespace VeryGoodXml.Context
{
    public abstract class VgXmlSerializerContext : IVgXmlSerializerContext
    {
        public VgXmlSerializer Serializer { get; protected set; }
        public abstract XmlReader Reader { get; protected set; }
        public abstract XmlWriter Writer { get; protected set; }

        public VgXmlSerializerContext(VgXmlSerializer serializer)
        {
            Serializer = serializer;
        }

        public static IVgXmlSerializerContext CreateWriterContext(VgXmlSerializer serializer, XmlWriter writer)
        {
            return new VgXmlWriterSerializerContext(serializer, writer);
        }

        public static IVgXmlSerializerContext CreateReaderContext(VgXmlSerializer serializer, XmlReader reader)
        {
            return new VgXmlReaderSerializerContext(serializer, reader);
        }


        private class VgXmlWriterSerializerContext : VgXmlSerializerContext
        {
            public override XmlWriter Writer { get; protected set; }
            public override XmlReader Reader
            {
                get => throw new InvalidOperationException("Cannot Get Reader from Writer Context");
                protected set { }
            }

            public VgXmlWriterSerializerContext(VgXmlSerializer serializer, XmlWriter writer) : base(serializer)
            {
                Writer = writer;
            }
        }

        private class VgXmlReaderSerializerContext : VgXmlSerializerContext
        {
            public override XmlReader Reader{ get; protected set; }
            public override XmlWriter Writer
            {
                get => throw new InvalidOperationException("Cannot Get Writer from Reader Context");
                protected set { }
            }

            public VgXmlReaderSerializerContext(VgXmlSerializer serializer, XmlReader reader) : base(serializer)
            {
                Reader = reader;
            }
        }
    }
}
