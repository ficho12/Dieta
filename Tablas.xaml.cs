﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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

    public class TablaEventArgs : EventArgs
    {
        public ObservableCollection<Fecha> listaDate {get; set; }

        public TablaEventArgs(ObservableCollection<Fecha> e)
        {
            listaDate = new ObservableCollection<Fecha>(e);
        }
        

    }

    public delegate void TablaEventHandler(Object sender, TablaEventArgs e );


    public partial class Tablas : Window
    {
        
        ObservableCollection<Fecha> listaDate;
        public event TablaEventHandler pasarTabla;
        public Tablas()
        {
            InitializeComponent();
            listaDate = new ObservableCollection<Fecha>();
            listaFecha.ItemsSource = listaDate;
        }
        private void CrearTabla_Click(object sender, RoutedEventArgs e)
        {
            Fecha fecha = new Fecha((DateTime)dp.SelectedDate);
            listaDate.Add(fecha);
            listaDia.ItemsSource = fecha.Comidas;
        }

        private void listaFecha_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);

            if (listaFecha.SelectedItem != null)
            {
                listaDia.ItemsSource = fecha.Comidas;
            }
        }

        private void AnadirComida_Click(object sender, RoutedEventArgs e)
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);
            //Fecha fecha2 = fecha;
            Debug.WriteLine("Comida:");
            Debug.WriteLine(COMIDA.Text);
            Debug.WriteLine(CALORIAS.Text);
            Debug.WriteLine(Convert.ToDouble(CALORIAS.Text));


            Comida comida = new Comida(COMIDA.Text,Convert.ToDouble(CALORIAS.Text));

            if (pasarTabla != null)
            {
                pasarTabla(this, new TablaEventArgs(listaDate));
            }

            if (listaDate.Remove(fecha))
            {
                fecha.Comidas.Add(comida);
                listaDate.Add(fecha);
                listaDia.ItemsSource = fecha.Comidas;

            }
            else
            {
                //Error
            }
            

        }

        private void EliminarFecha_Click(object sender, RoutedEventArgs e)
        {
            listaDate.Remove((Fecha)(listaFecha.SelectedItem));
        }

        private void EliminarComida_Click(object sender, RoutedEventArgs e)
        {
            Fecha fecha = (Fecha)(listaFecha.SelectedItem);
            listaDate.Remove(fecha);
            fecha.Comidas.Remove((Comida)(listaDia.SelectedItem));
            listaDate.Add(fecha);
        }

        
    }
}
