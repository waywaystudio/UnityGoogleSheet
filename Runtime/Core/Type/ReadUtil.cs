namespace Wayway.Engine.UnityGoogleSheet.Core
{
    public static class ReadUtil
    {
        public static string[] GetBracketValueToArray(string value)
        {
            if (value.Length == 0)
            {
                return null;
            }

            if (value[0] == '[' && value[^1] == ']')
            {
                value = value.Remove(0, 1);
                value = value.Remove(value.Length - 1, 1);
            }

            // TODO. 시트에 '[' 혹은 ']'를 안붙혀도 되게 수정했습니다. 원본은 반드시 붙히게 되어있었습니다.
            var split = value.Split(',');
            
            return split;
        } 
        
        public static string[] GetParenthesisValueToArray(string value)
        {
            if (value.Length == 0) return null;
            if (value[0] == '(' && value[^1] == ')')
            {
                value = value.Remove(0, 1);
                value = value.Remove(value.Length - 1, 1);
                var split = value.Split(',');
                return split;
            }
            return null;
        }
    }
}
