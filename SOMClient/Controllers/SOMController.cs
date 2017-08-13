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
            _model = new SOM(20, 20, 0.8, 10);
        }

        // GET: SOM
        public ActionResult Index()
        {

            string filepath = @"c:\Users\vilso\documents\visual studio 2017\Projects\MSThesis\SOMClient\Dataset\Iris.csv";
            _reader = new CSVReader(filepath);

            _model.GetData(_reader);
            _model.Dataset.SetKey("Id");
            _model.Dataset.SetLabel("Species");

            _model.InitializeMap();
            _model.Train();

            return View(_model);
        }
    }
}