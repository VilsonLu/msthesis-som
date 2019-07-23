using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Common.Implementation
{
    public class RunLengthCompression : ICompression
    {
        public string Compress(string text)
        {
            char[] splitText = text.ToCharArray();
            int txtLength = splitText.Length;
            List<Tuple<char, int>> characterCount = new List<Tuple<char, int>>();

            for(int i = 0; i < txtLength; i++)
            {
                int count = 1;
                while(i < txtLength - 1 && splitText[i] == splitText[i + 1])
                {
                    count++;
                    i++;
                }

                characterCount.Add(new Tuple<char, int>(splitText[i], count));
            }

            string compressedString = TransformToString(characterCount);

            return compressedString;
        }

        private string TransformToString(List<Tuple<char, int>> characterHash)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var item in characterHash)
            {
                char letter = item.Item1;
                int characterCount = item.Item2 > 2 ? 2 : item.Item2;

                for(int i = 0; i < characterCount; i++)
                {
                    builder.Append(letter.ToString());
                }
            }

            return builder.ToString();
        }
    }
}
