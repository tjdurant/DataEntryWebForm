using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataEntryWebForm.Helpers
{
    public static class TextParseHelper
    {
        public static string StripHtml(string html)
        {
            if (html == null || string.IsNullOrEmpty(html))
            {
                return "";
            }

            var htmlDoc = new HtmlDocument();

            // load a string
            htmlDoc.LoadHtml(html); 

            return htmlDoc.DocumentNode.InnerText;
        }
    }
}