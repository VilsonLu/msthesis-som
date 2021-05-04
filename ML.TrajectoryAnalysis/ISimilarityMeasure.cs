using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis
{
    public interface ISimilarityMeasure
    {
        bool IsLowest { get; }
        double MeasureSimilarity(List<TrajectoryPoint> a, List<TrajectoryPoint> b);
    }
}
