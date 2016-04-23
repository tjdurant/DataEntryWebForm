using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataEntryWebForm.Helpers
{
    public class SelectListHelper
    {
        public static IEnumerable<SelectListItem> GetStorageLocations()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="1",Text="HDFS"},
                 new SelectListItem{ Value="2",Text="Elastic"},
                 new SelectListItem{ Value="3",Text="HBase"},
                 new SelectListItem{ Value="4",Text="Other"},
             };
            myList = data.ToList();
            return myList;
        }
    }
}