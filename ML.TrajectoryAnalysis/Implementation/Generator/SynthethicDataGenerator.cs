using LumenWorks.Framework.IO.Csv;
using SOMLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis.Implementation.Generator
{
    public class SynthethicDataGenerator : IGenerate
    {
        public void AddToDataset(IList<TrajectoryMapper> dataset, TrajectoryMapper trajectory)
        {
            dataset.Add(trajectory);
        }

        public List<TrajectoryMapper> GenerateDataset(string filePath)
        {
            List<TrajectoryMapper> database = new List<TrajectoryMapper>();
            using (var csv = new CsvReader(new StreamReader(filePath), true))
            {
                string[] headers = csv.GetFieldHeaders();

                while(csv.ReadNextRecord()) {
                    var value = csv[0].ToCharArray().ToList();
                    var currentTrajectory = new TrajectoryMapper();

                    value.ForEach(x => currentTrajectory.AddTrajectory(new Node(x.ToString())));

                    AddToDataset(database, currentTrajectory);
       
                }
            }

            return database;
        }
    }
}
