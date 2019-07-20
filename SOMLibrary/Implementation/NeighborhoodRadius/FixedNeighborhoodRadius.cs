using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.NeighborhoodRadius
{
    public class FixedNeighborhoodRadius : INeighborhoodRadius
    {
        private double _neigborhoodRadius;

        public FixedNeighborhoodRadius(double neighborhoodRadius)
        {
            _neigborhoodRadius = neighborhoodRadius;
        }

        public double CalculateRadius(int iteration)
        {
            return _neigborhoodRadius;
        }
    }
}
