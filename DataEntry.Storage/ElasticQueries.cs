using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using DataEntry.Storage.Documents;

namespace DataEntry.Storage
{
    public class ElasticQueries : EsClient
    {

        // instantiate connection object
        private readonly EsClient _ec = new EsClient();

        // set queryDsl path
        private readonly string _queryDslPath;

        // Constructor
        public ElasticQueries(string dslPath)
        {
            _queryDslPath = dslPath;
        } 

        public List<HadoopMetaDataModels> IndexDetails()
        {
            // instatiate data objects
            var indexDetails = new List<HadoopMetaDataModels>();

            // combine path and read .txt file into string
            var detailsIndex = Path.Combine(_queryDslPath, "details_index.txt");
            var queryString = File.ReadAllText(detailsIndex);

            // run elastic search with raw JSON query string
            var searchResult = _ec.Current.Search<HadoopMetaDataModels>(s => s
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

            indexDetails = Enumerable.ToList(searchResult.Documents);
            
            return indexDetails;
        }


        public HadoopMetaDataModels IdDetails(string id)
        {

            string idQuery = Path.Combine(_queryDslPath, "id_query.txt");

            // instatiate data objects
            var idDetailsResult = new HadoopMetaDataModels();

            // read .txt file into string
            string queryString = File.ReadAllText(idQuery);

            queryString = queryString.Replace("***id", id);

            // run elastic search with raw JSON query string
            var searchResult = _ec.Current.Search<HadoopMetaDataModels>(s => s
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
                // TODO: test this 
                idDetailsResult.ClusterStorageLocation = item.Source.ClusterStorageLocation ?? new List<string>();
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
            string search = Path.Combine(_queryDslPath, "search_query.txt");

            // read .txt file into string
            string searchString = File.ReadAllText(search);

            searchString = searchString.Replace("***query", query);


            // run elastic search with raw JSON query string
            var elasticResult = _ec.Current.Search<HadoopMetaDataModels>(s => s
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

            searchResults = Enumerable.ToList(elasticResult.Documents);

            return searchResults;
        }
    }
}