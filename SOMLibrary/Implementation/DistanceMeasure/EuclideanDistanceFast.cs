using SOMLibrary.Interface;
using System;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.DistanceMeasure
{
    public class EuclideanDistanceFast : IDistanceMeasure
    {
        public double GetDistance(double[] a, double[] b)
        {
            int lengthA = a.Length;
            int lengthB = b.Length;

            object lockObject = new object();

            if(lengthA != lengthB)
            {
                throw new Exception("Inputs must have the same length");
            }

            double sum = 0;
            Parallel.For(0, lengthA, ctr =>
            {
                double partialSum = Math.Pow(a[ctr] - b[ctr], 2);

                lock (lockObject)
                {
                    sum += partialSum;
                }
            });

            double euclideanDistance = Math.Sqrt(sum);

            return euclideanDistance;
        }
    }
}
