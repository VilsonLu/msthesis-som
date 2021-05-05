using LumenWorks.Framework.IO.Csv;
using MinimumEditDistance;
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

        public List<TrajectoryMapper> GenerateData(string sequence, int n, int threshold)
        {
            var variants = new List<TrajectoryMapper>();
            char[] word = sequence.ToCharArray();

            var replacements = Enumerable.Range('A', 4).Select(x => (char)x).ToArray();

            var sequences = new List<string>();
            int length = word.Length - threshold;

            // generate strings - deletion
            for(int i = 0; i < length; i++)
            {
                var generatedString = sequence.Remove(i, threshold).Insert(i, "");
                var score = (double)Levenshtein.CalculateDistance(sequence, generatedString, 2);
                sequences.Add(generatedString);
            }

            // generate strings - insert/substitution
            foreach(var r in replacements)
            {
                for(int i = 0; i < word.Length; i++)
                {
                    var generatedString = sequence;
                    int numOfSubstitution = threshold / 2;
                    
                    // substitution
                    int x = 0;
                    int countSubstitution = 0;
                    do
                    {
                        if (i + x < word.Length - 1 && sequence[i + x] != r)
                        {
                            generatedString = generatedString.Remove(i + x, 1).Insert(i + x, r.ToString());
                            countSubstitution++;
                        }
                        x++;
                    } while (countSubstitution < numOfSubstitution && i + x < word.Length - 1);

                    if(generatedString != sequence)
                    {
                        var score = (double)Levenshtein.CalculateDistance(sequence, generatedString, 2);
                        sequences.Add(generatedString);
                    }

                    // insert
                    generatedString = sequence;
                    for(int a = 0; a < threshold; a++)
                    {
                        generatedString = generatedString.Insert(i + a, r.ToString());
                    }

                    sequences.Add(generatedString);

                }
            }

            var distinctVariants = sequences.Distinct().ToList();

            foreach(var g in distinctVariants)
            {
                var value = g.ToCharArray().ToList();
                var currentTrajectory = new TrajectoryMapper();
                value.ForEach(x => currentTrajectory.AddTrajectory(new Node(x.ToString())));

                variants.Add(currentTrajectory);
            }

            return variants;
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
