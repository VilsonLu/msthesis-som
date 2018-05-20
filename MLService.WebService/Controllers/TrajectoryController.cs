using ML.TrajectoryAnalysis;
using MLService.DataModels;
using SOMLibrary.Implementation;
using SOMLibrary.Interface;
using System.Web.Http;

namespace MLService.WebService.Controllers
{
    public class TrajectoryController : ApiController
    {
        public TrajectoryResponse GetTrajectory(TrajectoryRequest request)
        {
            TrajectoryMapper trajectoryMapper = new TrajectoryMapper(request.Map);
            IReader reader = new CSVReader(request.FilePath);

            // Get the data
            trajectoryMapper.GetData(reader);

           // Get the mapped trajectories
           var trajectories =  trajectoryMapper.GetTrajectories();

            return new TrajectoryResponse()
            {
                Trajectories = trajectories
            };

        }
    }
}
