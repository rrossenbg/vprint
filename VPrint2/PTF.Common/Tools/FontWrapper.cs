/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;

namespace VPrinting.Tools
{
    //<?xml version="1.0"?>
    //<FontWrapper xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    //  <FontValue>Microsoft Sans Serif, 8.25pt</FontValue>
    //</FontWrapper>
    [Serializable]
    public class FontWrapper
    {
        public FontWrapper()
        {
            Value = null;
        }

        public FontWrapper(Font font)
        {
            Value = font;
        }

        [XmlIgnore]
        public Font Value { get; set; }

        [XmlElement("Value")]
        public string SerializeFontAttribute
        {
            get
            {
                return FontXmlConverter.ConvertToString(Value);
            }
            set
            {
                Value = FontXmlConverter.ConvertToFont(value);
            }
        }

        public static implicit operator Font(FontWrapper serializeableFont)
        {
            if (serializeableFont == null)
                return null;
            return serializeableFont.Value;
        }

        public static implicit operator FontWrapper(Font font)
        {
            return new FontWrapper(font);
        }
    }

    public static class FontXmlConverter
    {
        public static string ConvertToString(Font font)
        {
            try
            {
                if (font != null)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                    return converter.ConvertToString(font);
                }
                else
                    return null;
            }
            catch
            {
                Debug.WriteLine("Unable to convert");
            }
            return null;
        }
        public static Font ConvertToFont(string fontString)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                return (Font)converter.ConvertFromString(fontString);
            }
            catch
            {
                Debug.WriteLine("Unable to convert");
            }
            return null;
        }
    }
}
