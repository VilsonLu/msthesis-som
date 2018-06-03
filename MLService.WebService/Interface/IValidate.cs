using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLService.DataModels;

namespace MLService.WebService.Interface
{
    public interface IValidate<T>
    {
        bool Validate(T request);
    }
}
