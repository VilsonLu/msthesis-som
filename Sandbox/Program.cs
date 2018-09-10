using SOMLibrary.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.Common;
using SOMLibrary;
using SOMLibrary.Implementation.Clusterer;
using SOMLibrary.Interface;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            string filepath = @"C:\Users\Vilson\Documents\Github\MSThesis\Sandbox\Dataset\Iris.csv";

            SOM _model = new SOM(10, 20, 0.3, 40);
            IReader _reader = new CSVReader(filepath);
            IClusterer _kmeans = new KMeansClustering();

            _model.GetData(_reader);
            _model.Dataset.SetLabel("Species");
            _model.Dataset.SetLabel("Id");

            _model.FeatureLabel = "Species";

            _model.InitializeMap();
            _model.Train();
            // _model.LabelNodes();

            var flattenedMap = ArrayHelper<Node>.FlattenMap(_model.Map);
            var clusteredNodes = _kmeans.Cluster(flattenedMap, 3);

            foreach (var node in clusteredNodes)
            {
                Console.WriteLine("X: {0} Y: {1} Cluster: {2}", node.Coordinate.X, node.Coordinate.Y, node.ClusterGroup);
            }

            Console.ReadLine();
        }

        private static List<Node> GetFlattenedMap(SOM mSom)
        {
            var flattenedNodes = new List<Node>();
            for (int i = 0; i < mSom.Width; i++)
            {
                for (int k = 0; k < mSom.Width; k++)
                {
                    flattenedNodes.Add(mSom.Map[i, k]);
                }
            }

            return flattenedNodes;
        }
    } 
}
