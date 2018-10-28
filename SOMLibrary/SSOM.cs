﻿using System;
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
        public List<Region> Regions { get; set; }

        #endregion

        public SSOM() 
        {
            Width = 0;
            Height = 0;
            ConstantLearningRate = 0;
            Epoch = 1;
            Map = new Node[Width, Height];
            Regions = new List<Region>();
        }

        public SSOM(int x, int y) : base(x, y)
        {
            Regions = new List<Region>();
        }

        public SSOM(int x, int y, double learningRate) : base(x, y, learningRate)
        {
            Regions = new List<Region>();
        }

        public SSOM(int x, int y, double learningRate, int epoch) : base(x, y, learningRate, epoch)
        {
            Regions = new List<Region>();
        }


        public override Node FindBestMatchingUnit(Instance rowInstance)
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
            var region = Regions.FirstOrDefault(x => x.Label == label);
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
                    if(IsInAnyRegion(row, col))
                    {
                        continue;
                    }

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
            
            if (Regions.Any(x => x.Label == label))
            {
                var index = Regions.FindIndex(x => x.Label == label);
                Regions[index] = region;
            }

            Regions.Add(region);
        }

        /// <summary>
        /// Checks if the given node is in any region or not
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsInAnyRegion(int x, int y)
        {
            var coordinate = new Coordinate(x, y);
            var isInAnyRegion = Regions.Any(r => r.IsWithinRegion(coordinate));
            return isInAnyRegion;
        }

        public bool IsValidRegion(Region region)
        {
            bool flag = true;

            var regions = Regions.ToList();

            foreach (var item in regions)
            {
                flag = flag && !item.IsOverlappedRegion(region);
            }

            return flag;
        }

        #endregion

    }
}
