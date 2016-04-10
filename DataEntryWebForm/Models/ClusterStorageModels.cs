using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataEntryWebForm.Models
{
    public class ClusterStorageModels
    {
        public IEnumerable<string> SelectedStorageLocations { get; set; }
        public IEnumerable<SelectListItem> StorageLocations { get; set; }
    }
}