using System;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(typeof(char), "char", "Char")]
    public class CharType : IType
    {
        public object DefaultValue => char.MinValue;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);

            var @char = char.MinValue;
            var b = char.TryParse(value, out @char);
            if (b == false)
            { 
                    throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);
            }
            return @char;
        }

        public string Write(object value)
        {
             return value.ToString();
        }
    }
}

