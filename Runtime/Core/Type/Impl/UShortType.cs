using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(Type: typeof(ushort), TypeName: new [] { "ushort" })]
    public class UShortType : IType
    {
        public object DefaultValue => 0;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name);

            var b = ushort.TryParse(value, out var @short);
            if (b == false)
            {
                throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name);
            }
            
            return @short;
        }


        public string Write(object value)
        {
            return value.ToString();
        }
    }
}
