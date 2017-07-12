using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary
{
    public class Node
    {
        public List<double> Weights { get; set; }

        public Coordinate Coordinate { get; set; }


        public Node()
        {
            Weights = new List<double>();
            Coordinate = new Coordinate(0, 0);
        }

        public Node(List<double> weights, int x, int y)
        {
            Weights = weights;
            Coordinate = new Coordinate(x, y);
        }

        /// <summary>
        /// Calculates the Euclidean distance between the weight vector and input vector
        /// </summary>
        /// <param name="inputVectors"></param>
        /// <returns>double - Euclidean distance</returns>
        public double GetDistance(List<double> inputVectors)
        {
            double sum = 0;

            for(int i = 0; i < Weights.Count; i++)
            {
                sum += Math.Pow(inputVectors[i] - Weights[i], 2);
            }

            return Math.Sqrt(sum);

        }
    }
}
