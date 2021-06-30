using SOMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ML.TrajectoryAnalysis.Implementation.Prediction
{
    public class DirectPrediction : IPredict
    {
        /// <summary>
        /// Number of nodes to be considered for prediction
        /// </summary>
        public int WindowSize { get; set; }

        /// <summary>
        /// Number of predictions
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// Number of similar trajectories to consider
        /// </summary>
        public int K { get; set; }

        /// <summary>
        /// Contains the database of trajectories 
        /// </summary>
        public List<TrajectoryMapper> TrajectoryDb { get; set; }

        /// <summary>
        /// Similarity Measure used to get the most similar trajectories
        /// </summary>
        public ISimilarityMeasure SimilarityMeasure { get; set; }

        /// <summary>
        /// Default Settings
        /// </summary>
        public DirectPrediction()
        {
            WindowSize = 5;
            Steps = 5;
            SimilarityMeasure = new EditDistanceMeasure();
            K = 3;
        }

        public DirectPrediction(List<TrajectoryMapper> dbTrajectory) : this()
        {
            TrajectoryDb = dbTrajectory;
        }

        public TrajectoryMapper Predict(TrajectoryMapper currentTrajectory)
        {
            // Step 1: Get the last n node of the trajectory that you want to predict (n = window size)
            currentTrajectory.PredictedTrajectories = GetFirstTrajectory(currentTrajectory, WindowSize);
            var lastIndex = currentTrajectory.PredictedTrajectories.Count;

            // Step 2: Get the most similar trajectory based on the partial trajectory
            var filteredDBTrajectories = TrajectoryDb;
            if (!string.IsNullOrEmpty(currentTrajectory.FileName))
            {
                filteredDBTrajectories = TrajectoryDb.Where(x => x.FileName != currentTrajectory.FileName).ToList();
            }

            var check = TrajectoryDb.Where(x => x.FileName == currentTrajectory.FileName).ToList();
            var similarTrajectories = GetSimilarTrajectory(filteredDBTrajectories, currentTrajectory.PredictedTrajectories, K);

            //similarTrajectories.ForEach(x => Console.WriteLine($"Similar Trajectories: {x.ToString()}"));

            // Step 3: Based on the similar trajectory, get the majority of the state (that will be label of the predicted node)
            // Step 4: Repeat step 3 until number of steps (prediction) has been achieved.

            int numOfSteps = this.Steps;
            for (int i = 0; i < numOfSteps; i++)
            {
                List<string> labels = new List<string>();
                foreach(var trajectory in similarTrajectories)
                {
                    var node = GetNodeAtIndex(trajectory, lastIndex + i);
                    if(node != null)
                    {
                        labels.Add(node.Node.ClusterLabel);
                    }
                }

                // get the majority of the labels for that step
                if(labels.Count > 0)
                {
                    var winner = labels.GroupBy(x => x).OrderByDescending(g => g.Count()).Select(x => x.Key).First();
                    Node predictedNode = new Node(winner);
                    currentTrajectory.AddPredictedTrajectory(predictedNode);
                }
 
            }

            return currentTrajectory;
        }


        /// <summary>
        /// Get the last n elements of the trajectory
        /// </summary>
        /// <param name="trajectory"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<TrajectoryPoint> GetLastTrajectory(TrajectoryMapper trajectory, int n)
        {
            var predictTrajectory = trajectory.Trajectories;
            int range = predictTrajectory.Count - n;
            var partialTrajectory = predictTrajectory.GetRange(range, n);

            return partialTrajectory;
        }

        /// <summary>
        /// Get the first n elements of the trajectory
        /// </summary>
        /// <param name="trajectory"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<TrajectoryPoint> GetFirstTrajectory(TrajectoryMapper trajectory, int n)
        {
            var predictTrajectory = trajectory.Trajectories;

            int length = n;
            if(trajectory.Trajectories.Count < n)
            {
                length = trajectory.Trajectories.Count;
            }

            var partialTrajectory = predictTrajectory.GetRange(0, length);

            return partialTrajectory;
        }


        /// <summary>
        /// Get the most similar trajectory based on K
        /// </summary>
        /// <param name="dbTrajectory"></param>
        /// <param name="currentTrajectory"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<TrajectoryMapper> GetSimilarTrajectory(List<TrajectoryMapper> dbTrajectory, List<TrajectoryPoint> currentTrajectory, int n)
        {
            var trajectories = new List<Tuple<TrajectoryMapper, double>>();

            foreach (var item in dbTrajectory)
            {
                double score = SimilarityMeasure.MeasureSimilarity(currentTrajectory, item.Trajectories);
                trajectories.Add(new Tuple<TrajectoryMapper, double>(item, score));
            }

            var distinctTrajectories = trajectories.Distinct().ToList();

            List<TrajectoryMapper> similarTrajectories;
            if (SimilarityMeasure.IsLowest)
            {
                similarTrajectories = distinctTrajectories.OrderBy(x => x.Item2).Take(n).Select(x => x.Item1).ToList();
            }
            else
            {
                similarTrajectories = distinctTrajectories.OrderByDescending(x => x.Item2).Take(n).Select(x => x.Item1).ToList();
            }

            return similarTrajectories;
        }

        private TrajectoryPoint GetNodeAtIndex(TrajectoryMapper trajectory, int index)
        {
            var count = trajectory.Trajectories.Count - 1;

            if(index > count)
            {
                return null;
            }

            return trajectory.Trajectories[index];
        }
    }
}
