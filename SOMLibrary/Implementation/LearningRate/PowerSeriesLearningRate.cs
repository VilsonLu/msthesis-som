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
        private const double LEARNING_RATE = 0.01;
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
            double decay = Math.Exp(-(double) iteration / (double) totalIteration);
            double learningRate = _constantLearningRate;

            double newLearningRate = decay * learningRate;

            if (newLearningRate < LEARNING_RATE)
            {
                newLearningRate = LEARNING_RATE;
            }

            return newLearningRate;
        }
    }
}
