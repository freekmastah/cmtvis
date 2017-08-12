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

namespace cmtviswpf
{
    /// <summary>
    /// Interaktionslogik für About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://wwwdb.inf.tu-dresden.de/cmtvis");
        }

        private void Label_TouchDown_1(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Process.Start("http://wwwdb.inf.tu-dresden.de/cmtvis");
        }
    }
}
