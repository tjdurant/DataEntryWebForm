using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataEntryWebForm.Models;
using System.IO;
using Nest;
using Newtonsoft.Json;
using DataEntryWebForm.Helpers;
using System.Globalization;
using DataEntry.Storage;
using DataEntry.Storage.Documents;

namespace DataEntryWebForm.Controllers
{

    public class DataEntryController : Controller
    {
        
        private ElasticQueries _eq = new ElasticQueries(System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data/ElasticQueryDsl"));

        
        [HttpGet]
        public ActionResult Index()
        {
            // List all data from default index
            return View(_eq.IndexDetails());
        }


        [HttpGet]
        public ActionResult Details(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // list details of document with specific id
            return View(_eq.IdDetails(id));
        }


        [HttpGet]
        public ActionResult Create()
        {

            var data = new List<SelectListItem>{
                 new SelectListItem{ Value="1",Text="HDFS"},
                 new SelectListItem{ Value="2",Text="Elastic"},
                 new SelectListItem{ Value="3",Text="HBase"},
                 new SelectListItem{ Value="4",Text="Other"},
             };

            ViewBag.StorageLocations = data;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ExtractName,Description,DescriptionHtml,Requestor,RequestorEmail,DataSources,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {

            // instantiate elastic client from data access layer
            EsClient es = new EsClient();

            // 
            hadoopMetaDataModels.Id = Guid.NewGuid().ToString();

            // create index; index doesn't exist
            es.Current.CreateIndex(ci => ci.Index("hadoop_metadata")
                .AddMapping<HadoopMetaDataModels>(m => m
                    .MapFromAttributes()));

            // index does exist; apply index for inserts; builds as per document(model)
            var response = es.Current.Map<HadoopMetaDataModels>(m =>
                m.MapFromAttributes()
                    .Type<HadoopMetaDataModels>()
                    .Indices("hadoop_metadata"));
            
            // set description(without html) to model.Description 
            hadoopMetaDataModels.Description = TextParseHelper.StripHtml(hadoopMetaDataModels.DescriptionHtml);

            //ModelState.SetModelValue("Description", new ValueProviderResult(description, "", CultureInfo.InvariantCulture));
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                es.Current.Index<HadoopMetaDataModels>(hadoopMetaDataModels);
                return RedirectToAction("Index");
            }

            return View(hadoopMetaDataModels);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {

            var data = new List<SelectListItem>{
                 new SelectListItem{ Value="1",Text="HDFS"},
                 new SelectListItem{ Value="2",Text="Elastic"},
                 new SelectListItem{ Value="3",Text="HBase"},
                 new SelectListItem{ Value="4",Text="Other"},
             };

            ViewBag.StorageLocations = data;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(_eq.IdDetails(id));
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ExtractName,Description,DescriptionHtml,Requestor,RequestorEmail,Request,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {

            // instantiate elastic client from data access layer
            EsClient es = new EsClient();

            // set description(without html) to model.Description 
            hadoopMetaDataModels.Description = TextParseHelper.StripHtml(hadoopMetaDataModels.DescriptionHtml);

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                es.Current.Index<HadoopMetaDataModels>(hadoopMetaDataModels);
                return RedirectToAction("Index");
            }

            return View(hadoopMetaDataModels);
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            EsClient es = new EsClient();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(_eq.IdDetails(id));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            EsClient es = new EsClient();

            var resDel = es.Current.Delete<HadoopMetaDataModels>(d => d
               .Id(id.ToString())
               .Index("hadoop_metadata"));

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModels model)
        {
            var searchResults = new List<HadoopMetaDataModels>();
            

            searchResults = _eq.SearchElastic(model.Query);



            var vmData = new List<HadoopMetaViewModels>();

            // TODO: MetaDataModel to ViewModel
            if (searchResults != null)
            {
                foreach (HadoopMetaDataModels item in searchResults)
                {
                    var vm = new HadoopMetaViewModels();
                    {
                        vm.Id = item.Id;
                        vm.ExtractName = item.ExtractName;
                        
                    };
                    vmData.Add(vm);
                }
            }
            else
            {
                return RedirectToAction("Search");
            }

            // This should list the data that is in your index
            return View("Results", vmData);
        }


        [HttpGet]
        public ActionResult Results(List<HadoopMetaDataModels> searchResults)
        {
            var result = new List<HadoopMetaDataModels>();
            result = searchResults;

            return View(searchResults);
        }
    }
}
