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
using System.Windows.Media.Animation;

namespace Dieta
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        ObservableCollection<Fecha> listaDate;
        string archivoTmp, archivoActual;
        int pos, posComidas;
        public MainWindow()
        {
            InitializeComponent();

            string directorioTmp = Directory.GetCurrentDirectory() + "\\saves";

            if (!Directory.Exists(directorioTmp))
            {
                Directory.CreateDirectory(directorioTmp);
            }

            archivoTmp = directorioTmp + "\\tmpData.bin";
            archivoActual = "0";

            if (File.Exists(archivoTmp))
            {
                CargarArchivoTmp(archivoTmp);
                MostrarCuadro("Se ha cargado el archivo de autoguardado de la última sesión", "Autoguardado");
            }
            else
            {
                listaDate = new ObservableCollection<Fecha>();
            }

            limpiarGrafico();
        }

        private void VerTablas_Click(object sender, RoutedEventArgs e)
        {
            Tablas tb = new Tablas(listaDate);
            tb.Owner = this;
            tb.Show();
            tb.pasarTabla += pasarTabla;
        }

        private void pasarTabla(object sender, TablaEventArgs e)
        {
            double[] calorias;
            calorias = new double[8];
            double caloriasMax, division, resto;
            pos = e.pos;
            posComidas = 0;
            listaDate = e.listaDate;
            List<Fecha> fecha = new List<Fecha>(listaDate.ToList());

            int numComidas = fecha[pos].Comidas.Count<Comida>();
            int numFechas = fecha.Count() - pos;

            var c = (Canvas)this.FindName("CanvasComida");
            c.Visibility = Visibility.Visible;

            var boton = (Button)this.FindName("DchComidas");
            boton.IsEnabled = true;
            boton.Visibility = Visibility.Visible;

            boton = (Button)this.FindName("IzqComidas");
            boton.IsEnabled = true;
            boton.Visibility = Visibility.Visible;

            boton = (Button)this.FindName("DchDias");
            boton.Visibility = Visibility.Hidden;
            boton.IsEnabled = false;

            boton = (Button)this.FindName("IzqDias");
            boton.Visibility = Visibility.Hidden;
            boton.IsEnabled = false;

            eliminarLineas();

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
                    nombreComida.Content = fecha[pos].Comidas[i].comida;
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

                    //Animacion:
                    Storyboard sb = new Storyboard();        //Rango [14.34,0.2]
                    DoubleAnimation da = new DoubleAnimation(line.X1, 14.36 - (calorias[i] / caloriasMax) * 14.34, new Duration(new TimeSpan(0, 0, 1)));
                    Storyboard.SetTargetProperty(da, new PropertyPath("(Line.X1)"));
                    sb.Children.Add(da);
                    line.BeginStoryboard(sb);
                    line.Visibility = Visibility.Visible;

                    Debug.WriteLine(i + " lineWidth: " + line.Width.ToString());
                }

                var numC = string.Format("NumCal{0}", 8 - i);
                var numCal = (Label)this.FindName(numC);

                if (i == 7)
                    resto = 0;

                numCal.Content = resto;
                resto -= division;
            }

            calorias = new double[18];

            for (int i = 0; i < 17; i++)
            {
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
                    calorias[i] = fecha[pos + i].totalCalorias;
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
                {                       //en orden inverso ( empezar por la última, la linea definida en xaml )
                    double restoCal = fecha[pos + i].totalCalorias;
                    if(restoCal==0)
                    {
                        lineDia.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        for (int j = fecha[pos + i].Comidas.Count(); j > 0; j--)          //Antes de empezar borrar lineas anteriores
                        {
                            if (j == fecha[pos + i].Comidas.Count())
                            {
                                Debug.WriteLine("pos " + pos + ", i " + i + ", comidasCount " + fecha[pos].Comidas.Count());
                                lineDia.Stroke = DevolverColor(j);

                                //Animacion:
                                Storyboard sb = new Storyboard();           //Rango [14.34,0.2]
                                DoubleAnimation da = new DoubleAnimation(lineDia.X1, 14.36 - (fecha[pos + i].totalCalorias / caloriasMax) * 14.34, new Duration(new TimeSpan(0, 0, 1)));
                                Storyboard.SetTargetProperty(da, new PropertyPath("(Line.X1)"));
                                sb.Children.Add(da);
                                lineDia.BeginStoryboard(sb);
                                lineDia.Visibility = Visibility.Visible;

                                restoCal -= fecha[pos + i].Comidas[j - 1].calorias;
                                Debug.WriteLine(i + " X1: " + lineDia.X1.ToString() + ", restoCal: " + restoCal);
                            }
                            else
                            {
                                linea[j - 1] = ElementClone<Line>(lineDia);
                                linea[j - 1].Uid += "aBorrar";
                                linea[j - 1].Name = string.Format("lineDia{0}_{0}", i + 1, j);
                                linea[j - 1].Stroke = DevolverColor(j);

                                //Animacion:
                                Storyboard sb = new Storyboard();                //Rango [14.34,0.2]
                                DoubleAnimation da = new DoubleAnimation(linea[j - 1].X1, 14.36 - (restoCal / caloriasMax) * 14.34, new Duration(new TimeSpan(0, 0, 1)));
                                Storyboard.SetTargetProperty(da, new PropertyPath("(Line.X1)"));
                                sb.Children.Add(da);
                                linea[j - 1].BeginStoryboard(sb);
                                linea[j - 1].Visibility = Visibility.Visible;

                                canvas.Children.Add(linea[j - 1]);
                                restoCal -= fecha[pos + i].Comidas[j - 1].calorias;
                                Debug.WriteLine(i + ", " + (j - 1) + " X1: " + linea[j - 1].X1.ToString() + ", restoCal: " + restoCal);
                            }
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
            canvas.Visibility = Visibility.Hidden;
        }

        private void moverGraficoComidas(bool direccion)    // True == Dch, False == Izq
        {
            double[] calorias;
            calorias = new double[8];
            double caloriasMax, division, resto;

            if(direccion)
                posComidas += 1;
            else if(!direccion && posComidas ==0)
            {
                MostrarCuadro("No se puede mover más a la izquierda, ya está en la posición inicial", "Error al mover Gráfico");
                return;
            }
            else // !direccion
            {
                posComidas -= 1;
            }

            List<Fecha> fecha = new List<Fecha>(listaDate.ToList());

            int numComidas = fecha[pos].Comidas.Count<Comida>() - posComidas;

            if(numComidas <= 0)
            {
                MostrarCuadro("No se puede mover más a la derecha, no hay comidas que mostrar", "Error al mover Gráfico");
                return;
            }

            eliminarLineas();

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
                    nombreComida.Content = fecha[pos].Comidas[i+posComidas].comida;
                    calorias[i] = fecha[pos].Comidas[i+posComidas].calorias;
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

                    //Animacion:
                    Storyboard sb = new Storyboard();        //Rango [14.34,0.2]
                    DoubleAnimation da = new DoubleAnimation(line.X1, 14.36 - (calorias[i] / caloriasMax) * 14.34, new Duration(new TimeSpan(0, 0, 1)));
                    Storyboard.SetTargetProperty(da, new PropertyPath("(Line.X1)"));
                    sb.Children.Add(da);
                    line.BeginStoryboard(sb);
                    line.Visibility = Visibility.Visible;

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

        private void moverGraficoDias(bool direccion)    // True == Dch, False == Izq
        { 
            double[] calorias;
            calorias = new double[18];
            double caloriasMax, division, resto;
           
            List<Fecha> fecha = new List<Fecha>(listaDate.ToList());

            if (direccion)
                pos += 1;
            else if (!direccion && pos == 0)
            {
                MostrarCuadro("No se puede mover más a la izquierda, ya está en la posición inicial", "Error al mover Gráfico");
                return;
            }
            else // !direccion
            {
                pos -= 1;
            }

            int numFechas = fecha.Count() - pos;

            if (numFechas <= 0)
            {
                MostrarCuadro("No se puede mover más a la derecha, no hay comidas que mostrar", "Error al mover Gráfico");
                return;
            }

            eliminarLineas();

            for (int i = 0; i < 17; i++)
            {
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
                    calorias[i] = fecha[pos + i].totalCalorias;
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
                {                       //en orden inverso ( empezar por la última, la linea definida en xaml )
                    double restoCal = fecha[pos + i].totalCalorias;
                    if (restoCal == 0)
                    {
                        lineDia.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        for (int j = fecha[pos + i].Comidas.Count(); j > 0; j--)          //Antes de empezar borrar lineas anteriores
                        {
                            if (j == fecha[pos + i].Comidas.Count())
                            {
                                Debug.WriteLine("pos " + pos + ", i " + i + ", comidasCount " + fecha[pos].Comidas.Count());
                                lineDia.Stroke = DevolverColor(j);

                                //Animacion:
                                Storyboard sb = new Storyboard();           //Rango [14.34,0.2]
                                DoubleAnimation da = new DoubleAnimation(lineDia.X1, 14.36 - (fecha[pos + i].totalCalorias / caloriasMax) * 14.34, new Duration(new TimeSpan(0, 0, 1)));
                                Storyboard.SetTargetProperty(da, new PropertyPath("(Line.X1)"));
                                sb.Children.Add(da);
                                lineDia.BeginStoryboard(sb);
                                lineDia.Visibility = Visibility.Visible;

                                restoCal -= fecha[pos + i].Comidas[j - 1].calorias;
                                Debug.WriteLine(i + " X1: " + lineDia.X1.ToString() + ", restoCal: " + restoCal);
                            }
                            else
                            {
                                linea[j - 1] = ElementClone<Line>(lineDia);
                                linea[j - 1].Uid += "aBorrar";
                                linea[j - 1].Name = string.Format("lineDia{0}_{0}", i + 1, j);
                                linea[j - 1].Stroke = DevolverColor(j);

                                canvas.Children.Add(linea[j - 1]);

                                //Animacion:
                                Storyboard sb = new Storyboard();
                                DoubleAnimation da = new DoubleAnimation(linea[j-1].X1, 14.36 - (restoCal / caloriasMax) * 14.34, new Duration(new TimeSpan(0, 0, 1)));
                                Storyboard.SetTargetProperty(da, new PropertyPath("(Line.X1)"));
                                sb.Children.Add(da);
                                linea[j-1].BeginStoryboard(sb);
                                linea[j-1].Visibility = Visibility.Visible;

                                restoCal -= fecha[pos + i].Comidas[j - 1].calorias;
                                Debug.WriteLine(i + ", " + (j - 1) + " X1: " + linea[j - 1].X1.ToString() + ", restoCal: " + restoCal);
                            }
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
            canvas.Visibility = Visibility.Visible;
        }
        private void NuevaTabla_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado;
            MessageBoxButton botones = MessageBoxButton.OKCancel;
            MessageBoxImage icono = MessageBoxImage.Question;
            
            resultado = MessageBox.Show("Si crea una nueva tabla perderá los datos de la tabla actual que no se hayan guardado\n" +
                "¿Desea crear una nueva tabla?", "Aviso nueva tabla", botones, icono);

            switch (resultado)
            {
                case MessageBoxResult.OK:
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.Title.Equals("Tablas"))
                        {
                            window.Close();
                            limpiarGrafico();
                        }
                    }

                    listaDate = new ObservableCollection<Fecha>();
                    archivoActual = "0";
                    break;

                case MessageBoxResult.Cancel: return;
            }
        }

        private void limpiarGrafico()
        {
            eliminarLineas();

            for(int i = 0; i<17; i++)
            {
                var d = string.Format("Dia{0}", i + 1);
                var dia = (Label)this.FindName(d);
                dia.Visibility = Visibility.Hidden;

                var lD = string.Format("lineDia{0}", i + 1);
                var lineDia = (Line)this.FindName(lD);
                lineDia.Visibility = Visibility.Hidden;

                if (i<8)
                {
                    var l = string.Format("line{0}", i + 1);
                    var line = (Line)this.FindName(l);

                    var nC = string.Format("NombreComida{0}", i + 1);
                    var nombreComida = (Label)this.FindName(nC);

                    nombreComida.Visibility = Visibility.Hidden;
                    line.Visibility = Visibility.Hidden;
                }
            }
        }

        private void CargarTablas_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado;
            MessageBoxButton botones = MessageBoxButton.OKCancel;
            MessageBoxImage icono = MessageBoxImage.Question;

            resultado = MessageBox.Show("Si carga una nueva tabla perderá los datos de la tabla actual que no se hayan guardado\n" +
                "¿Desea cargar una nueva tabla?", "Aviso cargar tabla", botones, icono);

            switch (resultado)
            {
                case MessageBoxResult.OK:
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "cal files (*.cal)|*.cal|All files (*.*)|*.*";
                    dialog.FilterIndex = 1;
                    dialog.RestoreDirectory = true;

                    if ((bool)dialog.ShowDialog())
                    {
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.Title.Equals("Tablas"))
                            {
                                window.Close();
                                limpiarGrafico();
                            }
                        }
                        archivoActual = dialog.FileName.ToString();
                        CargarArchivoTmp(archivoActual);
                        MostrarCuadro("Se ha cargado el archivo correctamente.", "Carga exitosa");
                    }
                    else
                    {
                        MostrarCuadro("No se ha podido cargar el archivo con el nombre especificado.", "Error al seleccionar archivo a cargar");
                    }

                    break;

                case MessageBoxResult.Cancel: return;
            }
        }
        private void GuardarTablas_Click(object sender, RoutedEventArgs e)
        {
            if (archivoActual.Equals("0"))
            {
                MostrarCuadro("Seleccione un archivo sobre el que guardar primero\nusando la ventana Guardar como...", "Guardado fallido");
            }
            else
            {
                GuardarArchivo(archivoActual);
                MostrarCuadro("Se ha guardado el archivo correctamente.", "Guardado exitoso");
            }
        }

        private void GuardarTablasComo_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "cal files (*.cal)|*.cal|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if ((bool)dialog.ShowDialog())
            {
                archivoActual = dialog.FileName.ToString();
                GuardarArchivo(archivoActual);
                MostrarCuadro("Se ha guardado el archivo correctamente.", "Guardado exitoso");
            }
            else
            {
                MostrarCuadro("No se ha podido guardar el archivo con el nombre especificado.", "Error al seleccionar archivo a guardar");
            }
        }

        private void VerTodo_Click(object sender, RoutedEventArgs e)
        {
            var canvas = (Canvas)this.FindName("CanvasComida");
            canvas.Visibility = Visibility.Hidden;
            canvas = (Canvas)this.FindName("CanvasDias");
            canvas.Visibility = Visibility.Visible;

            var boton = (Button)this.FindName("DchComidas");
            boton.Visibility = Visibility.Hidden;
            boton.IsEnabled = false;

            boton = (Button)this.FindName("IzqComidas");
            boton.Visibility = Visibility.Hidden;
            boton.IsEnabled = false;

            boton = (Button)this.FindName("DchDias");
            boton.Visibility = Visibility.Visible;
            boton.IsEnabled = true;

            boton = (Button)this.FindName("IzqDias");
            boton.Visibility = Visibility.Visible;
            boton.IsEnabled = true;
        }

        private void GuardarArchivo(String s)
        {
            BinarySerialization.WriteToBinaryFile(s, new List<Fecha>(listaDate));
        }
        private void BotonIzqComidas_Click(object sender, RoutedEventArgs e)
        {
            moverGraficoComidas(false);
        }

        private void BotonDchComidas_Click(object sender, RoutedEventArgs e)
        {
            moverGraficoComidas(true);
        }
        private void BotonIzqDias_Click(object sender, RoutedEventArgs e)
        {
            moverGraficoDias(false);
        }

        private void BotonDchDias_Click(object sender, RoutedEventArgs e)
        {
            moverGraficoDias(true);
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
            do
            {
                if (s >= 15)
                    s -= 14;
            } while (s >= 15);
            
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
                case 9:     return Brushes.AliceBlue;
                case 10:    return Brushes.Aquamarine;
                case 11:    return Brushes.Chocolate;
                case 12:    return Brushes.Crimson;
                case 13:    return Brushes.DeepPink;
                case 14:    return Brushes.Indigo;

                default:    return Brushes.Black;
            }
        }

        /// <summary>
        /// Clones an element.
        /// </summary>
        private static T ElementClone<T>(T element)
        {
            T clone = default(T);
            MemoryStream memStream = ElementToStream(element);
            clone = ElementFromStream<T>(memStream);
            return clone;
        }

        /// <summary>
        /// Saves an element as MemoryStream.
        /// </summary>
        private static MemoryStream ElementToStream(object element)
        {
            MemoryStream memStream = new MemoryStream();
            XamlWriter.Save(element, memStream);
            return memStream;
        }

        /// <summary>
        /// Rebuilds an element from a MemoryStream.
        /// </summary>
        private static T ElementFromStream<T>(MemoryStream elementAsStream)
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

        private void eliminarLineas()
        {
            Canvas canvas = (Canvas)this.FindName("CanvasDias");
            List<UIElement> aBorrar = new List<UIElement>();

            foreach (UIElement ui in canvas.Children)
            {
                if(ui.Uid.Contains("aBorrar"))
                    aBorrar.Add(ui);
            }

            foreach (UIElement ui in aBorrar)
            {
                canvas.Children.Remove(ui);
            }
        }
    }
  }

