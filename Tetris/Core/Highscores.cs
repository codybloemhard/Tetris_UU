using System;
using System.IO;
using System.Text;

namespace Core
{
    public class Highscores
    {
        private static uint highscore;
        private static char[] key = { '0', '9', '1', '4', '2', '3', '6', '8', '7', '5' };

        static Highscores() {
            
        }

        //Simpele encryptie zodat gebruikers niet zomaar waardes kunnen aanpassen
        public static string Encryption(bool encrypt, string source)
        {
            int dir = encrypt ? 1 : -1;
            StringBuilder src = new StringBuilder(source);
            for (int i = 0; i < source.Length; i++)
                src[i] += (char)(22 * dir);
            for (int i = 0; i < source.Length; i++)
                if (source[i] < 48 || source[i] > 57)
                    return "";
            for (int i = 0; i < source.Length; i++)
                src[i] = key[src[i] - 48];
            return src.ToString();
        }

        public static uint Decode(string source)
        {
            uint hscore = 0;
            for (int i = 0; i < source.Length; i++)
                hscore += (uint)((source[i] - 48)) * (uint)(Math.Pow(10, (source.Length - i - 1)));
            return hscore;
        }
    }
}