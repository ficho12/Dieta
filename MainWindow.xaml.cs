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
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Win32;

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

        public MainWindow()     // Añadir ventana que confirme que se ha cargado el último archivo temporal
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
            List<Fecha> fecha = new List<Fecha>(listaDate.ToList());


            //Label nombreComida;

            /*
            NombreComida1.Content = fecha[0].Comidas[0].comida;
            NombreComida2.Visibility = Visibility.Hidden;
            NombreComida3.Visibility = Visibility.Hidden;
            NombreComida4.Visibility = Visibility.Hidden;
            NombreComida5.Visibility = Visibility.Hidden;
            NombreComida6.Visibility = Visibility.Hidden;
            NombreComida7.Visibility = Visibility.Hidden;
            NombreComida8.Visibility = Visibility.Hidden;
            */
            int numComidas = fecha[0].Comidas.Count();

            for (int i = 0; i < 8; i++)
            {
                var nC = string.Format("NombreComida{0}", i+1);
                var nombreComida = (Label)this.FindName(nC);

                var l = string.Format("line{0}", i+1);
                var line = (Line)this.FindName(l);

                if (i>=numComidas)
                {
                    nombreComida.Visibility = Visibility.Hidden;
                    line.Visibility = Visibility.Hidden;
                }
                else
                {
                    //NombreComida1.Content = fecha[0].Comidas[0].comida;
                    nombreComida.Content = fecha[0].Comidas[i].comida;
                    //line.Content = fecha[i].Comidas[i].comida;
                }


                //label.Content = i * 10;
            }

            /*
            for (int i=0; i<8; i++)
            {
                nombreComida = 
                switch (i)
                {
                    case 0:
                        break;

                }
            }
            */


            /*
            foreach(Fecha x in fecha)
            {
                //foreach(Comida c in x.Comidas)
                //{
                //}
                NombreComida1.Content = x.Comidas[0].comida;
                //NombreComida2.Content = x.Comidas[1].comida;
                //NombreComida3.Content = x.Comidas[2].comida;
                //NombreComida4.Content = x.Comidas[3].comida;
                //NombreComida5.Content = x.Comidas[4].comida;
                //NombreComida6.Content = x.Comidas[5].comida;
                //NombreComida7.Content = x.Comidas[6].comida;
                //NombreComida8.Content = x.Comidas[7].comida;
            }
            */
        }

        private void CargarTablas_Click(object sender, RoutedEventArgs e)       // Añadir opcion cuando no se selecciona ninguna tabla
        {
            OpenFileDialog dialog = new OpenFileDialog();
            //FileDialog dialog = new FileDialog();
            dialog.Filter = "cal files (*.cal)|*.cal|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if ((bool)dialog.ShowDialog())
            {
                archivoActual = dialog.FileName.ToString();
                CargarArchivoTmp(archivoActual);
                //GuardarArchivo(archivoActual);
                MostrarCuadro("Se ha cargado el archivo correctamente.", "Carga exitosa");
            }
            else
            {
                MostrarCuadro("No se ha podido cargar el archivo con el nombre especificado.", "Error al seleccionar archivo a cargar");

            }


        }

        private void GuardarTablas_Click(object sender, RoutedEventArgs e)      // Añadir ventana que confirme
        {
            GuardarArchivo(archivoActual);
        }

        private void GuardarTablasComo_Click(object sender, RoutedEventArgs e)      // Añadir opcion cuando no se selecciona ninguna tabla
        {
            //var folderBrowserDialog1 = new FolderBrowserDialog();

            // Show the FolderBrowserDialog.
            //DialogResult result = ;
            //if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            //{
            //string folderName = folderBrowserDialog1.SelectedPath;
            //StreamWriter writer;

            SaveFileDialog dialog = new SaveFileDialog();

            //var dialog = new SaveFileDialog();
            dialog.Filter = "cal files (*.cal)|*.cal|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if((bool)dialog.ShowDialog())
            {
                archivoActual = dialog.FileName.ToString();
                GuardarArchivo(archivoActual);
                MostrarCuadro("Se ha guardado el archivo correctamente.", "Guardado exitosa");
            }
            else
            {
                MostrarCuadro("No se ha podido guardar el archivo con el nombre especificado.", "Error al seleccionar archivo a guardar");
            }

            

           // writer = new StreamWriter(dialog.FileName.ToString());

            
           // writer.WriteLine("prueba");
           // writer.Close();

        }

        private void EliminarTablas_Click(object sender, RoutedEventArgs e)     // Añadir opcion cuando no se selecciona ninguna tabla
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
        }

        private void MostrarCuadro(string msg, string titulo)
        {
            MessageBoxButton boton = MessageBoxButton.OK;
            MessageBox.Show(msg, titulo, boton);
        }
    }
  }

