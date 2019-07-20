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
        private double _totalIteration;

        public BasicNeighborhoodRadius(double mapRadius, double totalIteration)
        {
            _mapRadius = mapRadius;
            _totalIteration = totalIteration;
        }

        public double CalculateRadius(int iteration)
        {
            //tex: $$r = radius * e^{\frac{-iteration}{\lambda}}$$

            double neighboodRadius = _mapRadius * Math.Exp(-((double)iteration / (double)_totalIteration));
            return neighboodRadius;
        }

        /// <summary>
        /// Time Constant (lambda)
        /// </summary>
        /// <returns></returns>
        private double TimeConstant()
        {
            //tex: $$ \lambda = \frac{Total Iterations}{\log(map radius)}$$

            double timeConstant = _totalIteration / Math.Log(_mapRadius);
            return timeConstant;
        }
    }
}
