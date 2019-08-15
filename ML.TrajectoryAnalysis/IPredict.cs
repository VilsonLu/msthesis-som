using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis
{
    public interface IPredict
    {
        TrajectoryMapper Predict(TrajectoryMapper currentTrajectory);
    }
}
