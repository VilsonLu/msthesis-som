using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Implementation.Builder
{
    public class SSOMBuilder
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Region> Regions { get; set; }
        public double ConstantLearningRate { get; set; }
        public int Epoch { get; set; }
        public string FeatureLabel { get; set; }
        public int KNeighbor { get; set;  }

        public SSOM Build()
        {
            return new SSOM(this);
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

        public SSOMBuilder SetRegions(List<Region> regions)
        {
            Regions = regions;
            return this;
        }

        public SSOMBuilder SetLearningRate(double learningRate)
        {
            ConstantLearningRate = learningRate;
            return this;
        }

        public SSOMBuilder SetEpoch(int epoch)
        {
            Epoch = epoch;
            return this;
        }

        public SSOMBuilder SetFeatureLabel(string label)
        {
            FeatureLabel = label;
            return this;
        }

        public SSOMBuilder SetKNeighbor(int k)
        {
            KNeighbor = k;
            return this;
        }
    }
}
