using System.Collections.Generic;

namespace MLService.DataModels
{
    public class TrainSOMRequest
    {
        public int Epoch { get; set; }
        public int TotalIteration { get; set; }
        public double LearningRate { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int K { get; set; }
        public string FeatureLabel { get; set; }

        public List<string> Labels { get; set; }

        public string FilePath { get; set; }
    }
}
