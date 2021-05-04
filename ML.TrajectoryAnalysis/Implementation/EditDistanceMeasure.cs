using MinimumEditDistance;
using ML.Common;
using ML.Common.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ML.TrajectoryAnalysis.Implementation
{
    public class EditDistanceMeasure : ISimilarityMeasure
    {
        private ICompression _compression;

        public EditDistanceMeasure()
        {
            _compression = new RunLengthCompression();
        }

        public bool IsLowest => true;

        public double MeasureSimilarity(List<TrajectoryPoint> a, List<TrajectoryPoint> b)
        {
            // Concatenate the cluster group of the trajectories
            string processA = Concatenate(a);
            string processB = Concatenate(b);

            // Get the canonical form of the concatenated strings
            string trajectoryA = _compression.Compress(processA);
            string trajectoryB = _compression.Compress(processB);

            // Calculate the edit-distance based on the canonical form
            return (double) Levenshtein.CalculateDistance(trajectoryA, trajectoryB, 2);
        }

        private string Concatenate(List<TrajectoryPoint> trajectory)
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
