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
        public List<TrajectoryMapper> TrajectoryDb { get; set; }

        public ISimilarityMeasure SimilarityMeasure { get; set; }

        public SOM Model { get; set; }

        /// <summary>
        /// Default Settings
        /// </summary>
        public DirectPrediction()
        {
            WindowSize = 10;
            Steps = 5;
            SimilarityMeasure = new EditDistanceMeasure();
            K = 5;
        }

        public DirectPrediction(SOM model, List<TrajectoryMapper> dbTrajectory) : this()
        {
            Model = model;
            TrajectoryDb = dbTrajectory;
        }

        public TrajectoryMapper Predict(TrajectoryMapper currentTrajectory)
        {

            var lastIndex = currentTrajectory.Trajectories.Count - 1;

            // Step 1: Get the last n node of the trajectory that you want to predict (n = window size)
            var partialTrajectory = GetLastTrajectory(currentTrajectory, WindowSize);

            // Step 2: Get the most similar trajectory based on the partial trajectory
            var similarTrajectories = GetSimilarTrajectory(TrajectoryDb, partialTrajectory, K);

            for (int i = 0; i < Steps; i++)
            {
                List<Trajectory> nodes = new List<Trajectory>();
                foreach(var trajectory in similarTrajectories)
                {
                    var node = GetNodeAtIndex(trajectory, lastIndex);
                    if(node != null)
                    {
                        nodes.Add(node);
                    }
                }


            }

            // Step 3: Based on the similar trajectory, get the majority of the state (that will be label of the predicted node)
            // Step 4: Get the weights by averaging the weights of the nodes.
            // Step 5: Find the BMU on the model based on the predicted node. The BMU will then be the predicted node
            // Step 5: Repeat step 3-5 until number of steps (prediction) has been achieved.

            throw new NotImplementedException();
        }


        /// <summary>
        /// Get the last n elements of the trajectory
        /// </summary>
        /// <param name="trajectory"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<Trajectory> GetLastTrajectory(TrajectoryMapper trajectory, int n)
        {
            var predictTrajectory = trajectory.Trajectories;
            int range = predictTrajectory.Count - n;
            var partialTrajectory = predictTrajectory.GetRange(range, n);

            return partialTrajectory;
        }


        /// <summary>
        /// Get the most similar trajectory based on K
        /// </summary>
        /// <param name="dbTrajectory"></param>
        /// <param name="currentTrajectory"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<TrajectoryMapper> GetSimilarTrajectory(List<TrajectoryMapper> dbTrajectory, List<Trajectory> currentTrajectory, int n)
        {
            var trajectories = new List<Tuple<TrajectoryMapper, double>>();

            foreach (var item in dbTrajectory)
            {
                double score = SimilarityMeasure.MeasureSimilarity(currentTrajectory, item.Trajectories);
                trajectories.Add(new Tuple<TrajectoryMapper, double>(item, score));
            }

            List<TrajectoryMapper> similarTrajectories;
            if (SimilarityMeasure.IsLowest)
            {
                similarTrajectories = trajectories.OrderBy(x => x.Item2).Take(n).Select(x => x.Item1).ToList();
            }
            else
            {
                similarTrajectories = trajectories.OrderByDescending(x => x.Item2).Take(n).Select(x => x.Item1).ToList();
            }

            return similarTrajectories;
        }

        private Trajectory GetNodeAtIndex(TrajectoryMapper trajectory, int index)
        {
            var count = trajectory.Trajectories.Count;

            if(index > count)
            {
                return null;
            }

            return trajectory.Trajectories[index];
        }
    }
}
