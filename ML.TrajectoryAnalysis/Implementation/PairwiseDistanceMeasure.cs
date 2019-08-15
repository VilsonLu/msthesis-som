using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis.Implementation
{
    public class PairwiseDistanceMeasure : ISimilarityMeasure
    {
        public bool IsLowest => false;

        public double MeasureSimilarity(List<Trajectory> a, List<Trajectory> b)
        {
            int length = a.Count <= b.Count ? a.Count : b.Count;

            int count = 0;
            for(int i = 0; i < length; i++)
            {
                if(a[i].Node.ClusterLabel == b[i].Node.ClusterLabel)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
