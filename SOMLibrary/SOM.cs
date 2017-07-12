using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary
{
    public class SOM : Model
    {

        #region Properties
        public double LearningRate { get; set; }

        public Node[][] Map { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }


        public double MapRadius
        {
            get
            {
                return Math.Max(Width, Height) / 2;
            }
        }

        #endregion

        #region Constructor
        public SOM(int x, int y)
        {
            Width = x;
            Height = y;
            LearningRate = 0.5;
            Map = new Node[x, y];
        }

        public SOM(int x, int y, double learningRate)
        {
            Width = x;
            Height = y;
            LearningRate = learningRate;
            Map = new Node[x, y];
        }
        #endregion


       /// <summary>
       /// Initializes the SOM with random weights
       /// </summary>
        public void InitializeMap()
        {

        }


        public override void RetrieveDataset()
        {
            throw new NotImplementedException();
        }

        public override void Train()
        {
            throw new NotImplementedException();
        }
    }
}
