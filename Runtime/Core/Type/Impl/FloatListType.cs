using System.Collections.Generic;
using System.Linq;
using Wayway.Engine.UnityGoogleSheet.Core;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace GoogleSheet.Type
{
    [Type(Type: typeof(List<float>), TypeName: new string[] { "list<float>", "List<Float>" })]
    public class FloatListType : IType
    {
        public object DefaultValue => null;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name);

            var list = new System.Collections.Generic.List<float>();
            if (value == "[]") return list;

            var datas = ReadUtil.GetBracketValueToArray(value);
            if (datas != null)
            {
                list.AddRange(datas.Select(float.Parse));
            }
            else
            { 
                    throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name);

            }
            return list;
        }

        public string Write(object value)
        {
            var list = value as List<float>;
            return WriteUtil.SetValueToBracketArray(list);
        }
    }
}