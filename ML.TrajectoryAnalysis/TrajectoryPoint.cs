using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary;
using SOMLibrary.DataModel;

namespace ML.TrajectoryAnalysis
{
    public class TrajectoryPoint
    {
        public Node Node { get; set; }
        public Instance Instance { get; set; }
    }
}
