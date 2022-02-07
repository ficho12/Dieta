using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        String directorioTmp, archivoTmp, archivoActual;

        public Tablas()
        {
            InitializeComponent();


            //listaDate = new ObservableCollection<Fecha>();
            //listaFecha.ItemsSource = listaDate;

            directorioTmp = Directory.GetCurrentDirectory() + "\\saves";

            if (!Directory.Exists(directorioTmp))
            {
                Directory.CreateDirectory(directorioTmp);
            }

            archivoTmp = directorioTmp + "\\tmpData.bin";
            archivoActual = archivoTmp;

            if (File.Exists(archivoActual))
            {
                CargarArchivoTmp(archivoActual);
            }
            else
            {
                listaDate = new ObservableCollection<Fecha>();
                listaFecha.ItemsSource = listaDate;
            }
        }
        private void CrearTabla_Click(object sender, RoutedEventArgs e)
        {
            Fecha fecha = new Fecha((DateTime)dp.SelectedDate);
            listaDate.Add(fecha);
            listaDay = new ObservableCollection<Comida>(fecha.Comidas);
            listaDia.ItemsSource = listaDay;
            GuardarArchivoTmp();
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


            Comida comida = new Comida(COMIDA.Text,Convert.ToDouble(CALORIAS.Text));

            

            if (listaDate.Remove(fecha))
            {
                fecha.Comidas.Add(comida);
                listaDay.Add(comida);
                listaDate.Add(fecha);
                //listaDia.ItemsSource = fecha.Comidas;

                if (pasarTabla != null)
                {
                    pasarTabla(this, new TablaEventArgs(listaDate, listaDate.IndexOf(fecha)));
                }

            }
            else
            {
                //Error
            }

            

            GuardarArchivoTmp();
        }

        private void EliminarFecha_Click(object sender, RoutedEventArgs e)
        {
            listaDate.Remove((Fecha)(listaFecha.SelectedItem));
            GuardarArchivoTmp();
            listaFecha.ItemsSource = listaDate;
        }

        private void EliminarComida_Click(object sender, RoutedEventArgs e)
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);
            if(listaDate.Contains(fecha))
            {
                listaDate.Remove(fecha);
                fecha.Comidas.Remove((Comida)(listaDia.SelectedItem));
                listaDate.Add(fecha);
                listaDay.Remove((Comida)(listaDia.SelectedItem));
                //listaDia.ItemsSource = fecha.Comidas;
                GuardarArchivoTmp();
            }
        }


        private void GuardarArchivoTmp()
        {
            BinarySerialization.WriteToBinaryFile(archivoTmp, new List<Fecha>(listaDate));
        }

        private void CargarArchivoTmp(string s)
        {
            listaDate = new ObservableCollection<Fecha>(BinarySerialization.ReadFromBinaryFile<List<Fecha>>(s));
            listaFecha.ItemsSource = listaDate;
        }

    }
}
