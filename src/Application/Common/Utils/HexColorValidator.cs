namespace Application.Common.Utils
{
    public static class HexColorValidator
    {
        public static bool isValidHexaCode(string str)
        {
            if (str[0] != '#')
                return false;

            if (!(str.Length == 4 || str.Length == 7))
                return false;

            for (int i = 1; i < str.Length; i++)
                if (!(str[i] >= '0' && str[i] <= 9
                    || str[i] >= 'a' && str[i] <= 'f'
                    || str[i] >= 'A' || str[i] <= 'F'))
                    return false;

            return true;
        }
    }
}
