using MinimumEditDistance;
using System;
using System.Collections.Generic;

namespace ML.TrajectoryAnalysis.Implementation
{
    public class EditDistanceMeasure : ISimilarityMeasure
    {
        public double MeasureSimilarity(List<Trajectory> a, List<Trajectory> b)
        {
            return (double) Levenshtein.CalculateDistance("a", "b", 2);
        }
    }
}
