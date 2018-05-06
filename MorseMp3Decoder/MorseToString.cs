using System.Text;

namespace MorseMp3Decoder
{
    static class MorseToString
    {
        public static char GetChar(this string morseChar)
        {
            return GetString(morseChar)[0];
        }
        
        public static string GetString(this string morseChar)
        {
            switch (morseChar)
            {
                // Letters
                case ".-":
                    return "a";
                case "-...":
                    return "b";
                case "-.-.":
                    return "c";
                case "-..":
                    return "d";
                case ".":
                    return "e";
                case "..-.":
                    return "f";
                case "--.":
                    return "g";
                case "....":
                    return "h";
                case "..":
                    return "i";
                case ".---":
                    return "j";
                case "-.-":
                    return "k";
                case ".-..":
                    return "l";
                case "--":
                    return "m";
                case "-.":
                    return "n";
                case "---":
                    return "o";
                case ".--.":
                    return "p";
                case "--.-":
                    return "q";
                case ".-.":
                    return "r";
                case "...":
                    return "s";
                case "-":
                    return "t";
                case "..-":
                    return "u";
                case "...-":
                    return "v";
                case ".--":
                    return "w";
                case "-..-":
                    return "x";
                case "-.--":
                    return "y";
                case "--..":
                    return "z";
                // Digits
                case "-----":
                    return "0";
                case ".----":
                    return "1";
                case "..---":
                    return "2";
                case "...--":
                    return "3";
                case "....-":
                    return "4";
                case ".....":
                    return "5";
                case "-....":
                    return "6";
                case "--...":
                    return "7";
                case "---..":
                    return "8";
                case "----.":
                    return "9";
                // Special Characters
                case "..--.":
                    return "!";
                case "..--..":
                    return "?";
                case "-..-.":
                    return "/";
                case "-...-":
                    return "=";
                case "---...":
                    return ":";
                case "--..--":
                    return ",";
                case ".-.-.-":
                    return ".";
                case "-....-":
                    return "-";
                case "-.--.":
                    return "(";
                case "-.--.-":
                    return ")";
            }
            return "";
        }

        public static string GetFullString(string morseString)
        {
            var sb = new StringBuilder();
            var parts = morseString.Split(' ');
            foreach (var part in parts)
            {
                var parts2 = part.Split('\\');  //newlines
                sb.Append(GetChar(parts2[0]));
                if (parts2.Length > 0)
                {
                    for (int i = 1; i < parts2.Length; i++)
                    {
                        sb.Append(' ');
                        sb.Append(GetChar(parts2[i]));
                    }
                }
            }

            return sb.ToString();
        }
    }
}