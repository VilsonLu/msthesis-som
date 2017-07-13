using SOMLibrary.DataModel;
using SOMLibrary.Interface;
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

        public Node[,] Map { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double MapRadius
        {
            get
            {
                return Math.Max(Width, Height) / 2;
            }
        }

        public int Epoch { get; set; }

        #endregion

        #region Constructor

        public SOM()
        {
            Width = 0;
            Height = 0;
            LearningRate = 0;
            Epoch = 1;
            Map = new Node[Width, Height];
        }

        public SOM(int x, int y)
        {
            Width = x;
            Height = y;
            LearningRate = 0.5;
            Epoch = 1;
            Map = new Node[x, y];
        }

        public SOM(int x, int y, double learningRate)
        {
            Width = x;
            Height = y;
            LearningRate = learningRate;
            Epoch = 1;
            Map = new Node[x, y];
        }
        #endregion

        public void GetData(IReader reader)
        {
            base.Dataset = reader.Read();
        }



        /// <summary>
        /// Initializes the SOM with random weights
        /// </summary>
        public void InitializeMap()
        {
            int numOfIgnoreColumns = base.Dataset.GetIgnoreColumns().Count;
            int featureCounts = base.Dataset.Features.Length;

            int weightCount = featureCounts - numOfIgnoreColumns;
            Random rand = new Random();

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    var vectors = new double[weightCount];
                    for (int count = 0; count < weightCount; count++)
                    {
                        
                        vectors[count] = rand.NextDouble();
                    }


                    Node node = new Node(vectors, row, col);
                    Map[row, col] = node;
                }
            }




        }


        public override void Train()
        {
            // Initializes the nodes with random value
            InitializeMap();

            int instanceCount = base.Dataset.Instances.Length;
            var instances = base.Dataset.Instances;
            int t = 0; // iteration
            for(int i = 0; i < Epoch; i++)
            {
                for(int d = 0; d < instanceCount; d++)
                {
                    // Get data from dataset
                    var instance = base.Dataset.GetInstance<double>(d);

                    // Find the BMU
                    Node node = FindBMU(instance);

                    // Update the best node
                    UpdateBMU(node, instance);

                    // Update the neighbor
                    UpdateNeighborhood(node);

                    t++;
                }
            }
        }

        private Node FindBMU(double[] instance)
        {
            double bestDistance = double.MaxValue;
            Node bestNode = null;

            for(int row=0; row < Height; row++)
            {
                for(int col=0; col < Width; col++)
                {
                    Node currentNode = Map[row, col];
                    double currentDistance = currentNode.GetDistance(instance);

                    if(currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        bestNode = currentNode;
                    }

                }
            }

            return bestNode;
        }
        private void UpdateBMU(Node node, double[] instance)
        {
            // TODO: Implement the correct behavior for updating BMU
            node.Weights = instance;
        }
        private void UpdateNeighborhood(Node bmu)
        {

        }

    }
}
