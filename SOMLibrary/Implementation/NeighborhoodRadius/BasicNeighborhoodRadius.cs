using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.NeighborhoodRadius
{
    public class BasicNeighborhoodRadius : INeighborhoodRadius
    {
        private double _mapRadius;


        public BasicNeighborhoodRadius(double mapRadius)
        {
            _mapRadius = mapRadius;
        }

        public double CalculateRadius(int iteration, int totalIteration)
        {
            //tex: $$r = radius * e^{\frac{-iteration}{\lambda}}$$
            double neighboodRadius = _mapRadius * Math.Exp(-((double)iteration / totalIteration));
            return neighboodRadius;
        }

    }
}
