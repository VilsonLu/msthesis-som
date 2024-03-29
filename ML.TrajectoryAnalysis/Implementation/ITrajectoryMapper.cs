﻿using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis.Implementation
{
    public interface ITrajectoryMapper
    {
        void GetData(IReader reader);

        List<Trajectory> GetTrajectories();
    }
}
