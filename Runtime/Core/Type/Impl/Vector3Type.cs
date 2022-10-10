using UnityEngine;

namespace Wayway.Engine.UnityGoogleSheet.Core 
{
    [Type(typeof(Vector3))]
    public class Vector3Type : IType
    {
        public object DefaultValue => 0;

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

            return new Vector3(x, y, z);
        }

        public string Write(object value)
        {
            var data = (Vector3)value;
            return $"{data.x},{data.y},{data.z}";
        }
    }
}
