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

        public double ConstantLearningRate { get; set; }

        public Node[,] Map { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }


        /// <summary>
        /// Map Radius (sigma)
        /// Formula: Max(Width, Height) / 2
        /// </summary>
        public double MapRadius
        {
            get
            {
                return Math.Max(Width, Height) / 2;
            }
        }

        public int Epoch { get; set; }

        public int TotalIteration { get; set; }

        #endregion

        #region Constructor

        public SOM()
        {
            Width = 0;
            Height = 0;
            ConstantLearningRate = 0;
            Epoch = 1;
            Map = new Node[Width, Height];
        }

        public SOM(int x, int y)
        {
            Width = x;
            Height = y;
            ConstantLearningRate = 0.5;
            Epoch = 1;
            Map = new Node[x, y];
        }

        public SOM(int x, int y, double learningRate)
        {
            Width = x;
            Height = y;
            ConstantLearningRate = learningRate;
            Epoch = 1;
            Map = new Node[x, y];
        }

        public SOM(int x, int y, double learningRate, int epoch)
        {
            Width = x;
            Height = y;
            ConstantLearningRate = learningRate;
            Epoch = epoch;
            Map = new Node[x, y];
        }

        #endregion

        public void GetData(IReader reader)
        {
            base.Dataset = reader.Read();
            TotalIteration = base.Dataset.Instances.Length * Epoch;
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

        /// <summary>
        /// Train the SOM by adjusting the weights of the nodes.
        /// 
        /// Steps:
        /// 1. Initialize the SOM with random weights
        /// 2. Get an instance from the dataset
        /// 3. Find the best matching unit. (Find the node with the least distance)
        /// 4. Update the neighborhood that are within the neighborhood radius
        /// 
        /// </summary>
        public override void Train()
        {
            // Initializes the nodes with random value
            InitializeMap();

            int instanceCount = base.Dataset.Instances.Length;
            var instances = base.Dataset.Instances;
            int t = 1; // iteration
            for (int i = 0; i < Epoch; i++)
            {
                for (int d = 0; d < instanceCount; d++)
                {
                    // Get data from dataset
                    var instance = base.Dataset.GetInstance<double>(d);

                    // Find the BMU (Best Matching Unit)
                    Node winningNode = FindBMU(instance);

                    // Adjust the weights of the BMU and neighbor
                    UpdateNeighborhood(winningNode, instance, t);

                    t++;
                }
            }
        }

        #region SOM Functions
        private Node FindBMU(double[] instance)
        {
            double bestDistance = double.MaxValue;
            Node bestNode = null;

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
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

        private void UpdateNeighborhood(Node winningNode, double[] instance, int iteration)
        {
            for (int row = 0; row < Height; row++)
            {
                for(int col = 0; col < Width; col++)
                {
                    var currentNode = Map[row, col];
                    var distanceToWinningNode = Math.Pow(winningNode.GetGridDistance(currentNode), 2);
                    double neighborhoodRadius = Math.Pow(NeighborhoodRadius(iteration), 2);
                    if (distanceToWinningNode < neighborhoodRadius)
                    {
                        currentNode.Weights = AdjustWeights(winningNode, currentNode, instance, iteration);
                    }
                }
            }
        }

        /// <summary>
        /// Learning Rate Decay Function
        /// Formula: L(t+1) = LearningRate * exp(- iteration / total iterations)
        /// </summary>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public double LearningRateDecay(int iteration)
        {
            double learningRate = ConstantLearningRate * Math.Exp(-iteration / TotalIteration);
            return learningRate;
        }

        /// <summary>
        /// Time Constant (lambda)
        /// Formula: TotalIterations / log(map_radius)
        /// </summary>
        /// <returns></returns>
        public double TimeConstant()
        {
            double timeConstant = TotalIteration / Math.Log(MapRadius);
            return timeConstant;
        }

        /// <summary>
        /// Neighborhood Radius: The neighborhood shrinks as time passes by
        /// Formula: MapRadius * exp(-iteration/timeConstant)
        /// </summary>
        /// <returns></returns>
        public double NeighborhoodRadius(int iteration)
        {
            double neighboodRadius = MapRadius * Math.Exp(-iteration / TimeConstant());
            return neighboodRadius;
        }

        /// <summary>
        /// Formula for adjusting the weights of the node. 
        /// Weights are adjusted based on the distance of the node to the winning node
        /// Formula W(t+1) = W(t) + Influence(t) * LearningRate(t) * (V(t) - W(t))
        /// </summary>
        /// <param name="winningNode"></param>
        /// <param name="currentNode"></param>
        /// <param name="instance"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public double[] AdjustWeights(Node winningNode, Node currentNode, double[] instance, int iteration)
        {
            var currentWeight = currentNode.Weights;

            for (int i = 0; i < currentWeight.Length; i++)
            {
                double influence = Influence(winningNode, currentNode, iteration);
                double learningRate = LearningRateDecay(iteration);
                currentWeight[i] = currentWeight[i] +  influence * learningRate * (instance[i]/10 - currentWeight[i]);
            }

            return currentWeight;
        }

        /// <summary>
        /// Influence of the node based on distance
        /// Formula I(t) = Exp(-distance^2/ (2*NeighborhoodRadius^2))
        /// </summary>
        /// <param name="winningNode"></param>
        /// <param name="currentNode"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public double Influence(Node winningNode, Node currentNode, int iteration)
        {
            double distance = winningNode.GetGridDistance(currentNode);
            double radius = 2 * Math.Pow(NeighborhoodRadius(iteration), 2);
            double influence = Math.Exp(-Math.Pow(distance, 2) / radius);
            return influence;
        }

        #endregion
    }
}
