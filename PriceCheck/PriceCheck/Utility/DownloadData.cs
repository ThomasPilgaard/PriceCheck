using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriceCheck.Utility
{
    class DownloadData
    {
        public string FetchRawHtml(string urlFromPUrlTextBox)
        {
            WebClient w = new WebClient { Encoding = Encoding.UTF8 };
            try
            {
                return w.DownloadString(urlFromPUrlTextBox);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
