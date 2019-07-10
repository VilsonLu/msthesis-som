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
        private const double LEARNING_RATE = 0.1;
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
            double y = _constantLearningRate;

            double learningRate = x * y;

            if (learningRate < LEARNING_RATE)
            {
                learningRate = LEARNING_RATE;
            }

            return learningRate;
        }
    }
}
