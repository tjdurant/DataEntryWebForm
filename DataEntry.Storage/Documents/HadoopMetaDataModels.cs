using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
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

        // [Required(ErrorMessage = "Field Required")]
        [AllowHtml]
        [ElasticProperty(Name = "description_html", Index = FieldIndexOption.No)]
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
        public IList<string> ClusterStorageLocation { get; set; }

        [ElasticProperty(Name = "storage_path")]
        public string ClusterStoragePath { get; set; }

        [ElasticProperty(Name = "start_date")]
        public DateTime StartDate { get; set; }

    }


    public class EditHadoopDataModels
    {

        public double Id { get; set; }
        public IList<HadoopMetaDataModels> HadoopMetaData { get; set; }

    }


    public class SearchViewModels
    {

        public string Query { get; set; }

    }
}