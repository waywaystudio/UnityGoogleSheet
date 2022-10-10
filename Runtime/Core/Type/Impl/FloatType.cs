using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(typeof(float), "float", "Float")]
    public class FloatType : IType
    {
        public object DefaultValue => 0.0f;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);

            var b = float.TryParse(value, out var f);
            if (b == false)
            {
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);
            }
            return f;
        }

        public string Write(object value)
        {
            return value.ToString();
        }
    }
}
