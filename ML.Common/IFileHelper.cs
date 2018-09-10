using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Common
{
    public interface IFileHelper
    {
        string GetCurrentDirectory();
        long GetFileSize(string filePath);
        void WriteToTextFile(string content, string fileName);
        void DeleteFile(string filePath);
        void CompressFile(string sourcePath, string destinationPath);
    }
}
