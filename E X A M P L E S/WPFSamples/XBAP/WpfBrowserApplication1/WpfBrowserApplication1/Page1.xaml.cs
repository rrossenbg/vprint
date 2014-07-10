using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBrowserApplication1
{
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            label2.Content = "you have clicked first button thank you for click me";
            label2.FontSize = 20;
           
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            label2.Content = "Now you have clicked the second button thanx for press me";
            label2.FontSize = 26;
        }

        
    }
}
