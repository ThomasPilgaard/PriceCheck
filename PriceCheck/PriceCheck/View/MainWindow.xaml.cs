using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using PriceCheck.Model;

namespace PriceCheck.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Product> products = new List<Product>();
        public MainWindow()
        {
            InitializeComponent();
        }

        #region DataGrid

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO Vise lagerstatus på en god måde.
            //TODO evt have en checkbox efter pris, hvor når markeres indgår varen i en total pris.
            Product asdf1 = new Product("https://www.proshop.dk/Stroemforsyning/CX650M-650W-PSU/2537705");
            asdf1.pName = "Corsair CX650M - 650W PSU";
            asdf1.pNumber = "2537705";
            asdf1.pPrice = 591.00;
            asdf1.PriceHist = new List<KeyValuePair<DateTime, double>>();
            asdf1.pType = Product.ProductType.PSU;

            asdf1.PriceHist.Add(new KeyValuePair<DateTime, double>(DateTime.Today, 10.00));
            asdf1.PriceHist.Add(new KeyValuePair<DateTime, double>(DateTime.Today, 1337.00));

            Product asdf2 = new Product("https://www.komplett.dk/product/836033/hardware/blaeserekoelerevandkoeling/cpu-vandkoeling/fractal-design-kelvin-s36-cpu-koeler");
            asdf2.pName = "Fractal Design Kelvin S36 CPU Køler";
            asdf2.pNumber = "836033";
            asdf2.pPrice = 1129.00;
            asdf2.PriceHist = new List<KeyValuePair<DateTime, double>>();
            asdf2.pType = Product.ProductType.Cooling;

            asdf2.PriceHist.Add(new KeyValuePair<DateTime, double>(DateTime.Today, 110.00));
            asdf2.PriceHist.Add(new KeyValuePair<DateTime, double>(DateTime.Today, 42.00));

            Product asdf3 = new Product("https://www.komplett.dk/product/841415/hardware/kabinetterbarebone/midi-tower/fractal-design-define-s-black-window");
            asdf3.pName = "Fractal Design Define S Black Window";
            asdf3.pNumber = "836033";
            asdf3.pPrice = 699.00;
            asdf3.PriceHist = new List<KeyValuePair<DateTime, double>>();
            asdf3.pType = Product.ProductType.Case;

            Product asdf4 = new Product("https://www.proshop.dk/RAM/HyperX-Fury-DDR4-2133-DC-16GB/2474223");
            asdf4.pName = "test";
            asdf4.pNumber = "test";
            asdf4.pPrice = 73357;
            //asdf4.PriceHist = new List<KeyValuePair<DateTime, double>>();
            asdf4.pType = Product.ProductType.CPU;
            //asdf4.PStockStatus = "test";

            products.Add(asdf1);
            products.Add(asdf2);
            products.Add(asdf3);
            products.Add(asdf4);

            UpdateGrid();
        }

        private void productsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsGrid.SelectedItem != null)
            {
                Product choosenProduct = (Product) ProductsGrid.SelectedItem;
                priceHist priceHistory = new priceHist(choosenProduct);
                priceHistory.ShowDialog();
            }
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            try
            {
                Process.Start(link.NavigateUri.AbsoluteUri);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please provide a proper URL.");
            }
        }

        #endregion

        #region Buttons

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            var addedProduct = new Product(string.Empty) {pName = string.Empty, pNumber = string.Empty, pPrice = 0, pType = Product.ProductType.Misc, PStockStatus = string.Empty};
            AddProduct addProduct = new AddProduct(addedProduct);
            addProduct.ShowDialog();
            if (addedProduct.PUrl != string.Empty ||
                addedProduct.pName != string.Empty ||
                addedProduct.pNumber != string.Empty ||
                !addedProduct.pPrice.Equals(0) ||
                addedProduct.pType != Product.ProductType.Misc ||
                addedProduct.PStockStatus != string.Empty)
            {
                products.Add(addedProduct);
            }
            UpdateGrid();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem != null)
            {
                Product selectedProduct = (Product)ProductsGrid.SelectedItem;
                EditProduct editProduct = new EditProduct(selectedProduct);
                editProduct.ShowDialog();
                UpdateGrid();
            }
            else MessageBox.Show("Please choose a product.");
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem != null)
            {
                var selectedItem = ProductsGrid.SelectedItem.GetHashCode();
                products.RemoveAll(x => x.GetHashCode() == selectedItem);
                UpdateGrid();
            }
            else MessageBox.Show("Please choose a product.");
        }
        #endregion

        private void expander_Pushed(object sender, RoutedEventArgs e)
        {
            ProductsGrid.SelectedItem = null;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Save products here
            //Look here: http://www.dotnetperls.com/datagrid-wpf
        }

        private void UpdateGrid()
        {
            ProductsGrid.Items.Refresh();
            ProductsGrid.ItemsSource = null;
            ListCollectionView collectionView = new ListCollectionView(products);
            collectionView.GroupDescriptions?.Add(new PropertyGroupDescription("pType"));
            ProductsGrid.ItemsSource = collectionView;
            ProductsGrid.SelectedItem = null;
        }
    }
}
