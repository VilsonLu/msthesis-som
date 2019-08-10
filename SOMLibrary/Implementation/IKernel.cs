using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation
{
    public interface INeighborhoodKernel
    {
        double CalculateNeighborhoodFunction(double distance, double radius);
    }
}
