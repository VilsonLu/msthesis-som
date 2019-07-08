using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using ML.Common;
using ML.TrajectoryAnalysis;
using MLService.DataModels;
using MLService.WebService.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SOMLibrary;
using SOMLibrary.Implementation;
using SOMLibrary.Implementation.Clusterer;
using SOMLibrary.Interface;

namespace MLService.WebService.Controllers
{
    public class MLController : ApiController
    {

        private const string FILE_UPLOAD_PATH = "~/App_Data/Uploadfiles";
        private IValidate<TrainSOMRequest> _validator;

        public MLController()
        {
            _validator = new SOMRequestValidator();
        }

        [HttpPost]
        public async Task<HttpResponseMessage> GetTrainSOM()
        {
            try
            {

                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var root = HttpContext.Current.Server.MapPath(FILE_UPLOAD_PATH);
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
                var k = (int)parsedModel["K"];
                var regions = JsonConvert.DeserializeObject<List<Region>>(parsedModel["Regions"].ToString());

 
                var csvFile = result.FileData.First();

                SSOM model = new SSOM(width, height, learningRate, epoch, k);
                model.Regions = regions;

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
                    MapId = Guid.NewGuid(),
                    Model = model
                };

                var message = Request.CreateResponse(HttpStatusCode.OK, response);

                File.Delete(result.FileData.First().LocalFileName);

                return message;

            } catch(Exception ex)
            {
                var message = Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
                return message;
            }

        }

        [HttpPost]
        public async Task<HttpResponseMessage> PlotTrajectory()
        {
            try
            {
                HttpRequestMessage requestMessage = this.Request;

                if (!requestMessage.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                TrajectoryRequest request = new TrajectoryRequest();
                
                string root = System.Web.HttpContext.Current.Server.MapPath(FILE_UPLOAD_PATH);
                var provider = new MultipartFormDataStreamProvider(root);

                var result = await Request.Content.ReadAsMultipartAsync(provider);

                var map = result.FormData["map"];

                SOM som = JsonConvert.DeserializeObject<SOM>(map);

                string file = provider.FileData.Last().LocalFileName;

                IReader reader = new CSVReader(file);

                TrajectoryMapper trajectoryMapper = new TrajectoryMapper(som);
                trajectoryMapper.GetData(reader);

                TrajectoryResponse trajectoryResponse = new TrajectoryResponse()
                {
                    Trajectories = trajectoryMapper.GetTrajectories().ToArray()
                };

                File.Delete(result.FileData.First().LocalFileName);

                return Request.CreateResponse(HttpStatusCode.OK, trajectoryResponse);

            } catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
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

            return null;
        }
    }
}
