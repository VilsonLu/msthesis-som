using SOMLibrary;
using System;

namespace MLService.DataModels
{
    public class TrainSSOMResponse
    {
        public Guid MapId { get; set; }
        public SSOM Model { get; set; }
    }
}
