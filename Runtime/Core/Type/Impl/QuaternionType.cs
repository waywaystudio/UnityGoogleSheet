using UnityEngine;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(typeof(Quaternion))]
    public class QuaternionType : IType
    {
        public object DefaultValue => Quaternion.identity;

        /// <summary>
        /// value = google sheet data value. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Read(string value)
        { 
            var split = value.Split(',');
            var x = float.Parse(split[0]);
            var y = float.Parse(split[1]);
            var z = float.Parse(split[2]);
            var w = float.Parse(split[3]);
            
            return new Quaternion(x, y,z,w);
        }

        public string Write(object value)
        {
            var data = (Quaternion)value;
            
            return $"{data.x},{data.y}.{data.z},{data.w}";
        }
    }
}
