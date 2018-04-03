using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis.Implementation
{
    public interface ITrajectoryMapper
    {
        List<Trajectory> GetTrajectories();
    }
}
