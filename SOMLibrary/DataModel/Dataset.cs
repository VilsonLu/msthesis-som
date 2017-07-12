using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.DataModel
{
    public class Dataset
    {
        public Feature[] Features { get; set; }

        public Instance[] Instances { get; set; }

        /// <summary>
        /// Set the feature to a label
        /// </summary>
        /// <param name="feature"></param>
        public void SetLabel(string feature)
        {
            var selectedFeature = Features.First(x => x.FeatureName == feature);

            if (selectedFeature == null)
            {
                throw new Exception("Feature does not exists");
            }

            selectedFeature.IsLabel = true;
        }

        public void SetKey(string feature)
        {
            var selectedFeature = Features.First(x => x.FeatureName == feature);

            if (selectedFeature == null)
            {
                throw new Exception("Feature does not exists");
            }

            selectedFeature.IsKey = true;
        }
    }
}
