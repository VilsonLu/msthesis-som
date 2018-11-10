using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ML.Common;
using MLService.DataModels;
using MLService.WebService.Interface;
using Newtonsoft.Json.Linq;
using SOMLibrary;
using SOMLibrary.Implementation;
using SOMLibrary.Implementation.Clusterer;
using SOMLibrary.Interface;

namespace MLService.WebService.Controllers
{
    public class MLController : ApiController
    {

        private IValidate<TrainSOMRequest> _validator;

        public MLController()
        {
            _validator = new SOMRequestValidator();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetTrainSOM()
        {

            try
            {


                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var root = HttpContext.Current.Server.MapPath("~/App_Data/Uploadfiles");
                Directory.CreateDirectory(root);
                var provider = new MultipartFormDataStreamProvider(root);
                var result = await Request.Content.ReadAsMultipartAsync(provider);

                var jsonModel = result.FormData["model"];
                if (jsonModel == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                JObject parsedModel = JObject.Parse(jsonModel);

                var epoch = (int)parsedModel["Epoch"];
                var learningRate = (double)parsedModel["LearningRate"];
                var height = (int)parsedModel["Height"];
                var width = (int)parsedModel["Width"];
                var kmeans = (int)parsedModel["KMeans"];


                var csvFile = result.FileData[0];

                SOM model = new SOM(width, height, learningRate, epoch);

                var featureLabel = (string)parsedModel["FeatureLabel"];
                var labels = ((string)parsedModel["Labels"]).Split(',').ToList();


                IReader reader = new CSVReader(csvFile.LocalFileName);

                model.GetData(reader);

                foreach (var item in labels)
                {
                    model.Dataset.SetLabel(item);
                }

                model.FeatureLabel = featureLabel;
                model.InitializeMap();
                model.Train();
                model.LabelNodes();

                IClusterer cluster = new KMeansClustering();

                var flattenedMap = ArrayHelper<Node>.FlattenMap(model.Map);
                var clusteredNodes = cluster.Cluster(flattenedMap, kmeans);

                foreach (var node in clusteredNodes)
                {
                    model.Map[node.Coordinate.X, node.Coordinate.Y].ClusterGroup = node.ClusterGroup;
                }

                FileInfo fileInfo = new FileInfo(csvFile.LocalFileName);
                fileInfo.Delete();

                TrainSOMResponse response = new TrainSOMResponse()
                {
                    Model = model
                };

                var message = Request.CreateResponse(HttpStatusCode.OK, response);
                return message;

            } catch(Exception ex)
            {
                var message = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                return message;
            }

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
