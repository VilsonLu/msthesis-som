using SOMLibrary;
using System.Collections.Generic;

namespace MLService.DataModels
{
    public class TrainSSOMRequest : TrainSOMRequest
    {
        public IEnumerable<Region> Regions { get; set; }
    }
}
