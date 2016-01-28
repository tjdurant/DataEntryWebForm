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
        public List<HadoopMetaDataModels> IndexDetails()
        {

            string queryDslPath = HttpContext.Current.Server.MapPath(@"~\App_Data\ElasticQueryDsl");

            string details_index = Path.Combine(queryDslPath, "details_index.txt");

            // instantiate connection object
            EsClient ec = new EsClient();
            
            // instatiate data objects
            var indexDetails = new List<HadoopMetaDataModels>();

            // read .txt file into string
            string queryString = File.ReadAllText(details_index);

            // run elastic search with raw JSON query string
            var searchResult = ec.Current.Search<HadoopMetaDataModels>(s => s
                .QueryRaw(queryString)
                );

            indexDetails = searchResult.Documents.ToList();
            
            return indexDetails;
        }
    }
}