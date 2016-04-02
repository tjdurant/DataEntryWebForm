using DataEntryWebForm.DataAccessLayer;
using DataEntryWebForm.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DataEntryWebForm.Content.ElasticAPI
{
    public class ElasticQueries : EsClient
    {
        // instantiate connection object
        private EsClient ec = new EsClient();

        // set queryDsl path
        private string queryDslPath = HttpContext.Current.Server.MapPath(@"~\App_Data\ElasticQueryDsl");

        public List<HadoopMetaDataModels> IndexDetails()
        {

            string details_index = Path.Combine(queryDslPath, "details_index.txt");

            // instatiate data objects
            var indexDetails = new List<HadoopMetaDataModels>();

            // read .txt file into string
            string queryString = File.ReadAllText(details_index);

            // run elastic search with raw JSON query string
            var searchResult = ec.Current.Search<HadoopMetaDataModels>(s => s
                .QueryRaw(queryString)
                );

            var list = searchResult.Hits.Select(h =>
            {
                return h.Source;
            }).ToList();

            //var results = searchResult.Hits.Select(hit =>
            //{
            //    var run = hit.Source;
            //    run.Id = hit.Id;
            //    return run;
            //});

            indexDetails = searchResult.Documents.ToList();
            
            return indexDetails;
        }


        public HadoopMetaDataModels IdDetails(string id)
        {

            string id_query = Path.Combine(queryDslPath, "id_query.txt");

            // instatiate data objects
            var idDetailsResult = new HadoopMetaDataModels();

            // read .txt file into string
            string queryString = File.ReadAllText(id_query);

            queryString = queryString.Replace("***id", id);

            // run elastic search with raw JSON query string
            var searchResult = ec.Current.Search<HadoopMetaDataModels>(s => s
                .QueryRaw(queryString)
                );

            foreach(var item in searchResult.Hits)
            {
                idDetailsResult.Id = item.Source.Id;
                idDetailsResult.ExtractName = item.Source.ExtractName;
                idDetailsResult.Description = item.Source.Description;
                idDetailsResult.DescriptionHtml = item.Source.DescriptionHtml;
                idDetailsResult.Requestor = item.Source.Requestor;
                idDetailsResult.RequestorEmail = item.Source.RequestorEmail;
                idDetailsResult.Request = item.Source.Request;
                idDetailsResult.DataSources = item.Source.DataSources;
                idDetailsResult.DataExtractDetails = item.Source.DataExtractDetails;
                idDetailsResult.ClusterStorageLocation = item.Source.ClusterStorageLocation;
                idDetailsResult.ClusterStoragePath = item.Source.ClusterStoragePath;
                idDetailsResult.StartDate = item.Source.StartDate;
            }

            return idDetailsResult;
        }

        public List<HadoopMetaDataModels> SearchElastic(string query)
        {

            // instatiate data objects
            var searchResults = new List<HadoopMetaDataModels>();

            // instantiate .txt file
            string search = Path.Combine(queryDslPath, "search_query.txt");

            // read .txt file into string
            string searchString = File.ReadAllText(search);

            searchString = searchString.Replace("***query", query);


            // run elastic search with raw JSON query string
            var elasticResult = ec.Current.Search<HadoopMetaDataModels>(s => s
                .QueryRaw(searchString)
                );

            var list = elasticResult.Hits.Select(h =>
            {
                return h.Source;
            }).ToList();

            //var results = searchResult.Hits.Select(hit =>
            //{
            //    var run = hit.Source;
            //    run.Id = hit.Id;
            //    return run;
            //});

            searchResults = elasticResult.Documents.ToList();

            return searchResults;
        }
    }
}