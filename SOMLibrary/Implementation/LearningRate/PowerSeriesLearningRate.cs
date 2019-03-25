using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.LearningRate
{
    /// <summary>
    /// Class that implements a Power Series Learning Decay
    /// </summary>
    public class PowerSeriesLearningRate : ILearningRate
    {

        private double _constantLearningRate;

        public PowerSeriesLearningRate(double learningRate)
        {
            _constantLearningRate = learningRate;
        }

        /// <summary>
        /// Learning Rate Decay Function
        /// Formula: L(t+1) = LearningRate * exp(- iteration / total iterations)
        /// </summary>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public double CalculateLearningRate(double iteration, double totalIteration)
        {

            double x = Math.Exp(-(double) iteration / (double) totalIteration);
            var y = _constantLearningRate;

            var test = x * y;
            return test;
        }
    }
}
