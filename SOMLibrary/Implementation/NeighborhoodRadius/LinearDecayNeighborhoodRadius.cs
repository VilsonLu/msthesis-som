using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.NeighborhoodRadius
{
    /// <summary>
    /// Implementation of Linear Decay Neighborhood Radius using slopes
    /// </summary>
    public class LinearDecayNeighborhoodRadius : INeighborhoodRadius
    {

        private double _initialRadius;
        private double FINAL_MAP_RADIUS = 1;
        public LinearDecayNeighborhoodRadius(double mapRadius)
        {
            _initialRadius = mapRadius;
        }

        public double CalculateRadius(int iteration, int totalIteration)
        {
            double b = _initialRadius;
            double x = iteration;
            double m = (_initialRadius - FINAL_MAP_RADIUS) / (0.0 - totalIteration);

            double y = Math.Floor(m * x + b);

            if(y < FINAL_MAP_RADIUS)
            {
                y = FINAL_MAP_RADIUS;
            }

            return y;
        }
    }
}
