using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary.Implementation.DistanceMeasure;
using SOMLibrary.Interface;

namespace SOMLibrary.Implementation.Clusterer
{
    public class KMeansClustering : IClusterer
    {
        private readonly IRandom rand;
        private readonly IDistanceMeasure distanceMeasure;

        private List<int> usedNumber;

        public KMeansClustering()
        {
            rand = new RandomNumberGenerator();
            distanceMeasure = new EuclideanDistance(); 
        }

        public IEnumerable<Node> Cluster(List<Node> nodes, int k)
        {
            var hasChanged = false;

            var centroids = GetCentroids(nodes.ToList(), k);

            do
            {
                hasChanged = false;
                
                // assign the each nodes to a cluster
                for (var i = 0; i < nodes.Count(); i++)
                {
                    var nearestCentroid = GetNearestCentroids(nodes[i], centroids);

                    if (nodes[i].ClusterGroup != nearestCentroid.ClusterGroup)
                    {
                        hasChanged = true;
                    }

                    nodes[i].ClusterGroup = nearestCentroid.ClusterGroup;
                }

                // Adjust the centroids based 
                centroids = UpdateCentroids(nodes, centroids);

            } while (hasChanged);


            return nodes;
        }

        private List<Node> UpdateCentroids(List<Node> nodes, List<Node> currentCentroids)
        {
            var centroids = new List<Node>();
            var clusters = currentCentroids.Select(x => x.ClusterGroup).ToList();
            foreach (var cluster in clusters)
            {
                var clusterNodes = nodes.Where(x => x.ClusterGroup == cluster).ToList();

                // get the number of weights
                var numWeights = nodes[0].Weights.Length;

                var weights = new double[numWeights];
                for (int i = 0; i < numWeights; i++)
                {
                    var sum = 0.0;
                    foreach (var node in clusterNodes)
                    {
                        sum += node.Weights[i];
                    }

                    var average = sum / clusterNodes.Count;

                    weights[i] = average;

                }

                var averageNode = new Node(weights, 0, 0);
                var centroid = GetNearestCentroids(averageNode, clusterNodes);

                centroids.Add(centroid);
            }

            return centroids;
        }

        private List<int> GetCluster(List<Node> nodes)
        {
            return nodes.Select(x => x.ClusterGroup).Distinct().ToList();
        }

        private List<Node> GetCentroids(List<Node> nodes, int k)
        {
            var centroids = new List<Node>();
            usedNumber = new List<int>();

            for (int i = 0; i < k; i++)
            {    
                var randomNodes = GetRandomNodes(nodes.ToList());
                centroids.Add(randomNodes);
            }

            return centroids;
        }

        private Node GetNearestCentroids(Node currentNode, List<Node> centroids)
        {
            var minDistance = double.MaxValue;
            Node minNode = null;

            foreach (var centroid in centroids)
            {
                var distance = distanceMeasure.GetDistance(currentNode.Weights, centroid.Weights);
                if (distance >= minDistance)
                {
                    continue;
                }

                minDistance = distance;
                minNode = centroid;
            }

            return minNode;
        }



        private Node GetRandomNodes(IList<Node> nodes)
        {
            var randomNumber = rand.GetRandomInteger(0, nodes.Count);

            while (usedNumber.Contains(randomNumber))
            {
                randomNumber = rand.GetRandomInteger(0, nodes.Count);
            }
            

            var randomNode = nodes[randomNumber];

            var node = new Node(randomNode.Weights, randomNode.Coordinate.X, randomNode.Coordinate.Y)
            {
                ClusterGroup = randomNumber
            };

            usedNumber.Add(randomNumber);

            return node;
        }
    }
}
