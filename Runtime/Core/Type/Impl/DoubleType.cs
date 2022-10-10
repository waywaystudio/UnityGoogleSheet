using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(Type: typeof(double), TypeName: new [] { "double", "Double" })]
    public class DoubleType : IType
    {
        public object DefaultValue => 0.0d;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name);

            double @double = 0;
            var b = double.TryParse(value, out @double);
            if (b == false)
            {
                throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name); 
            }
            return @double;
        }

        public string Write(object value)
        {
            return value.ToString();
        }
    }
}

