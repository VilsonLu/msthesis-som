using LumenWorks.Framework.IO.Csv;
using SOMLibrary.DataModel;
using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SOMLibrary.Implementation
{
    public class CSVReader : IReader
    {
        private readonly string _filePath;
        private string[] _ignoreColumns;
        private string _label;
        private Dictionary<int, string> _ignoreColumnDictionary;
        private int _labelIndex;
        
        public string FileName
        {
            get
            {
                return Path.GetFileName(_filePath);
            }
        }

        public CSVReader(string filepath)
        {
            _filePath = filepath;
            _ignoreColumns = new string[0];
            _labelIndex = -1;
            _ignoreColumnDictionary = new Dictionary<int, string>();
        }

        public CSVReader(string filepath, string[] ignoreColumns, string label)
        {
            _filePath = filepath;
            _ignoreColumns = ignoreColumns;
            _label = label;
            _ignoreColumnDictionary = new Dictionary<int, string>();
            
        }

        public Dataset Read()
        {
            var dataset = new Dataset();
            using (var csv = new CsvReader(new StreamReader(_filePath), true))
            {
                dataset.Features = GetHeaders(csv.GetFieldHeaders());
                dataset.Instances = GetInstances(csv);
                dataset.WeightVectorCount = csv.FieldCount - _ignoreColumns.Length;
            }

            return dataset;
        }

        /// <summary>
        /// Get the headers from CSV file
        /// </summary>
        /// <param name="fieldHeaders"></param>
        /// <returns></returns>
        private Feature[] GetHeaders(string[] fieldHeaders)
        {
            int length = fieldHeaders.Length - _ignoreColumns.Length;

            if(_labelIndex > -1)
            {
                length -= 1;
            }

            List<Feature> features = new List<Feature>();
            for (var i = 0; i < fieldHeaders.Length; i++)
            {
                string fieldHeader = fieldHeaders[i];

                if (_ignoreColumns.Any(x => x == fieldHeader))
                {
                    _ignoreColumnDictionary.Add(i, fieldHeader);
                    continue;
                }

                if(fieldHeader == _label)
                {
                    _labelIndex = i;
                    continue;
                }

                var feature = new Feature()
                {
                    OrderNo = i,
                    FeatureName = fieldHeaders[i],
                    IsLabel = false
                };

                features.Add(feature);
            }

            return features.ToArray();
        }


        /// <summary>
        /// Retrieve the data (row) from CSV
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        private Instance[] GetInstances(CsvReader csv)
        {
            List<Instance> instances = new List<Instance>();
            int fieldCount = csv.FieldCount - _ignoreColumns.Length;

            int rowNumber = 0;
            
            while (csv.ReadNextRecord())
            {
                double[] values = new double[fieldCount];
                string label = string.Empty;
                int currentColumn = 0;
                for (int i = 0; i < fieldCount; i++)
                {
                    // check if the column is in ignore columns
                    // if in ignore columns don't add the value
                    if (_ignoreColumnDictionary.ContainsKey(i))
                    {
                        continue;
                    }

                    // Check if the index is the label
                    if(_labelIndex == i)
                    {
                        label = csv[i];
                    }
                    else
                    {
                        values[currentColumn] = Double.Parse(csv[i]);
                        currentColumn++;
                    }  
                }

                Instance instance = new Instance()
                {
                    OrderNo = rowNumber,
                    Values = values,
                    Label = label
                };

                instances.Add(instance);
                rowNumber++;
            }

            return instances.ToArray();
        }
    }
}
