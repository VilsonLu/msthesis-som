using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.LearningRate
{
    /// <summary>
    /// Learning Rate that decays to 0.0001 after 1000 training cycles
    /// </summary>
    public class FastLearningRateDecay : ILearningRate
    {
        private const double LEARNING_RATE = 0.0001;
        private double _constantLearningRate;

        public FastLearningRateDecay(double learningRate)
        {
            _constantLearningRate = learningRate;
        }

        public double CalculateLearningRate(double iteration, double totalIteration)
        {
            double decay = Math.Exp(-(double) iteration / (double) 1000.0);
            double learningRate = _constantLearningRate;

            double newLearningRate = decay * learningRate;

            if (iteration >= 1000)
            {
                newLearningRate = LEARNING_RATE;
            }

            return newLearningRate;
        }
    }
}
