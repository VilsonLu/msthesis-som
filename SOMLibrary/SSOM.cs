using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOMLibrary.DataModel;

namespace SOMLibrary
{
    /// <summary>
    /// Structured SOM
    /// </summary>
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

        public SSOM(int x, int y) : base(x, y)
        {
            Regions = new Dictionary<string, Region>();
        }

        public SSOM(int x, int y, double learningRate) : base(x, y, learningRate)
        {
            Regions = new Dictionary<string, Region>();
        }

        public SSOM(int x, int y, double learningRate, int epoch) : base(x, y, learningRate, epoch)
        {
            Regions = new Dictionary<string, Region>();
        }


        protected override Node FindBestMatchingUnit(Instance rowInstance)
        {
            double bestDistance = double.MaxValue;
            Node bestNode = null;

            var instance = base.Dataset.GetInstance<double>(rowInstance.OrderNo);
            var label = base.Dataset.GetInstanceLabel(rowInstance.OrderNo, this.FeatureLabel);

            int startRow = 0;
            int startCol = 0;

            int currentWidth = Width;
            int currentHeight = Height;

            // Check if the label has a region
            Regions.TryGetValue(label, out Region region);
            if (region != null)
            {
                startRow = region.TopLeft.X;
                startCol = region.TopLeft.Y;
                currentWidth = region.Height + startRow;
                currentHeight = region.Width + startCol;
            }

            for (int row = startRow; row < currentHeight; row++)
            {
                for (int col = startCol; col < currentWidth; col++)
                {
                    Node currentNode = Map[row, col];
                    double currentDistance = currentNode.GetDistance(instance);

                    if (currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        bestNode = currentNode;
                    }

                }
            }

            return bestNode;
        }

        #region SSOM Helper Functions

        public void AddRegion(string label, Region region)
        {
            if (!IsValidRegion(region))
            {
                throw new Exception("The regions to be added is not a valid region");
            }

            // TODO: Add a validation to check if the region will be out of bound

            if (Regions.ContainsKey(label))
            {
                Regions[label] = region;
            }

            Regions.Add(label, region);
        }

        public bool IsValidRegion(Region region)
        {
            bool flag = true;

            var regions = Regions.Values.ToList();

            foreach (var item in regions)
            {
                flag = flag && !item.IsOverlappedRegion(region);
            }

            return flag;
        }

        #endregion

    }
}
