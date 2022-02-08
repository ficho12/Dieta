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
using System.Windows.Markup;

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
        bool cambioGrafica;

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

            if (File.Exists(archivoActual))
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
            tb.cambiarGrafica += cambiarGrafica;


            /*
            if (tablas.DialogResult == true)
            {
                Title = tablas.cadena;
            }
            */
        }

        private void cambiarGrafica(object sender, CambiarGraficaEventArgs e)
        {
            if (e.cambioGrafica == true)
            {
                var canvas = (Canvas)this.FindName("CanvasDias");
                canvas.Visibility = Visibility.Visible;
            }
            else
            {
                var canvas = (Canvas)this.FindName("CanvasDias");
                canvas.Visibility = Visibility.Hidden;
            }
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
                var nC = string.Format("NombreComida{0}", i + 1);
                var nombreComida = (Label)this.FindName(nC);

                var l = string.Format("line{0}", i + 1);
                var line = (Line)this.FindName(l);

                if (i >= numComidas)
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
            }

            caloriasMax = calorias.Max();
            division = caloriasMax / 7;
            division = Math.Truncate(division);
            resto = caloriasMax;

            for (int i = 0; i < 8; i++)
            {
                if (i < numComidas)
                {
                    var l = string.Format("line{0}", i + 1);
                    var line = (Line)this.FindName(l);

                    var nC = string.Format("NombreComida{0}", i + 1);
                    var nombreComida = (Label)this.FindName(nC);

                    nombreComida.Visibility = Visibility.Visible;
                    line.Visibility = Visibility.Visible;

                    line.Width = calorias[i] / caloriasMax * 14 + 9;  //9 = 0% 23 = 100% [14-23]

                    Debug.WriteLine(i + " lineWidth: " + line.Width.ToString());

                }

                var numC = string.Format("NumCal{0}", 8 - i);
                var numCal = (Label)this.FindName(numC);

                if (i == 7)
                    resto = 0;

                numCal.Content = resto;
                resto -= division;

            }

            calorias = new double[7];

            for (int i = 0; i < 17; i++)
            {
                var d = string.Format("Dia{0}", i + 1);
                var dia = (Label)this.FindName(d);

                var lD = string.Format("lineDia{0}", i + 1);
                var lineDia = (Line)this.FindName(lD);

                if (i >= numFechas)
                {
                    //Debug.WriteLine("1." + i + dia.ToString());
                    dia.Visibility = Visibility.Hidden; //null reference(?)
                    //Debug.WriteLine("2." + i + dia.ToString());
                    lineDia.Visibility = Visibility.Hidden;
                }
                else
                {
                    //NombreComida1.Content = fecha[0].Comidas[0].comida;

                    calorias[i] = fecha[pos + i].totalCalorias;

                    //line.Content = fecha[i].Comidas[i].comida;
                    //calorias[i] = fecha[pos].Comidas[i].calorias;
                }
            }

            caloriasMax = calorias.Max();
            division = caloriasMax / 7;
            division = Math.Truncate(division);
            resto = caloriasMax;

            Canvas canvas = (Canvas)this.FindName("CanvasDias");
            Line[] linea;
            linea = new Line[18];

            for (int i = 0; i < 17; i++)
            {

                var d = string.Format("Dia{0}", i + 1);
                var dia = (Label)this.FindName(d);

                var lD = string.Format("lineDia{0}", i + 1);
                var lineDia = (Line)this.FindName(lD);


                if (i < numFechas)      //meter bucle con el for de arriba para crear y calcular el tamaño de las lineas, como se superponen hacer
                {                       //en orden inverso ( empezar por la última, la linea definida en xaml
                    double restoCal = fecha[pos + i].totalCalorias;

                    for (int j = fecha[pos+i].Comidas.Count(); j > 0; j--)          //Antes de empezar borrar lineas anteriores
                    {
                        if (j== fecha[pos+i].Comidas.Count())
                        {
                            Debug.WriteLine("pos " + pos + ", i " + i + ", comidasCount " + fecha[pos].Comidas.Count());
                            lineDia.Visibility = Visibility.Visible;
                            lineDia.Stroke = Brushes.Red; //DevolverColor(j);
                            lineDia.Width = fecha[pos + i].totalCalorias / caloriasMax * 14 + 9;  //9 = 0% 23 = 100% [14-23]
                            restoCal -= fecha[pos + i].Comidas[j-1].calorias;
                            Debug.WriteLine(i + " lineWidth: " + lineDia.Width.ToString());
                        }
                        else
                        {
                            linea[j-1] = ElementClone<Line>(lineDia);           //  Parece que el error está aquí

                            linea[j-1].Name = string.Format("lineDia{0}_{0}", i + 1, j);
                            linea[j - 1].Stroke = Brushes.Violet;// DevolverColor(j);
                            linea[j-1].Visibility = Visibility.Visible;
                            linea[j-1].Width = restoCal / caloriasMax * 14 + 9;  //9 = 0% 23 = 100% [14-23] //Ojo con esto  no se si está bien recemos
                            Debug.WriteLine(i + ", " + (j-1) + " lineWidth: " + linea[j-1].Width.ToString());
                            canvas.Children.Add(linea[j - 1]);
                            restoCal -= fecha[pos + i].Comidas[j].calorias;
                        }
                    }

                    dia.Content = fecha[pos + i].fecha.Day + "/" + fecha[pos + i].fecha.Month;
                    dia.Visibility = Visibility.Visible;
                }

                if (i < 8)
                {
                    var numC = string.Format("NumCalDia{0}", 8 - i);
                    var numCal = (Label)this.FindName(numC);

                    if (i == 7)
                        resto = 0;

                    numCal.Content = resto;
                    resto -= division;
                }
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

            if ((bool)dialog.ShowDialog())
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

        private void BotonIzq_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BotonDch_Click(object sender, RoutedEventArgs e)
        {

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

        private SolidColorBrush DevolverColor(int s)
        {
            if (s >= 9)
                s -= 8;

            switch(s)
            {
                case 1:     return Brushes.Red;
                case 2:     return Brushes.Blue;
                case 3:     return Brushes.Pink;
                case 4:     return Brushes.Gray;
                case 5:     return Brushes.Green;
                case 6:     return Brushes.Orange;
                case 7:     return Brushes.Yellow;
                case 8:     return Brushes.Purple;
            }

            return Brushes.Black;
        }

        /// <summary>
        /// Clones an element.
        /// </summary>
        public static T ElementClone<T>(T element)
        {
            T clone = default(T);
            MemoryStream memStream = ElementToStream(element);
            clone = ElementFromStream<T>(memStream);
            return clone;
        }

        /// <summary>
        /// Saves an element as MemoryStream.
        /// </summary>
        public static MemoryStream ElementToStream(object element)
        {
            MemoryStream memStream = new MemoryStream();
            XamlWriter.Save(element, memStream);
            return memStream;
        }

        /// <summary>
        /// Rebuilds an element from a MemoryStream.
        /// </summary>
        public static T ElementFromStream<T>(MemoryStream elementAsStream)
        {
            object reconstructedElement = null;

            if (elementAsStream.CanRead)
            {
                elementAsStream.Seek(0, SeekOrigin.Begin);
                reconstructedElement = XamlReader.Load(elementAsStream);
                elementAsStream.Close();
            }

            return (T)reconstructedElement;
        }
    }
  }

