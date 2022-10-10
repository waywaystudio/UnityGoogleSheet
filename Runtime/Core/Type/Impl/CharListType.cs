using System.Collections.Generic;
using System.Linq;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(Type: typeof(List<char>), TypeName: new [] { "list<char>", "List<Char>" })]
    public class CharListType : IType
    {
        public object DefaultValue => null;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);

            var list = new System.Collections.Generic.List<char>();
            if (value == "[]") return list;

            var datas = ReadUtil.GetBracketValueToArray(value);
            if (datas != null)
            {
                list.AddRange(datas.Select(char.Parse));
            }
            else
            {
                throw new UGSValueParseException("Parse Failed => " + value + " To " + GetType().Name);
            }
            return list;
        }

        public string Write(object value)
        {
            var list = value as List<char>;
            return WriteUtil.SetValueToBracketArray(list);
        }
    }
}