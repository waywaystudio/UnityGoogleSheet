using System.Reflection;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public class EnumType
    {
        public System.Type Type { get; set; }
        public Assembly Assembly { get; set; }
        public string NameSpace { get; set; }
        public string EnumName { get; set; }

        public object Read(string value)
        {
            return System.Enum.Parse(Type, value);
        }
        
        public string Write(object value)
        {
            return value.ToString();
        } 
    }
}
