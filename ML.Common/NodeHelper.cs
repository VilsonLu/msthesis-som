using SOMLibrary;
using System.Collections.Generic;

namespace ML.Common
{
    public class NodeHelper
    {

        public static List<Node> FlattenMap(Node[,] map)
        {
            var flattenedNodes = new List<Node>();

            var width = map.GetLength(0);
            var height = map.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    flattenedNodes.Add(map[i, k]);
                }
            }

            return flattenedNodes;
        }
    }
}
