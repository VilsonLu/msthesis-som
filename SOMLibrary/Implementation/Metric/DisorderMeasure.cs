using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.Metric
{
    public class DisorderMeasure : IMetric
    {
        public double Measure(SOM map)
        {
            int row = map.Width;
            int col = map.Height;

            List<double> disorderMeasures = new List<double>();
            for (int i = 0; i < row; i++)
            {
                for(int j=0; j < col; j++)
                {
                    Node currentNode = map.Map[i, j];
                    List<Node> neighbors = GetDirectNeighbor(map, i, j);
                    disorderMeasures.Add(neighbors.Average(x => x.GetDistance(currentNode.Weights)));
                }
            }

            return disorderMeasures.Average();
        }

        private List<Node> GetDirectNeighbor(SOM map, int i, int j)
        {
            var directNeighbor = new List<Node>();
            var _map = map.Map;
            var width = map.Width;
            var height = map.Height;

            // upper left
            if(IsBound(width, height, i - 1, j - 1))
            {
                directNeighbor.Add(_map[i - 1, j - 1]);
            }

            // top
            if (IsBound(width, height, i - 1, j))
            {
                directNeighbor.Add(_map[i - 1, j]);
            }

            // upper right
            if (IsBound(width, height, i - 1, j + 1))
            {
                directNeighbor.Add(_map[i - 1, j + 1]);
            }

            // left
            if (IsBound(width, height, i, j - 1))
            {
                directNeighbor.Add(_map[i, j - 1]);
            }

            // right
            if (IsBound(width, height, i, j + 1))
            {
                directNeighbor.Add(_map[i, j + 1]);
            }

            // bottom left
            if (IsBound(width, height, i + 1, j - 1))
            {
                directNeighbor.Add(_map[i + 1, j - 1]);
            }

            // bottom
            if (IsBound(width, height, i + 1, j))
            {
                directNeighbor.Add(_map[i + 1, j]);
            }

            // bottom right
            if (IsBound(width, height, i + 1, j + 1))
            {
                directNeighbor.Add(_map[i + 1, j + 1]);
            }

            return directNeighbor;
        }

        private bool IsBound(int width, int height, int i, int j)
        {
            // if negative numbers, automatically out of bound
            if(i < 0 || j < 0)
            {
                return false;
            }

            if(i >= width || j >= height)
            {
                return false;
            }

            return true;
        }    

        private void PrintInfo(Node currentNode, List<Node> neighbors)
        {
            Console.WriteLine("---------");
            Console.WriteLine("X:{0}, Y{1}", currentNode.Coordinate.X, currentNode.Coordinate.Y);
            foreach (var item in neighbors)
            {
                var distance = item.GetDistance(currentNode.Weights);

                Console.WriteLine("X:{0}, Y:{1}: {2}", item.Coordinate.X, item.Coordinate.Y, distance);
            }

            Console.WriteLine("Average: {0} ", neighbors.Average(x => x.GetDistance(currentNode.Weights)));
            Console.WriteLine("---------");
        }
    }
}
