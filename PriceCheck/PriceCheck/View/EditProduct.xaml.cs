using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PriceCheck.Model;

namespace PriceCheck.View
{
    /// <summary>
    /// Interaction logic for EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        private Product _choosenProduct;
        
        public EditProduct(Product choosenProduct)
        {
            InitializeComponent();
            _choosenProduct = choosenProduct;

            PUrlTxtBox.Text = _choosenProduct.PUrl;
            PNumberTxtBox.Text = _choosenProduct.pNumber;
            PNameTxtBox.Text = _choosenProduct.pName;
            PPriceTxtBox.Text = _choosenProduct.pPrice.ToString("0.00");
            PStockStatus.Text = _choosenProduct.PStockStatus;

            PTypeCombobox.ItemsSource = Enum.GetValues(typeof(Product.ProductType)).Cast<Product.ProductType>();
            PTypeCombobox.SelectedItem = _choosenProduct.pType;
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }
        private void EditProduct_OnClosing(object sender, CancelEventArgs e)
        {
            if (UnsavedChanges())
            {
                //MessageBox.Show("Unsaved changes.");
                //TODO Evt lave det om til Yes No Cancel, "Do you want to save the changes?"
                if (MessageBox.Show("There are unsaved changes. Do you wish do continue closing the window and destroy the changes?", "Unsaved changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                     //do yes stuff, continue closing
                }
            }
        }
        private void SaveData()
        {
            _choosenProduct.pName = PNameTxtBox.Text;
            _choosenProduct.PUrl = PUrlTxtBox.Text;
            _choosenProduct.pNumber = PNumberTxtBox.Text;
            _choosenProduct.pType = (Product.ProductType)PTypeCombobox.SelectedItem;
            _choosenProduct.pPrice = CheckPriceFormat();
            _choosenProduct.PStockStatus = PStockStatus.Text;
        }

        private double CheckPriceFormat()
        {
            double returnVal = _choosenProduct.pPrice;
            if (!PPriceTxtBox.Text.Contains(","))
            {
                try
                {
                    return returnVal = Math.Round(Convert.ToDouble(PPriceTxtBox.Text),2);
                }
                catch (FormatException)
                {

                    MessageBox.Show("Please provide a price containing only numbers.", "Price contains letters.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return returnVal;
                }
            }
            else MessageBox.Show("Please use a dot instead of comma in the product price.", "Price contains comma.", MessageBoxButton.OK, MessageBoxImage.Warning);
            return returnVal;
        }

        private bool UnsavedChanges()
        {
            if (PNameTxtBox.Text != _choosenProduct.pName  ||
                PUrlTxtBox.Text != _choosenProduct.PUrl ||
                PNumberTxtBox.Text != _choosenProduct.pNumber ||
                (Product.ProductType) PTypeCombobox.SelectedItem != _choosenProduct.pType ||
                PPriceTxtBox.Text != _choosenProduct.pPrice.ToString("0.00") ||
                PStockStatus.Text != _choosenProduct.PStockStatus)
            {
                return true;
            }
            else return false;
        }

        private void FetchData_Click(object sender, RoutedEventArgs e)
        {


            //TODO Lav for Komplett

            Regex proshopUrlRegex = new Regex(@"^(https|http)://www.proshop.dk/");
            Regex komplettUrlRegex = new Regex(@"^(https|http)://www.komplett.dk");

            if (proshopUrlRegex.IsMatch(_choosenProduct.PUrl))
            {
                //TODO evt. HTML decode Proshop titles
                PPriceTxtBox.Text = FetchProshopPrice();
                PNameTxtBox.Text = FetchProshopTitle();
                PNumberTxtBox.Text = FetchProshopPNumber();
                PStockStatus.Text = FetchProshopStockStatus();
                testTextBox.Text = FetchProshopPType();
                PTypeCombobox.SelectedItem = (Product.ProductType) Enum.Parse(typeof(Product.ProductType), productPTypeMapping(FetchProshopPType()));
            }
            else if (komplettUrlRegex.IsMatch(_choosenProduct.PUrl))
            {
                PPriceTxtBox.Text = FetchKomplettPrice() + ".00";
                PNameTxtBox.Text = FetchKomplettTitle();
                PNumberTxtBox.Text = FetchKomplettPNumber();
                PStockStatus.Text = FetchKomplettStockStatus();
                PTypeCombobox.SelectedItem = (Product.ProductType)Enum.Parse(typeof(Product.ProductType), productPTypeMapping(FetchKomplettPType()));
                TestLabel.Content = FetchKomplettPType();
            }
            else MessageBox.Show("No match");

        }
#region KomplettRegex
        private string FetchKomplettPrice()
        {
            Regex komplettGetPrice = new Regex(@"<span class=""product-price-now"" itemprop=price content=(?<price>\d+)\.?(?<price2>\d+?)>");
            string price = komplettGetPrice.Match(FetchRawHtml()).Groups["price"].ToString();
            string price2 = komplettGetPrice.Match(FetchRawHtml()).Groups["price2"].ToString();
            return price + price2;
        }

        private string FetchKomplettTitle()
        {
            Regex kompettGetPTitle = new Regex(@"<span data-bind=""text: hereText"">(?<title>.+?)<");
            string title = WebUtility.HtmlDecode(kompettGetPTitle.Match(FetchRawHtml()).Groups["title"].ToString());
            return title;
        }

        private string FetchKomplettPNumber()
        {
            Regex kompettGetPTitle = new Regex(@"<span itemprop=""sku"">(?<pNumber>\d+?)<");
            string pNumber = kompettGetPTitle.Match(FetchRawHtml()).Groups["pNumber"].ToString();
            return pNumber;
        }

        private string FetchKomplettStockStatus()
        {
            Regex kompettGetPStockStatus = new Regex(@"<span class=""stockstatus-stock-details"">(?<stockStatus>.+?)<");
            string stockStatus = WebUtility.HtmlDecode(kompettGetPStockStatus.Match(FetchRawHtml()).Groups["stockStatus"].ToString());
            return stockStatus;
        }
        private string FetchKomplettPType()
        {
            //TODO HDD og SSD har samme pType, kig på harddiskssd/ssd-25 
            Regex proshopPType = new Regex(@"(https|http)://www.komplett.dk/product/\d+?/.+?/(?<pType>.+?)/");
            string pType = proshopPType.Match(_choosenProduct.PUrl).Groups["pType"].ToString().TrimEnd();
            return pType;
        }
        #endregion

        #region ProshopRegex
        private string productPTypeMapping(string pTypeString)
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
                case "blaeserekoelerevandkoeling":
                    return "Cooling";
                case "Bundkort":
                    return "MOBO";
                case "Grafikkort":
                case "grafikkort":
                    return "GPU";
                case "RAM":
                    return "RAM";
                case "Kabinet":
                case "kabinetterbarebone":
                    return "Case";
                default:
                    return "Misc";
            }
        }
        private string FetchProshopPType()
        {
            Regex proshopPType = new Regex(@"(https|http)://www.proshop.dk/(?<pType>.+?)/");
            string pType = proshopPType.Match(_choosenProduct.PUrl).Groups["pType"].ToString().TrimEnd();
            return pType;
        }
        private string FetchProshopStockStatus()
        {
            //TODO Overvej at udvide med om den ikke er på lager/bestilt/på vej osv.
            Regex proshopGetStockStatus = new Regex(@"<div class=""site-stock-text site-inline"" itemprop=""availability"" content="".+"">(?<stockStatus>.+)<");
            string stockStatus = WebUtility.HtmlDecode(proshopGetStockStatus.Match(FetchRawHtml()).Groups["stockStatus"].ToString().TrimEnd());
            return stockStatus;
        }
        private string FetchProshopPrice()
        {
            Regex proshopGetPriceTag = new Regex(@"<span class=""site-currency-attention"" itemprop=""price"" content=""(?<price>\d+\.\d+)");
            string price = proshopGetPriceTag.Match(FetchRawHtml()).Groups["price"].ToString().TrimEnd();
            return price;
        }

        private string FetchProshopTitle()
        {
            Regex proshopGetName = new Regex(@"<h1 itemprop=""name""\s+\n\s+data-productid=""\d+""\s+\n\s+data-siteid="".+"">\s+\n\s+(?<name>.+)");
            string name = proshopGetName.Match(FetchRawHtml()).Groups["name"].ToString().TrimEnd();
            return name;
        }

        private string FetchProshopPNumber()
        {
            Regex proshopGetPNumber = new Regex(@"<h1 itemprop=""name"".+\n.+data-productid=""(?<pNumber>\d+)""");
            string pNumber = proshopGetPNumber.Match(FetchRawHtml()).Groups["pNumber"].ToString().TrimEnd();
            return pNumber;
        }
#endregion
        private string FetchRawHtml()
        {
            WebClient w = new WebClient();
            string rawProductHtml = w.DownloadString(_choosenProduct.PUrl);
            return rawProductHtml;
        }
    }
}
