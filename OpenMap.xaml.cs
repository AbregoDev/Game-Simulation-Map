using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace GameMap
{
    /// <summary>
    /// Interaction logic for OpenMap.xaml
    /// </summary>
    public partial class OpenMap : Window
    {
        public OpenMap()
        {
            InitializeComponent();
        }

        private void btnOpenMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Game Map|*.gmp",
            };

            if(ofd.ShowDialog() ?? false)
            {
                string[] file = File.ReadAllText(ofd.FileName).Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

                try
                {
                    if (ValidMap(file))
                    {
                        new Map(file).Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Mapa inválido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error. No se ha podido leer el archivo\n" + ex.Message, "Excepción", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnNewMap_Click(object sender, RoutedEventArgs e)
        {
            new NewMap().Show();
            this.Close();
        }

        private bool ValidMap(string[] file)
        {
            try
            {
                byte.Parse(file[0]);
                byte.Parse(file[1]);

                string s = file[2];
                byte count4 = 0, count5 = 0;

                foreach (char ch in s)
                {
                    if(ch != '1' && ch != '2' && ch != '3')
                    {
                        if (ch == '4')
                            count4++;
                        else if (ch == '5')
                            count5++;
                        else
                            return false;
                    }
                }

                if (count4 != 1 || count5 != 1)
                    return false;
            }
            catch(Exception e)
            {
                throw e;
            }
            
            return true;
        }
    }
}
