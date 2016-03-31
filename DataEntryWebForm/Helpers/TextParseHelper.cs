using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataEntryWebForm.Helpers
{
    public class TextParseHelper
    {
        public string StripHtml(string html)
        {
            var htmlDoc = new HtmlDocument();

            // load a string
            htmlDoc.LoadHtml(html); 

            return htmlDoc.DocumentNode.InnerText;
        }
    }
}