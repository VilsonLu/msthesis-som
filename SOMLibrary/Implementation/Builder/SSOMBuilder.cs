using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.Builder
{
    public class SSOMBuilder
    {
        public List<Region> Regions { get; set; }
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

        public SSOM Build()
        {
            try
            {
                Validate(this);
            }
            catch (Exception ex)
            {
                throw;
            }

            return new SSOM(this);
        }

        public SSOMBuilder SetRegions(List<Region> regions)
        {
            Regions = regions;
            return this;
        }

        public SSOMBuilder SetWidth(int width)
        {
            Width = width;
            return this;
        }

        public SSOMBuilder SetHeight(int height)
        {
            Height = height;
            return this;
        }

        public SSOMBuilder SetInitialLearningRate(double learningRate)
        {
            InitialLearningRate = learningRate;
            return this;
        }

        public SSOMBuilder SetFinalLearningRate(double learningRate)
        {
            FinalLearningRate = learningRate;
            return this;
        }

        public SSOMBuilder SetInitialRadius(double radius)
        {
            InitialRadius = radius;
            return this;
        }

        public SSOMBuilder SetFinalRadius(double radius)
        {
            FinalRadius = radius;
            return this;
        }

        public SSOMBuilder SetEpoch(int epoch)
        {
            Epoch = epoch;
            return this;
        }

        public SSOMBuilder SetGlobalEpoch(int epoch)
        {
            GlobalEpoch = epoch;
            return this;
        }

        public SSOMBuilder SetClusters(int clusters)
        {
            Clusters = clusters;
            return this;
        }

        public SSOMBuilder SetFeatureLabel(string label)
        {
            FeatureLabel = label;
            return this;
        }

        public SSOMBuilder SetLabels(string label)
        {
            Labels = label;
            return this;
        }

        public SSOMBuilder SetKNeighbor(int k)
        {
            K = k;
            return this;
        }

        public SSOMBuilder SetLearningRateCalculator(ILearningRate learningRate)
        {
            LearningRateCalculator = learningRate;
            return this;
        }

        public SSOMBuilder SetNeighborhoodRadiusCalculator(INeighborhoodRadius neighborhoodRadius)
        {
            NeighborhoodRadiusCalculator = neighborhoodRadius;
            return this;
        }

        public SSOMBuilder SetNeighborhoodFunctionCalculator(INeighborhoodKernel kernel)
        {
            NeighborhoodFunctionCalculator = kernel;
            return this;
        }

        public void Validate(SSOMBuilder builder)
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
