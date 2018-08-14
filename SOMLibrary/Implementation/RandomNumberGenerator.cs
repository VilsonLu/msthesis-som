using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary.Interface;

namespace SOMLibrary.Implementation
{
    public class RandomNumberGenerator : IRandom
    {
        public int GetRandomInteger(int min, int max)
        {
            Random rand = new Random();
            return rand.Next(min, max);
        }
    }
}
