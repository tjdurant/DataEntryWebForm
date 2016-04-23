using System;
using System.Configuration;
using DataEntry.Storage.Documents;
using Nest;

namespace DataEntry.Storage
{
    public class EsClient
    {

        /// URI 
        private const string ElasticUri = "http://localhost:9200";

        /// Elastic settings
        private ConnectionSettings _settings;

        /// Current instantiated client
        public ElasticClient Current { get; set; }

       

        /// Constructor
        public EsClient()
        {
            var node = new Uri(ElasticUri);

            _settings = new ConnectionSettings(node);
            _settings.SetDefaultIndex(ConfigurationManager.AppSettings["DefaultIndex"]);
            _settings.MapDefaultTypeNames(m => m.Add(typeof(HadoopMetaDataModels), ConfigurationManager.AppSettings["DefaultIndexType"]));

            Current = new ElasticClient(_settings);
            Current.Map<HadoopMetaDataModels>(m => m
                .MapFromAttributes());
            
        }

    }
}