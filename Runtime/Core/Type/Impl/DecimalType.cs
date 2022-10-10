using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(Type: typeof(decimal), TypeName: new [] { "decimal", "Decimal" })]
    public class DecimalType : IType
    {
        public object DefaultValue => 0;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);

            decimal @decimal = 0;
            var b = decimal.TryParse(value, out @decimal);
            if (b == false)
            {
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);
           
            }
            
            return @decimal;
        }

        public string Write(object value)
        {
            return value.ToString();
        }
    }
}
