using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Dieta
{
    /// <summary>
    /// Lógica de interacción para Page1.xaml
    /// </summary>
    /// 
    [Serializable]
    public class TablaEventArgs : EventArgs
    {
        public ObservableCollection<Fecha> listaDate {get; set; }
        public int pos { get; set; }

        public TablaEventArgs(ObservableCollection<Fecha> e, int p)
        {
            listaDate = new ObservableCollection<Fecha>(e);
            pos = p;
        }
    }

    public delegate void TablaEventHandler(Object sender, TablaEventArgs e );

    public partial class Tablas : Window
    {
        ObservableCollection<Fecha> listaDate;
        ObservableCollection<Comida> listaDay;
        public event TablaEventHandler pasarTabla;
        string archivoTmp;

        public Tablas(ObservableCollection<Fecha> l)
        {
            InitializeComponent();

            string directorioTmp = Directory.GetCurrentDirectory() + "\\saves";

            if (!Directory.Exists(directorioTmp))
            {
                Directory.CreateDirectory(directorioTmp);
            }

            archivoTmp = directorioTmp + "\\tmpData.bin";
           
            listaDate = l;
            listaFecha.ItemsSource = listaDate;
            blackout();
        }

        private void blackout()
        {
            foreach (Fecha fecha in listaDate)
            {
                dp.BlackoutDates.Add(new CalendarDateRange (fecha.fecha));
            }
        }

        private void AnadirFecha_Click(object sender, RoutedEventArgs e)
        {
            if (dp.SelectedDate.HasValue)
            {
                Fecha fecha = new Fecha((DateTime)dp.SelectedDate);
                if (listaDate.Any(f => f.fecha == fecha.fecha)) //Ya existe la fecha, no se añade
                {
                    MessageBoxButton boton = MessageBoxButton.OK;
                    MessageBox.Show("Seleccione una fecha que no se encuentre en la lista", "Error al añadir fecha", boton);
                }
                else
                {
                    listaDate.Add(fecha);
                    listaDate = new ObservableCollection<Fecha>(listaDate.OrderBy(i => i.fecha));
                    listaFecha.ItemsSource = listaDate;
                    listaDay = new ObservableCollection<Comida>(fecha.Comidas);
                    listaDia.ItemsSource = listaDay;
                    GuardarArchivoTmp();
                }
            }
            else
            {
                MessageBoxButton boton = MessageBoxButton.OK;
                MessageBox.Show("Seleccione un día del calendario para añadirlo a la lista", "Error al añadir día", boton);
            }
        }

        private void listaFecha_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);

            if (listaFecha.SelectedItem != null)
            {
                listaDay = new ObservableCollection<Comida>(fecha.Comidas);
                listaDia.ItemsSource = listaDay;

                if (pasarTabla != null)
                {
                    pasarTabla(this, new TablaEventArgs(listaDate, listaDate.IndexOf(fecha)));
                }
            }
        }

        private void AnadirComida_Click(object sender, RoutedEventArgs e)
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);
            
            if(COMIDA.Text.Length>1 && CALORIAS.Text.Length>1)
            {
                Comida comida = new Comida(COMIDA.Text, Convert.ToDouble(CALORIAS.Text));

                if (listaDate.Remove(fecha))
                {
                    fecha.Comidas.Add(comida);
                    fecha.totalCalorias += comida.calorias;
                    listaDay.Add(comida);
                    listaDate.Add(fecha);

                    if (pasarTabla != null)
                    {
                        pasarTabla(this, new TablaEventArgs(listaDate, listaDate.IndexOf(fecha)));
                    }
                }

                listaFecha.SelectedItem = fecha;                                     
                GuardarArchivoTmp();
            }
            else
            {
                MessageBoxButton boton = MessageBoxButton.OK;
                MessageBox.Show("Escriba el nombre de la comida y sus calorías en los recuadros\npara poder añadir una comida a la lista", "Error al añadir comida", boton);
            }
           
        }

        private void EliminarFecha_Click(object sender, RoutedEventArgs e)
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);
            listaDate.Remove((Fecha)(listaFecha.SelectedItem));
            GuardarArchivoTmp();
            listaFecha.ItemsSource = listaDate;
        }

        private void EliminarComida_Click(object sender, RoutedEventArgs e)         
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);
            Comida comida = (Comida)(listaDia.SelectedItem);
            if (listaDate.Contains(fecha))
            {
                listaDate.Remove(fecha);
                fecha.totalCalorias -= comida.calorias;
                fecha.Comidas.Remove((Comida)(listaDia.SelectedItem));
                listaDate.Add(fecha);
                listaDay.Remove((Comida)(listaDia.SelectedItem));
                GuardarArchivoTmp();
            }

            listaFecha.SelectedItem = fecha;                                       
        }

        private void GuardarArchivoTmp()
        {
            BinarySerialization.WriteToBinaryFile(archivoTmp, new List<Fecha>(listaDate));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pasarTabla(this, new TablaEventArgs(listaDate, 0)); //Muestra el principio de la lista
        }

        private void esNumero(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void COMIDA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (COMIDA.Text.Length >= 1)
                COMIDA_HINT.Visibility = Visibility.Hidden;
            else
                COMIDA_HINT.Visibility = Visibility.Visible;
        }

        private void CALORIAS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CALORIAS.Text.Length >= 1)
                CALORIAS_HINT.Visibility = Visibility.Hidden;
            else
                CALORIAS_HINT.Visibility = Visibility.Visible;
        }
    }
}
