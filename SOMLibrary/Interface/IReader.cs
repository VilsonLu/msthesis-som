using SOMLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Interface
{
    public interface IReader
    {
        string FileName { get; }

        Dataset Read();
        
    }
}
