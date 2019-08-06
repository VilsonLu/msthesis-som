using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.LearningRate
{
    public class LinearLearningRate : ILearningRate
    {
        public LinearLearningRate()
        {

        }

        /// <summary>
        /// y = mx + b
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="totalIteration"></param>
        /// <returns></returns>
        public double CalculateLearningRate(double iteration, double totalIteration)
        {
            throw new NotImplementedException();
        }
    }
}
