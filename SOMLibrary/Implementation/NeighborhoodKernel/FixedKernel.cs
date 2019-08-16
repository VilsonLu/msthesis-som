using SOMLibrary.Interface;

namespace SOMLibrary.Implementation.NeighborhoodKernel
{
    public class FixedKernel : INeighborhoodKernel
    {
        public double CalculateNeighborhoodFunction(double distance, double radius)
        {
            return 1.0;
        }
    }
}
