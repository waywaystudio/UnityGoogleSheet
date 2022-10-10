using System.Collections.Generic;
using System.Linq;
using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(Type: typeof(List<double>), TypeName: new [] { "list<double>", "List<Double>" })]
    public class DoubleListType : IType
    {
        public object DefaultValue => null;
        public object Read(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name);


            var list = new System.Collections.Generic.List<double>();
            if (value == "[]") return list;

            var datas = ReadUtil.GetBracketValueToArray(value);
            if (datas != null)
            {
                list.AddRange(datas.Select(double.Parse));
            }
            else
            { 
                    throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name); 
            }
            return list;
        }

        public string Write(object value)
        {
            var list = value as List<double>;
            return WriteUtil.SetValueToBracketArray(list);
        }
    }
}