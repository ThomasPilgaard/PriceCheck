using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceCheck.Model
{
    [Serializable]
    public class Product
    {
        public Product(string url)
        {
            PUrl = url;
        }
        public string PUrl { get; set; }
        public string pNumber { get; set; }
        public string pName { get; set; }
        public double pPrice { get; set; }
        public IList<KeyValuePair<DateTime, double>> PriceHist { get; set; }
        public string PStockStatus { get; set; }
        public ProductType pType { get; set; }

        public enum ProductType
        {
            Case,
            CPU,
            MOBO,
            PSU,
            Cooling,
            GPU,
            RAM,
            SSD,
            HDD,
            Misc
        }
    }
}
