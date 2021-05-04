using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary.Extensions;

namespace SOMLibrary.DataModel
{
    public class Dataset
    {
        public string File { get; set; }
        public Feature[] Features { get; set; }

        public Instance[] Instances { get; set; }

        public int WeightVectorCount { get; set; }


        /// <summary>
        /// Returns the number of instances
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return Instances.Length;
            }
        }



        /// <summary>
        /// Shuffles the order of the instance
        /// </summary>
        public void Shuffle()
        {
            Random rng = new Random();
            int n = Instances.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Instance value = Instances[k];
                Instances[k] = Instances[n];
                Instances[n] = value;
            }
        }
    }
}
