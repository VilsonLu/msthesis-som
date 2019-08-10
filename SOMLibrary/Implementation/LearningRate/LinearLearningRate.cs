using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.LearningRate
{
    /// <summary>
    /// Implementation of Learning Rate Decay using slopes
    /// </summary>
    public class LinearLearningRate : ILearningRate
    {
        private double _initialLearningRate;
        private const double FINAL_LEARNING_RATE = 0.1;

        public LinearLearningRate(double learningRate)
        {
            _initialLearningRate = learningRate;
        }

        /// <summary>
        /// y = mx + b
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="totalIteration"></param>
        /// <returns></returns>
        public double CalculateLearningRate(double iteration, double totalIteration)
        {
            double b = _initialLearningRate;
            double x = iteration;
            double m = (_initialLearningRate - FINAL_LEARNING_RATE) / (0.0 - totalIteration);

            double y = m * x + b;

            if(y < FINAL_LEARNING_RATE)
            {
                y = FINAL_LEARNING_RATE;
            }

            return y;   
        }
    }
}
