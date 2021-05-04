using ML.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.TrajectoryAnalysis.Implementation
{
    public class CompressionDissimilarityMeasure : ISimilarityMeasure
    {

        private IFileHelper _fileHelper;

        public CompressionDissimilarityMeasure(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public bool IsLowest => true;

        public double MeasureSimilarity(List<TrajectoryPoint> a, List<TrajectoryPoint> b)
        {
            string directoryPath = _fileHelper.GetCurrentDirectory();

            StringBuilder contentA = new StringBuilder();
            string fileNameA = "FileA";
            string textFileA = fileNameA + ".txt";
            string filePathA = directoryPath + @"\" + fileNameA + ".txt";
            string zipPathA = directoryPath + fileNameA + ".zip";
            foreach (var item in a)
            {
                contentA.AppendLine(item.Node.ToString());
            }

            _fileHelper.WriteToTextFile(contentA.ToString(), filePathA);
            _fileHelper.CompressFile(filePathA, zipPathA, textFileA);

            long sizeA = _fileHelper.GetFileSize(zipPathA);


            StringBuilder contentB = new StringBuilder();
            string fileNameB = "FileB";
            string textFileB = fileNameB + ".txt";
            string filePathB = directoryPath + fileNameB + ".txt";
            string zipPathB = directoryPath + fileNameB + ".zip";
            foreach (var item in b)
            {
                contentB.AppendLine(item.Node.ToString());
            }

            _fileHelper.WriteToTextFile(contentB.ToString(), filePathB);
            _fileHelper.CompressFile(filePathB, zipPathB, textFileB);
            long sizeB = _fileHelper.GetFileSize(zipPathB);


            List<TrajectoryPoint> combinedList = a.Concat(b).ToList();

            StringBuilder contentAB = new StringBuilder();
            string fileNameAB = "FileAB";
            string textFileAB = fileNameAB + ".txt";
            string filePathAB = directoryPath + fileNameAB + ".txt";
            string zipPathAB = directoryPath + fileNameAB + ".zip";
            foreach (var item in combinedList)
            {
                contentAB.AppendLine(item.Node.ToString());
            }

            _fileHelper.WriteToTextFile(contentAB.ToString(), filePathAB);
            _fileHelper.CompressFile(filePathAB, zipPathAB, textFileAB);
            long sizeAB = _fileHelper.GetFileSize(zipPathAB);


            double similarityMeasureScore = (double) sizeAB / (double) (sizeA + sizeB);

            // Delete the files after calculating the measure
            _fileHelper.DeleteFile(filePathA);
            _fileHelper.DeleteFile(filePathB);
            _fileHelper.DeleteFile(filePathAB);
            _fileHelper.DeleteFile(zipPathA);
            _fileHelper.DeleteFile(zipPathB);
            _fileHelper.DeleteFile(zipPathAB);

            return similarityMeasureScore;
        }


    }
}
