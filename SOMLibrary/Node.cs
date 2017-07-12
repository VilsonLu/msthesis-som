using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary
{
    public class Node
    {
        public double[] Weights { get; set; }

        public Coordinate Coordinate { get; set; }


        public Node(double[] weights, int x, int y)
        {
            Weights = weights;
            Coordinate = new Coordinate(x, y);
        }

        /// <summary>
        /// Calculates the Euclidean distance between the weight vector and input vector
        /// </summary>
        /// <param name="inputVectors"></param>
        /// <returns>double - Euclidean distance</returns>
        public double GetDistance(double[] inputVectors)
        {
            double sum = 0;

            for(int i = 0; i < Weights.Length; i++)
            {
                sum += Math.Pow(inputVectors[i] - Weights[i], 2);
            }

            return Math.Sqrt(sum);

        }
    }
}
