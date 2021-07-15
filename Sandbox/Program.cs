using LumenWorks.Framework.IO.Csv;
using ML.Common;
using ML.Common.Implementation;
using ML.TrajectoryAnalysis;
using ML.TrajectoryAnalysis.Implementation;
using ML.TrajectoryAnalysis.Implementation.Generator;
using ML.TrajectoryAnalysis.Implementation.Prediction;
using Newtonsoft.Json;
using SOMLibrary;
using SOMLibrary.Implementation;
using SOMLibrary.Implementation.Builder;
using SOMLibrary.Implementation.Clusterer;
using SOMLibrary.Implementation.ClusterMeasure;
using SOMLibrary.Implementation.LearningRate;
using SOMLibrary.Implementation.Metric;
using SOMLibrary.Implementation.NeighborhoodKernel;
using SOMLibrary.Implementation.NeighborhoodRadius;
using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sandbox
{
    class Program
    {
        /// <summary>
        /// Configuration for the location to save the text files
        /// </summary>
        private static string OUTPUT_LOCATION = @"C:\Users\User\Desktop\experiment3\";

        /// <summary>
        /// Configuration for the frequency to calculate the disorder measure
        /// </summary>
        private static int FREQUENCY = 10;

        /// <summary>
        /// Configuration to save the model per frequency
        /// </summary>
        private static bool IS_PRINT_MODEL = false;

        public static void Main(string[] args)
        {
            //Program8(args);
            //Program7(args);
            //Program2(args);
            //Program5(args);
            //Program1(args);
            //CreateSyntheticDataset();
            SSOM_Experiment(args);
            //Program2(args);
        }


        /// <summary>
        /// Training models
        /// </summary>
        /// <param name="args"></param>
        public static void Program3(string[] args)
        {
            IFileHelper fileHelper = new FileHelper();
            fileHelper.DeleteFile(string.Format("{0}disorder-measure.txt", OUTPUT_LOCATION));

            string filePath = @"D:\src\test\config_iris.json";
            if (args.Length > 0)
            {
                filePath = args[0];
            }

            var disorderScores = new List<Tuple<int, double>>();
            var content = System.IO.File.ReadAllText(filePath);
            var config = ReadToObject(content);

            // Build the Model
            SOM model = new SOM(config.Width, config.Height, config.ConstantLearningRate, config.Epoch, config.K);
            model.InitialMapRadius = config.Neighborhood;
            //model.Regions = config.Regions;
            model.LearningRateCalculator = new FixedLearningRate(config.ConstantLearningRate);

            // Subscribe to OnTrainingEvent
            model.Training += _model_Training;

            // Instantiate the reader 
            string[] labels = config.Labels.Split(',');
            string feature = config.FeatureLabel;
            IReader _reader = new CSVReader(config.Dataset, labels, feature);

            // Instantiate the clusterer
            IClusterer clusterer = new KMeansClustering();

            model.GetData(_reader);

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
            string filePath = @"C:\Users\User\Desktop\experiment3\aplusix_experiments\som_training\config_aplusix_ssom.json";

            FREQUENCY = 100;
            IS_PRINT_MODEL = false;

            if (args.Length > 0)
            {
                filePath = args[0];
            }

            var disorderScores = new List<Tuple<int, double>>();
            var content = System.IO.File.ReadAllText(filePath);
            var config = ReadToObject(content);

            OUTPUT_LOCATION = config.Export;
            IFileHelper fileHelper = new FileHelper();
            string exportFile = string.Format("{0}disorder-measure.csv", OUTPUT_LOCATION);
            fileHelper.DeleteFile(exportFile);

            fileHelper.WriteToTextFile("Iteration,Disorder Measure,Learning Rate,Radius\n", exportFile);

            // Build the Model
            var learningRate = new LinearLearningRate(config.ConstantLearningRate, config.FinalLearningRate);
            var neighborhoodRadius = new LinearDecayNeighborhoodRadius(config.Neighborhood, config.FinalNeighborhoodRadius);
            var kernel = new GaussianKernel();

            SOMBuilder builder = new SOMBuilder()
                                    .SetWidth(config.Width)
                                    .SetHeight(config.Height)
                                    .SetInitialLearningRate(config.ConstantLearningRate)
                                    .SetFinalLearningRate(config.FinalLearningRate)
                                    .SetInitialRadius(config.Neighborhood)
                                    .SetFinalRadius(config.FinalNeighborhoodRadius)
                                    .SetEpoch(config.Epoch)
                                    .SetGlobalEpoch(config.GlobalEpoch)
                                    .SetClusters(config.Clusters)
                                    .SetKNeighbor(config.K)
                                    .SetLearningRateCalculator(learningRate)
                                    .SetNeighborhoodFunctionCalculator(kernel)
                                    .SetNeighborhoodRadiusCalculator(neighborhoodRadius);

            SOM model = builder.Build();

            Console.WriteLine("Learning Rate Type: {0}", learningRate.GetType().Name);
            Console.WriteLine("Neighborhood Radius Type: {0}", neighborhoodRadius.GetType().Name);
            Console.WriteLine("Neighborhood Function Type: {0}", kernel.GetType().Name);

            // Subscribe to OnTrainingEvent
            model.Training += _model_Training;

            // Instantiate the reader 
            string[] ignoreColumns = config.Labels.Split(',');
            if (string.IsNullOrWhiteSpace(config.Labels))
            {
                ignoreColumns = new string[0];
            }

            string label = config.FeatureLabel;
            IReader _reader = new CSVReader(config.Dataset, ignoreColumns, label);

            // Instantiate the clusterer
            IClusterer clusterer = new KMeansClustering();

            model.GetData(_reader);
          
            // Set the feature label
            model.FeatureLabel = config.FeatureLabel;

            // Initialize the training
            Stopwatch stopwatch = new Stopwatch();

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

            //if (config.Clusters > 0)
            //{
            //    Console.WriteLine("Start clustering nodes...");

            //    stopwatch.Restart();
            //    IClusterMeasure clusterMetric = new DaviesBouldinIndex();
            //    var flattenedMap = ArrayHelper<Node>.FlattenMap(model.Map);
            //    var clusteredNodes = clusterer.Cluster(flattenedMap, config.Clusters);

            //    double dbi = clusterMetric.MeasureScore(clusteredNodes.ToList(), clusterer.Centroids.ToList());

            //    Console.WriteLine("Davies Bouldin Index: {0}", dbi);

            //    foreach (var node in clusteredNodes)
            //    {
            //        model.Map[node.Coordinate.X, node.Coordinate.Y].ClusterGroup = node.ClusterGroup;
            //    }

            //    stopwatch.Stop();
            //    Console.WriteLine("Completed clustering nodes...");
            //    Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            //    model.AssignClusterLabel();
            //}

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
        /// SSOM Experiments
        /// </summary>
        /// <param name="args"></param>
        public static void SSOM_Experiment(string[] args)
        {
            string filePath = @"C:\Users\User\Desktop\experiment3\aplusix_experiments\som_training\config_aplusix_ssom.json";

            FREQUENCY = 100;
            IS_PRINT_MODEL = false;

            if (args.Length > 0)
            {
                filePath = args[0];
            }

            var disorderScores = new List<Tuple<int, double>>();
            var content = System.IO.File.ReadAllText(filePath);
            var config = ReadToObject(content);

            OUTPUT_LOCATION = config.Export;
            IFileHelper fileHelper = new FileHelper();
            string exportFile = string.Format("{0}disorder-measure.csv", OUTPUT_LOCATION);
            fileHelper.DeleteFile(exportFile);

            fileHelper.WriteToTextFile("Iteration,Disorder Measure,Learning Rate,Radius\n", exportFile);

            // Build the Model
            var learningRate = new LinearLearningRate(config.ConstantLearningRate, config.FinalLearningRate);
            var neighborhoodRadius = new LinearDecayNeighborhoodRadius(config.Neighborhood, config.FinalNeighborhoodRadius);
            var kernel = new GaussianKernel();

            SSOMBuilder builder = new SSOMBuilder()
                                    .SetWidth(config.Width)
                                    .SetHeight(config.Height)
                                    .SetRegions(config.Regions)
                                    .SetInitialLearningRate(config.ConstantLearningRate)
                                    .SetFinalLearningRate(config.FinalLearningRate)
                                    .SetInitialRadius(config.Neighborhood)
                                    .SetFinalRadius(config.FinalNeighborhoodRadius)
                                    .SetEpoch(config.Epoch)
                                    .SetGlobalEpoch(config.GlobalEpoch)
                                    .SetClusters(config.Clusters)
                                    .SetKNeighbor(config.K)
                                    .SetLearningRateCalculator(learningRate)
                                    .SetNeighborhoodFunctionCalculator(kernel)
                                    .SetNeighborhoodRadiusCalculator(neighborhoodRadius);

            SSOM model = builder.Build();

            Console.WriteLine("Learning Rate Type: {0}", learningRate.GetType().Name);
            Console.WriteLine("Neighborhood Radius Type: {0}", neighborhoodRadius.GetType().Name);
            Console.WriteLine("Neighborhood Function Type: {0}", kernel.GetType().Name);

            // Subscribe to OnTrainingEvent
            model.Training += _model_Training;

            // Instantiate the reader 
            string[] ignoreColumns = config.Labels.Split(',');
            if (string.IsNullOrWhiteSpace(config.Labels))
            {
                ignoreColumns = new string[0];
            }

            string label = config.FeatureLabel;
            IReader _reader = new CSVReader(config.Dataset, ignoreColumns, label);

            // Instantiate the clusterer
            IClusterer clusterer = new KMeansClustering();

            model.GetData(_reader);

            // Set the feature label
            model.FeatureLabel = config.FeatureLabel;

            // Initialize the training
            Stopwatch stopwatch = new Stopwatch();

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

            //if (config.Clusters > 0)
            //{
            //    Console.WriteLine("Start clustering nodes...");

            //    stopwatch.Restart();
            //    IClusterMeasure clusterMetric = new DaviesBouldinIndex();
            //    var flattenedMap = ArrayHelper<Node>.FlattenMap(model.Map);
            //    var clusteredNodes = clusterer.Cluster(flattenedMap, config.Clusters);

            //    double dbi = clusterMetric.MeasureScore(clusteredNodes.ToList(), clusterer.Centroids.ToList());

            //    Console.WriteLine("Davies Bouldin Index: {0}", dbi);

            //    foreach (var node in clusteredNodes)
            //    {
            //        model.Map[node.Coordinate.X, node.Coordinate.Y].ClusterGroup = node.ClusterGroup;
            //    }

            //    stopwatch.Stop();
            //    Console.WriteLine("Completed clustering nodes...");
            //    Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            //    model.AssignClusterLabel();
            //}

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

            string trainedModelFile = @"C:\Users\User\Desktop\experiment3\som\Map_experiment_3.json";

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


            string trainingPath = @"C:\Users\User\Desktop\experiment3\male_pleasant_trajectories";
            List<TrajectoryMapper> dbTrajectories = new List<TrajectoryMapper>();

            Console.WriteLine("Start plotting trajectories...");
            stopwatch.Restart();

            string[] labels = { "USER", "SEG", "CLASS_PLEASANTNESS" };
            string feature = "CLASS_PLEASANTNESS";

            foreach (var file in Directory.EnumerateFiles(trainingPath))
            {
                TrajectoryMapper trajectoryMapper = new TrajectoryMapper(_model);
                IReader trajectoryReader = new CSVReader(file, labels, feature);

                trajectoryMapper.GetData(trajectoryReader);
                trajectoryMapper.PlotTrajectory();

                dbTrajectories.Add(trajectoryMapper);

            }

            stopwatch.Stop();
            Console.WriteLine("Completed plotting trajectories...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            string testingPath = @"C:\Users\User\Desktop\experiment3\unknown_trajectory\m_pleasant_22_43.csv";

            ICompression compress = new RunLengthCompression();
            TrajectoryMapper testMapper = new TrajectoryMapper(_model);
            IReader trajectoryDataReader = new CSVReader(testingPath, labels, feature);

            testMapper.GetData(trajectoryDataReader);

            testMapper.PlotTrajectory();
            Console.WriteLine("Unknown Trajectory: {0}", compress.Compress(testMapper.ToString()));
            var unknownTrajectory = testMapper.Trajectories;

            IFileHelper fileHelper = new FileHelper();
            ISimilarityMeasure similarityMeasure = new EditDistanceMeasure();

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

            string fileResultLocation = @"C:\Users\User\Desktop\experiment3\results\trajectory-results.csv";
            fileHelper.WriteToTextFile(header, fileResultLocation);

            foreach (var item in topNScores)
            {
                string result = string.Format("{0},{1},{2},{3}\n", item.Item1, item.Item2, item.Item3, compress.Compress(item.Item4));
                fileHelper.WriteToTextFile(result, fileResultLocation);
                Console.WriteLine("{0}:{1}:{2}:{3}", item.Item1, item.Item2, item.Item3, compress.Compress(item.Item4));
            }

            Console.WriteLine("Training completed...");

            Console.ReadLine();
        }

        /// <summary>
        /// Experiments for SOMphony
        /// </summary>
        /// <param name="args"></param>
        public static void Program5(string[] args)
        {
            // Load model
            string trainedModelFile = @"C:\Users\Vilson\Desktop\Datasets\Music-Dataset\map_somphony.json";

            Console.WriteLine("Loading model...");
            var jsonContent = System.IO.File.ReadAllText(trainedModelFile);
            SOM _model = JsonConvert.DeserializeObject<SOM>(jsonContent);
            _model.AssignClusterLabel();
            _model.Training += _model_Training;
            Console.WriteLine("Model loaded.");

            string trajectoryPath = @"C:\Users\Vilson\Desktop\Datasets\Music-Dataset\trajectories";
            List<TrajectoryMapper> dbTrajectories = new List<TrajectoryMapper>();

            // Plot the trajectories
            foreach (var file in Directory.EnumerateFiles(trajectoryPath))
            {
                TrajectoryMapper trajectoryMapper = new TrajectoryMapper(_model);
                IReader csvReader = new CSVReader(file);
                List<TrajectoryPoint> trajectory = ReadMusicTrajectory(file, _model);

                //trajectoryMapper.SetTrajectory(trajectory, csvReader.FileName);

                dbTrajectories.Add(trajectoryMapper);
            }

            // Setup unknown trajectory
            string testingPath = @"C:\Users\Vilson\Desktop\Datasets\Music-Dataset\unknown_trajectory\music_12.csv";

            ICompression compress = new RunLengthCompression();
            TrajectoryMapper testMapper = new TrajectoryMapper(_model);
            IReader trajectoryDataReader = new CSVReader(testingPath);

            var unknownMapper = ReadMusicTrajectory(testingPath, _model);
            //testMapper.SetTrajectory(unknownMapper, trajectoryDataReader.FileName);

            Console.WriteLine("Unknown Trajectory: {0}", compress.Compress(testMapper.ToString()));
            var unknownTrajectory = testMapper.Trajectories;

            // Measure Similarity
            IFileHelper fileHelper = new FileHelper();
            ISimilarityMeasure similarityMeasure = new EditDistanceMeasure();

            Console.WriteLine("Computing similarity measure using {0}", similarityMeasure.GetType().Name);

            var scores = new List<Tuple<string, double, int, string>>();
            foreach (var trajectory in dbTrajectories)
            {
                var currentTrajectory = trajectory.Trajectories;

                var score = similarityMeasure.MeasureSimilarity(currentTrajectory, unknownTrajectory);
                scores.Add(new Tuple<string, double, int, string>(trajectory.FileName, score, trajectory.Trajectories.Count, trajectory.ToString()));
                Console.WriteLine("{0}:{1}:{2}", trajectory.FileName, score, trajectory.Trajectories.Count);
            }

            // Print Scores
            var topNScores = scores.OrderBy(x => x.Item2);

            Console.WriteLine();
            Console.WriteLine("Trajectories...");

            string header = "Trajectory,Score,TrajectoryCount,SimplifiedSequence\n";

            // Export Scores
            string fileResultLocation = @"C:\Users\Vilson\Desktop\Datasets\Music-Dataset\trajectory-results.csv";
            fileHelper.WriteToTextFile(header, fileResultLocation);

            foreach (var item in topNScores)
            {
                string result = string.Format("{0},{1},{2},{3}\n", item.Item1, item.Item2, item.Item3, compress.Compress(item.Item4));
                fileHelper.WriteToTextFile(result, fileResultLocation);
                Console.WriteLine("{0}:{1}:{2}:{3}", item.Item1, item.Item2, item.Item3, compress.Compress(item.Item4));
            }

            Console.WriteLine("Training completed...");

            Console.ReadLine();

        }

        /// <summary>
        /// Experiment for DBI
        /// </summary>
        /// <param name="args"></param>
        public static void Program6(string[] args)
        {
            string trainedModelFile = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\cluster_experiment\map_dbi.json";

            SOM model = LoadModel(trainedModelFile);

            int maxCluster = 30;
           
            Console.WriteLine("Start clustering nodes...");

            IClusterer clusterer = new KMeansClustering();
            IClusterMeasure clusterMetric = new DaviesBouldinIndex();


            for (int i = 2; i <= maxCluster; i++)
            {
                var flattenedMap = ArrayHelper<Node>.FlattenMap(model.Map);
                var clusteredNodes = clusterer.Cluster(flattenedMap, i);

                double dbi = clusterMetric.MeasureScore(clusteredNodes.ToList(), clusterer.Centroids.ToList());

                Console.WriteLine("{0},{1}", i, dbi);
            }
            

            Console.ReadLine();


        }


        /// <summary>
        /// Experiment for Trajectory Prediction (intrapersonal)
        /// </summary>
        /// <param name="args"></param>
        public static void Program7(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            string trainedModelFile = @"C:\Users\User\Desktop\experiment3\som\Map_experiment_3.json";

            Console.WriteLine("Loading model...");
            var jsonContent = System.IO.File.ReadAllText(trainedModelFile);
            SSOM _model = JsonConvert.DeserializeObject<SSOM>(jsonContent);
            _model.AssignClusterLabel();
            _model.Training += _model_Training;
            Console.WriteLine("Model loaded.");

            string[] users = { "9", "10", "11", "14", "18", "22", "23", "25", "28" };

            foreach (var user in users)
            {
                string trainingPath = string.Format(@"C:\Users\User\Desktop\experiment3\m_pleasant_user{0}", user);
                List<TrajectoryMapper> dbTrajectories = new List<TrajectoryMapper>();

                Console.WriteLine($"User: { user }");
                Console.WriteLine("Start plotting trajectories...");
                stopwatch.Restart();

                string[] labels = { "USER", "SEG", "CLASS_PLEASANTNESS" };
                string feature = "CLASS_PLEASANTNESS";

                foreach (var file in Directory.EnumerateFiles(trainingPath))
                {
                    TrajectoryMapper trajectoryMapper = new TrajectoryMapper(_model);
                    IReader trajectoryReader = new CSVReader(file, labels, feature);

                    trajectoryMapper.GetData(trajectoryReader);
                    trajectoryMapper.PlotTrajectory();

                    dbTrajectories.Add(trajectoryMapper);
                }

                stopwatch.Stop();
                Console.WriteLine("Completed plotting trajectories...");
                Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

                int[] k = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                string experiment = "Window";

                string fileResultLocation = @"C:\Users\User\Desktop\experiment3\other\prediction-results-intrapersonal-3.csv";

                IFileHelper fileHelper = new FileHelper();

                foreach (var item in k)
                {
                    Console.WriteLine($"{ experiment } = { item }");
                    IPredict predictionModel = new DirectPrediction(dbTrajectories);
                    predictionModel.WindowSize = item;
                    predictionModel.K = 17;
                    predictionModel.Steps = 5;

                    int numberofNodes = predictionModel.WindowSize + predictionModel.Steps;

                    string testingPath = trainingPath;

                    if (!fileHelper.IsFileExists(fileResultLocation))
                    {
                        string header = "Trajectory,Expected Prediction,Actual Prediction,K,Steps,Window Size,Score\n";
                        fileHelper.WriteToTextFile(header, fileResultLocation);
                    }

                    foreach (var file in Directory.EnumerateFiles(testingPath))
                    {
                        var unknownTrajectory = new TrajectoryMapper(_model);
                        IReader unknownData = new CSVReader(file, labels, feature);
                        unknownTrajectory.GetData(unknownData);
                        unknownTrajectory.PlotTrajectory();

                        if(unknownTrajectory.Trajectories.Count < 10)
                        {
                            continue;
                        }

                        string expectedResult = unknownTrajectory.ToString();
                        string logExpectedResult = string.Concat(expectedResult.Take(numberofNodes));

                        // Predict the incoming label
                        var result = predictionModel.Predict(unknownTrajectory);

                        // Measure the difference between the predicted and the actual string
                        ISimilarityMeasure scorer = new LevenshteinDistanceMeasure();

                        var expectedPred = result.Trajectories.Take(numberofNodes).ToList();
                        var actualPred = result.PredictedTrajectories.Take(numberofNodes).ToList();

                        var score = scorer.MeasureSimilarity(expectedPred, actualPred);

                        string logs = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\n", unknownData.FileName, logExpectedResult, result.GetPredictedString(), predictionModel.K, predictionModel.Steps, predictionModel.WindowSize, score, experiment);
                        fileHelper.WriteToTextFile(logs, fileResultLocation);
                    }
                }
            }

            Console.WriteLine("Experiment complete...");
            Console.ReadLine();
        }

        /// <summary>
        /// Experiment for Trajectory Prediction (interpersonal)
        /// </summary>
        /// <param name="args"></param>
        public static void Program8(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            string trainedModelFile = @"C:\Users\User\Desktop\experiment3\som\Map_experiment_3.json";

            Console.WriteLine("Loading model...");
            var jsonContent = System.IO.File.ReadAllText(trainedModelFile);
            SSOM _model = JsonConvert.DeserializeObject<SSOM>(jsonContent);
            _model.AssignClusterLabel();
            _model.Training += _model_Training;
            Console.WriteLine("Model loaded.");


            string trainingPath = @"C:\Users\User\Desktop\experiment3\male_pleasant_trajectories";
            List<TrajectoryMapper> dbTrajectories = new List<TrajectoryMapper>();

            Console.WriteLine("Start plotting trajectories...");
            stopwatch.Restart();

            string[] labels = { "USER", "SEG", "CLASS_PLEASANTNESS" };
            string feature = "CLASS_PLEASANTNESS";

            foreach (var file in Directory.EnumerateFiles(trainingPath))
            {
                TrajectoryMapper trajectoryMapper = new TrajectoryMapper(_model);
                IReader trajectoryReader = new CSVReader(file, labels, feature);

                trajectoryMapper.GetData(trajectoryReader);
                trajectoryMapper.PlotTrajectory();

                dbTrajectories.Add(trajectoryMapper);
            }

            stopwatch.Stop();
            Console.WriteLine("Completed plotting trajectories...");
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);

            int[] k = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            string experiment = "Window";

            string fileResultLocation = @"C:\Users\User\Desktop\experiment3\other\prediction-results-interpersonal-3.csv";

            IFileHelper fileHelper = new FileHelper();

            foreach (var item in k)
            {
                Console.WriteLine($"{ experiment } = { item }");
                IPredict predictionModel = new DirectPrediction(dbTrajectories);
                predictionModel.WindowSize = item;
                predictionModel.K = 17;
                predictionModel.Steps = 5;

                int numberofNodes = predictionModel.WindowSize + predictionModel.Steps;

                string testingPath = trainingPath;

                if (!fileHelper.IsFileExists(fileResultLocation))
                {
                    string header = "Trajectory,Expected Prediction,Actual Prediction,K,Steps,Window Size,Error,Experiment\n";
                    fileHelper.WriteToTextFile(header, fileResultLocation);
                }

                foreach (var file in Directory.EnumerateFiles(testingPath))
                {
                    var unknownTrajectory = new TrajectoryMapper(_model);
                    IReader unknownData = new CSVReader(file, labels, feature);
                    unknownTrajectory.GetData(unknownData);
                    unknownTrajectory.PlotTrajectory();

                    if (unknownTrajectory.Trajectories.Count < numberofNodes)
                    {
                        continue;
                    }

                    string expectedResult = unknownTrajectory.ToString();
                    string logExpectedResult = string.Concat(expectedResult.Take(numberofNodes));

                    // Predict the incoming label
                    var result = predictionModel.Predict(unknownTrajectory);

                    // Measure the difference between the predicted and the actual string
                    ISimilarityMeasure scorer = new LevenshteinDistanceMeasure();

                    var expectedPred = result.Trajectories.Take(numberofNodes).ToList();
                    var actualPred = result.PredictedTrajectories.Take(numberofNodes).ToList();

                    var score = scorer.MeasureSimilarity(expectedPred, actualPred);

                    string logs = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\n", unknownData.FileName, expectedResult, result.GetPredictedString(), predictionModel.K, predictionModel.Steps, predictionModel.WindowSize, score, experiment);
                    fileHelper.WriteToTextFile(logs, fileResultLocation);
                }
            }

            Console.WriteLine("Experiment complete...");
            Console.ReadLine();
        }

        /// <summary>
        /// Experiment on synthetic data (prediction)
        /// </summary>
        /// <param name="args"></param>
        public static void Program9(string[] args)
        {
            string[] samples = { "AAAAABBBBB", "AAABBBBBBAAAAAA", "AAAABBBBCCCC", "AAAABCBABBCCABAA", "ABABACCAABBAAABB" };
            string fileResultLocation = @"C:\Users\User\Desktop\experiment3\results\prediction-results-synthetic-fixed.csv";
            string experiment = "Window Size";
            IFileHelper fileHelper = new FileHelper();
            foreach (var unknownSample in samples)
            {
                Console.WriteLine($"Unknown Trajectory: {unknownSample}");
                IGenerate generator = new SynthethicDataGenerator();

                var unknown = unknownSample.ToCharArray().ToList();

                var unknownTrajectory = new TrajectoryMapper();
                unknown.ForEach(x => unknownTrajectory.AddTrajectory(new Node(x.ToString())));

                var dataset = new List<TrajectoryMapper>();
                int[] thresholds = { 2, 6, 10, 20 };

                thresholds.ToList().ForEach(x => dataset.AddRange(generator.GenerateData(unknownSample, 0, x)));

                IPredict prediction = new DirectPrediction(dataset);
                prediction.WindowSize = 9;
                prediction.K = 17;
                prediction.Steps = 5;

                var result = prediction.Predict(unknownTrajectory);

                var actualResult = result.GetPredictedString();
                var expectedResult = result.ToString();

                ISimilarityMeasure scorer = new LevenshteinDistanceMeasure();

                Console.WriteLine($"Predicted Results: {actualResult}");
                Console.WriteLine($"Expected Results: {expectedResult}");

                int numberofNodes = prediction.WindowSize + prediction.Steps;
                var expectedPred = result.Trajectories.Take(numberofNodes).ToList();
                var actualPred = result.PredictedTrajectories.Take(numberofNodes).ToList();

                var score = scorer.MeasureSimilarity(expectedPred, actualPred);
                Console.WriteLine($"Edit Distance Score: {score}");

                if (!fileHelper.IsFileExists(fileResultLocation))
                {
                    string header = "Trajectory,Actual Prediction,K,Steps,Window Size,Score\n";
                    fileHelper.WriteToTextFile(header, fileResultLocation);
                }

                string logs = string.Format("{0},{1},{2},{3},{4},{5},{6}\n", unknownSample, expectedResult, prediction.K, prediction.Steps, prediction.WindowSize, score, experiment);
                fileHelper.WriteToTextFile(logs, fileResultLocation);


            }
            
            Console.ReadLine();
        }

        #region Helpers
        private static void _model_Training(object sender, OnTrainingEventArgs args)
        {
            //ClearLastLine();
            var currentIteration = args.CurrentIteration;
            //Console.WriteLine("Current Iteration: {0} / {1}", currentIteration + 1, args.TotalIteration);

            var model = sender as SOM;

            if (model == null)
            {
                return;
            }

            IFileHelper fileHelper = new FileHelper();

            IMetric disorderMetric = new DisorderMeasure();
            if (currentIteration % FREQUENCY == 0)
            {
                var score = disorderMetric.Measure(model);
                string record = string.Format("{0},{1},{2},{3}\n", currentIteration, score, model.LearningRateDisplay, model.RadiusDisplay);
                string outputFile = string.Format("{0}disorder-measure.csv", OUTPUT_LOCATION);

                Console.WriteLine(string.Format("Iteration {0}: {1}, Learning Rate: {2}, Radius: {3}", currentIteration, score, model.LearningRateDisplay, model.RadiusDisplay));
                fileHelper.WriteToTextFile(record, outputFile);

                if (IS_PRINT_MODEL)
                {
                    string mapJson = JsonConvert.SerializeObject(model, Formatting.Indented);
                    string mapName = string.Format("{0}map_iteration_{1}.json", OUTPUT_LOCATION, currentIteration);
                    fileHelper.WriteToTextFile(mapJson, mapName);
                }

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

        public static SOM LoadModel(string file)
        {
            Console.WriteLine("Loading model...");
            var jsonContent = System.IO.File.ReadAllText(file);
            SSOM _model = JsonConvert.DeserializeObject<SSOM>(jsonContent);
            Console.WriteLine("Model loaded.");

            return _model;

        }

        public static Config ReadToObject(string json)
        {
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(json);
            return config;
        }

        public static SOM ReadSomphony(string filePath)
        {
            SOM som = new SOM();
            Node[,] map = new Node[16, 16];


            string[] headers;
            using (var csv = new CsvReader(new StreamReader(filePath), true))
            {
                headers = csv.GetFieldHeaders();

                int fieldCount = csv.FieldCount;

                while (csv.ReadNextRecord())
                {
                    int x = Int32.Parse(csv[0]);
                    int y = Int32.Parse(csv[1]);
                    int clusters = Int32.Parse(csv[2]);
                    string label = csv[2];

                    Node node = new Node(null, x, y);
                    node.ClusterGroup = clusters;
                    node.Label = label;

                    map[x, y] = node;
                }
            }

            som.Map = map;

            return som;
        }

        public static List<TrajectoryPoint> ReadMusicTrajectory(string filePath, SOM model)
        {
            List<TrajectoryPoint> trajectories = new List<TrajectoryPoint>();
            using (var csv = new CsvReader(new StreamReader(filePath), true))
            {
                int fieldCount = csv.FieldCount;

                while (csv.ReadNextRecord())
                {
                    int x = Int32.Parse(csv[3]);
                    int y = Int32.Parse(csv[4]);

                    Node node = model.Map[y, x];

                    TrajectoryPoint trajectory = new TrajectoryPoint()
                    {
                        Node = node
                    };

                    trajectories.Add(trajectory);
                }
            }

            return trajectories;
        }

        private static void DisplayWinningNodeCount(SOM model)
        {
            for(int i = 0; i < model.Width; i++)
            {
                for(int j = 0; j < model.Height; j++)
                {
                    var node = model.Map[i, j].Count;
                    Console.Write(string.Format("{0}\t", node));
                }

                Console.WriteLine();
            }

        }

        private static void CreateSyntheticDataset()
        {
            IFileHelper fileHelper = new FileHelper();

            string outputPath = @"C:\Users\Vilson\Desktop\Datasets\Kalaw-Dataset\som_experiment\synthetic_xy.csv";

            fileHelper.WriteToTextFile("x,y\n", outputPath);

            // Region 1
            Random rndx1 = new Random();
            Random rndy1= new Random();

            for(int i = 0; i < 2500; i++)
            {
                double x = rndx1.NextDouble() * 0.25;
                double y = rndy1.NextDouble() * 0.40;
                string record = string.Format("{0},{1}\n", x, y);
                fileHelper.WriteToTextFile(record, outputPath);
            }

            // Region 2
            Random rndx2 = new Random();
            Random rndy2 = new Random();

            for (int i = 0; i < 2500; i++)
            {
                double x = rndx2.NextDouble() * 0.25;
                double y = rndy2.NextDouble() * 0.40 + 0.80;
                string record = string.Format("{0},{1}\n", x, y);
                fileHelper.WriteToTextFile(record, outputPath);
            }

            // Region 3
            Random rndx3 = new Random();
            Random rndy3 = new Random();

            for (int i = 0; i < 2500; i++)
            {
                double x = rndx3.NextDouble() * 0.25 + 0.75;
                double y = rndy3.NextDouble() * 0.40;
                string record = string.Format("{0},{1}\n", x, y);
                fileHelper.WriteToTextFile(record, outputPath);
            }

            // Region 4
            Random rndx4 = new Random();
            Random rndy4 = new Random();

            for (int i = 0; i < 2500; i++)
            {
                double x = rndx4.NextDouble() * 0.25 + 0.75;
                double y = rndy4.NextDouble() * 0.40 + 0.80;
                string record = string.Format("{0},{1}\n", x, y);
                fileHelper.WriteToTextFile(record, outputPath);
            }
        }
        #endregion
    }
}
