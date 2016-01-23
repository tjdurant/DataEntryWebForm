using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataEntryWebForm.Models
{
    [ElasticType(Name = "hadoop_metadata")]
    public class HadoopMetaDataModels
    {
        public string Id { get; set; }

        public string ExtractName { get; set; }
        public string Description { get; set; }
        public string Requestor { get; set; }
        public string RequestorEmail { get; set; }
        public string Request { get; set; }
        public string[] DataSources { get; set; }
        public string DataExtractDetails { get; set; }

        // select box; hdfs, hbase, elastic, other
        public string ClusterStorageLocation { get; set; }

        public string ClusterStoragePath { get; set; }
        public DateTime StartDate { get; set; }
    }
}