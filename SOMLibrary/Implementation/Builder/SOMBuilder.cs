using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.Builder
{
    public class SOMBuilder
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Epoch { get; set; }
        public int GlobalEpoch { get; set; }
        public double InitialLearningRate { get; set; }
        public double FinalLearningRate { get; set; }
        public string FeatureLabel { get; set; }
        public string Labels { get; set; }
        public int K { get; set; }
        public int Clusters { get; set; }
        public double InitialRadius { get; set; }
        public double FinalRadius { get; set; }


        #region Implementations
        public ILearningRate LearningRateCalculator { get; set; }
        public INeighborhoodRadius NeighborhoodRadiusCalculator { get; set; }
        public INeighborhoodKernel NeighborhoodFunctionCalculator { get; set; }
        #endregion 

        public SOM Build()
        {
            try
            {
                Validate(this);
            }
            catch (Exception ex)
            {
                throw;
            }

            return new SOM(this);
        }

        public SOMBuilder SetWidth(int width)
        {
            Width = width;
            return this;
        }

        public SOMBuilder SetHeight(int height)
        {
            Height = height;
            return this;
        }

        public SOMBuilder SetInitialLearningRate(double learningRate)
        {
            InitialLearningRate = learningRate;
            return this;
        }

        public SOMBuilder SetFinalLearningRate(double learningRate)
        {
            FinalLearningRate = learningRate;
            return this;
        }

        public SOMBuilder SetInitialRadius(double radius)
        {
            InitialRadius = radius;
            return this;
        }

        public SOMBuilder SetFinalRadius(double radius)
        {
            FinalRadius = radius;
            return this;
        }

        public SOMBuilder SetEpoch(int epoch)
        {
            Epoch = epoch;
            return this;
        }

        public SOMBuilder SetGlobalEpoch(int epoch)
        {
            GlobalEpoch = epoch;
            return this;
        }

        public SOMBuilder SetClusters(int clusters)
        {
            Clusters = clusters;
            return this;
        }

        public SOMBuilder SetFeatureLabel(string label)
        {
            FeatureLabel = label;
            return this;
        }

        public SOMBuilder SetLabels(string label)
        {
            Labels = label;
            return this;
        }

        public SOMBuilder SetKNeighbor(int k)
        {
            K = k;
            return this;
        }

        public SOMBuilder SetLearningRateCalculator(ILearningRate learningRate)
        {
            LearningRateCalculator = learningRate;
            return this;
        }

        public SOMBuilder SetNeighborhoodRadiusCalculator(INeighborhoodRadius neighborhoodRadius)
        {
            NeighborhoodRadiusCalculator = neighborhoodRadius;
            return this;
        }

        public SOMBuilder SetNeighborhoodFunctionCalculator(INeighborhoodKernel kernel)
        {
            NeighborhoodFunctionCalculator = kernel;
            return this;
        }

        public void Validate(SOMBuilder builder)
        {
            if (builder.NeighborhoodFunctionCalculator == null)
            {
                throw new Exception("Provide implementation for neighborhood function");
            }

            if (builder.NeighborhoodRadiusCalculator == null)
            {
                throw new Exception("Provide implementation for neighborhood radius calculator");
            }

            if (builder.LearningRateCalculator == null)
            {
                throw new Exception("Provide implementation for learning rate calculator");
            }
        }
    }
}
