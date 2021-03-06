﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PriceCheck.Exceptions;
using PriceCheck.Model;
using PriceCheck.Utility;

namespace PriceCheck.View
{
    /// <summary>
    /// Interaction logic for EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        private Product _choosenProduct;
        //TODO Ryd op, evt alt gui op øverst, methods nederst
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
                MessageBoxResult result = MessageBox.Show("Do you wish to save your changes?", "Closing the window", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        if (!SaveData())
                        {
                            e.Cancel = true;
                        }
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private bool SaveData()
        {
            try
            {
                _choosenProduct.pPrice = CheckPrice.CheckPriceFormat(PPriceTxtBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please provide a price containing only numbers and a single dot.",
                    "Price contains letters.",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            catch (OverflowException)
            {
                MessageBox.Show("The price is too high.", "Price too high.",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            catch (PriceTooLongException)
            {
                MessageBox.Show("The price is too long.", "Price too long.",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            catch (PriceContainsCommaException)
            {
                MessageBox.Show("Please use a dot instead of comma in the product price.", "Price contains comma.",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            _choosenProduct.pName = PNameTxtBox.Text;
            _choosenProduct.PUrl = PUrlTxtBox.Text;
            _choosenProduct.pNumber = PNumberTxtBox.Text;
            _choosenProduct.pType = (Product.ProductType) PTypeCombobox.SelectedItem;
            _choosenProduct.PStockStatus = PStockStatus.Text;
            return true;
        }

        private bool UnsavedChanges()
        {
            return PNameTxtBox.Text != _choosenProduct.pName 
                || PUrlTxtBox.Text != _choosenProduct.PUrl 
                || PNumberTxtBox.Text != _choosenProduct.pNumber 
                || (Product.ProductType) PTypeCombobox.SelectedItem != _choosenProduct.pType 
                || PPriceTxtBox.Text != _choosenProduct.pPrice.ToString("0.00") 
                || PStockStatus.Text != _choosenProduct.PStockStatus;
        }

        private void FetchDataBtn_Click(object sender, RoutedEventArgs e)
        { 
            FetchData();
        }

        public async void FetchData()
        {
            Regex urlRegex = new Regex(@"(https|http)://(www.)?.+?\.\w+");
            Regex proshopUrlRegex = new Regex(@"(https|http)://(www.)?proshop\.dk");
            Regex komplettUrlRegex = new Regex(@"^(https|http)://(www.)?komplett\.dk");

            if (!string.IsNullOrEmpty(PUrlTxtBox.Text))
            {
                if (urlRegex.IsMatch(PUrlTxtBox.Text))
                {
                    testTextBox.Text = urlRegex.Match(PUrlTxtBox.Text).ToString();
                    if (proshopUrlRegex.IsMatch(PUrlTxtBox.Text) || komplettUrlRegex.IsMatch(PUrlTxtBox.Text))
                    {
                        var dl = new DownloadData();
                        string urlFromUrlTextBox = PUrlTxtBox.Text;
                        var downloadRawHtml = Task<string>.Factory.StartNew(() => dl.FetchRawHtml(urlFromUrlTextBox));
                        TestLabel.Content = "Waiting";
                        FetchDataBtn.IsEnabled = false;

                        await downloadRawHtml;
                        //TODO Evt. have mulighed for at cancel download med en knap oven på Fetch Data.
                        FetchDataBtn.IsEnabled = true;
                        var rawHtml = downloadRawHtml.Result;
                        TestLabel.Content = "Done";
                        if (rawHtml == string.Empty)
                        {
                            MessageBox.Show("Cannot connect to \"" + urlFromUrlTextBox + "\"");
                        }
                        else if (proshopUrlRegex.IsMatch(PUrlTxtBox.Text))
                        {
                            AssignTexboxesProshop(rawHtml);
                        }
                        else if (komplettUrlRegex.IsMatch(PUrlTxtBox.Text))
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
                PNameTxtBox.Text = pScraper.FetchProshopTitle(rawHtml);
                PNumberTxtBox.Text = pScraper.FetchProshopPNumber(rawHtml);
                PPriceTxtBox.Text = pScraper.FetchProshopPrice(rawHtml);
                PStockStatus.Text = pScraper.FetchProshopStockStatus(rawHtml);
                PTypeCombobox.SelectedItem =
                    (Product.ProductType)
                        Enum.Parse(typeof(Product.ProductType),
                            pScraper.ProshopPTypeMapping(pScraper.FetchProshopPType(PUrlTxtBox.Text)));
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
                PNameTxtBox.Text = kScraper.FetchKomplettTitle(rawHtml);
                PNumberTxtBox.Text = kScraper.FetchKomplettPNumber(rawHtml);
                PPriceTxtBox.Text = kScraper.FetchKomplettPrice(rawHtml);
                PStockStatus.Text = kScraper.FetchKomplettStockStatus(rawHtml);
                PTypeCombobox.SelectedItem =
                    (Product.ProductType)
                        Enum.Parse(typeof(Product.ProductType),
                            kScraper.KomplettPTypeMapping(kScraper.FetchKomplettPType(PUrlTxtBox.Text)));
            }
            else MessageBox.Show("No product found at the given URL.");
        }
    }
}
