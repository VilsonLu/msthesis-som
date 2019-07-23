using ML.Common;
using SOMLibrary.DataModel;
using SOMLibrary.Implementation.LearningRate;
using SOMLibrary.Implementation.NeighborhoodRadius;
using SOMLibrary.Implementation.NodeLabeller;
using SOMLibrary.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SOMLibrary
{
    /// <summary>
    /// Self-Organizing Map
    /// </summary>
    public class SOM : Model
    {

        #region Properties
        public Guid MapId { get; set; }

        /// <summary>
        /// Learning Rate
        /// </summary>
        public double ConstantLearningRate { get; set; }

        public Node[,] Map { get; set; }

        /// <summary>
        /// Width of the map (X)
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the map (Y)
        /// </summary>
        public int Height { get; set; }


        /// <summary>
        /// Number of times it will train using the dataset
        /// </summary>
        public int Epoch { get; set; }

        public int TotalIteration { get; set; }

        public string FeatureLabel { get; set; }

        /// <summary>
        /// Number of neighbors for K-NN
        /// </summary>
        public int K { get; set; } = 3;

        #endregion

        #region Events
        public delegate void OnTrainingEventHandler(object sender, OnTrainingEventArgs args);
        public event OnTrainingEventHandler Training;
        #endregion 

        #region Calculated

        /// <summary>
        /// Map Radius (sigma)
        /// Formula: Max(Width, Height) / 2
        /// </summary>
        public double MapRadius
        {
            get;
            set;
        }

        #endregion

        #region SOM Functions

        /// <summary>
        /// To label the nodes
        /// </summary>
        private ILabel _labeller;

        /// <summary>
        /// To calculate the decay of the learning rate
        /// </summary>
        private ILearningRate _learningRate;
        public ILearningRate LearningRate
        {
            set
            {
                _learningRate = value;
            }
        }

        /// <summary>
        /// To calculate the neighborhood radius
        /// </summary>
        private INeighborhoodRadius _neighborhoodRadius;
        #endregion

        #region Constructor

        public SOM()
        {
            Width = 0;
            Height = 0;
            ConstantLearningRate = 0;
            Epoch = 1;
            Map = new Node[Width, Height];
            _learningRate = new PowerSeriesLearningRate(ConstantLearningRate);

        }

        public SOM(int x, int y)
        {
            Width = x;
            Height = y;
            ConstantLearningRate = 0.5;
            Epoch = 1;
            Map = new Node[x, y];
            _learningRate = new PowerSeriesLearningRate(ConstantLearningRate);
        }

        public SOM(int x, int y, double learningRate) : this(x, y)
        {
            ConstantLearningRate = learningRate;
            _learningRate = new PowerSeriesLearningRate(ConstantLearningRate);
            Epoch = 1;
        }

        public SOM(int x, int y, double learningRate, int epoch) : this(x, y, learningRate)
        {
            Epoch = epoch;
        }

        public SOM(int x, int y, double learningRate, int epoch, int k) : this(x, y, learningRate, epoch)
        {
            K = k;
        }

        #endregion

        public void GetData(IReader reader)
        {
            base.Dataset = reader.Read();
            TotalIteration = base.Dataset.Instances.Length * Epoch;

            _neighborhoodRadius = new BasicNeighborhoodRadius(MapRadius, TotalIteration);
            //_neighborhoodRadius = new FixedNeighborhoodRadius(MapRadius);
        }

        /// <summary>
        /// Initializes the SOM with random weights
        /// </summary>
        public void InitializeMap()
        {
            int numOfIgnoreColumns = base.Dataset.GetIgnoreColumns().Count;
            int featureCounts = base.Dataset.Features.Length;

            this.MapId = Guid.NewGuid();

            int weightCount = featureCounts - numOfIgnoreColumns;
            Random rand = new Random();

            for (int row = 0; row < Width; row++)
            {
                for (int col = 0; col < Height; col++)
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
        /// 2. Randomize the order of the instance
        /// 3. Get an instance from the dataset
        /// 4. Find the best matching unit. (Find the node with the least distance)
        /// 5. Update the neighborhood that are within the neighborhood radius
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
                // Randomize the order of the instance after every epoch
                base.Dataset.Shuffle();

                for (int d = 0; d < instanceCount; d++)
                {
                    // Get data from datase
                    var instance = instances[d];

                    // Find the BMU (Best Matching Unit)
                    Node winningNode = FindBestMatchingUnit(instance);

                    // Adjust the weights of the BMU and neighbor
                    UpdateNeighborhood(winningNode, instance, t);

                    t++;
                }

                if (Training != null)
                {
                    Training(this, new OnTrainingEventArgs() { CurrentIteration = i, TotalIteration = Epoch });
                }
            }
        }

        /// <summary>
        /// Give label to each node in the map
        /// </summary>
        public void LabelNodes()
        {
            if (string.IsNullOrEmpty(this.FeatureLabel))
            {
                return;
            }

            _labeller = new KNNLabeller(base.Dataset, K, this.FeatureLabel);
            for (int row = 0; row < Width; row++)
            {
                for (int col = 0; col < Height; col++)
                {
                    Node node = Map[row, col];
                    Map[row, col].Label = _labeller.GetLabel(node);
                }
            }
        }

        #region SOM Functions
        public virtual Node FindBestMatchingUnit(Instance rowInstance)
        {
            double bestDistance = double.MaxValue;
            Node bestNode = null;

            var instance = base.Dataset.GetInstance<double>(rowInstance.OrderNo);

            for (int row = 0; row < Width; row++)
            {
                for (int col = 0; col < Height; col++)
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

        /// <summary>
        /// Assign a 2-letter ID to all nodes
        /// </summary>
        public virtual void AssignNodeId()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(UtilityHelper.GetLetter(i));
                    builder.Append(UtilityHelper.GetLetter(j));
                    Map[i, j].NodeId = builder.ToString();
                }
            }
        }

        public virtual void AssignClusterLabel()
        {
            Hashtable clusterLabels = new Hashtable();
            Queue<string> letters = UtilityHelper.GetLetterQueue();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    int currentClusterGroup = Map[i, j].ClusterGroup;
                    if (clusterLabels.ContainsKey(currentClusterGroup))
                    {
                        Map[i, j].ClusterLabel = clusterLabels[currentClusterGroup].ToString();
                    }
                    else
                    {
                        string label = string.Empty;
                        if (letters.Count > 0)
                        {
                            label = letters.Dequeue();
                        }
                        
                        clusterLabels.Add(currentClusterGroup, label);

                        Map[i, j].ClusterLabel = label;
                    }
                }
            }
        }

        protected virtual void UpdateNeighborhood(Node winningNode, Instance rowInstance, int iteration)
        {
            var instance = base.Dataset.GetInstance<double>(rowInstance.OrderNo);

            for (int row = 0; row < Width; row++)
            {
                for (int col = 0; col < Height; col++)
                {
                    var currentNode = Map[row, col];
                    var distanceToWinningNode = winningNode.GetGridDistance(currentNode);
                    double neighborhoodRadius = Math.Pow(NeighborhoodRadius(iteration), 2);
                    if (distanceToWinningNode <= neighborhoodRadius)
                    {
                        currentNode.Weights = AdjustWeights(winningNode, currentNode, instance, iteration);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate how fast model will learn every iteration
        /// </summary>
        /// <param name="iteration"></param>
        /// <returns></returns>
        protected double LearningRateDecay(int iteration)
        {
            double learningRate = _learningRate.CalculateLearningRate(iteration, TotalIteration);
            return learningRate;
        }


        /// <summary>
        /// Neighborhood Radius: The neighborhood shrinks as time passes by
        /// </summary>
        /// <returns></returns>
        protected double NeighborhoodRadius(int iteration)
        {

            return _neighborhoodRadius.CalculateRadius(iteration);

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
        protected double[] AdjustWeights(Node winningNode, Node currentNode, double[] instance, int iteration)
        {
            var currentWeight = currentNode.Weights;
            var inputWeight = winningNode.Weights;

            double learningRate = LearningRateDecay(iteration);
            double influence = Influence(winningNode, currentNode, iteration);


            for (int i = 0; i < currentWeight.Length; i++)
            {
                double newWeight = currentWeight[i] + (learningRate * influence * (instance[i] - currentWeight[i]));
                currentWeight[i] = newWeight;
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
        protected double Influence(Node winningNode, Node currentNode, int iteration)
        {
            double distance = Math.Pow(winningNode.GetGridDistance(currentNode), 2);
            double radius = 2 * Math.Pow(NeighborhoodRadius(iteration), 2);
            double influence = Math.Exp(-((double)distance / (double)radius));
            return influence;
        }
        #endregion


    }

    public class OnTrainingEventArgs : EventArgs
    {
        public int CurrentIteration { get; set; }
        public int TotalIteration { get; set; }
    }

}
