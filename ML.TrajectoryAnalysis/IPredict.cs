using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis
{
    public interface IPredict
    {
        int K { get; set; }

        int WindowSize { get; set; }

        int Steps { get; set; }

        TrajectoryMapper Predict(TrajectoryMapper currentTrajectory);
    }
}
