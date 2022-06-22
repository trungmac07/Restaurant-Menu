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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void test1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("haha");
        }
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Button a = new Button();
            a.Content = "123124534";
            a.Height = 50;
            a.Click += test1;
            panelTest.Children.Add(a);

        }
    }
}
