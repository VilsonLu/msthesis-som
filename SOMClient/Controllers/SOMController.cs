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
            _model = new SOM(20, 20, 0.8, 50);
        }

        // GET: SOM
        public ActionResult Index()
        {

            string filepath = Server.MapPath("~/Dataset/Animal_Dataset.csv");
            _reader = new CSVReader(filepath);

            _model.GetData(_reader);
            _model.Dataset.SetLabel("Class");
            _model.Dataset.SetLabel("Name");

            _model.FeatureLabel = "Class";
         
            _model.InitializeMap();
             _model.Train();
            _model.LabelNodes();

            return View(_model);
        }
    }
}