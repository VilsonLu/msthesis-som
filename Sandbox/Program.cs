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
using System.Diagnostics;
using System.IO;
using ML.TrajectoryAnalysis.Implementation;
using ML.TrajectoryAnalysis;
using ML.Common.Implementation;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            Stopwatch stopwatch = new Stopwatch();


            string filepath = @"C:\Users\Vilson\Desktop\Datasets\Synthetic\synthetic_1000.csv";

            SOM _model = new SOM(10, 10, 0.3, 5);
            IReader _reader = new CSVReader(filepath);
            IClusterer _kmeans = new KMeansClustering();

            _model.GetData(_reader);


            Console.WriteLine("Start initializing map...");
            stopwatch.Start();
            _model.InitializeMap();
            stopwatch.Stop();
            Console.WriteLine("Completed initialization...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            Console.WriteLine("Start training model...");
            stopwatch.Restart();
            _model.Train();
            stopwatch.Stop();
            Console.WriteLine("Completed training model...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            PrintSOM(_model);

            Console.ReadLine();
        }


        private static void PrintSOM(SOM som)
        {
            for(int i = 0; i < som.Width; i++)
            {
                for (int j = 0; j < som.Height; j++)
                {
                    var weight = string.Format("{0:0.##}", som.Map[i, j].Weights[0]);
                    Console.Write(weight + "\t");
                }

                Console.WriteLine();
            }
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
