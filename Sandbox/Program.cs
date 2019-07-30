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
using SOMLibrary.Implementation.LearningRate;

namespace Sandbox
{
    class Program
    {
        private static List<Tuple<int, double>> disorderScores = new List<Tuple<int, double>>();
        private static string OUTPUT_LOCATION = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\som_experiment\";
        private static int FREQUENCY = 10;
        public static void Main(string[] args)
        {
            Program1(args);
        }

        public static void Program3(string[] args)
        {
            IFileHelper fileHelper = new FileHelper();
            fileHelper.DeleteFile(string.Format("{0}disorder-measure.txt", OUTPUT_LOCATION));

            string filePath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\som_experiment\config.json";
            if (args.Length > 0)
            {
                filePath = args[0];
            }

            var disorderScores = new List<Tuple<int, double>>();
            var content = System.IO.File.ReadAllText(filePath);
            var config = ReadToObject(content);

            // Build the Model
            SOM model = new SOM(config.Width, config.Height, config.ConstantLearningRate, config.Epoch, config.K);
            model.MapRadius = config.Neighborhood;
            //model.Regions = config.Regions;
            model.LearningRate = new FixedLearningRate(config.ConstantLearningRate);

            // Subscribe to OnTrainingEvent
            model.Training += _model_Training;

            // Instantiate the reader 
            IReader _reader = new CSVReader(config.Dataset);

            // Instantiate the clusterer
            IClusterer clusterer = new KMeansClustering();

            model.GetData(_reader);

            // Get the labels
            string[] labels = config.Labels.Split(',');

            foreach (var label in labels)
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

        /// <summary>
        /// SOM Experiments
        /// </summary>
        /// <param name="args"></param>
        public static void Program2(string[] args)
        {
            string filePath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\experiment_4\config.json";
            
            FREQUENCY = 10;
            if (args.Length > 0)
            {
                filePath = args[0];
            }

            var disorderScores = new List<Tuple<int, double>>();
            var content = System.IO.File.ReadAllText(filePath);
            var config = ReadToObject(content);

            OUTPUT_LOCATION = config.Export;

            // Build the Model
            SSOM model = new SSOM(config.Width, config.Height, config.ConstantLearningRate, config.Epoch, config.K);
            model.MapRadius = config.Neighborhood;
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

                model.AssignClusterLabel();
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

        /// <summary>
        /// Trajectory Experiments
        /// </summary>
        /// <param name="args"></param>
        public static void Program1(string[] args)
        {

            Stopwatch stopwatch = new Stopwatch();

            string trainedModelFile = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\experiment_3\Map_experiment_3.json";

            Console.WriteLine("Loading model...");
            var jsonContent = System.IO.File.ReadAllText(trainedModelFile);
            SSOM _model = JsonConvert.DeserializeObject<SSOM>(jsonContent);
            _model.AssignClusterLabel();
            _model.Training += _model_Training; 
            Console.WriteLine("Model loaded.");


            Console.WriteLine("Measuring disorder...");
            IMetric disorderMeasure = new DisorderMeasure();
            var disorderScore = disorderMeasure.Measure(_model);
            Console.WriteLine("Disorder Score: {0}", disorderScore);


            string trainingPath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\male_pleasant_trajectories";
            List<TrajectoryMapper> dbTrajectories = new List<TrajectoryMapper>();

            Console.WriteLine("Start plotting trajectories...");
            stopwatch.Restart();

            string[] labels = { "USER", "SEG", "CLASS_PLEASANTNESS" };

            foreach (var file in Directory.EnumerateFiles(trainingPath))
            {
                TrajectoryMapper trajectoryMapper = new TrajectoryMapper(_model);
                IReader trajectoryReader = new CSVReader(file);

                trajectoryMapper.GetData(trajectoryReader);
                foreach (var label in labels)
                {
                    trajectoryMapper.SetLabel(label);
                }
                trajectoryMapper.PlotTrajectory();

                dbTrajectories.Add(trajectoryMapper);
                
            }

            stopwatch.Stop();
            Console.WriteLine("Completed plotting trajectories...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);


            string testingPath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\experiment_3\unknown_trajectory\m_pleasant_14_23.csv";

            ICompression compress = new RunLengthCompression();
            TrajectoryMapper testMapper = new TrajectoryMapper(_model);
            IReader trajectoryDataReader = new CSVReader(testingPath);

            testMapper.GetData(trajectoryDataReader);

            foreach (var label in labels)
            {
                testMapper.SetLabel(label);
            }

            testMapper.PlotTrajectory();
            Console.WriteLine("Unknown Trajectory: {0}", compress.Compress(testMapper.ToString()));
            var unknownTrajectory = testMapper.Trajectories;

            IFileHelper fileHelper = new FileHelper();
            ISimilarityMeasure similarityMeasure = new PairwiseDistanceMeasure();

            Console.WriteLine("Computing similarity measure using {0}", similarityMeasure.GetType().Name);

            var scores = new List<Tuple<string, double, int, string>>();
            foreach (var trajectory in dbTrajectories)
            {
                var currentTrajectory = trajectory.Trajectories;

                var score = similarityMeasure.MeasureSimilarity(currentTrajectory, unknownTrajectory);
                scores.Add(new Tuple<string, double, int, string>(trajectory.FileName, score, trajectory.Trajectories.Count, trajectory.ToString()));
                Console.WriteLine("{0}:{1}:{2}", trajectory.FileName, score, trajectory.Trajectories.Count);
            }


            var topNScores = scores.OrderBy(x => x.Item2);

   

            Console.WriteLine();
            Console.WriteLine("Trajectories...");

            string header = "Trajectory,Score,TrajectoryCount\n";

            string fileResultLocation = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\experiment_3\trajectory-results.csv";
            fileHelper.WriteToTextFile(header, fileResultLocation);

            foreach(var item in topNScores)
            {
                string result = string.Format("{0},{1},{2},{3}\n", item.Item1, item.Item2, item.Item3, compress.Compress(item.Item4) );
                fileHelper.WriteToTextFile(result, fileResultLocation);
                Console.WriteLine("{0}:{1}:{2}:{3}", item.Item1, item.Item2, item.Item3, compress.Compress(item.Item4));
            }

            Console.WriteLine("Training completed...");

            Console.ReadLine();
        }

        private static void _model_Training(object sender, OnTrainingEventArgs args)
        {
            //ClearLastLine();
            var currentIteration = args.CurrentIteration;
            //Console.WriteLine("Current Iteration: {0} / {1}", currentIteration + 1, args.TotalIteration);

            var model = sender as SSOM;

            if(model == null)
            {
                return;
            }

            IFileHelper fileHelper = new FileHelper();

            IMetric disorderMetric = new DisorderMeasure();
            if(currentIteration % FREQUENCY == 0)
            {
                var score = disorderMetric.Measure(model);
                string record = string.Format("{0},{1}\n", currentIteration, score);
                string outputFile = string.Format("{0}disorder-measure.txt", OUTPUT_LOCATION);
                Console.WriteLine(string.Format("Iteration {0}: {1}", currentIteration, score));
                fileHelper.WriteToTextFile(record, outputFile);


                string mapJson = JsonConvert.SerializeObject(model, Formatting.Indented);
                string mapName = string.Format("{0}map_iteration_{1}.json", OUTPUT_LOCATION, currentIteration);
                fileHelper.WriteToTextFile(mapJson, mapName);

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
