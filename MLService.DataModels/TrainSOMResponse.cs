using SOMLibrary;
using System;

namespace MLService.DataModels
{
    public class TrainSOMResponse
    {
        public Guid MapId { get; set; }
        public SOM Model { get; set; }
    }
}
