using System.Collections.Generic;
using System.Text;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public static class WriteUtil
    {
        public static string SetValueToBracketArray<T,G>(object value) where T : List<G>
        {
            return SetValueToBracketArray(value as T);
        }
        
        public static string SetValueToBracketArray<T>(List<T> value)
        {
            var builder = new StringBuilder();
            builder.Append("[");
            for (var i = 0; i < value.Count; i++)
            {
                var data = value[i].ToString();
                builder.Append(data);
                if (i != value.Count)
                    builder.Append(",");
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
