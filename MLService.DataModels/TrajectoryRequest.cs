using SOMLibrary;

namespace MLService.DataModels
{
    public class TrajectoryRequest
    {
        public SOM Map { get; set; }

        public string FilePath { get; set; }
    }
}
