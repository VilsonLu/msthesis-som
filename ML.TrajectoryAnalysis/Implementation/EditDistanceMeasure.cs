using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis.Implementation
{
    public class EditDistanceMeasure : ISimilarityMeasure
    {
        public double MeasureSimilarity(List<Trajectory> a, List<Trajectory> b)
        {
            string word1 = "minimum";
            string word2 = "maximum";

           return (double) MinimumEditDistance.Levenshtein.CalculateDistance(word1, word2, 2);
        }
    }
}
