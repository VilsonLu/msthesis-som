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
using SOMLibrary.Implementation.Builder;
using Newtonsoft.Json;

namespace Sandbox
{
    class Program
    {

        public static void Main(string[] args)
        {
            Program2(args);
        }

        public static void Program2(string[] args)
        {
            string filePath = @"C:\Users\Vilson\Desktop\Datasets\EEG\config.json";
            if(args.Length > 0)
            {
                filePath = args[0];
            }

            var content = System.IO.File.ReadAllText(filePath);
            var config = ReadToObject(content);

            // Build the Model
            SSOM model = new SSOM(config.Width, config.Height, config.ConstantLearningRate, config.Epoch, config.K);
            model.Regions = config.Regions;

            // Subscribe to OnTrainingEvent
            model.Training += _model_Training;

            // Instantiate the reader 
            IReader _reader = new CSVReader(config.Dataset);

            // Instantiate the clusterer
            IClusterer clusterer = new KMeansClustering();

            model.GetData(_reader);

            // Get the labels
            string[] labels = config.Labels.Split(',');

            foreach(var label in labels)
            {
                model.Dataset.SetLabel(label);
            }

            // Set the feature label
            model.FeatureLabel = config.FeatureLabel;

            // Initialize the training
            Stopwatch stopwatch = new Stopwatch();

             Console.WriteLine("Start initializing map...");
            stopwatch.Start();
            model.InitializeMap();
            stopwatch.Stop();
            Console.WriteLine("Completed initialization...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            Console.WriteLine("Start training model...");
            stopwatch.Restart();
            model.Train();
            stopwatch.Stop();
            Console.WriteLine("Completed training model...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            Console.WriteLine("Start labelling node...");
            stopwatch.Restart();
            model.LabelNodes();
            stopwatch.Stop();
            Console.WriteLine("Completed labelling node...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            if(config.Clusters > 0)
            {
                Console.WriteLine("Start clustering nodes...");
                stopwatch.Restart();
                var flattenedMap = ArrayHelper<Node>.FlattenMap(model.Map);
                var clusteredNodes = clusterer.Cluster(flattenedMap, config.Clusters);

                foreach(var node in clusteredNodes)
                {
                    model.Map[node.Coordinate.X, node.Coordinate.Y].ClusterGroup = node.ClusterGroup;
                }

                stopwatch.Stop();
                Console.WriteLine("Completed clustering nodes...");
                Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
            }

            // Export the model
            Console.WriteLine("Exporting model...");
            var guid = Guid.NewGuid();
            model.MapId = guid;
            model.Dataset = null;

            var serializeObject = JsonConvert.SerializeObject(model, Formatting.Indented);

            string exportFileName = string.Format("{0}Map_{1}.json", config.Export, guid);

            System.IO.File.WriteAllText(exportFileName, serializeObject);

            Console.WriteLine("Training completed...");

            Console.ReadLine();

        }

        public static void Program1(string[] args)
        {

            Stopwatch stopwatch = new Stopwatch();


            string filepath = @"C:\Users\Vilson\Desktop\Datasets\Financial Distress\FD_Training.csv";

            SOM _model = new SOM(10, 10, 0.3, 20);
            _model.Training += _model_Training; 
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
                
            }

            stopwatch.Stop();
            Console.WriteLine("Completed plotting trajectories...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);


            string testingPath = @"C:\Users\Vilson\Desktop\Datasets\Financial Distress\Test\fd_297.csv";

            TrajectoryMapper testMapper = new TrajectoryMapper(_model);
            IReader trjaectoryDataReader = new CSVReader(testingPath);

            testMapper.GetData(trjaectoryDataReader);
            var unknownTrajectory = testMapper.GetTrajectories();

            IFileHelper fileHelper = new FileHelper();
            ISimilarityMeasure similarityMeasure = new CompressionDissimilarityMeasure(fileHelper);

            foreach (var trajectory in dbTrajectories)
            {
                var currentTrajectory = trajectory.GetTrajectories();

                var score = similarityMeasure.MeasureSimilarity(currentTrajectory, unknownTrajectory);
                Console.WriteLine("{0}: {1}", trajectory.FileName, score);
            }

            Console.ReadLine();
        }

        private static void _model_Training(object sender, OnTrainingEventArgs args)
        {
            ClearLastLine();
            Console.WriteLine("Current Iteration: {0} / {1}", args.CurrentIteration + 1, args.TotalIteration);
            
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

        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        public static Config ReadToObject(string json)
        {
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(json);
            return config;
        }
    }
}
