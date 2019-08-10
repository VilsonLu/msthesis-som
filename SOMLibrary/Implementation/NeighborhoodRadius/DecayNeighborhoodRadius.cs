using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.NeighborhoodRadius
{
    public class DecayNeighborhoodRadius : INeighborhoodRadius
    {

        private double _mapRadius;

        public DecayNeighborhoodRadius(double mapRadius)
        {
            _mapRadius = mapRadius;
        }

        public double CalculateRadius(int iteration, int totalIteration)
        {

            //tex: $$r(t) = \left \lfloor{max(r) - \frac{t}{d}}\right \rfloor $$
       
            double rateOfDecay = CalculateDecay(totalIteration);
            double newRadius = Math.Floor(_mapRadius - (iteration / rateOfDecay));
            return newRadius;
        }

        /// <summary>
        /// Calculate the decay of the radius
        /// </summary>
        /// <returns></returns>
        private double CalculateDecay(int totalIteration)
        {
            //tex: $$d = \frac{max(t)}{max(r) - 1}$$

            return (double) totalIteration / (_mapRadius - 1.0);
        }
    }
}
