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
        private SSOM _model;
        private IReader _reader;

        public SOMController()
        {
            _model = new SSOM(20, 20, 0.8, 20);
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


            var p1 = new Coordinate(1, 0);
            var p2 = new Coordinate(1, 5);
            var p3 = new Coordinate(5, 0);
            var p4 = new Coordinate(5, 5);


            var mammalRegion = new Region(p1, p2, p3, p4);
            _model.AddRegion("Mammals", mammalRegion);

            var i1 = new Coordinate(15, 15);
            var i2 = new Coordinate(15, 19);
            var i3 = new Coordinate(19, 15);
            var i4 = new Coordinate(19, 19);


            var insectRegion = new Region(i1, i2, i3, i4);
            _model.AddRegion("Insects", insectRegion);


            _model.InitializeMap();
             _model.Train();
            _model.LabelNodes();

            return View(_model);
        }
    }
}