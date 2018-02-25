using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOMLibrary.Extensions
{
    public static class Extension
    {
        public static T ConvertType<T>(this object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
