using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using PriceCheck.Exceptions;
using PriceCheck.Model;
using PriceCheck.Utility;

namespace PriceCheck.View
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        private Product _addedProduct;
        public AddProduct(Product addedProduct)
        {
            InitializeComponent();
            _addedProduct = addedProduct;
        }
        private void AddProductBtn_Click(object sender, RoutedEventArgs e)
        {
            _addedProduct.PUrl = AddProductUrlTextBox.Text;
            //TODO Convert doubles to string, den gamle convert fra EditProduct skal nok generaliseres mere.
            //TODO Implement FetchData() her. 
            FetchDataAdd();
            Close();
        }

        private void FetchDataAdd()
        {
            Regex urlRegex = new Regex(@"(https|http)://(www.)?.+?\.\w+");
            Regex proshopUrlRegex = new Regex(@"(https|http)://(www.)?proshop\.dk");
            Regex komplettUrlRegex = new Regex(@"^(https|http)://(www.)?komplett\.dk");

            if (!string.IsNullOrEmpty(AddProductUrlTextBox.Text))
            {
                if (urlRegex.IsMatch(AddProductUrlTextBox.Text))
                {
                    if (proshopUrlRegex.IsMatch(AddProductUrlTextBox.Text) || komplettUrlRegex.IsMatch(AddProductUrlTextBox.Text))
                    {
                        var dl = new DownloadData();
                        string urlFromUrlTextBox = AddProductUrlTextBox.Text;
                        var rawHtml = dl.FetchRawHtml(urlFromUrlTextBox);
                        if (rawHtml == string.Empty)
                        {
                            MessageBox.Show("Cannot connect to \"" + urlFromUrlTextBox + "\"");
                        }
                        else if (proshopUrlRegex.IsMatch(AddProductUrlTextBox.Text))
                        {
                            AssignTexboxesProshop(rawHtml);
                        }
                        else if (komplettUrlRegex.IsMatch(AddProductUrlTextBox.Text))
                        {
                            AssignTexboxesKomplett(rawHtml);
                        }
                    }
                    else MessageBox.Show("Please provide a link to a supported website.");
                }
                else MessageBox.Show("Please provide a valid URL with \"http://\".");
            }
            else MessageBox.Show("Please enter a URL.");
        }
        private void AssignTexboxesProshop(string rawHtml)
        {
            var pScraper = new ProshopScraper();
            if (pScraper.FetchProshopTitle(rawHtml) != string.Empty
            || pScraper.FetchProshopPNumber(rawHtml) != string.Empty
            || pScraper.FetchProshopPrice(rawHtml) != string.Empty
            || pScraper.FetchProshopStockStatus(rawHtml) != string.Empty)
            {
                try
                {
                    _addedProduct.pPrice = CheckPrice.CheckPriceFormat(pScraper.FetchProshopPrice(rawHtml));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please provide a price containing only numbers and a single dot.",
                        "Price contains letters.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("The price is too high.", "Price too high.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                catch (PriceTooLongException)
                {
                    MessageBox.Show("The price is too long.", "Price too long.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                catch (PriceContainsCommaException)
                {
                    MessageBox.Show("Please use a dot instead of comma in the product price.", "Price contains comma.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                _addedProduct.pName = pScraper.FetchProshopTitle(rawHtml);
                _addedProduct.pNumber = pScraper.FetchProshopPNumber(rawHtml);
                
                _addedProduct.PStockStatus = pScraper.FetchProshopStockStatus(rawHtml);
                _addedProduct.pType =
                    (Product.ProductType)
                        Enum.Parse(typeof(Product.ProductType),
                            pScraper.ProshopPTypeMapping(pScraper.FetchProshopPType(AddProductUrlTextBox.Text)));
            }
            else MessageBox.Show("No product found at the given URL.");
        }
        
        private void AssignTexboxesKomplett(string rawHtml)
        {
            var kScraper = new KomplettScraper();
            if (kScraper.FetchKomplettTitle(rawHtml) != string.Empty
                || kScraper.FetchKomplettPNumber(rawHtml) != string.Empty
                || kScraper.FetchKomplettPrice(rawHtml) != string.Empty
                || kScraper.FetchKomplettStockStatus(rawHtml) != string.Empty)
            {
                try
                {
                    _addedProduct.pPrice = CheckPrice.CheckPriceFormat(kScraper.FetchKomplettPrice(rawHtml));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please provide a price containing only numbers and a single dot.",
                        "Price contains letters.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                catch (OverflowException)
                {
                    MessageBox.Show("The price is too high.", "Price too high.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                catch (PriceTooLongException)
                {
                    MessageBox.Show("The price is too long.", "Price too long.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                catch (PriceContainsCommaException)
                {
                    MessageBox.Show("Please use a dot instead of comma in the product price.", "Price contains comma.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _addedProduct.pPrice = 0.00;
                }
                _addedProduct.pName = kScraper.FetchKomplettTitle(rawHtml);
                _addedProduct.pNumber = kScraper.FetchKomplettPNumber(rawHtml);
                _addedProduct.PStockStatus = kScraper.FetchKomplettStockStatus(rawHtml);
                _addedProduct.pType =
                    (Product.ProductType)
                        Enum.Parse(typeof(Product.ProductType),
                            kScraper.KomplettPTypeMapping(kScraper.FetchKomplettPType(AddProductUrlTextBox.Text)));
            }
            else MessageBox.Show("No product found at the given URL.");
        }
        
        private void AddProduct_OnClosing(object sender, CancelEventArgs e)
        {

        }
    }
}
