using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PriceCheck.Utility
{
    class KomplettScraper
    {
        public string FetchKomplettTitle(string rawHtml)
        {
            Regex kompettGetPTitle = new Regex(@"<span data-bind=""text: hereText"">(?<title>.+?)<");
            return WebUtility.HtmlDecode(kompettGetPTitle.Match(rawHtml).Groups["title"].ToString());
        }
        public string FetchKomplettPNumber(string rawHtml)
        {
            Regex komplettGetPTitle = new Regex(@"<span itemprop=""sku"">(?<pNumber>\d+?)<");
            return komplettGetPTitle.Match(rawHtml).Groups["pNumber"].ToString();
        }
        public string FetchKomplettPrice(string rawHtml)
        {
            Regex komplettGetPrice = new Regex(@"<span class=""product-price-now"" itemprop=price content=(?<price>\d+)\.?(?<price2>\d+?)>");
            return komplettGetPrice.Match(rawHtml).Groups["price"].ToString() + komplettGetPrice.Match(rawHtml).Groups["price2"].ToString();
        }
        public string FetchKomplettStockStatus(string rawHtml)
        {
            Regex kompettGetPStockStatus = new Regex(@"<span class=""stockstatus-stock-details"">(?<stockStatus>.+?)<");
            return WebUtility.HtmlDecode(kompettGetPStockStatus.Match(rawHtml).Groups["stockStatus"].ToString());
        }

        public string FetchKomplettPType(string pUrl)
        {
            //TODO HDD og SSD har samme pType, kig på harddiskssd/ssd-25 
            Regex proshopPType = new Regex(@"(https|http)://www.komplett.dk/product/\d+?/.+?/(?<pType>.+?)/");
            return proshopPType.Match(pUrl).Groups["pType"].ToString().TrimEnd();
        }
        public string KomplettPTypeMapping(string pTypeString)
        {
            switch (pTypeString)
            {
                case "stroemforsyninger":
                    return "PSU";
                case "Harddisk-SSD":
                    return "SSD";
                case "Harddisk":
                    return "HDD";
                case "processorer":
                    return "CPU";
                case "blaeserekoelerevandkoeling":
                    return "Cooling";
                case "bundkort":
                    return "MOBO";
                case "grafikkort":
                    return "GPU";
                case "hukommelse-ram":
                    return "RAM";
                case "kabinetterbarebone":
                    return "Case";
                default:
                    return "Misc";
            }
        }
    }
}
