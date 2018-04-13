﻿using System;
using System.Collections.Generic;
using System.Text;
using VeryGoodXml.Entities.Enumerations;
using VeryGoodXml.Serializers;

namespace VeryGoodXml.Attributes
{
    public class VgXmlElementAttribute : VgXmlEntityAttribute
    {
        public override VgXmlEntityType Type => VgXmlEntityType.Element;
    }
}
