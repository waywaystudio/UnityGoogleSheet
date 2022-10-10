using Wayway.Engine.UnityGoogleSheet.Core.Exception;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(typeof((int, int)), "(int,int)", "(Int32,Int32)")]
    public class IntTupleX2Type : IType
    {
        public object DefaultValue => null;
        public object Read(string value)
        {
            var datas = ReadUtil.GetBracketValueToArray(value);
            if(datas.Length is 0 or 1 or > 2)
            { 
                    throw new UGSValueParseException("Parse Failed => " + value + " To " + this.GetType().Name); 
            }

            return (datas[0], datas[1]);
        }

        public string Write(object value)
        {
            var tuple = (((int,int))value); 
            return $"[{tuple.Item1},{tuple.Item2}]";
        }
    }
}
