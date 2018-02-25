using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary.DataModel;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace SOMLibrary.Implementation
{
    public class CSVReader : IReader
    {
        private string _filePath;

        public CSVReader(string filepath)
        {
            _filePath = filepath;
        }

        public Dataset Read()
        {
            Dataset dataset = new Dataset();
            using (CsvReader csv = new CsvReader(new StreamReader(_filePath), true))
            {
                dataset.Features = GetHeaders(csv.GetFieldHeaders());
                dataset.Instances = GetInstances(csv);
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
            var headers = new Feature[fieldHeaders.Length];
            for (int i = 0; i < fieldHeaders.Length; i++)
            {
                var feature = new Feature()
                {
                    OrderNo = i,
                    FeatureName = fieldHeaders[i],
                    IsLabel = false
                };

                headers[i] = feature;
            }

            return headers;
        }


        /// <summary>
        /// Retrieve the data (row) from CSV
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        private Instance[] GetInstances(CsvReader csv)
        {
            List<Instance> instances = new List<Instance>();
            int fieldCount = csv.FieldCount;

            int rowNumber = 0;


            while (csv.ReadNextRecord())
            {
                object[] values = new object[fieldCount];
                for (int i = 0; i < fieldCount; i++)
                {
                    values[i] = (object)csv[i];
                }

                var instance = new Instance()
                {
                    OrderNo = rowNumber,
                    Values = values
                };

                instances.Add(instance);
                rowNumber++;
            }

            return instances.ToArray();
        }
    }
}
