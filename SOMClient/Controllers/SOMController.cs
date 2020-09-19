using SOMLibrary;
using SOMLibrary.Implementation;
using SOMLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOMClient.Controllers
{
    public class SOMController : Controller
    {
        private SOM _model;
        private IReader _reader;

        public SOMController()
        {
            _model = new SOM(10, 10, 0.3, 40);
        }

        // GET: SOM
        public ActionResult Index()
        {

            string filepath = Server.MapPath("~/Dataset/epileptic_dataset.csv");
            _reader = new CSVReader(filepath);

            _model.GetData(_reader);

            _model.FeatureLabel = "y";

            _model.InitializeMap();
             _model.Train();
            _model.LabelNodes();

            return View(_model);
        }
    }
}