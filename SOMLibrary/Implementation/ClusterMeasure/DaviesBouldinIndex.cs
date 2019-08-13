using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary.Implementation.DistanceMeasure;
using SOMLibrary.Interface;

namespace SOMLibrary.Implementation.ClusterMeasure
{
    public class DaviesBouldinIndex : IClusterMeasure
    {
        private IDistanceMeasure distanceMeasure;

        public DaviesBouldinIndex()
        {
            distanceMeasure = new EuclideanDistance();
        }


        /// <summary>
        /// Measure the quality of the clustering algorithm 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="centroids"></param>
        /// <returns></returns>
        public double MeasureScore(List<Node> nodes, List<Node> centroids)
        {
            int numOfCluster = centroids.Count;

            List<double> WithinClusterDistance = new List<double>();

            foreach(var centroid in centroids)
            {
                List<Node> clusterNodes = GetClusterNodes(nodes, centroid);
                WithinClusterDistance.Add(DistanceWithinCluster(clusterNodes, centroid));
            }

            double dbi = 0.0;
            for(int i = 1; i < numOfCluster; i++)
            {
                double max = Double.NegativeInfinity;

                for(int j = 0; j < numOfCluster; j++)
                {
                    if(i == j)
                    {
                        continue;
                    }

                    double val = (WithinClusterDistance[i] + WithinClusterDistance[j]) / DistanceBetweenClusters(centroids[i], centroids[j]);

                    if(val > max)
                    {
                        max = val;
                    }
                }

                dbi += max;

            }

            //foreach (var centroid in centroids)
            //{
            //    var clusterNodes = GetClusterNodes(nodes, centroid);
            //    var otherCentroids = centroids.Where(x => x.ClusterGroup != centroid.ClusterGroup);

            //    double si = DistanceWithinCluster(clusterNodes, centroid);


            //    List<double> clusterMeasure = new List<double>();
            //    // Compute for R(i,j)
            //    foreach (var c in otherCentroids)
            //    {
            //        var cNodes = GetClusterNodes(nodes, c);
            //        double sj = DistanceWithinCluster(cNodes, c);
            //        double distanceBetweenCluster = DistanceBetweenClusters(centroid, c);

            //        double r = (si + sj) / distanceBetweenCluster;
            //        clusterMeasure.Add(r);
            //    }

            //    davidBouldIndexes.Add(clusterMeasure.Max());
            //}


            return dbi / (double) numOfCluster;
        }

        /// <summary>
        /// Average distance between the cluster nodes and the centroid
        /// </summary>
        /// <param name="clusterNodes">Nodes that belongs to the centroid</param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        private double DistanceWithinCluster(List<Node> clusterNodes, Node centroid)
        {
            double distance = clusterNodes.Average(x => x.GetDistance(centroid.Weights));

            return distance;
        }

        /// <summary>
        /// Measures the separation between 2 clusters
        /// </summary>
        /// <param name="centroidA"></param>
        /// <param name="centroidB"></param>
        /// <returns></returns>
        private double DistanceBetweenClusters(Node centroidA, Node centroidB)
        {
            return distanceMeasure.GetDistance(centroidA.Weights, centroidB.Weights);

        }

        /// <summary>
        /// Get the nodes that belongs to the centroid
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        private List<Node> GetClusterNodes(List<Node> nodes, Node centroid)
        {
            return nodes.Where(x => x.ClusterGroup == centroid.ClusterGroup).ToList();
        }
    }
}
