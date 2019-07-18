using MinimumEditDistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace ML.TrajectoryAnalysis.Implementation
{
    public class EditDistanceMeasure : ISimilarityMeasure
    {
        public double MeasureSimilarity(List<Trajectory> a, List<Trajectory> b)
        {
            string trajectoryA = ConcatenateNodeId(a);
            string trajectoryB = ConcatenateNodeId(b);
            return (double) Levenshtein.CalculateDistance(trajectoryA, trajectoryB, 2);
        }

        private string ConcatenateNodeId(List<Trajectory> trajectory)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var item in trajectory)
            {
                builder.Append(item.Node.NodeId);
            }

            return builder.ToString();
        }
    }
}
