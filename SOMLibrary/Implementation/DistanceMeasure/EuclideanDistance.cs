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
            return Accord.Math.Distance.Euclidean(a, b);
        }
    }
}
