using System;
using System.Collections.Generic;
using Nest;

// Data transfer objects
namespace DataEntry.Storage.Documents
{
    [ElasticType(Name = "hadoop_metadata")]
    public class HadoopMetaDataModels
    {

        /// _id field
        [ElasticProperty(Name = "Id")]
        public string Id { get; set; }

        // [Required(ErrorMessage = "Field Required")]
        [ElasticProperty(Name = "extract_name")]
        public string ExtractName { get; set; }

        [ElasticProperty(Name = "description")]
        public string Description { get; set; }

        // BUG: This is still searchable with query despite the NotAnalyzed
        // [AllowHtml]
        // [Required(ErrorMessage = "Field Required")]
        [ElasticProperty(Name = "description_html", Index = FieldIndexOption.NotAnalyzed)]
        public string DescriptionHtml { get; set; }

        [ElasticProperty(Name = "requestor")]
        public string Requestor { get; set; }

        [ElasticProperty(Name = "requestor_email")]
        public string RequestorEmail { get; set; }

        [ElasticProperty(Name = "request")]
        public string Request { get; set; }

        [ElasticProperty(Name = "data_sources")]
        public string DataSources { get; set; }

        [ElasticProperty(Name = "data_extract_details")]
        public string DataExtractDetails { get; set; }

        // select box; hdfs, hbase, elastic, other
        [ElasticProperty(Name = "storage_location")]
        public List<string> ClusterStorageLocation { get; set; }

        // public IEnumerable<SelectListItem> StorageLocations { get; set; }

        [ElasticProperty(Name = "storage_path")]
        public string ClusterStoragePath { get; set; }

        [ElasticProperty(Name = "start_date")]
        public DateTime StartDate { get; set; }

    }


    public class EditHadoopDataModels
    {
        private ElasticQueries _eq = new ElasticQueries();

        // GET: DataEntry
        public EditHadoopDataModels()
        {
            // Id = eq.IndexDetails();
        }

        public double Id { get; set; }
        public List<HadoopMetaDataModels> HadoopMetaData { get; set; }

    }

    public class SearchElasticModels
    {

        public string Query { get; set; }

    }
}