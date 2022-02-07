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
        int pos;

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

        private void pasarTabla(object sender, TablaEventArgs e) //Meter indice al pasar la fecha hacer porcentajes y modificar el tamaño de las lineas
        {
            double[] calorias;
            calorias = new double[7];
            double caloriasMax, division, resto;
            pos = e.pos;
            listaDate = e.listaDate;
            List<Fecha> fecha = new List<Fecha>(listaDate.ToList());

            int numComidas = fecha[pos].Comidas.Count<Comida>();
            int numFechas = fecha.Count() - pos;

            Debug.WriteLine("Pos: " + pos.ToString());
            Debug.WriteLine("NumComidas: " + numComidas.ToString());
            Debug.WriteLine("NumFechas: " + numFechas.ToString());
            Debug.WriteLine("fechaCount: " + fecha.Count());

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
                    nombreComida.Content = fecha[pos].Comidas[i].comida;
                    Debug.WriteLine("i: " + i);

                    //line.Content = fecha[i].Comidas[i].comida;
                    calorias[i] = fecha[pos].Comidas[i].calorias;
                }

                var d = string.Format("Dia{0}", i + 1);
                var dia = (Label)this.FindName(d);

                var lD = string.Format("lineDia{0}", i + 1);
                var lineDia = (Line)this.FindName(lD);

                if (i >= numFechas)
                {
                    dia.Visibility = Visibility.Hidden;
                    lineDia.Visibility = Visibility.Hidden;
                }
                else
                {
                    //NombreComida1.Content = fecha[0].Comidas[0].comida;
                    dia.Content = fecha[pos + i].fecha.ToString();
                    //line.Content = fecha[i].Comidas[i].comida;
                    //calorias[i] = fecha[pos].Comidas[i].calorias;
                }
            }

            caloriasMax = calorias.Max();
            division = caloriasMax / 7;
            division = Math.Truncate(division);
            resto = caloriasMax;

            for(int i=0; i <8; i++)
            {
                if (i < numComidas)
                {
                    var l = string.Format("line{0}", i + 1);
                    var line = (Line)this.FindName(l);

                    var nC = string.Format("NombreComida{0}", i + 1);
                    var nombreComida = (Label)this.FindName(nC);

                    nombreComida.Visibility = Visibility.Visible;
                    line.Visibility = Visibility.Visible;

                    line.Width = calorias[i]/caloriasMax * 14 + 9;  //9 = 0% 23 = 100% [14-23]

                    Debug.WriteLine(i + " lineWidth: " + line.Width.ToString());

                }

                var numC = string.Format("NumCal{0}", 8 - i);
                var numCal = (Label)this.FindName(numC);

                if (i == 7)
                    resto = 0;

                numCal.Content = resto;
                resto -= division;

            }
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
            var canvas = (Canvas)this.FindName("CanvasDias");
            canvas.Visibility = Visibility.Visible;
        }
        public void GuardarListaTemp(string s)
        {
            Debug.WriteLine(s);

            return;
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

