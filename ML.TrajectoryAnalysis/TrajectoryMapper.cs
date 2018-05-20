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

        public List<Trajectory> Trajectories { get; set; }

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
        }

        /// <summary>
        /// Gets the best matching node for each instance in the dataset
        /// </summary>
        /// <returns>List of trajectories</returns>
        public List<Trajectory> GetTrajectories()
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

            return Trajectories;
        }
    }
}
