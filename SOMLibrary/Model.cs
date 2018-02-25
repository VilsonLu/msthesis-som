using SOMLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary
{
    public abstract class Model
    {
        public Dataset Dataset { get; set; }

        public abstract void Train();

    }
}
