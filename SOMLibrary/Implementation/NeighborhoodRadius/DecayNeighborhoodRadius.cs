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
        private double _totalIteration;

        public DecayNeighborhoodRadius(double mapRadius, double totalIteration)
        {
            _mapRadius = mapRadius;
            _totalIteration = totalIteration;
        }

        public double CalculateRadius(int iteration)
        {

            //tex: $$r(t) = \left \lfloor{max(r) - \frac{t}{d}}\right \rfloor $$
       
            double rateOfDecay = CalculateDecay();
            double newRadius = Math.Floor(_mapRadius - (iteration / rateOfDecay));
            return newRadius;
        }

        /// <summary>
        /// Calculate the decay of the radius
        /// </summary>
        /// <returns></returns>
        private double CalculateDecay()
        {
            //tex: $$d = \frac{max(t)}{max(r) - 1}$$

            return _totalIteration / (_mapRadius - 1.0);
        }
    }
}
