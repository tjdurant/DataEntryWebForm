using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

// Data transfer objects
namespace DataEntryWebForm.Models
{
    public class HadoopMetaViewModels
    {

        public string Id { get; set; }
        public string ExtractName { get; set; }
        public string Description { get; set; }
        
        [AllowHtml]
        public string DescriptionHtml { get; set; }
        public string Requestor { get; set; }
        public string RequestorEmail { get; set; }
        public string Request { get; set; }
        public string DataSources { get; set; }
        public string DataExtractDetails { get; set; }
        public IList<string> ClusterStorageLocation { get; set; }
        public string ClusterStoragePath { get; set; }
        public DateTime StartDate { get; set; }

    }

}