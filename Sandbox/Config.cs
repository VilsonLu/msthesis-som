using SOMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    public class Config
    {
        public List<Region> Regions { get; set; }
        public double ConstantLearningRate { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Epoch { get; set; }
        public int GlobalEpoch { get; set; }
        public int LocalEpoch { get; set; }
        public string FeatureLabel { get; set; }
        public string Labels { get; set; }
        public int K { get; set; }
        public string Dataset { get; set; }
        public int Clusters { get; set; }
        public string Export { get; set; }
        public int Neighborhood { get; set; }
    }
}
