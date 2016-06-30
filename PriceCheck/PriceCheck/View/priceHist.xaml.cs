using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for priceHist.xaml
    /// </summary>
    public partial class priceHist : Window
    {
        readonly Product choosenProduct;
        public priceHist(Product choosenProduct)
        {
            this.choosenProduct = choosenProduct;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void PriceHistGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (choosenProduct.PriceHist != null)
            {
                PriceHistGrid.ItemsSource = choosenProduct.PriceHist;
            }
        }
    }
}
