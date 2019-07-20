using SOMLibrary.Interface;

namespace SOMLibrary.Implementation.LearningRate
{
    public class FixedLearningRate : ILearningRate
    {
        private double _learningRate;
        public FixedLearningRate(double learningRate)
        {
            _learningRate = learningRate;
        }

        public double CalculateLearningRate(double iteration, double totalIteration)
        {
            return _learningRate;
        }
    }
}
