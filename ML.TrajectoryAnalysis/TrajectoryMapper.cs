﻿using ML.TrajectoryAnalysis.Implementation;
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

        public string FeatureLabel { get; set; }

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

        public void SetLabel(string labels)
        {
            _dataset.SetLabel(labels);
        }

        public Node FindBestMatchingUnit(Instance instance)
        {
            double bestDistance = double.MaxValue;
            Node bestNode = null;

            var currentInstance = _dataset.GetInstance<double>(instance.OrderNo);
            for(int i = 0; i < _som.Width; i++)
            {
                for(int j = 0; j < _som.Height; j++)
                {
                    var currentNode = _som.Map[i, j];
                    var currentDistance = currentNode.GetDistance(currentInstance);

                    if(currentDistance < bestDistance)
                    {
                        bestNode = currentNode;
                        bestDistance = currentDistance;
                    }
                }
            }

            return bestNode;
        }

        /// <summary>
        /// Plot the time-series data in the map
        /// </summary>
        public void PlotTrajectory()
        {
            foreach (var instance in _dataset.Instances)
            {
                var node = FindBestMatchingUnit(instance);

                Trajectories.Add(new Trajectory()
                {
                    Instance = instance,
                    Node = node
                });
            }
        }



    }
}
