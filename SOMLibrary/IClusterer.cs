using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary
{
    public interface IClusterer
    {
        IEnumerable<Node> Cluster(List<Node> Nodes, int k);
    }
}
