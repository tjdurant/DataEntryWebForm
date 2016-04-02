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

namespace DataEntryWebForm.Controllers
{

    public class DataEntryController : Controller
    {
        private DataEntryWebFormContext db = new DataEntryWebFormContext();
        private ElasticQueries eq = new ElasticQueries();

        public ActionResult Index()
        {
            // This should list the data that is in your index
            return View(eq.IndexDetails());
        }

        public ActionResult Details(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(eq.IdDetails(id));
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: DataEntry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ExtractName,Description,DescriptionHtml,Requestor,RequestorEmail,DataSources,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {
            // instantiate elastic client from data access layer
            EsClient es = new EsClient();
            TextParseHelper th = new TextParseHelper();

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

            var description = th.StripHtml(hadoopMetaDataModels.DescriptionHtml);

            hadoopMetaDataModels.Description = description;

            if (ModelState.IsValid)
            {
                es.Current.Index<HadoopMetaDataModels>(hadoopMetaDataModels);
                return RedirectToAction("Index");
            }

            return View(hadoopMetaDataModels);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(eq.IdDetails(id));
        }

        // POST: DataEntry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ExtractName,Description,Requestor,RequestorEmail,Request,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {

            // instantiate elastic client from data access layer
            EsClient es = new EsClient();

            if (ModelState.IsValid)
            {
                es.Current.Index<HadoopMetaDataModels>(hadoopMetaDataModels);
                return RedirectToAction("Index");
            }
            return View(hadoopMetaDataModels);
        }

        // GET: DataEntry/Delete/5
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

        public ActionResult Results(List<HadoopMetaDataModels> searchResults)
        {
            //var result = new List<HadoopMetaDataModels>();
            //result = searchResults;

            return View(searchResults);
        }



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
