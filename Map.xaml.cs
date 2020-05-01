using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace GameMap
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        byte rows, columns;
        byte[,] matriz;
        byte[] startFlag, endFlag;
        byte[] position = new byte[2];
        byte[] validNumbers = new byte[] { 1, 4, 5 };
        byte[] directions = new byte[4];
        bool firstStart = true;
        Ellipse bolita;

        public Map()
        {
            InitializeComponent();
        }

        public Map(string[] file)
        {
            InitializeComponent();

            columns = byte.Parse(file[0]);
            rows = byte.Parse(file[1]);

            matriz = new byte[columns, rows];

            //Grid definitions
            for (byte c = 0; c < columns; c++)
                grdMap.ColumnDefinitions.Add(new ColumnDefinition());

            for (byte r = 0; r < rows; r++)
                grdMap.RowDefinitions.Add(new RowDefinition());

            //Window size
            if (columns > rows)
            {
                grdMap.Width = 600;
                grdMap.Height = 600 * grdMap.RowDefinitions.Count / grdMap.ColumnDefinitions.Count;
            }
            else
            {
                grdMap.Width = 600 * grdMap.ColumnDefinitions.Count / grdMap.RowDefinitions.Count;
                grdMap.Height = 600;
            }

            //Bolita
            bolita = new Ellipse()
            {
                Height = Math.Ceiling(0.6 * (grdMap.Width / grdMap.ColumnDefinitions.Count)),
                Width = Math.Ceiling(0.6 * (grdMap.Width / grdMap.ColumnDefinitions.Count)),
                Fill = Brushes.Black,
            };

            //Logic matrix
            matriz = new byte[columns, rows];
            byte k = 0;
            for (byte i = 0; i < matriz.GetLength(0); i++)
            {
                for (byte j = 0; j < matriz.GetLength(1); j++)
                {
                    if (file[2][k] == '4')
                        startFlag = new byte[] { i, j };
                    else if (file[2][k] == '5')
                        endFlag = new byte[] { i, j };

                    matriz[i, j] = byte.Parse(file[2][k++].ToString());
                }
            }

            //Labels
            for (byte c = 0; c < columns; c++)
            {
                for (byte r = 0; r < rows; r++)
                {
                    var lbl = new Label()
                    {
                        BorderThickness = new Thickness(1),
                    };

                    switch(matriz[c, r])
                    {
                        case 1:
                            lbl.Background = new SolidColorBrush(Color.FromRgb(238, 90, 36));
                            break;

                        case 2:
                            lbl.Background = new SolidColorBrush(Color.FromRgb(6, 82, 221));
                            break;

                        case 3:
                            lbl.Background = new SolidColorBrush(Color.FromRgb(163, 203, 56));
                            break;

                        case 4:
                            lbl.Background = new SolidColorBrush(Color.FromRgb(181, 52, 113));
                            break;

                        case 5:
                            lbl.Background = new SolidColorBrush(Color.FromRgb(234, 32, 39));
                            break;
                    }

                    lbl.MouseEnter += Label_MouseEnter;
                    lbl.MouseLeave += Label_MouseLeave;
                    lbl.MouseDown += Label_MouseDown;

                    Grid.SetColumn(lbl, c);
                    Grid.SetRow(lbl, r);

                    grdMap.Children.Add(lbl);
                }
            }
        }

        public Map(byte columns, byte rows)
        {
            InitializeComponent();

            this.columns = columns;
            this.rows = rows;

            //Window size
            for (byte r = 0; r < rows; r++)
                grdMap.RowDefinitions.Add(new RowDefinition());

            for (byte c = 0; c < columns; c++)
                grdMap.ColumnDefinitions.Add(new ColumnDefinition());

            if (columns > rows)
            {
                grdMap.Width = 600;
                grdMap.Height = 600 * grdMap.RowDefinitions.Count / grdMap.ColumnDefinitions.Count;
            }
            else
            {
                grdMap.Width = 600 * grdMap.ColumnDefinitions.Count / grdMap.RowDefinitions.Count;
                grdMap.Height = 600;
            }

            //Bolita
            bolita = new Ellipse()
            {
                Height = Math.Ceiling(0.6 * (grdMap.Width / grdMap.ColumnDefinitions.Count)),
                Width = Math.Ceiling(0.6 * (grdMap.Width / grdMap.ColumnDefinitions.Count)),
                Fill = Brushes.Black
            };

            //Logic matrix
            matriz = new byte[columns, rows];
            for (int i = 0; i < matriz.GetLength(0); i++)
                for (int j = 0; j < matriz.GetLength(1); j++)
                    matriz[i, j] = 0;

            //Labels
            for (byte r = 0; r < rows; r++)
            {
                for (byte c = 0; c < columns; c++)
                {
                    var lbl = new Label()
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black
                    };

                    lbl.MouseEnter += Label_MouseEnter;
                    lbl.MouseLeave += Label_MouseLeave;
                    lbl.MouseDown += Label_MouseDown;

                    Grid.SetColumn(lbl, c);
                    Grid.SetRow(lbl, r);

                    grdMap.Children.Add(lbl);
                }
            }
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            var lbl = sender as Label;

            lbl.Background = rdbHierba.IsChecked ?? true ? new SolidColorBrush(Color.FromRgb(196, 229, 56))
                : rdbAgua.IsChecked ?? true ? new SolidColorBrush(Color.FromRgb(18, 137, 167))
                : rdbTierra.IsChecked ?? true ? new SolidColorBrush(Color.FromRgb(230, 126, 34))
                : rdbStart.IsChecked ?? true ? new SolidColorBrush(Color.FromRgb(131, 52, 113))
                : new SolidColorBrush(Color.FromRgb(237, 76, 103));
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            var lbl = sender as Label;
            byte c = (byte)Grid.GetColumn(lbl);
            byte r = (byte)Grid.GetRow(lbl);


            switch(matriz[c, r])
            {
                case 0:
                    lbl.Background = Brushes.Transparent;
                    break;
                case 1:
                    lbl.Background = new SolidColorBrush(Color.FromRgb(238, 90, 36));
                    break;
                case 2:
                    lbl.Background = new SolidColorBrush(Color.FromRgb(6, 82, 221));
                    break;
                case 3:
                    lbl.Background = new SolidColorBrush(Color.FromRgb(163, 203, 56));
                    break;
                case 4:
                    lbl.Background = new SolidColorBrush(Color.FromRgb(181, 52, 113));
                    break;
                case 5:
                    lbl.Background = new SolidColorBrush(Color.FromRgb(234, 32, 39));
                    break;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if(startFlag != null && endFlag != null)
            {
                //Rellenar
                for(byte c = 0; c < matriz.GetLength(0); c++)
                {
                    for (byte r = 0; r < matriz.GetLength(1); r++)
                    {
                        if(matriz[c, r] == 0)
                        {
                            matriz[c, r] = 3;
                            (grdMap.Children.Cast<UIElement>().First(a => Grid.GetColumn(a) == c && Grid.GetRow(a) == r) as Label).Background = Brushes.DarkGreen;
                        }
                    }
                }

                //Orden de direcciones
                directions[0] = byte.Parse(tbxPrioridad.Text[0].ToString());
                directions[1] = byte.Parse(tbxPrioridad.Text[1].ToString());
                directions[2] = byte.Parse(tbxPrioridad.Text[2].ToString());
                directions[3] = byte.Parse(tbxPrioridad.Text[3].ToString());

                //Bolita
                if (firstStart)
                {
                    grdMap.Children.Add(bolita);
                    firstStart = false;
                }

                Grid.SetColumn(bolita, startFlag[0]);
                Grid.SetRow(bolita, startFlag[1]);

                startFlag.CopyTo(position, 0);

                while (position[0] != endFlag[0] || position[1] != endFlag[1])
                {
                    Thread.Sleep(500);
                    
                    if (IsDirectionable(directions[0]))
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new ThreadStart(delegate
                        {
                            MoveBolita(bolita, directions[0]);
                        }));
                    }
                    else
                    {
                        if (IsDirectionable(directions[1]))
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new ThreadStart(delegate
                            {
                                MoveBolita(bolita, directions[1]);
                            }));
                        }
                        else
                        {
                            if (IsDirectionable(directions[2]))
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new ThreadStart(delegate
                                {
                                    MoveBolita(bolita, directions[2]);
                                }));
                            }
                            else
                            {
                                if (IsDirectionable(directions[3]))
                                {
                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new ThreadStart(delegate
                                    {
                                        MoveBolita(bolita, directions[3]);
                                    }));
                                }
                                else
                                {
                                    MessageBox.Show("Estás encerrado ggg");
                                    break;
                                }
                            }
                        }
                    }
                }

                if (position[0] == endFlag[0] && position[1] == endFlag[1])
                    MessageBox.Show("Crack. Llegaste");
                else
                    MessageBox.Show("Pos no llegaste");
            }
            else
            {
                MessageBox.Show("Seleccione inicio y final, pofabo");
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            byte c = (byte)Grid.GetColumn(sender as UIElement);
            byte r = (byte)Grid.GetRow(sender as UIElement);

            if (rdbTierra.IsChecked ?? true)
            {
                (sender as Label).Background = new SolidColorBrush(Color.FromRgb(238, 90, 36));
                matriz[c, r] = 1;
            }
            else if (rdbAgua.IsChecked ?? true)
            {
                (sender as Label).Background = new SolidColorBrush(Color.FromRgb(6, 82, 221));
                matriz[c, r] = 2;
            }
            else if(rdbHierba.IsChecked ?? true)
            {
                (sender as Label).Background = new SolidColorBrush(Color.FromRgb(163, 203, 56));
                matriz[c, r] = 3;
            }
            else if (rdbStart.IsChecked ?? true)
            {
                if(startFlag != null)
                {
                    matriz[startFlag[0], startFlag[1]] = 0;
                    (grdMap.Children.Cast<UIElement>().First(a => Grid.GetColumn(a) == startFlag[0] && Grid.GetRow(a) == startFlag[1]) as Label).Background = Brushes.Transparent;
                    startFlag[0] = c;
                    startFlag[1] = r;
                }
                else
                {
                    startFlag = new byte[] { c, r };
                }

                (sender as Label).Background = new SolidColorBrush(Color.FromRgb(181, 52, 113));
                matriz[c, r] = 4;
            }
            else
            {
                if (endFlag != null)
                {
                    matriz[endFlag[0], endFlag[1]] = 0;
                    (grdMap.Children.Cast<UIElement>().First(a => Grid.GetColumn(a) == endFlag[0] && Grid.GetRow(a) == endFlag[1]) as Label).Background = Brushes.Transparent;
                    endFlag[0] = c;
                    endFlag[1] = r;
                }
                else
                {
                    endFlag = new byte[] { c, r };
                }

                (sender as Label).Background = new SolidColorBrush(Color.FromRgb(234, 32, 39));
                matriz[c, r] = 5;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (startFlag != null && endFlag != null)
            {
                //Rellenar
                for (byte c = 0; c < matriz.GetLength(0); c++)
                    for (byte r = 0; r < matriz.GetLength(1); r++)
                        if (matriz[c, r] == 0)
                        {
                            matriz[c, r] = 3;
                            (grdMap.Children.Cast<UIElement>().First(a => Grid.GetColumn(a) == c && Grid.GetRow(a) == r) as Label).Background = Brushes.DarkGreen;
                        }

                string map = string.Empty;
                map += $"{matriz.GetLength(0)}\r\n{matriz.GetLength(1)}\r\n";
                foreach (byte c in matriz)
                    map += c;

                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "Game Map|*.gmp",
                };

                if (sfd.ShowDialog() ?? false)
                    File.WriteAllText(sfd.FileName, map);
            }
            else
            {
                MessageBox.Show("Asegúrese de marcar un inicio y final del trayecto", "No se puede guardar el mapa", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            new OpenMap().Show();
            this.Close();
        }

        private bool IsDirectionable(byte direction)
        {
            switch (direction)
            {
                //Derecha
                case 0:
                    return position[0] + 1 < this.columns && validNumbers.Contains(matriz[position[0] + 1, position[1]]);

                //Abajo
                case 1:
                    return position[1] + 1 < this.rows && validNumbers.Contains(matriz[position[0], position[1] + 1]);

                //Izquierda
                case 2:
                    return position[0] - 1 < this.columns && validNumbers.Contains(matriz[position[0] - 1, position[1]]);

                //Arriba
                case 3:
                    return position[1] - 1 < this.rows && validNumbers.Contains(matriz[position[0], position[1] - 1]);
            }

            return true;
        }

        private void MoveBolita(UIElement elem, byte direction)
        {
            switch(direction)
            {
                //Derecha
                case 0:
                    Grid.SetColumn(elem, ++position[0]);
                    break;

                    //Abajo
                case 1:
                    Grid.SetRow(elem, ++position[1]);
                    break;

                    //Izquierda
                case 2:
                    Grid.SetColumn(elem, --position[0]);
                    break;

                    //Arriba
                case 3:
                    Grid.SetRow(elem, --position[1]);
                    break;
            }
        }
    }
}