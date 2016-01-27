using DataEntryWebForm.DataAccessLayer;
using DataEntryWebForm.Models;
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

            var queryDslPath = @"~/Content/ElasticQuery/Dsl/details_index.txt";

            // instantiate connection object
            EsClient ec = new EsClient();
            
            // instatiate data objects
            var indexDetails = new List<HadoopMetaDataModels>();

            // read .txt file into string
            string aggString = File.ReadAllText(queryDslPath);

            // run elastic search with raw JSON query string
            var searchResult = ec.Current.Search<HadoopMetaDataModels>(s => s
                .QueryRaw(aggString)
                );

            // instantiate bucket of aggregations based on timeInterval 
            var agBucket = (Bucket)searchResult.Aggregations["my_date_histo"];

            // iterate through each date-item an aggregation was performed on
            foreach (HistogramItem item in agBucket.Items)
            {
                // access the valueMetric for each nested aggregation 
                var mov_avg = (ValueMetric)item.Aggregations["agg_avg"];

                // convert date_histogram to dateTime object
                var date = item.Date;

                // convert valueMetric to value
                var avg_value = mov_avg.Value;

                // pass aggregation data into model 
                var avgResult = new MovingAverageVals
                {
                    Value = avg_value,
                    Date = date
                };

                avgList.Add(avgResult);
            }

            // pass component, timeInterval and avgList into model
            var cur = new MovingAverageModels
            {
                Component = component,
                TimeResolution = timeInterval,
                AvgVals = avgList
            };

            ma.Add(cur);
            return ma;
        }
    }
}