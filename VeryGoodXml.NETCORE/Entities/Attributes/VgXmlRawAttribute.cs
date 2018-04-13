using System;
using System.Collections.Generic;
using System.Text;
using VeryGoodXml.Attributes;
using VeryGoodXml.Entities.Enumerations;

namespace VeryGoodXml.Entities.Attributes
{
    public class VgXmlRawElementAttribute : VgXmlEntityAttribute
    {
        public override VgXmlEntityType Type => VgXmlEntityType.RawElement;
    }
}
