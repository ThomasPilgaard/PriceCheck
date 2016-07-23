using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PriceCheck.Utility
{
    public class ProshopScraper
    {
        //TODO Have tjek for udgåede produkter
        public string FetchProshopTitle(string rawHtml)
        {
            Regex proshopGetName = new Regex(@"<h1 itemprop=""name""\s+\n\s+data-productid=""\d+""\s+\n\s+data-siteid="".+"">\s+\n\s+(?<name>.+)");
            return proshopGetName.Match(rawHtml).Groups["name"].ToString().TrimEnd();
        }
        public string FetchProshopPNumber(string rawHtml)
        {
            Regex proshopGetPNumber = new Regex(@"<h1 itemprop=""name"".+\n.+data-productid=""(?<pNumber>\d+)""");
            return proshopGetPNumber.Match(rawHtml).Groups["pNumber"].ToString().TrimEnd();
        }
        public string FetchProshopPrice(string rawHtml)
        {
            Regex proshopGetPriceTag = new Regex(@"<span class=""site-currency-attention""><span class=""text-right"">((?<price1>\d+?)\.)?(?<price2>\d+?),(?<price3>\d+)");
            return proshopGetPriceTag.Match(rawHtml).Groups["price1"].ToString() +
                   proshopGetPriceTag.Match(rawHtml).Groups["price2"].ToString() +
                   "." +
                   proshopGetPriceTag.Match(rawHtml).Groups["price3"].ToString();
        }//
        public string FetchProshopStockStatus(string rawHtml)
        {
            //TODO Overvej at udvide med om den ikke er på lager/bestilt/på vej osv.
            Regex proshopGetStockStatus = new Regex(@"<div class=""site-stock-text site-inline"">(?<stockStatus>.+)<");
            return WebUtility.HtmlDecode(proshopGetStockStatus.Match(rawHtml).Groups["stockStatus"].ToString().TrimEnd());
        }
        public string FetchProshopPType(string pUrl)
        {
            Regex proshopPType = new Regex(@"(https|http)://www.proshop.dk/(?<pType>.+?)/");
            return proshopPType.Match(pUrl).Groups["pType"].ToString().TrimEnd();
        }
        public string ProshopPTypeMapping(string pTypeString)
        {
            switch (pTypeString)
            {
                case "Stroemforsyning":
                    return "PSU";
                case "Harddisk-SSD":
                    return "SSD";
                case "Harddisk":
                    return "HDD";
                case "CPU":
                    return "CPU";
                case "CPU-Koeler":
                    return "Cooling";
                case "Bundkort":
                    return "MOBO";
                case "Grafikkort":
                    return "GPU";
                case "RAM":
                    return "RAM";
                case "Kabinet":
                    return "Case";
                case "Skaerm":
                    return "Monitor";
                default:
                    return "Misc";
            }
        }
    }
}
