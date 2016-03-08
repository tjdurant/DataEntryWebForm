using DataEntryWebForm.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataEntryWebForm.DataAccessLayer
{
    public class EsClient
    {

        /// URI 
        private const string elastic_uri = "http://localhost:9200";

        /// Elastic settings
        private ConnectionSettings _settings;

        /// Current instantiated client
        public ElasticClient Current { get; set; }

       

        /// Constructor
        public EsClient()
        {
            var node = new Uri(elastic_uri);

            _settings = new ConnectionSettings(node);
            _settings.SetDefaultIndex(Constants.DEFAULT_INDEX);
            _settings.MapDefaultTypeNames(m => m.Add(typeof(HadoopMetaDataModels), Constants.DEFAULT_INDEX_TYPE));

            Current = new ElasticClient(_settings);
            Current.Map<HadoopMetaDataModels>(m => m
                .MapFromAttributes());
            
        }

    }
}