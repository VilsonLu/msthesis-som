using SOMLibrary.Interface;

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
