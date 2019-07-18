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
using Newtonsoft.Json.Linq;
using SOMLibrary.Implementation.Metric;

namespace Sandbox
{
    class Program
    {
        private static List<Tuple<int, double>> disorderScores = new List<Tuple<int, double>>();
        public static void Main(string[] args)
        {
            Program2(args);
        }

        public static void Program2(string[] args)
        {
            string filePath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\config.json";
            if(args.Length > 0)
            {
                filePath = args[0];
            }

            var disorderScores = new List<Tuple<int, double>>();
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

            Console.WriteLine("Printing disorder scores...");
            Console.WriteLine();
            foreach(var item in disorderScores)
            {
                Console.WriteLine("Iteration {0}: {1}", item.Item1, item.Item2);
            }
            Console.WriteLine("End of Disorder Scores");

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

            string trainedModelFile = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\experiment_1\Map_experiment_1.json";
            string filepath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\MALE_PLEASANT_UNNORMALIZED.csv";
            string mapLabels = "USER,SEG,CLASS_PLEASANTNESS";

            Console.WriteLine("Loading model...");
            var jsonContent = System.IO.File.ReadAllText(trainedModelFile);
            SSOM _model = JsonConvert.DeserializeObject<SSOM>(jsonContent);
            
            _model.Training += _model_Training; 
            IReader _reader = new CSVReader(filepath);
            IClusterer _kmeans = new KMeansClustering();

            _model.GetData(_reader);

            // Get the labels
            string[] labels = mapLabels.Split(',');

            foreach (var label in labels)
            {
                _model.Dataset.SetLabel(label);
            }

            Console.WriteLine("Model loaded.");


            Console.WriteLine("Measuring disorder...");
            IMetric disorderMeasure = new DisorderMeasure();
            var disorderScore = disorderMeasure.Measure(_model);
            Console.WriteLine("Disorder Score: {0}", disorderScore);

            Console.WriteLine("Assigning Node ID...");
            _model.AssignNodeId();
            Console.WriteLine("Completed assigning ID");


            string trainingPath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\male_pleasant_trajectories";
            List<TrajectoryMapper> dbTrajectories = new List<TrajectoryMapper>();

            Console.WriteLine("Start plotting trajectories...");
            stopwatch.Restart();

            foreach (var file in Directory.EnumerateFiles(trainingPath))
            {
                TrajectoryMapper trajectoryMapper = new TrajectoryMapper(_model);
                IReader trajectoryReader = new CSVReader(file);

                trajectoryMapper.GetData(trajectoryReader);
                trajectoryMapper.PlotTrajectory();

                dbTrajectories.Add(trajectoryMapper);
                
            }

            stopwatch.Stop();
            Console.WriteLine("Completed plotting trajectories...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);


            string testingPath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\experiment_1\unknown_trajectory\m_pleasant_9_1.csv";

            TrajectoryMapper testMapper = new TrajectoryMapper(_model);
            IReader trajectoryDataReader = new CSVReader(testingPath);

            testMapper.GetData(trajectoryDataReader);

            testMapper.PlotTrajectory();
            var unknownTrajectory = testMapper.Trajectories;

            IFileHelper fileHelper = new FileHelper();
            ISimilarityMeasure similarityMeasure = new CompressionDissimilarityMeasure(fileHelper);

            Console.WriteLine("Computing similarity measure using {0}", similarityMeasure.GetType().Name);

            var scores = new List<Tuple<string, double, int>>();
            foreach (var trajectory in dbTrajectories)
            {
                var currentTrajectory = trajectory.Trajectories;

                var score = similarityMeasure.MeasureSimilarity(currentTrajectory, unknownTrajectory);
                scores.Add(new Tuple<string, double, int>(trajectory.FileName, score, trajectory.Trajectories.Count));
                Console.WriteLine("{0}:{1}:{2}", trajectory.FileName, score, trajectory.Trajectories.Count);
            }


            var topNScores = scores.OrderBy(x => x.Item2).Take(20);

            Console.WriteLine();
            Console.WriteLine("Top 20 Trajectories");
            foreach(var item in topNScores)
            {
                Console.WriteLine("{0}:{1}:{2}", item.Item1, item.Item2, item.Item3);
            }


            Console.ReadLine();
        }

        private static void _model_Training(object sender, OnTrainingEventArgs args)
        {
            //ClearLastLine();
            var currentIteration = args.CurrentIteration;
            //Console.WriteLine("Current Iteration: {0} / {1}", currentIteration + 1, args.TotalIteration);

            var model = sender as SOM;

            if(model == null)
            {
                return;
            }


            IMetric disorderMetric = new DisorderMeasure();
            if(currentIteration % 5 == 0)
            {
                var score = disorderMetric.Measure(model);
                Console.WriteLine("Iteration {0}: {1}", currentIteration, score);
                disorderScores.Add(new Tuple<int, double>(currentIteration, score));
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
