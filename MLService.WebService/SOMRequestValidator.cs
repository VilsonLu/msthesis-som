using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MLService.DataModels;
using MLService.WebService.Interface;

namespace MLService.WebService
{
    public class SOMRequestValidator : IValidate<TrainSOMRequest>
    {
        public bool Validate(TrainSOMRequest request)
        {
            if (request.Labels == null || string.IsNullOrEmpty(request.FeatureLabel))
            {
                return false;
            }

            return true;

        }
    }
}