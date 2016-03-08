using DataEntryWebForm.Content.ElasticAPI;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

// Data transfer objects
namespace DataEntryWebForm.Models
{
    [ElasticType(Name = "hadoop_metadata")]
    public class HadoopMetaDataModels
    {

        /// _id field
        [ElasticProperty(Name = "_id", NumericType = NumberType.Long, Index = FieldIndexOption.Analyzed)]
        public Guid Id { get; set; }

        [ElasticProperty(Name = "extract_name")]
        public string ExtractName { get; set; }

        [ElasticProperty(Name = "description")]
        public string Description { get; set; }

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
        public string ClusterStorageLocation { get; set; }

        [ElasticProperty(Name = "storage_path")]
        public string ClusterStoragePath { get; set; }

        [ElasticProperty(Name = "start_date")]
        public DateTime StartDate { get; set; }

    }


    public class EditHadoopDataModels
    {
        private ElasticQueries eq = new ElasticQueries();

        // GET: DataEntry
        public EditHadoopDataModels()
        {
            // Id = eq.IndexDetails();
        }

        public double Id { get; set; }
        public List<HadoopMetaDataModels> HadoopMetaData { get; set; }

    }
}