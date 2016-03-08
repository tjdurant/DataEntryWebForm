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

namespace DataEntryWebForm.Controllers
{

    public class DataEntryController : Controller
    {
        private DataEntryWebFormContext db = new DataEntryWebFormContext();
        private ElasticQueries eq = new ElasticQueries();

        // GET: DataEntry
        public ActionResult Index()
        {
            // This should list the data that is in your index
            return View(eq.IndexDetails());
        }

        // GET: DataEntry/Details/5
        public ActionResult Details(string id)
        {
            // The id is coming from Idex.cshtml item.Id from the details action link. 
            // Need to figure out how to index/analyze the actual _id or _uid field, and then return it.
            // Went to go work on search box for documents that can appear on index page. 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // This should list the data for the id selected
            return View(eq.IdDetails(id));
        }

        // GET: DataEntry/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataEntry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ExtractName,Description,Requestor,RequestorEmail,DataSources,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {
            // instantiate elastic client from data access layer
            EsClient es = new EsClient();

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

            if (ModelState.IsValid)
            {
                es.Current.Index<HadoopMetaDataModels>(hadoopMetaDataModels);
                return RedirectToAction("Index");
            }

            return View(hadoopMetaDataModels);
        }

        // GET: DataEntry/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tommy: will have to change this to NEST format.Currently using EF formatting
            HadoopMetaDataModels hadoopMetaDataModels = db.HadoopMetaDataModels.Find(id);
            if (hadoopMetaDataModels == null)
            {
                return HttpNotFound();
            }
            return View(hadoopMetaDataModels);
        }

        // POST: DataEntry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ExtractName,Description,Requestor,RequestorEmail,Request,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {
            // system.
            if (ModelState.IsValid)
            {
                // Tommy: will have to change this to NEST format.Currently using EF formatting
                db.Entry(hadoopMetaDataModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hadoopMetaDataModels);
        }

        // GET: DataEntry/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tommy: will have to change this to NEST format. Currently using EF formatting
            HadoopMetaDataModels hadoopMetaDataModels = db.HadoopMetaDataModels.Find(id);
            if (hadoopMetaDataModels == null)
            {
                return HttpNotFound();
            }
            return View(hadoopMetaDataModels);
        }

        // POST: DataEntry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HadoopMetaDataModels hadoopMetaDataModels = db.HadoopMetaDataModels.Find(id);
            db.HadoopMetaDataModels.Remove(hadoopMetaDataModels);
            db.SaveChanges();
            return RedirectToAction("Index");
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
