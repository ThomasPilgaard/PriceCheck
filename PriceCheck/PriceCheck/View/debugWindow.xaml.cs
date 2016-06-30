using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for debugWindow.xaml
    /// </summary>
    public partial class debugWindow : Window
    {
        public enum ExampleEnum
        {
            Red,
            Green,
            Yellow
        }
        public debugWindow()
        {
            InitializeComponent();
            test.ItemsSource = Enum.GetValues(typeof(Product.ProductType)).Cast<Product.ProductType>();
        }

    }
}
