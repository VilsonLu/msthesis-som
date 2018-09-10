using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void CompressFile(string sourcePath, string destinationPath)
        {
            using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(File.Create(destinationPath)))
            {
                zipStream.SetLevel(1);

                byte[] buffer = new byte[4096];
                ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(System.IO.Path.GetFileName(sourcePath));

                entry.DateTime = DateTime.Now;
                zipStream.PutNextEntry(entry);

                using (FileStream fs = File.OpenRead(sourcePath))
                {
                    int sourceBytes = 0;
                    do
                    {
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        zipStream.Write(buffer, 0, sourceBytes);
                    } while (sourceBytes > 0);
                }

                zipStream.Finish();
                zipStream.Close();
                zipStream.Dispose();
            }
        }


    }
}
