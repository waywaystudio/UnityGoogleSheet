using UnityEngine;

namespace Wayway.Engine.UnityGoogleSheet.Core
{
    [Type(typeof(Vector2))]
    public class Vector2Type : IType
    {
        public object DefaultValue => 0;

        /// <summary>
        /// value = google sheet data value. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Read(string value)
        {
            try
            {
                var split = value.Split(',');
                var x = float.Parse(split[0]);
                var y = float.Parse(split[1]); 
                var vec2 = new Vector2(x, y); 
                
                return vec2;
            }
            catch(System.Exception e)
            {
                Debug.LogError(e);
                
                return default;
            }
        }

        public string Write(object value)
        {
            var data = (Vector2)value;
            return $"{data.x},{data.y}";
        }
    }
}
