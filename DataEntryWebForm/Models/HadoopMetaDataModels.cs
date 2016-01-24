using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Data transfer objects
namespace DataEntryWebForm.Models
{
    [ElasticType(Name = "hadoop_metadata")]
    public class HadoopMetaDataModels
    {
        [ElasticProperty(Name = "_id", NumericType = NumberType.Long)]
        public string Id { get; set; }

        [ElasticProperty(Name = "hasChildren")]
        public string ExtractName { get; set; }

        [ElasticProperty(Name = "hasChildren")]
        public string Description { get; set; }

        [ElasticProperty(Name = "hasChildren")]
        public string Requestor { get; set; }

        [ElasticProperty(Name = "hasChildren")]
        public string RequestorEmail { get; set; }

        [ElasticProperty(Name = "hasChildren")]
        public string Request { get; set; }

        [ElasticProperty(Name = "hasChildren")]
        public string[] DataSources { get; set; }

        [ElasticProperty(Name = "hasChildren")]
        public string DataExtractDetails { get; set; }

        // select box; hdfs, hbase, elastic, other
        public string ClusterStorageLocation { get; set; }

        public string ClusterStoragePath { get; set; }
        public DateTime StartDate { get; set; }
    }
}