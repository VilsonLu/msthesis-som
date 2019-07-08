using System;
using SOMLibrary.Interface;
using SOMLibrary.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace SOMLibrary.Implementation.NodeLabeller
{
    public class KNNLabeller : ILabel
    {
        /// <summary>
        /// Dataset containing labels
        /// </summary>
        private Dataset _dataset;

        /// <summary>
        /// k - number of neighbors
        /// </summary>
        private int _k;

        /// <summary>
        /// Feature where to get the label
        /// </summary>
        private string _feature;

        public KNNLabeller(Dataset dataset, int k, string feature)
        {
            _dataset = dataset;
            _k = k;
            _feature = feature;
        }


        /// <summary>
        /// Get the label using KNN method
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string GetLabel(Node node)
        {
            var currentInstances = new List<Tuple<int, double>>();
            int numInstances = _dataset.Count;

            for (int i = 0; i < numInstances; i++)
            {
                double[] vectors = _dataset.GetInstance<double>(i);

                double distance = node.GetDistance(vectors);
                var tuple = new Tuple<int, double>(i, distance);

                currentInstances.Add(tuple);

            }

            List<Tuple<int, double>> bestInstances = currentInstances.OrderBy(x => x.Item2).Take(_k).ToList();


            var labels = new List<string>();
            bestInstances.ForEach(x => labels.Add(_dataset.GetInstanceLabel(x.Item1, _feature)));

            

            return labels.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        }

    }   
  
}
