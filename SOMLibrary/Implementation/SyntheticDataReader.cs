using LumenWorks.Framework.IO.Csv;
using SOMLibrary.DataModel;
using SOMLibrary.Interface;
using System;
using System.IO;

namespace SOMLibrary.Implementation
{
    public class SyntheticDataReader : IReader
    {
        private string _filePath;
        public string FileName
        {
            get
            {
                return Path.GetFileName(_filePath);
            }
        }

        public SyntheticDataReader(string _filePath)
        {
          
        }

        public Dataset Read()
        {
            using (var csv = new CsvReader(new StreamReader(_filePath), true))
            {
                return new Dataset();
            }
        }
    }
}
