using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis
{
    public interface IGenerate
    {
        List<TrajectoryMapper> GenerateData(string sequence, int n, int threshold);
        List<TrajectoryMapper> GenerateDataset(string filepath);

        void AddToDataset(IList<TrajectoryMapper> dataset, TrajectoryMapper trajectory);
    }
}
