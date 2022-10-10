using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(Type : typeof(uint), TypeName : new[] { "uint", "UInt"})]
    public class UIntType : IType
    {
        public object DefaultValue => 0;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);

            uint @int = 0;
            var b = uint.TryParse(value, out @int);
            if (b == false)
            { 
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);
            }
            return @int;
        }

        public string Write(object value)
        {
            return value.ToString();
        }
    }
}
