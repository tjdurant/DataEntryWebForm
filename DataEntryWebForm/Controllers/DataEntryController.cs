using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataEntryWebForm.Models;
using DataEntryWebForm.DataAccessLayer;
using DataEntryWebForm.Content.ElasticAPI;
using System.IO;
using Nest;
using Newtonsoft.Json;
using DataEntryWebForm.Helpers;
using System.Globalization;

namespace DataEntryWebForm.Controllers
{

    public class DataEntryController : Controller
    {
        private DataEntryWebFormContext db = new DataEntryWebFormContext();
        private ElasticQueries eq = new ElasticQueries();

        
        [HttpGet]
        public ActionResult Index()
        {
            // List all data from default index
            return View(eq.IndexDetails());
        }


        [HttpGet]
        public ActionResult Details(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // list details of document with specific id
            return View(eq.IdDetails(id));
        }


        [HttpGet]
        public ActionResult Create()
        {

            var model = new HadoopMetaDataModels();

            // populates list for ListBoxFor from a method within the HaddopMetaDataModels
            model.StorageLocations = model.getStorageLocations();

            return View(model);
        }


        // may need to add 'IEnumerable<string> SelectItems' in parameters http://dotnetvisio.blogspot.com/2014/01/get-values-of-multiselect-listbox-in.html
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ExtractName,Description,DescriptionHtml,Requestor,RequestorEmail,DataSources,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {
            // instantiate elastic client from data access layer
            EsClient es = new EsClient();
            TextParseHelper th = new TextParseHelper();

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

            // strip html from ckeditor description input
            var description = th.StripHtml(hadoopMetaDataModels.DescriptionHtml);
            
            // set description(without html) to model.Description 
            hadoopMetaDataModels.Description = description;

            // for testing
            if (hadoopMetaDataModels.ClusterStorageLocation == null)
            {
                return RedirectToAction("Create");
            }

            ModelState.SetModelValue("Description", new ValueProviderResult(description, "", CultureInfo.InvariantCulture));

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

            var model = new HadoopMetaDataModels();

            model.StorageLocations = model.getStorageLocations();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(eq.IdDetails(id));
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ExtractName,Description,DescriptionHtml,Requestor,RequestorEmail,Request,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {

            // instantiate elastic client from data access layer
            EsClient es = new EsClient();

            // instantiate textParseHelper
            TextParseHelper th = new TextParseHelper();

            // strip html from ckeditor description input
            var description = th.StripHtml(hadoopMetaDataModels.DescriptionHtml);

            // set description(without html) to model.Description 
            hadoopMetaDataModels.Description = description;

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

            return View(eq.IdDetails(id));
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
        public ActionResult Search(SearchElasticModels model)
        {
            var searchResults = new List<HadoopMetaDataModels>();
            searchResults = eq.SearchElastic(model.Query);

            if(searchResults.Count() == 0)
            {
                return RedirectToAction("Search");
            }

            // This should list the data that is in your index
            return View("Results", searchResults );
        }


        [HttpGet]
        public ActionResult Results(List<HadoopMetaDataModels> searchResults)
        {
            //var result = new List<HadoopMetaDataModels>();
            //result = searchResults;

            return View(searchResults);
        }


        // Don't know what this does.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
