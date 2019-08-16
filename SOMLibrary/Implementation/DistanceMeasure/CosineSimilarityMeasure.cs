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

            for(int i = 0; i < length; i++)
            {
                sum += a[i] * b[i];
                aSum += Math.Pow(a[i], 2);
                bSum += Math.Pow(b[i], 2);
            }

            double distance = (double)sum / (Math.Sqrt(aSum) + Math.Sqrt(bSum));

            return distance;
        }
    }
}
