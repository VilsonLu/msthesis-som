using MinimumEditDistance;
using ML.Common;
using ML.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ML.TrajectoryAnalysis.Implementation
{
    public class LevenshteinDistanceMeasure : ISimilarityMeasure
    {

        public LevenshteinDistanceMeasure()
        {
        }

        public bool IsLowest => true;

        public double MeasureSimilarity(List<Trajectory> a, List<Trajectory> b)
        {
            // Concatenate the cluster group of the trajectories
            string processA = Concatenate(a);
            string processB = Concatenate(b);

            // Calculate the edit-distance based on the canonical form
            return (double) Levenshtein.CalculateDistance(processA, processB, 2);
        }

        private string Concatenate(List<Trajectory> trajectory)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var item in trajectory)
            {
                builder.Append(item.Node.ClusterLabel);
            }

            return builder.ToString();
        }
    }
}
