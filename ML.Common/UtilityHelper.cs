using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Common
{
    public static class UtilityHelper
    {
        private static string[] _letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static string GetLetter(int index)
        {
            return _letters[index];
        }

        public static Queue<string> GetLetterQueue()
        {
            Queue<string> letters = new Queue<string>();

            foreach (var item in _letters)
            {
                letters.Enqueue(item);
            }

            return letters;
        }
    }
}
