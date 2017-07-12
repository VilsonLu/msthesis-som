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

        #endregion

        #region Constructor

        public SOM()
        {
            Width = 0;
            Height = 0;
            LearningRate = 0;
            Map = new Node[Width, Height];
        }

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

        public void GetData(IReader reader)
        {
            base.Dataset = reader.Read();
        }



        /// <summary>
        /// Initializes the SOM with random weights
        /// </summary>
        public void InitializeMap()
        {
            int numOfIgnoreColumns = IgnoreColumns().Count;
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
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get list of columns not to be used for training
        /// Example: Id, Labels
        /// </summary>
        /// <returns></returns>
        public List<int> IgnoreColumns()
        {
            var ignoreColumns = base.Dataset.Features.Where(x => x.IsKey || x.IsLabel).Select(x => x.OrderNo);
            return ignoreColumns.ToList();
        }
    }
}
