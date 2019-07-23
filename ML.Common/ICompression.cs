using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Common
{
    public interface ICompression
    {
        string Compress(string text);
    }
}
