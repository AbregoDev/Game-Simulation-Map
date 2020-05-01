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

namespace GameMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NewMap : Window
    {
        public NewMap()
        {
            InitializeComponent();

            tbxColumns.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Map(byte.Parse(tbxColumns.Text), byte.Parse(tbxRows.Text)).Show();
            this.Close();
        }
    }
}
