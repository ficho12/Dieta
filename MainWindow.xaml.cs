﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Dieta
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        Tablas tb;
        ObservableCollection<Fecha> listaDate;
        String directorioTmp, archivoTmp, archivoActual;

        public MainWindow()
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

            if(File.Exists(archivoActual))
            {
                CargarArchivoTmp(archivoActual);
            }
            else
            {
                listaDate = new ObservableCollection<Fecha>();
                listaFecha.ItemsSource = listaDate;
            }

        }

        private void VerTablas_Click(object sender, RoutedEventArgs e)
        {
            tb = new Tablas();
            tb.Owner = this;
            tb.Show();
            tb.pasarTabla += pasarTabla;

            
            /*
            if (tablas.DialogResult == true)
            {
                Title = tablas.cadena;
            }
            */
        }

        private void pasarTabla(object sender, TablaEventArgs e)
        {
            listaDate = e.listaDate;
            listaFecha.ItemsSource = listaDate;
        }

        private void CargarTablas_Click(object sender, RoutedEventArgs e)
        {
            /*FileDialog dialog = new FileDialog();
            dialog.Filter = "cal files (*.cal)|*.cal|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            dialog.ShowDialog();

            archivoActual = dialog.FileName.ToString();
            GuardarArchivo(archivoActual);
            */
        }

        private void GuardarTablas_Click(object sender, RoutedEventArgs e)
        {
            GuardarArchivo(archivoActual);
        }

        private void GuardarTablasComo_Click(object sender, RoutedEventArgs e)
        {
            //var folderBrowserDialog1 = new FolderBrowserDialog();

            // Show the FolderBrowserDialog.
            //DialogResult result = ;
            //if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            //{
            //string folderName = folderBrowserDialog1.SelectedPath;
            //StreamWriter writer;

            var dialog = new SaveFileDialog();
            dialog.Filter = "cal files (*.cal)|*.cal|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            dialog.ShowDialog();

            archivoActual = dialog.FileName.ToString();
            GuardarArchivo(archivoActual);

           // writer = new StreamWriter(dialog.FileName.ToString());

            
           // writer.WriteLine("prueba");
           // writer.Close();

        }

        private void EliminarTablas_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void VerTodo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public void GuardarListaTemp(string s)
        {
            Debug.WriteLine(s);

            return;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listaFecha_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listaFecha_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GuardarArchivo(String s)
        {
            BinarySerialization.WriteToBinaryFile(s, new List<Fecha>(listaDate));
        }

        private void CargarArchivoTmp(string s)
        {
            listaDate = new ObservableCollection<Fecha>(BinarySerialization.ReadFromBinaryFile<List<Fecha>>(s));
            listaFecha.ItemsSource = listaDate;
        }
    }
  }

