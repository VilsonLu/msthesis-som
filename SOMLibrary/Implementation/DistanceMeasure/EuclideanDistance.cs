using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.DistanceMeasure
{
    public class EuclideanDistance : IDistanceMeasure
    {
        public double GetDistance(double[] a, double[] b)
        {
            double sum = 0;

            for (int i = 0; i < a.Length; i++)
            {
                sum += Math.Pow(a[i] - b[i], 2);
            }

            return Math.Sqrt(sum);
        }
    }
}
