using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MLService.DataModels;
using SOMLibrary;
using SOMLibrary.Implementation;
using SOMLibrary.Interface;

namespace MLService.WebService.Controllers
{
    public class MLController : ApiController
    {

        [HttpPost]
        public TrainSOMResponse GetTrainSOM(TrainSOMRequest request)
        {
           SOM model = new SOM(request.Width, request.Height, request.LearningRate, request.Epoch);
           IReader reader = new CSVReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Iris.csv"));

           model.GetData(reader);

            foreach (var item in request.Labels)
            {
                model.Dataset.SetLabel(item);
            }

            model.FeatureLabel = request.FeatureLabel;
            model.InitializeMap();
            model.Train();
            model.LabelNodes();

            TrainSOMResponse response = new TrainSOMResponse()
            {
                Model = model
            };

            return response;

        }

        [HttpGet]
        public TrainSSOMResponse GetTrainSSOM(TrainSSOMRequest request)
        {
            SSOM model = new SSOM(request.Width, request.Height, request.LearningRate, request.Epoch);
            IReader reader = new CSVReader(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Iris.csv"));

            model.GetData(reader);

            foreach (var item in request.Labels)
            {
                model.Dataset.SetLabel(item);
            }

            model.FeatureLabel = request.FeatureLabel;
            model.InitializeMap();
            model.Train();
            model.LabelNodes();

            TrainSSOMResponse response = new TrainSSOMResponse()
            {
                Model = model
            };

            return response;

        }
    }
}
