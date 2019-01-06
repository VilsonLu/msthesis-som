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


            string filepath = @"C:\Users\Vilson\Desktop\Datasets\Financial Distress\FD_Training.csv";

            SOM _model = new SOM(10, 20, 0.3, 5);
            IReader _reader = new CSVReader(filepath);
            IClusterer _kmeans = new KMeansClustering();

            _model.GetData(_reader);
            _model.Dataset.SetLabel("Company");
            _model.Dataset.SetLabel("Time");
            _model.Dataset.SetLabel("Financial Distress");
            _model.Dataset.SetLabel("Status");

            _model.FeatureLabel = "Status";

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

            Console.WriteLine("Start labelling node...");
            stopwatch.Restart();
            _model.LabelNodes();
            stopwatch.Stop();
            Console.WriteLine("Completed labelling node...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            Console.WriteLine("Start clustering nodes...");
            stopwatch.Restart();
            var flattenedMap = ArrayHelper<Node>.FlattenMap(_model.Map);
            var clusteredNodes = _kmeans.Cluster(flattenedMap, 3);
            stopwatch.Stop();
            Console.WriteLine("Completed clustering nodes...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            string trainingPath = @"C:\Users\Vilson\Desktop\Datasets\Financial Distress\Training";

            List<TrajectoryMapper> dbTrajectories = new List<TrajectoryMapper>();

            Console.WriteLine("Start plotting trajectories...");
            stopwatch.Restart();

            foreach (var file in Directory.EnumerateFiles(trainingPath))
            {
                TrajectoryMapper trajectoryMapper = new TrajectoryMapper(_model);
                IReader trajectoryReader = new CSVReader(file);

                trajectoryMapper.GetData(trajectoryReader);
                trajectoryMapper.GetTrajectories();

                dbTrajectories.Add(trajectoryMapper);
;
            }

            stopwatch.Stop();
            Console.WriteLine("Completed plotting trajectories...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);


            string testingPath = @"C:\Users\Vilson\Desktop\Datasets\Financial Distress\Test\fd_297.csv";

            TrajectoryMapper testMapper = new TrajectoryMapper(_model);
            IReader unknwownReader = new CSVReader(testingPath);

            testMapper.GetData(unknwownReader);
            var unknownTrajectory = testMapper.GetTrajectories();

            IFileHelper fileHelper = new FileHelper();
            ISimilarityMeasure similarityMeasure = new CompressionDissimilarityMeasure(fileHelper);

            foreach(var trajectory in dbTrajectories)
            {
                var currentTrajectory = trajectory.GetTrajectories();

                var score = similarityMeasure.MeasureSimilarity(currentTrajectory, unknownTrajectory);
                Console.WriteLine("{0}: {1}", trajectory.FileName, score);
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
