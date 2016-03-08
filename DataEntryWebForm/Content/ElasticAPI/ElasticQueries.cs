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


        public List<HadoopMetaDataModels> IdDetails(string id)
        {

            string id_query = Path.Combine(queryDslPath, "id_query.txt");

            // instatiate data objects
            var idDetailsResult = new List<HadoopMetaDataModels>();

            // read .txt file into string
            string queryString = File.ReadAllText(id_query);

            queryString = queryString.Replace("***id", id);

            // run elastic search with raw JSON query string
            var searchResult = ec.Current.Search<HadoopMetaDataModels>(s => s
                .QueryRaw(queryString)
                );

            idDetailsResult = searchResult.Documents.ToList();

            return idDetailsResult;
        }
    }
}