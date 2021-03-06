﻿using System;
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

        // HttpPost Methods ################################################################


        // TODO: change to ViewModel
        [HttpGet]
        public ActionResult Index()
        {
            // List all data from default index
            return View(_eq.IndexDetails());
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


        // TODO: change to ViewModel
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


        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }


        // HttpPost Methods ################################################################

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ExtractName,Description,DescriptionHtml,Requestor,RequestorEmail,DataSources,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {

            // instantiate elastic client from data access layer
            EsClient es = new EsClient();

            hadoopMetaDataModels.Id = Guid.NewGuid().ToString();

            // TODO: Reduce to function?
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ExtractName,Description,DescriptionHtml,Requestor,RequestorEmail,Request,DataExtractDetails,ClusterStorageLocation,ClusterStoragePath,StartDate")] HadoopMetaDataModels hadoopMetaDataModels)
        {

            // set description(without html) to model.Description 
            hadoopMetaDataModels.Description = TextParseHelper.StripHtml(hadoopMetaDataModels.DescriptionHtml);

            // Clear model state for IsValid Check
            ModelState.Clear();

            // If modelstate not valid; return to Edit_Post
            if (!ModelState.IsValid) return View(hadoopMetaDataModels);

            // Else: Index changes and return to Index Action
            _eq.Current.Index<HadoopMetaDataModels>(hadoopMetaDataModels);
            return RedirectToAction("Index");
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            _eq.Current.Delete<HadoopMetaDataModels>(d => d
               .Id(id.ToString())
               .Index("hadoop_metadata"));
            
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModels model)
        {
            // Search elastic index with search Query
            var searchResults = _eq.SearchElastic(model.Query);

            // Instantiate ViewModel
            var vmData = new List<HadoopMetaViewModels>();

            if (searchResults != null)
            {
                foreach (HadoopMetaDataModels item in searchResults)
                {
                    var vm = new HadoopMetaViewModels();
                    {
                        vm.Id = item.Id;
                        vm.ExtractName = item.ExtractName;
                        vm.ClusterStorageLocation = item.ClusterStorageLocation;
                        vm.ClusterStoragePath = item.ClusterStoragePath;
                        vm.DataExtractDetails = item.DataExtractDetails;
                        vm.DataSources = item.DataSources;
                        vm.Description = item.Description;
                        vm.DescriptionHtml = item.DescriptionHtml;
                        vm.Request = item.Request;
                        vm.RequestorEmail = item.RequestorEmail;
                        vm.Requestor = item.Requestor;
                        vm.StartDate = item.StartDate;
                    }
                    ;
                    vmData.Add(vm);
                }
            }
            else
            {
                return RedirectToAction("Search");
            }

            // Return results page with ViewModel
            return View("Results", vmData);
        }

    }
}
