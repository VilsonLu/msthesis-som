using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.DistanceMeasure
{
    public class CosineSimilarityMeasure : IDistanceMeasure
    {
        public double GetDistance(double[] a, double[] b)
        {
            double sum = 0.0;
            double aSum = 0.0;
            double bSum = 0.0;
            int length = a.Length;

            object locker = new object();

            Parallel.For(0, length, (i) =>
            {
                double tempSum = a[i] * b[i];
                double tempASum = a[i] * a[i];
                double tempBSum = b[i] * b[i];
                lock (locker)
                {
                    sum += tempSum;
                    aSum += tempASum;
                    bSum += tempBSum;
                }
            });

            double den = Math.Sqrt(aSum) + Math.Sqrt(bSum);
            double distance = sum / (den);

            return distance;
        }
    }
}
