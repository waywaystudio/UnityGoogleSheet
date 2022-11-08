using System.Collections.Generic;
using System.Linq;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(Type: typeof(List<int>), TypeName: new [] { "list<int>", "List<int>" })]
    public class IntListType : IType
    {
        public object DefaultValue => null;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);

            var list = new List<int>();
            if (value == "[]") return list;

            var datas = ReadUtil.GetBracketValueToArray(value);
            if (datas != null)
            {
                list.AddRange(datas.Select(int.Parse));
            }
            else
            { 
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name); 
            }
            
            return list;
        }

        public string Write(object value)
        {
            var list = value as List<int>;
            return WriteUtil.SetValueToBracketArray(list);
        }
    }
}
