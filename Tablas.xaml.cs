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

    [Serializable]
    public class CambiarGraficaEventArgs : EventArgs
    {
        public bool cambioGrafica { get; set; }         //True = grafica Dias, False = Gráfica Comida

        public CambiarGraficaEventArgs(bool c)
        {
            cambioGrafica = c;
        }
    }

    public delegate void CambiarGraficaEventHandler(Object sender, CambiarGraficaEventArgs e);


    public partial class Tablas : Window
    {
        
        ObservableCollection<Fecha> listaDate;
        ObservableCollection<Comida> listaDay;
        public event TablaEventHandler pasarTabla;
        public event CambiarGraficaEventHandler cambiarGrafica;

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

            if(cambiarGrafica != null)
            {
                cambiarGrafica(this, new CambiarGraficaEventArgs(true));
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
                    pasarTabla(this, new TablaEventArgs(listaDate, listaDate.IndexOf(fecha)));      //Problema Pos
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
                fecha.totalCalorias += comida.calorias;
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

            listaFecha.SelectedItem = fecha;                                       // Arreglo de lo de arriba, hacer con el boton de añadir también

            GuardarArchivoTmp();
        }

        private void EliminarFecha_Click(object sender, RoutedEventArgs e)
        {
            listaDate.Remove((Fecha)(listaFecha.SelectedItem));
            GuardarArchivoTmp();
            listaFecha.ItemsSource = listaDate;
        }

        private void EliminarComida_Click(object sender, RoutedEventArgs e)         //No deja eliminar Dos a la vez (Usar variable para last fecha selection?)
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
                //listaDia.ItemsSource = fecha.Comidas;
                GuardarArchivoTmp();
            }

            listaFecha.SelectedItem = fecha;                                       // Arreglo de lo de arriba, hacer con el boton de añadir también
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

        private void esNumero(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
