using System.IO;
using System.IO.Compression;

namespace ML.Common.Implementation
{
    public class FileHelper : IFileHelper
    {
        /// <summary>
        /// Delete the specified file
        /// </summary>
        /// <param name="filePath"></param>
        public void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath)) { return; }

            File.Delete(filePath);
        }

        /// <summary>
        /// Get the current directory
        /// </summary>
        /// <returns></returns>
        public string GetCurrentDirectory()
        {
           return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Returns the file size in bytes
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public long GetFileSize(string filePath)
        {
            long length = new FileInfo(filePath).Length;
            return length;
        }

        /// <summary>
        /// Write the content to a text file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        public void WriteToTextFile(string content, string fileName)
        {
            System.IO.File.AppendAllText(fileName, content);
        }

        public void CompressFile(string sourcePath, string destinationPath, string entryName)
        {
            using (ZipArchive zip = ZipFile.Open(destinationPath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(sourcePath, entryName);
            }

        }


    }
}
