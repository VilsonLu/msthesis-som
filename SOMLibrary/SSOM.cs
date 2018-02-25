using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary
{
    // Structured SOM
    public class SSOM : SOM
    {


        #region Properties
        public Dictionary<string, Region> Regions { get; set; }

        #endregion

        public SSOM() 
        {
            Width = 0;
            Height = 0;
            ConstantLearningRate = 0;
            Epoch = 1;
            Map = new Node[Width, Height];
            Regions = new Dictionary<string, Region>();
        }

        public SSOM(int x, int y) : base(x, y) { }

        public SSOM(int x, int y, double learningRate) : base(x, y, learningRate) { }

        public SSOM(int x, int y, double learningRate, int epoch) : base(x, y, learningRate, epoch) { }


        public override void Train()
        {
            throw new NotImplementedException();
        }

        protected override Node FindBMU(double[] instance)
        {
            throw new NotImplementedException();
        }

        #region SSOM Helper Functions

        public void AddRegion(string label, Region region)
        {
            throw new NotImplementedException();
        }

        public bool ValidateRegion(Region region)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
