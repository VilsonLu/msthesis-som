using ML.TrajectoryAnalysis.Implementation;
using SOMLibrary;
using SOMLibrary.DataModel;
using SOMLibrary.Interface;
using System.Collections.Generic;

namespace ML.TrajectoryAnalysis
{
    public class TrajectoryMapper : ITrajectoryMapper
    {
        private readonly SOM _som;

        private Dataset _dataset;

        public List<Trajectory> Trajectories { get; }

        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }

            private set
            {
                _fileName = value;
            }
        }

        public TrajectoryMapper(SOM som)
        {
            _som = som;
            Trajectories = new List<Trajectory>();
        }

        /// <summary>
        /// Read the data from the source
        /// </summary>
        /// <param name="reader"></param>
        public void GetData(IReader reader)
        {
            _dataset = reader.Read();
            FileName = reader.FileName;
        }

        /// <summary>
        /// Plot the time-series data in the map
        /// </summary>
        public void PlotTrajectory()
        {
            foreach (var instance in _dataset.Instances)
            {
                var node = _som.FindBestMatchingUnit(instance);

                Trajectories.Add(new Trajectory()
                {
                    Instance = instance,
                    Node = node
                });
            }
        }

    }
}
