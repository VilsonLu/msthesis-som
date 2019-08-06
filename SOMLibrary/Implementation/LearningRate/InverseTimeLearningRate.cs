using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.LearningRate
{
    public class InverseTimeLearningRate : ILearningRate
    {
        private double _learningRate;
        private const double LEARNING_RATE = 0.01;

        public InverseTimeLearningRate(double learningRate)
        {
            _learningRate = learningRate;
        }
        public double CalculateLearningRate(double iteration, double totalIteration)
        {
            double newLearningRate = _learningRate * (1.0 - iteration/ totalIteration);

            if (newLearningRate < LEARNING_RATE)
            {
                newLearningRate = LEARNING_RATE;
            }
            return newLearningRate;
        }
    }
}
