using Newtonsoft.Json;
using SOMLibrary.DataModel;

namespace SOMLibrary
{
    public abstract class Model
    {
        [JsonIgnore]
        public Dataset Dataset { get; set; }

        public abstract void Train();

    }
}
